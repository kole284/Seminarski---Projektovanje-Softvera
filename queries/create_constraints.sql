-- SQL skripta za kreiranje strukturnih ograničenja (Foreign Keys i Constraints)
-- Prema ER dijagramu - sprečava brisanje objekata koji imaju reference

USE ProdavnicaDB;
GO

-- ===== PRVO: Obriši postojeće foreign keys =====
-- Obriši iz StavkaRacuna
IF OBJECT_ID('FK_StavkaRacuna_Racun', 'F') IS NOT NULL
    ALTER TABLE StavkaRacuna DROP CONSTRAINT FK_StavkaRacuna_Racun;
    
IF OBJECT_ID('FK_StavkaRacuna_Oprema', 'F') IS NOT NULL
    ALTER TABLE StavkaRacuna DROP CONSTRAINT FK_StavkaRacuna_Oprema;

-- Obriši iz Racun
IF OBJECT_ID('FK_Racun_Prodavac', 'F') IS NOT NULL
    ALTER TABLE Racun DROP CONSTRAINT FK_Racun_Prodavac;
    
IF OBJECT_ID('FK_Racun_Kupac', 'F') IS NOT NULL
    ALTER TABLE Racun DROP CONSTRAINT FK_Racun_Kupac;

-- Obriši iz ProdSklad (ako postoji)
IF OBJECT_ID('FK_ProdSklad_Prodavac', 'F') IS NOT NULL
    ALTER TABLE ProdSklad DROP CONSTRAINT FK_ProdSklad_Prodavac;
    
IF OBJECT_ID('FK_ProdSklad_Skladiste', 'F') IS NOT NULL
    ALTER TABLE ProdSklad DROP CONSTRAINT FK_ProdSklad_Skladiste;

-- Obriši iz Firma i FizickoLice
IF OBJECT_ID('FK_Firma_Kupac', 'F') IS NOT NULL
    ALTER TABLE Firma DROP CONSTRAINT FK_Firma_Kupac;
    
IF OBJECT_ID('FK_FizickoLice_Kupac', 'F') IS NOT NULL
    ALTER TABLE FizickoLice DROP CONSTRAINT FK_FizickoLice_Kupac;

GO

-- ===== DRUGO: Kreiraj nove foreign keys sa odgovarajućim opcijama =====

-- FK: Firma → Kupac (CASCADE DELETE - ako se obriše kupac, briše se i firma)
ALTER TABLE Firma
ADD CONSTRAINT FK_Firma_Kupac
FOREIGN KEY (idKupac) REFERENCES Kupac(idKupac)
ON DELETE CASCADE;
PRINT 'Kreirano ograničenje: Firma → Kupac (CASCADE DELETE)';
GO

-- FK: FizickoLice → Kupac (CASCADE DELETE - ako se obriše kupac, briše se i fizičko lice)
ALTER TABLE FizickoLice
ADD CONSTRAINT FK_FizickoLice_Kupac
FOREIGN KEY (idKupac) REFERENCES Kupac(idKupac)
ON DELETE CASCADE;
PRINT 'Kreirano ograničenje: FizickoLice → Kupac (CASCADE DELETE)';
GO

-- FK: Racun → Kupac (RESTRICT - ne može obrisati kupca ako ima račune)
ALTER TABLE Racun
ADD CONSTRAINT FK_Racun_Kupac
FOREIGN KEY (idKupac) REFERENCES Kupac(idKupac)
ON DELETE RESTRICT
ON UPDATE RESTRICT;
PRINT 'Kreirano ograničenje: Racun → Kupac (RESTRICT - ne može obrisati kupca sa računima)';
GO

-- FK: Racun → Prodavac (RESTRICT - ne može obrisati prodavca ako ima račune)
ALTER TABLE Racun
ADD CONSTRAINT FK_Racun_Prodavac
FOREIGN KEY (idProdavac) REFERENCES Prodavac(idProdavac)
ON DELETE RESTRICT
ON UPDATE RESTRICT;
PRINT 'Kreirano ograničenje: Racun → Prodavac (RESTRICT - ne može obrisati prodavca sa računima)';
GO

-- FK: StavkaRacuna → Racun (CASCADE DELETE - ako se obriše račun, brišu se i stavke)
ALTER TABLE StavkaRacuna
ADD CONSTRAINT FK_StavkaRacuna_Racun
FOREIGN KEY (idRacun) REFERENCES Racun(idRacun)
ON DELETE CASCADE;
PRINT 'Kreirano ograničenje: StavkaRacuna → Racun (CASCADE DELETE)';
GO

-- FK: StavkaRacuna → Oprema (RESTRICT - ne može obrisati opremu ako je u nekoj stavci)
ALTER TABLE StavkaRacuna
ADD CONSTRAINT FK_StavkaRacuna_Oprema
FOREIGN KEY (idOprema) REFERENCES Oprema(idOprema)
ON DELETE RESTRICT
ON UPDATE RESTRICT;
PRINT 'Kreirano ograničenje: StavkaRacuna → Oprema (RESTRICT - ne može obrisati opremu ako je u stavci)';
GO

-- FK: ProdSklad → Prodavac (ako tabela postoji) (CASCADE DELETE)
IF OBJECT_ID('ProdSklad', 'U') IS NOT NULL
BEGIN
    ALTER TABLE ProdSklad
    ADD CONSTRAINT FK_ProdSklad_Prodavac
    FOREIGN KEY (idProdavac) REFERENCES Prodavac(idProdavac)
    ON DELETE CASCADE;
    PRINT 'Kreirano ograničenje: ProdSklad → Prodavac (CASCADE DELETE)';
END
GO

-- FK: ProdSklad → Skladiste (ako tabela postoji) (RESTRICT)
IF OBJECT_ID('ProdSklad', 'U') IS NOT NULL
BEGIN
    ALTER TABLE ProdSklad
    ADD CONSTRAINT FK_ProdSklad_Skladiste
    FOREIGN KEY (idSkladiste) REFERENCES Skladiste(idSkladiste)
    ON DELETE RESTRICT
    ON UPDATE RESTRICT;
    PRINT 'Kreirano ograničenje: ProdSklad → Skladiste (RESTRICT)';
END
GO

-- ===== TREĆE: Dodaj CHECK ograničenja za validaciju podataka =====

-- Check za Kupac.popust (mora biti >= 0)
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Kupac_popust')
BEGIN
    ALTER TABLE Kupac
    ADD CONSTRAINT CK_Kupac_popust CHECK (popust >= 0);
    PRINT 'Dodano CHECK ograničenje: Kupac.popust >= 0';
END
GO

-- Check za Oprema.cena (mora biti > 0)
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Oprema_cena')
BEGIN
    ALTER TABLE Oprema
    ADD CONSTRAINT CK_Oprema_cena CHECK (cena > 0);
    PRINT 'Dodano CHECK ograničenje: Oprema.cena > 0';
END
GO

-- Check za StavkaRacuna.kolicina (mora biti > 0)
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_StavkaRacuna_kolicina')
BEGIN
    ALTER TABLE StavkaRacuna
    ADD CONSTRAINT CK_StavkaRacuna_kolicina CHECK (kolicina > 0);
    PRINT 'Dodano CHECK ograničenje: StavkaRacuna.kolicina > 0';
END
GO

-- Check za StavkaRacuna.cena (mora biti > 0)
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_StavkaRacuna_cena')
BEGIN
    ALTER TABLE StavkaRacuna
    ADD CONSTRAINT CK_StavkaRacuna_cena CHECK (cena > 0);
    PRINT 'Dodano CHECK ograničenje: StavkaRacuna.cena > 0';
END
GO

PRINT '===============================================';
PRINT 'Sva strukturna ograničenja su uspešno kreirana!';
PRINT '===============================================';
PRINT '';
PRINT 'Ograničenja:';
PRINT '- Kupac se NE može obrisati ako ima Račune (RESTRICT)';
PRINT '- Prodavac se NE može obrisati ako ima Račune (RESTRICT)';
PRINT '- Oprema se NE može obrisati ako je u StavkaRacuna (RESTRICT)';
PRINT '- Skladište se NE može obrisati ako je u ProdSklad (RESTRICT)';
PRINT '- Firma se AUTOMATSKI briše ako se obriše Kupac (CASCADE)';
PRINT '- FizickoLice se AUTOMATSKI briše ako se obriše Kupac (CASCADE)';
PRINT '- StavkaRacuna se AUTOMATSKI briše ako se obriše Račun (CASCADE)';
