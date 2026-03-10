-- Primer INSERT skripte za dodavanje testnih podataka
-- Koristi novu strukturu: prvo Kupac, pa Firma ili FizickoLice

USE ProdavnicaDB;
GO

-- Dodajemo primer firme
DECLARE @idKupac1 INT;
INSERT INTO Kupac (popust) VALUES (0.15);
SET @idKupac1 = SCOPE_IDENTITY();

INSERT INTO Firma (idKupac, naziv, pib, adresa, partnerstvo)
VALUES (@idKupac1, 'Tehno Haus DOO', '123456789', 'Bulevar kralja Aleksandra 123, Beograd', 1);
PRINT 'Dodata firma: Tehno Haus DOO';

-- Dodajemo primer fizičkog lica
DECLARE @idKupac2 INT;
INSERT INTO Kupac (popust) VALUES (0.05);
SET @idKupac2 = SCOPE_IDENTITY();

INSERT INTO FizickoLice (idKupac, imePrezime, email, telefon, loyaltyClan)
VALUES (@idKupac2, 'Jovan Jovanović', 'jovan@email.com', '0601234567', 1);
PRINT 'Dodato fizičko lice: Jovan Jovanović';

-- Dodajemo još jednu firmu
DECLARE @idKupac3 INT;
INSERT INTO Kupac (popust) VALUES (0.20);
SET @idKupac3 = SCOPE_IDENTITY();

INSERT INTO Firma (idKupac, naziv, pib, adresa, partnerstvo)
VALUES (@idKupac3, 'Mega Kompanija AD', '987654321', 'Kneza Miloša 55, Beograd', 0);
PRINT 'Dodata firma: Mega Kompanija AD';

-- Dodajemo još jedno fizičko lice
DECLARE @idKupac4 INT;
INSERT INTO Kupac (popust) VALUES (0.10);
SET @idKupac4 = SCOPE_IDENTITY();

INSERT INTO FizickoLice (idKupac, imePrezime, email, telefon, loyaltyClan)
VALUES (@idKupac4, 'Marija Petrović', 'marija@email.com', '0641234567', 0);
PRINT 'Dodato fizičko lice: Marija Petrović';

PRINT 'Testni podaci su uspešno dodati.';
