-- ISPRAVLJENA SQL skripta - Table per Type inheritance
-- idKupac je primarni ključ i u Firma i u FizickoLice tabelama

USE ProdavnicaDB;
GO

-- Brišemo stare tabele ako postoje
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Firma')
    DROP TABLE Firma;
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'FizickoLice')
    DROP TABLE FizickoLice;
GO

-- Kreiramo tabelu Firma - idKupac je PK i FK
CREATE TABLE Firma (
    idKupac INT PRIMARY KEY FOREIGN KEY REFERENCES Kupac(idKupac) ON DELETE CASCADE,
    naziv VARCHAR(100) NOT NULL,
    pib VARCHAR(20) NULL,
    adresa VARCHAR(200) NULL,
    partnerstvo BIT NULL DEFAULT 0
);
PRINT 'Tabela Firma je kreirana.';
GO

-- Kreiramo tabelu FizickoLice - idKupac je PK i FK
CREATE TABLE FizickoLice (
    idKupac INT PRIMARY KEY FOREIGN KEY REFERENCES Kupac(idKupac) ON DELETE CASCADE,
    imePrezime VARCHAR(100) NOT NULL,
    email VARCHAR(100) NULL,
    telefon VARCHAR(20) NULL,
    loyaltyClan BIT NULL DEFAULT 0
);
PRINT 'Tabela FizickoLice je kreirana.';
GO

PRINT 'Struktura je ispravljena!';
PRINT 'Kupac: idKupac (PK), popust';
PRINT 'Firma: idKupac (PK+FK), naziv, pib, adresa, partnerstvo';
PRINT 'FizickoLice: idKupac (PK+FK), imePrezime, email, telefon, loyaltyClan';
