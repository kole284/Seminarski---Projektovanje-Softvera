-- SQL skripta za refaktorisanje Kupac, Firma i FizickoLice tabela
-- Kupac postaje osnovna tabela sa samo idKupac i popust
-- Firma i FizickoLice postaju posebne tabele sa vezom ka Kupac

USE ProdavnicaDB;
GO

-- Prvo kreiramo novu tabelu Firma
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Firma')
BEGIN
    CREATE TABLE Firma (
        idFirma INT PRIMARY KEY IDENTITY(1,1),
        idKupac INT NOT NULL FOREIGN KEY REFERENCES Kupac(idKupac) ON DELETE CASCADE,
        naziv VARCHAR(100) NOT NULL,
        pib VARCHAR(20) NULL,
        adresa VARCHAR(200) NULL,
        partnerstvo BIT NULL DEFAULT 0
    );
    PRINT 'Tabela Firma je kreirana.';
END
GO

-- Kreiramo novu tabelu FizickoLice
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'FizickoLice')
BEGIN
    CREATE TABLE FizickoLice (
        idFizickoLice INT PRIMARY KEY IDENTITY(1,1),
        idKupac INT NOT NULL FOREIGN KEY REFERENCES Kupac(idKupac) ON DELETE CASCADE,
        naziv VARCHAR(100) NOT NULL,
        imePrezime VARCHAR(100) NULL,
        email VARCHAR(100) NULL,
        telefon VARCHAR(20) NULL,
        loyaltyClan BIT NULL DEFAULT 0
    );
    PRINT 'Tabela FizickoLice je kreirana.';
END
GO

-- Migrirajmo postojeće podatke iz Kupac tabele
-- Prvo proveravamo da li postoje potrebne kolone za migraciju
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Kupac') AND name = 'tipKupca')
   AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Kupac') AND name = 'naziv')
BEGIN
    -- Prvo firme (ako postoje podaci sa tipKupca = 'Firma')
    IF EXISTS (SELECT 1 FROM Kupac WHERE tipKupca = 'Firma')
    BEGIN
        INSERT INTO Firma (idKupac, naziv, pib, adresa, partnerstvo)
        SELECT idKupac, naziv, pib, adresa, ISNULL(partnerstvo, 0)
        FROM Kupac
        WHERE tipKupca = 'Firma';
        PRINT 'Podaci firmi su migrirani.';
    END
    ELSE
    BEGIN
        PRINT 'Nema podataka za migraciju firmi.';
    END

    -- Zatim fizička lica
    IF EXISTS (SELECT 1 FROM Kupac WHERE tipKupca = 'FizickoLice')
    BEGIN
        INSERT INTO FizickoLice (idKupac, naziv, imePrezime, email, telefon, loyaltyClan)
        SELECT idKupac, naziv, imePrezime, email, telefon, ISNULL(loyaltyClan, 0)
        FROM Kupac
        WHERE tipKupca = 'FizickoLice';
        PRINT 'Podaci fizičkih lica su migrirani.';
    END
    ELSE
    BEGIN
        PRINT 'Nema podataka za migraciju fizičkih lica.';
    END
END
ELSE
BEGIN
    PRINT 'Kupac tabela nema stare kolone - migracija podataka se preskače.';
    PRINT 'Tabele Firma i FizickoLice su kreirane, ali su prazne.';
END
GO

-- Sada možemo obrisati stare kolone iz Kupac tabele
-- Prvo moramo obrisati kolone koje nisu potrebne
DECLARE @sql NVARCHAR(MAX) = '';

-- Brišemo constraint-e vezane za kolone koje ćemo obrisati
DECLARE constraint_cursor CURSOR FOR
SELECT 'ALTER TABLE Kupac DROP CONSTRAINT ' + QUOTENAME(name) + ';'
FROM sys.default_constraints
WHERE parent_object_id = OBJECT_ID('Kupac')
AND parent_column_id IN (
    SELECT column_id FROM sys.columns 
    WHERE object_id = OBJECT_ID('Kupac') 
    AND name IN ('naziv', 'tipKupca', 'pib', 'adresa', 'partnerstvo', 'imePrezime', 'email', 'telefon', 'loyaltyClan')
);

OPEN constraint_cursor;
FETCH NEXT FROM constraint_cursor INTO @sql;

WHILE @@FETCH_STATUS = 0
BEGIN
    EXEC sp_executesql @sql;
    FETCH NEXT FROM constraint_cursor INTO @sql;
END

CLOSE constraint_cursor;
DEALLOCATE constraint_cursor;
GO

-- Sada brišemo kolone
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Kupac') AND name = 'naziv')
    ALTER TABLE Kupac DROP COLUMN naziv;
    
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Kupac') AND name = 'tipKupca')
    ALTER TABLE Kupac DROP COLUMN tipKupca;
    
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Kupac') AND name = 'pib')
    ALTER TABLE Kupac DROP COLUMN pib;
    
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Kupac') AND name = 'adresa')
    ALTER TABLE Kupac DROP COLUMN adresa;
    
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Kupac') AND name = 'partnerstvo')
    ALTER TABLE Kupac DROP COLUMN partnerstvo;
    
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Kupac') AND name = 'imePrezime')
    ALTER TABLE Kupac DROP COLUMN imePrezime;
    
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Kupac') AND name = 'email')
    ALTER TABLE Kupac DROP COLUMN email;
    
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Kupac') AND name = 'telefon')
    ALTER TABLE Kupac DROP COLUMN telefon;
    
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Kupac') AND name = 'loyaltyClan')
    ALTER TABLE Kupac DROP COLUMN loyaltyClan;
GO

PRINT 'Refaktorisanje je uspešno završeno!';
PRINT 'Kupac tabela sada sadrži samo: idKupac, popust';
PRINT 'Firma tabela: idFirma, idKupac, naziv, pib, adresa, partnerstvo';
PRINT 'FizickoLice tabela: idFizickoLice, idKupac, naziv, imePrezime, email, telefon, loyaltyClan';
