-- SQL skripta za kreiranje strukturnih ograničenja (Foreign Keys i Constraints)
-- SQL Server koristi NO ACTION umjesto RESTRICT

USE ProdavnicaDB;
GO

PRINT '===============================================';
PRINT 'Kreiranje strukturnih ogranicenja...';
PRINT '===============================================';
PRINT '';

-- ===== PRVO: Obriši postojeće foreign keys =====
PRINT 'Brisanje starih ogranicenja...';

IF OBJECT_ID('FK_StavkaRacuna_Racun', 'F') IS NOT NULL
    ALTER TABLE StavkaRacuna DROP CONSTRAINT FK_StavkaRacuna_Racun;
    
IF OBJECT_ID('FK_StavkaRacuna_Oprema', 'F') IS NOT NULL
    ALTER TABLE StavkaRacuna DROP CONSTRAINT FK_StavkaRacuna_Oprema;

IF OBJECT_ID('FK_Racun_Prodavac', 'F') IS NOT NULL
    ALTER TABLE Racun DROP CONSTRAINT FK_Racun_Prodavac;
    
IF OBJECT_ID('FK_Racun_Kupac', 'F') IS NOT NULL
    ALTER TABLE Racun DROP CONSTRAINT FK_Racun_Kupac;

IF OBJECT_ID('FK_ProdSklad_Prodavac', 'F') IS NOT NULL
    ALTER TABLE ProdSklad DROP CONSTRAINT FK_ProdSklad_Prodavac;
    
IF OBJECT_ID('FK_ProdSklad_Skladiste', 'F') IS NOT NULL
    ALTER TABLE ProdSklad DROP CONSTRAINT FK_ProdSklad_Skladiste;

IF OBJECT_ID('FK_Firma_Kupac', 'F') IS NOT NULL
    ALTER TABLE Firma DROP CONSTRAINT FK_Firma_Kupac;
    
IF OBJECT_ID('FK_FizickoLice_Kupac', 'F') IS NOT NULL
    ALTER TABLE FizickoLice DROP CONSTRAINT FK_FizickoLice_Kupac;

PRINT 'Stara ogranicenja obrisana.';
PRINT '';

GO

-- ===== DRUGO: Kreiraj nove foreign keys sa odgovarajućim opcijama =====

PRINT 'Kreiranje novih ogranicenja...';
PRINT '';

-- FK: Firma → Kupac (CASCADE DELETE)
ALTER TABLE Firma
ADD CONSTRAINT FK_Firma_Kupac
FOREIGN KEY (idKupac) REFERENCES Kupac(idKupac)
ON DELETE CASCADE;
PRINT '✓ Firma → Kupac (CASCADE DELETE)';
GO

-- FK: FizickoLice → Kupac (CASCADE DELETE)
ALTER TABLE FizickoLice
ADD CONSTRAINT FK_FizickoLice_Kupac
FOREIGN KEY (idKupac) REFERENCES Kupac(idKupac)
ON DELETE CASCADE;
PRINT '✓ FizickoLice → Kupac (CASCADE DELETE)';
GO

-- FK: Racun → Kupac (NO ACTION - sprečava brisanje kupca ako ima račune)
ALTER TABLE Racun
ADD CONSTRAINT FK_Racun_Kupac
FOREIGN KEY (idKupac) REFERENCES Kupac(idKupac)
ON DELETE NO ACTION;
PRINT '✓ Racun → Kupac (NO ACTION - ne može obrisati kupca sa računima)';
GO

-- FK: Racun → Prodavac (NO ACTION - sprečava brisanje prodavca ako ima račune)
ALTER TABLE Racun
ADD CONSTRAINT FK_Racun_Prodavac
FOREIGN KEY (idProdavac) REFERENCES Prodavac(idProdavac)
ON DELETE NO ACTION;
PRINT '✓ Racun → Prodavac (NO ACTION - ne može obrisati prodavca sa računima)';
GO

-- FK: StavkaRacuna → Racun (CASCADE DELETE)
ALTER TABLE StavkaRacuna
ADD CONSTRAINT FK_StavkaRacuna_Racun
FOREIGN KEY (idRacun) REFERENCES Racun(idRacun)
ON DELETE CASCADE;
PRINT '✓ StavkaRacuna → Racun (CASCADE DELETE)';
GO

-- FK: StavkaRacuna → Oprema (NO ACTION - sprečava brisanje opreme ako je u stavkama)
ALTER TABLE StavkaRacuna
ADD CONSTRAINT FK_StavkaRacuna_Oprema
FOREIGN KEY (idOprema) REFERENCES Oprema(idOprema)
ON DELETE NO ACTION;
PRINT '✓ StavkaRacuna → Oprema (NO ACTION - ne može obrisati opremu iz stavki)';
GO

-- FK: ProdSklad → Prodavac (CASCADE DELETE)
IF OBJECT_ID('ProdSklad', 'U') IS NOT NULL
BEGIN
    ALTER TABLE ProdSklad
    ADD CONSTRAINT FK_ProdSklad_Prodavac
    FOREIGN KEY (idProdavac) REFERENCES Prodavac(idProdavac)
    ON DELETE CASCADE;
    PRINT '✓ ProdSklad → Prodavac (CASCADE DELETE)';
END
GO

-- FK: ProdSklad → Skladiste (NO ACTION - sprečava brisanje skladišta ako je u upotrebi)
IF OBJECT_ID('ProdSklad', 'U') IS NOT NULL
BEGIN
    ALTER TABLE ProdSklad
    ADD CONSTRAINT FK_ProdSklad_Skladiste
    FOREIGN KEY (idSkladiste) REFERENCES Skladiste(idSkladiste)
    ON DELETE NO ACTION;
    PRINT '✓ ProdSklad → Skladiste (NO ACTION - ne može obrisati skladište u upotrebi)';
END
GO

PRINT '';
PRINT '===============================================';
PRINT 'STRUKTURNA OGRANICENJA USPJESNO KREIRANA!';
PRINT '===============================================';
PRINT '';
PRINT 'ZASTITA:';
PRINT '✓ KUPAC - NE MOZE OBRISATI ako ima Racune';
PRINT '✓ PRODAVAC - NE MOZE OBRISATI ako ima Racune';
PRINT '✓ OPREMA - NE MOZE OBRISATI ako je u StavkaRacuna';
PRINT '✓ SKLADISTE - NE MOZE OBRISATI ako je u ProdSklad';
PRINT '';
PRINT 'CASCADE BRISANJA:';
PRINT '→ Firma se brisa ako se obrise Kupac';
PRINT '→ FizickoLice se brisa ako se obrise Kupac';
PRINT '→ StavkaRacuna se brisu ako se obrise Racun';
PRINT '→ ProdSklad se brisu ako se obrise Prodavac';
PRINT '';
