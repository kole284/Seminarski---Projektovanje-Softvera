-- Ispravka strukture Racun tabele
USE ProdavnicaDB;
GO

-- KORAK 1: Obriši sve Foreign Key constraint-e na StavkaRacuna tabeli
DECLARE @sql NVARCHAR(MAX);
DECLARE @constraintName NVARCHAR(128);

DECLARE constraint_cursor CURSOR FOR
SELECT fk.name
FROM sys.foreign_keys AS fk
WHERE fk.parent_object_id = OBJECT_ID('StavkaRacuna');

OPEN constraint_cursor;
FETCH NEXT FROM constraint_cursor INTO @constraintName;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @sql = 'ALTER TABLE StavkaRacuna DROP CONSTRAINT ' + QUOTENAME(@constraintName) + ';';
    BEGIN TRY
        EXEC sp_executesql @sql;
        PRINT 'Obrisan FK constraint: ' + @constraintName;
    END TRY
    BEGIN CATCH
        PRINT 'Greška pri brisanju constraint-a: ' + @constraintName;
    END CATCH
    FETCH NEXT FROM constraint_cursor INTO @constraintName;
END

CLOSE constraint_cursor;
DEALLOCATE constraint_cursor;
GO

-- KORAK 2: Obriši StavkaRacuna tabelu
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'StavkaRacuna')
BEGIN
    DROP TABLE StavkaRacuna;
    PRINT 'Tabela StavkaRacuna je obrisana.';
END
GO

-- KORAK 3: Obriši sve Foreign Key constraint-e koji pokazuju NA Racun tabelu
DECLARE @sql2 NVARCHAR(MAX);
DECLARE @constraintName2 NVARCHAR(128);
DECLARE @tableName NVARCHAR(128);

DECLARE constraint_cursor2 CURSOR FOR
SELECT fk.name, OBJECT_NAME(fk.parent_object_id) AS TableName
FROM sys.foreign_keys AS fk
WHERE fk.referenced_object_id = OBJECT_ID('Racun');

OPEN constraint_cursor2;
FETCH NEXT FROM constraint_cursor2 INTO @constraintName2, @tableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @sql2 = 'ALTER TABLE ' + QUOTENAME(@tableName) + ' DROP CONSTRAINT ' + QUOTENAME(@constraintName2) + ';';
    BEGIN TRY
        EXEC sp_executesql @sql2;
        PRINT 'Obrisan FK constraint: ' + @constraintName2 + ' iz tabele ' + @tableName;
    END TRY
    BEGIN CATCH
        PRINT 'Greška pri brisanju constraint-a: ' + @constraintName2;
    END CATCH
    FETCH NEXT FROM constraint_cursor2 INTO @constraintName2, @tableName;
END

CLOSE constraint_cursor2;
DEALLOCATE constraint_cursor2;
GO

-- KORAK 4: Obriši Racun tabelu
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Racun')
BEGIN
    DROP TABLE Racun;
    PRINT 'Tabela Racun je obrisana.';
END
GO

-- KORAK 2: Kreiraj ispravnu Racun tabelu
CREATE TABLE Racun (
    idRacun INT PRIMARY KEY IDENTITY(1,1),
    datumIzdavanja DATE NOT NULL,
    konacanIznos FLOAT NOT NULL,
    pdv FLOAT NOT NULL,
    cenaSaPopustom FLOAT NOT NULL,
    cenaStavke FLOAT NOT NULL,
    idProdavac INT NOT NULL FOREIGN KEY REFERENCES Prodavac(idProdavac),
    idKupac INT NOT NULL FOREIGN KEY REFERENCES Kupac(idKupac)
);
PRINT 'Tabela Racun je kreirana sa ispravnom strukturom.';
GO

-- KORAK 3: Kreiraj StavkaRacuna tabelu
CREATE TABLE StavkaRacuna (
    idRacun INT NOT NULL,
    rbStavke INT NOT NULL,
    kolicina INT NOT NULL,
    datumKupovine DATE NOT NULL,
    iznos FLOAT NOT NULL,
    cena FLOAT NOT NULL,
    idOprema INT NOT NULL FOREIGN KEY REFERENCES Oprema(idOprema),
    PRIMARY KEY (idRacun, rbStavke),
    FOREIGN KEY (idRacun) REFERENCES Racun(idRacun) ON DELETE CASCADE
);
PRINT 'Tabela StavkaRacuna je kreirana.';
GO

PRINT 'Struktura je ispravljena!';
PRINT 'Racun: idRacun, datumIzdavanja, konacanIznos, pdv, cenaSaPopustom, cenaStavke, idProdavac (FK), idKupac (FK)';
PRINT 'StavkaRacuna: idRacun (FK), rbStavke, kolicina, datumKupovine, iznos, cena, idOprema (FK)';
