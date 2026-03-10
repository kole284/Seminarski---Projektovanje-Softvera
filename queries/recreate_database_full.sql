-- ===================================================================
-- MASTER SQL SKRIPTA - Kreiranje Kompletnne Baze sa Ograničenjima
-- ===================================================================
-- Ova skripta:
-- 1. BRIŠE celu bazu ProdavnicaDB
-- 2. KREIRA bazu ispočetka
-- 3. KREIRA sve tabele sa Foreign Keys i Check ograničenjima
-- 4. UBACUJE test podatke
-- ===================================================================

USE master;
GO

-- ===== KORAK 1: Obriši staru bazu =====
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'ProdavnicaDB')
BEGIN
    ALTER DATABASE ProdavnicaDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ProdavnicaDB;
    PRINT 'Stara baza ProdavnicaDB je obrisana.';
END
GO

-- ===== KORAK 2: Kreiraj novu bazu =====
CREATE DATABASE ProdavnicaDB;
PRINT 'Nova baza ProdavnicaDB je kreirana.';
GO

USE ProdavnicaDB;
GO

-- ===== KORAK 3: Kreiraj sve PARENT tabele =====

-- Tabela: Prodavac (PK: idProdavac)
CREATE TABLE Prodavac (
    idProdavac INT PRIMARY KEY IDENTITY(1,1),
    ime VARCHAR(100) NOT NULL,
    prezime VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    telefon VARCHAR(20) NULL,
    password VARCHAR(100) NOT NULL
);
PRINT 'Tabela Prodavac kreirana.';
GO

-- Tabela: Kupac (PK: idKupac)
CREATE TABLE Kupac (
    idKupac INT PRIMARY KEY IDENTITY(1,1),
    popust FLOAT NOT NULL DEFAULT 0,
    CONSTRAINT CK_Kupac_popust CHECK (popust >= 0)
);
PRINT 'Tabela Kupac kreirana.';
GO

-- Tabela: Firma (specijalizacija Kupaca)
CREATE TABLE Firma (
    idKupac INT PRIMARY KEY,
    naziv VARCHAR(100) NOT NULL,
    pib VARCHAR(20) NULL,
    adresa VARCHAR(200) NULL,
    partnerstvo BIT NULL DEFAULT 0,
    CONSTRAINT FK_Firma_Kupac FOREIGN KEY (idKupac) REFERENCES Kupac(idKupac) ON DELETE NO ACTION
);
PRINT 'Tabela Firma kreirana.';
GO

-- Tabela: FizickoLice (specijalizacija Kupaca)
CREATE TABLE FizickoLice (
    idKupac INT PRIMARY KEY,
    imePrezime VARCHAR(100) NOT NULL,
    email VARCHAR(100) NULL,
    telefon VARCHAR(20) NULL,
    loyaltyClan BIT NULL DEFAULT 0,
    CONSTRAINT FK_FizickoLice_Kupac FOREIGN KEY (idKupac) REFERENCES Kupac(idKupac) ON DELETE NO ACTION
);
PRINT 'Tabela FizickoLice kreirana.';
GO

-- Tabela: Skladiste (PK: idSkladiste)
CREATE TABLE Skladiste (
    idSkladiste INT PRIMARY KEY IDENTITY(1,1),
    naziv VARCHAR(100) NOT NULL,
    adresa VARCHAR(200) NULL
);
PRINT 'Tabela Skladiste kreirana.';
GO

-- Tabela: Oprema (PK: idOprema)
CREATE TABLE Oprema (
    idOprema INT PRIMARY KEY IDENTITY(1,1),
    ime VARCHAR(100) NOT NULL,
    kategorija INT NOT NULL DEFAULT 18, -- 18 = Ostalo
    cena FLOAT NOT NULL,
    CONSTRAINT CK_Oprema_cena CHECK (cena > 0)
);
PRINT 'Tabela Oprema kreirana.';
GO

-- Tabela: Racun (PK: idRacun) - FK: idProdavac, idKupac
CREATE TABLE Racun (
    idRacun INT PRIMARY KEY IDENTITY(1,1),
    datumIzdavanja DATE NOT NULL,
    konacanIznos FLOAT NOT NULL DEFAULT 0,
    pdv FLOAT NOT NULL DEFAULT 0,
    cenaSaPopustom FLOAT NOT NULL DEFAULT 0,
    cenaStavke FLOAT NOT NULL DEFAULT 0,
    idProdavac INT NOT NULL,
    idKupac INT NOT NULL,
    CONSTRAINT FK_Racun_Prodavac FOREIGN KEY (idProdavac) REFERENCES Prodavac(idProdavac) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT FK_Racun_Kupac FOREIGN KEY (idKupac) REFERENCES Kupac(idKupac) ON DELETE NO ACTION ON UPDATE NO ACTION
);
PRINT 'Tabela Racun kreirana.';
GO

-- Tabela: StavkaRacuna (PK: idRacun, idOprema) - FK: idRacun, idOprema
CREATE TABLE StavkaRacuna (
    idRacun INT NOT NULL,
    idOprema INT NOT NULL,
    kolicina INT NOT NULL,
    cena FLOAT NOT NULL,
    PRIMARY KEY (idRacun, idOprema),
    CONSTRAINT FK_StavkaRacuna_Racun FOREIGN KEY (idRacun) REFERENCES Racun(idRacun) ON DELETE CASCADE,
    CONSTRAINT FK_StavkaRacuna_Oprema FOREIGN KEY (idOprema) REFERENCES Oprema(idOprema) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT CK_StavkaRacuna_kolicina CHECK (kolicina > 0),
    CONSTRAINT CK_StavkaRacuna_cena CHECK (cena > 0)
);
PRINT 'Tabela StavkaRacuna kreirana.';
GO

-- Tabela: ProdSklad (PK: idProdavac, idSkladiste) - FK: idProdavac, idSkladiste
CREATE TABLE ProdSklad (
    idProdavac INT NOT NULL,
    idSkladiste INT NOT NULL,
    PRIMARY KEY (idProdavac, idSkladiste),
    CONSTRAINT FK_ProdSklad_Prodavac FOREIGN KEY (idProdavac) REFERENCES Prodavac(idProdavac) ON DELETE CASCADE,
    CONSTRAINT FK_ProdSklad_Skladiste FOREIGN KEY (idSkladiste) REFERENCES Skladiste(idSkladiste) ON DELETE NO ACTION ON UPDATE NO ACTION
);
PRINT 'Tabela ProdSklad kreirana.';
GO

PRINT '';
PRINT '===============================================';
PRINT 'Sve tabele su uspešno kreirane!';
PRINT '===============================================';
PRINT '';

-- ===== KORAK 4: Ubaci TEST PODATKE =====

-- Prodavci
INSERT INTO Prodavac (ime, prezime, email, telefon, password)
VALUES 
    ('Marko', 'Marković', 'marko@prodavnica.com', '0611234567', 'sifra123'),
    ('Ana', 'Anić', 'ana@prodavnica.com', '0612234567', 'sifra123'),
    ('Petar', 'Petrović', 'petar@prodavnica.com', '0613234567', 'sifra123');
PRINT 'Dodano 3 Prodavca.';
GO

-- Kupci i Firme
INSERT INTO Kupac (popust) VALUES (0.10);
DECLARE @idKupac1 INT = SCOPE_IDENTITY();
INSERT INTO Firma (idKupac, naziv, pib, adresa, partnerstvo)
VALUES (@idKupac1, 'Tehno Haus DOO', '123456789', 'Bulevar Aleksandra 123, Beograd', 1);
PRINT 'Dodata Firma: Tehno Haus DOO';
GO

INSERT INTO Kupac (popust) VALUES (0.15);
DECLARE @idKupac2 INT = SCOPE_IDENTITY();
INSERT INTO Firma (idKupac, naziv, pib, adresa, partnerstvo)
VALUES (@idKupac2, 'Mega Kompanija AD', '987654321', 'Kneza Miloša 55, Beograd', 0);
PRINT 'Dodata Firma: Mega Kompanija AD';
GO

-- Kupci i Fizička lica
INSERT INTO Kupac (popust) VALUES (0.05);
DECLARE @idKupac3 INT = SCOPE_IDENTITY();
INSERT INTO FizickoLice (idKupac, imePrezime, email, telefon, loyaltyClan)
VALUES (@idKupac3, 'Jovan Jovanović', 'jovan@email.com', '0601234567', 1);
PRINT 'Dodato Fizičko lice: Jovan Jovanović';
GO

INSERT INTO Kupac (popust) VALUES (0.10);
DECLARE @idKupac4 INT = SCOPE_IDENTITY();
INSERT INTO FizickoLice (idKupac, imePrezime, email, telefon, loyaltyClan)
VALUES (@idKupac4, 'Marija Marjanović', 'marija@email.com', '0641234567', 0);
PRINT 'Dodato Fizičko lice: Marija Marjanović';
GO

INSERT INTO Kupac (popust) VALUES (0.20);
DECLARE @idKupac5 INT = SCOPE_IDENTITY();
INSERT INTO FizickoLice (idKupac, imePrezime, email, telefon, loyaltyClan)
VALUES (@idKupac5, 'Nikola Nikolić', 'nikola@email.com', '0651234567', 1);
PRINT 'Dodato Fizičko lice: Nikola Nikolić';
GO

-- Skladišta
INSERT INTO Skladiste (naziv, adresa)
VALUES 
    ('Glavno Skladište', 'Beograd, Voždovac'),
    ('Skladište Beograd', 'Beograd, Voždovac'),
    ('Skladište Novi Sad', 'Novi Sad, Telep');
PRINT 'Dodano 3 Skladišta.';
GO

-- Oprema/Proizvodi
INSERT INTO Oprema (ime, kategorija, cena)
VALUES 
    ('Laptop Dell XPS 13', 1, 1200.00),
    ('Monitor LG 27\"', 2, 350.00),
    ('Miš Logitech', 3, 45.00),
    ('Tastatura Mechanical', 3, 120.00),
    ('Monitor ASUS 32\"', 2, 450.00),
    ('SSD 1TB Samsung', 4, 120.00),
    ('RAM 16GB DDR4', 4, 90.00),
    ('Grafička kartaRTX 3080', 5, 1500.00);
PRINT 'Dodano 8 Proizvoda/Opreme.';
GO

-- ProdSklad (Prodavci i njihova skladišta)
INSERT INTO ProdSklad (idProdavac, idSkladiste)
VALUES 
    (1, 1), (1, 2),
    (2, 2), (2, 3),
    (3, 1), (3, 3);
PRINT 'Dodana povezanost Prodavac-Skladiste.';
GO

-- Računi i Stavke
DECLARE @idRacun1 INT;
INSERT INTO Racun (datumIzdavanja, konacanIznos, pdv, cenaSaPopustom, cenaStavke, idProdavac, idKupac)
VALUES ('2026-01-15', 2545.00, 509.00, 2290.50, 2545.00, 1, 1);
SET @idRacun1 = SCOPE_IDENTITY();
INSERT INTO StavkaRacuna (idRacun, idOprema, kolicina, cena)
VALUES 
    (@idRacun1, 1, 1, 1200.00),
    (@idRacun1, 3, 3, 45.00),
    (@idRacun1, 4, 1, 120.00);
PRINT 'Dodat Račun 1 sa 3 stavke.';
GO

DECLARE @idRacun2 INT;
INSERT INTO Racun (datumIzdavanja, konacanIznos, pdv, cenaSaPopustom, cenaStavke, idProdavac, idKupac)
VALUES ('2026-01-20', 800.00, 160.00, 720.00, 800.00, 2, 3);
SET @idRacun2 = SCOPE_IDENTITY();
INSERT INTO StavkaRacuna (idRacun, idOprema, kolicina, cena)
VALUES 
    (@idRacun2, 2, 1, 350.00),
    (@idRacun2, 3, 9, 50.00);
PRINT 'Dodat Račun 2 sa 2 stavke.';
GO

DECLARE @idRacun3 INT;
INSERT INTO Racun (datumIzdavanja, konacanIznos, pdv, cenaSaPopustom, cenaStavke, idProdavac, idKupac)
VALUES ('2026-01-22', 450.00, 90.00, 405.00, 450.00, 3, 4);
SET @idRacun3 = SCOPE_IDENTITY();
INSERT INTO StavkaRacuna (idRacun, idOprema, kolicina, cena)
VALUES 
    (@idRacun3, 5, 1, 450.00);
PRINT 'Dodat Račun 3 sa 1 stavkom.';
GO

DECLARE @idRacun4 INT;
INSERT INTO Racun (datumIzdavanja, konacanIznos, pdv, cenaSaPopustom, cenaStavke, idProdavac, idKupac)
VALUES ('2026-01-25', 1410.00, 282.00, 1269.00, 1410.00, 1, 5);
SET @idRacun4 = SCOPE_IDENTITY();
INSERT INTO StavkaRacuna (idRacun, idOprema, kolicina, cena)
VALUES 
    (@idRacun4, 6, 1, 120.00),
    (@idRacun4, 7, 2, 90.00),
    (@idRacun4, 8, 1, 1200.00);
PRINT 'Dodat Račun 4 sa 3 stavke.';
GO

PRINT '';
PRINT '===============================================';
PRINT 'Svi test podaci su uspešno ubačeni!';
PRINT '===============================================';
PRINT '';
PRINT 'STATISTIKA:';
PRINT '- Prodavci: 3';
PRINT '- Firme: 2';
PRINT '- Fizička lica: 3';
PRINT '- Skladišta: 3';
PRINT '- Proizvodi/Oprema: 8';
PRINT '- Računi: 4';
PRINT '- Stavke u računima: 9';
PRINT '';
PRINT 'Baza je spreman za testiranje sa svim ograničenjima!';
