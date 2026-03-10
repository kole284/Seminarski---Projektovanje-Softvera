-- Jednostavna skripta za kreiranje Firma i FizickoLice tabela
-- Koristi se kada Kupac tabela već ima samo idKupac i popust

USE ProdavnicaDB;
GO

-- Kreiramo tabelu Firma ako ne postoji
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
ELSE
BEGIN
    PRINT 'Tabela Firma već postoji.';
END
GO

-- Kreiramo tabelu FizickoLice ako ne postoji
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
ELSE
BEGIN
    PRINT 'Tabela FizickoLice već postoji.';
END
GO

PRINT 'Struktura je spremna!';
PRINT 'Kupac tabela: idKupac, popust';
PRINT 'Firma tabela: idFirma, idKupac, naziv, pib, adresa, partnerstvo';
PRINT 'FizickoLice tabela: idFizickoLice, idKupac, naziv, imePrezime, email, telefon, loyaltyClan';
