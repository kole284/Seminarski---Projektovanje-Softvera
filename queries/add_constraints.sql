-- SQL skripta za dodavanje Foreign Key ograničenja sa NO ACTION
-- Ova ograničenja sprječavaju brisanje objekata ako su oni referencirani gdje drugdje
-- Korisnik će dobiti grešku ako pokušala da obriše kupca koji ima račune

USE ProdavnicaDB;
GO

PRINT '';
PRINT '===============================================';
PRINT 'Dodavanje Foreign Key ograničenja...';
PRINT '===============================================';
PRINT '';

-- Prvo provjeravamo da li već postoje FK constraints i brišemo ih ako postoje
-- Racun tabela
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Racun_Kupac')
BEGIN
    ALTER TABLE Racun DROP CONSTRAINT FK_Racun_Kupac;
    PRINT 'Obrisano staro ograničenje FK_Racun_Kupac';
END
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Racun_Prodavac')
BEGIN
    ALTER TABLE Racun DROP CONSTRAINT FK_Racun_Prodavac;
    PRINT 'Obrisano staro ograničenje FK_Racun_Prodavac';
END
GO

-- StavkaRacuna tabela
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_StavkaRacuna_Oprema')
BEGIN
    ALTER TABLE StavkaRacuna DROP CONSTRAINT FK_StavkaRacuna_Oprema;
    PRINT 'Obrisano staro ograničenje FK_StavkaRacuna_Oprema';
END
GO

-- ProdSklad tabela
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ProdSklad_Skladiste')
BEGIN
    ALTER TABLE ProdSklad DROP CONSTRAINT FK_ProdSklad_Skladiste;
    PRINT 'Obrisano staro ograničenje FK_ProdSklad_Skladiste';
END
GO

PRINT '';
PRINT 'Dodavanje NOVIH Foreign Key ograničenja sa NO ACTION...';
PRINT '';

-- 1. FK: Racun -> Kupac (NO ACTION - ne dozvoli brisanje Kupca ako ima računa)
ALTER TABLE Racun
ADD CONSTRAINT FK_Racun_Kupac 
FOREIGN KEY (idKupac) REFERENCES Kupac(idKupac) 
ON DELETE NO ACTION ON UPDATE NO ACTION;
PRINT '✓ Dodano ograničenje FK_Racun_Kupac: Ne možeš obrisati Kupca ako ima račune!';

-- 2. FK: Racun -> Prodavac (NO ACTION - ne dozvoli brisanje Prodavca ako ima izdanih računa)
ALTER TABLE Racun
ADD CONSTRAINT FK_Racun_Prodavac 
FOREIGN KEY (idProdavac) REFERENCES Prodavac(idProdavac) 
ON DELETE NO ACTION ON UPDATE NO ACTION;
PRINT '✓ Dodano ograničenje FK_Racun_Prodavac: Ne možeš obrisati Prodavca ako ima račune!';

-- 3. FK: StavkaRacuna -> Oprema (NO ACTION - ne dozvoli brisanje Opreme ako je u stavkama računa)
ALTER TABLE StavkaRacuna
ADD CONSTRAINT FK_StavkaRacuna_Oprema 
FOREIGN KEY (idOprema) REFERENCES Oprema(idOprema) 
ON DELETE NO ACTION ON UPDATE NO ACTION;
PRINT '✓ Dodano ograičenje FK_StavkaRacuna_Oprema: Ne možeš obrisati Opremu ako je u stavkama!';

-- 4. FK: ProdSklad -> Skladiste (NO ACTION - ne dozvoli brisanje Skladista ako je dodjeljeno nekom prodavcu)
ALTER TABLE ProdSklad
ADD CONSTRAINT FK_ProdSklad_Skladiste 
FOREIGN KEY (idSkladiste) REFERENCES Skladiste(idSkladiste) 
ON DELETE NO ACTION ON UPDATE NO ACTION;
PRINT '✓ Dodano ograničenje FK_ProdSklad_Skladiste: Ne možeš obrisati Skladište ako je u upotrebi!';

PRINT '';
PRINT '===============================================';
PRINT 'OGRANIČENJA USPJEŠNO DODANA!';
PRINT '===============================================';
PRINT '';
PRINT 'Što je sada zaštićeno:';
PRINT '1. KUPAC - ne možeš obrisati ako ima račune';
PRINT '2. PRODAVAC - ne možeš obrisati ako ima izdanih računa';
PRINT '3. OPREMA - ne možeš obrisati ako je u stavkama računa';
PRINT '4. SKLADIŠTE - ne možeš obrisati ako je dodjeljeno prodavcu';
PRINT '';
