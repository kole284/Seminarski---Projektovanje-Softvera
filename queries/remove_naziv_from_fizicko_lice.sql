-- SQL skripta za uklanjanje kolone 'naziv' iz FizickoLice tabele
-- FizickoLice treba da ima samo imePrezime, bez naziv kolone

USE ProdavnicaDB;
GO

-- Proverimo da li postoji kolona 'naziv' u FizickoLice tabeli
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FizickoLice') AND name = 'naziv')
BEGIN
    PRINT 'Uklanjam kolonu naziv iz FizickoLice tabele...';
    
    -- Prvo moramo ukloniti sve constraint-e vezane za ovu kolonu
    DECLARE @sql NVARCHAR(MAX) = '';
    DECLARE constraint_cursor CURSOR FOR
    SELECT 'ALTER TABLE FizickoLice DROP CONSTRAINT ' + QUOTENAME(name) + ';'
    FROM sys.default_constraints
    WHERE parent_object_id = OBJECT_ID('FizickoLice')
    AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('FizickoLice') AND name = 'naziv');

    OPEN constraint_cursor;
    FETCH NEXT FROM constraint_cursor INTO @sql;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        EXEC sp_executesql @sql;
        PRINT 'Constraint obrisan: ' + @sql;
        FETCH NEXT FROM constraint_cursor INTO @sql;
    END

    CLOSE constraint_cursor;
    DEALLOCATE constraint_cursor;

    -- Sada možemo obrisati kolonu
    ALTER TABLE FizickoLice DROP COLUMN naziv;
    PRINT 'Kolona naziv je uspešno uklonjena iz FizickoLice tabele.';
END
ELSE
BEGIN
    PRINT 'Kolona naziv ne postoji u FizickoLice tabeli - nema šta da se briše.';
END
GO

-- Proveravamo da li je imePrezime NOT NULL
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FizickoLice') AND name = 'imePrezime' AND is_nullable = 1)
BEGIN
    PRINT 'Postavljam imePrezime kolonu kao NOT NULL...';
    -- Prvo postavimo default vrednost za NULL zapise ako postoje
    UPDATE FizickoLice SET imePrezime = 'Nepoznato' WHERE imePrezime IS NULL;
    ALTER TABLE FizickoLice ALTER COLUMN imePrezime VARCHAR(100) NOT NULL;
    PRINT 'Kolona imePrezime je sada NOT NULL.';
END
GO

PRINT '-------------------------------------------';
PRINT 'Struktura FizickoLice tabele je ažurirana!';
PRINT 'FizickoLice: idKupac (PK+FK), imePrezime (NOT NULL), email, telefon, loyaltyClan';
