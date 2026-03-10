-- SQL skripta za ispravljanje SVIH FK constraint-a vezanih za Prodavac tabelu
-- Omogućava brisanje prodavca bez konflikta

USE ProdavnicaDB;
GO

PRINT '===========================================';
PRINT 'ISPRAVKA FK CONSTRAINT-A ZA PRODAVAC TABELU';
PRINT '===========================================';

-- ==================================================
-- 1. ISPRAVI FK: Prodavac -> Skladiste (SET NULL)
-- ==================================================
PRINT '';
PRINT '1. Ispravljam FK_Prodavac_Skladiste...';

DECLARE @fkProdavacSkladiste NVARCHAR(128);

SELECT @fkProdavacSkladiste = fk.name
FROM sys.foreign_keys AS fk
WHERE fk.parent_object_id = OBJECT_ID('Prodavac')
AND fk.referenced_object_id = OBJECT_ID('Skladiste');

IF @fkProdavacSkladiste IS NOT NULL
BEGIN
    DECLARE @sql1 NVARCHAR(MAX);
    SET @sql1 = 'ALTER TABLE Prodavac DROP CONSTRAINT ' + QUOTENAME(@fkProdavacSkladiste) + ';';
    EXEC sp_executesql @sql1;
    PRINT '   - Obrisan stari constraint: ' + @fkProdavacSkladiste;
    
    -- Prvo promeni kolonu da može biti NULL
    ALTER TABLE Prodavac
    ALTER COLUMN idSkladiste INT NULL;
    PRINT '   - Kolona idSkladiste je sada nullable';
    
    -- Dodaj novi sa SET NULL (kada se obriše skladište, prodavac ostaje ali bez skladišta)
    ALTER TABLE Prodavac
    ADD CONSTRAINT FK_Prodavac_Skladiste
    FOREIGN KEY (idSkladiste) REFERENCES Skladiste(idSkladiste) ON DELETE SET NULL;
    
    PRINT '   - Kreiran novi constraint sa ON DELETE SET NULL';
    PRINT '   ✓ FK_Prodavac_Skladiste ispravljeno!';
END
ELSE
BEGIN
    PRINT '   ! FK_Prodavac_Skladiste ne postoji.';
END
GO

-- ==================================================
-- 2. ISPRAVI FK: ProdSklad -> Prodavac (CASCADE)
-- ==================================================
PRINT '';
PRINT '2. Ispravljam FK_ProdSklad_Prodavac...';

DECLARE @fkProdSklad NVARCHAR(128);

SELECT @fkProdSklad = fk.name
FROM sys.foreign_keys AS fk
WHERE fk.parent_object_id = OBJECT_ID('ProdSklad')
AND fk.referenced_object_id = OBJECT_ID('Prodavac');

IF @fkProdSklad IS NOT NULL
BEGIN
    DECLARE @sql2 NVARCHAR(MAX);
    SET @sql2 = 'ALTER TABLE ProdSklad DROP CONSTRAINT ' + QUOTENAME(@fkProdSklad) + ';';
    EXEC sp_executesql @sql2;
    PRINT '   - Obrisan stari constraint: ' + @fkProdSklad;
    
    -- Dodaj novi sa CASCADE (kada se obriše prodavac, obriši i njegove ProdSklad zapise)
    ALTER TABLE ProdSklad
    ADD CONSTRAINT FK_ProdSklad_Prodavac
    FOREIGN KEY (idProdavac) REFERENCES Prodavac(idProdavac) ON DELETE CASCADE;
    
    PRINT '   - Kreiran novi constraint sa ON DELETE CASCADE';
    PRINT '   ✓ FK_ProdSklad_Prodavac ispravljeno!';
END
ELSE
BEGIN
    PRINT '   ! ProdSklad tabela ili FK ne postoji.';
END
GO

-- ==================================================
-- 3. ISPRAVI FK: Racun -> Prodavac (SET NULL ili CASCADE?)
-- ==================================================
PRINT '';
PRINT '3. Proveravam FK_Racun_Prodavac...';

DECLARE @fkRacunProdavac NVARCHAR(128);

SELECT @fkRacunProdavac = fk.name
FROM sys.foreign_keys AS fk
WHERE fk.parent_object_id = OBJECT_ID('Racun')
AND fk.referenced_object_id = OBJECT_ID('Prodavac');

IF @fkRacunProdavac IS NOT NULL
BEGIN
    DECLARE @sql3 NVARCHAR(MAX);
    SET @sql3 = 'ALTER TABLE Racun DROP CONSTRAINT ' + QUOTENAME(@fkRacunProdavac) + ';';
    EXEC sp_executesql @sql3;
    PRINT '   - Obrisan stari constraint: ' + @fkRacunProdavac;
    
    -- Prvo promeni kolonu da može biti NULL
    ALTER TABLE Racun
    ALTER COLUMN idProdavac INT NULL;
    PRINT '   - Kolona idProdavac je sada nullable';
    
    -- Za Racun: SET NULL je bezbednija opcija (čuva račune, ali bez prodavca)
    -- Alternativa: CASCADE bi obrisalo sve račune prodavca
    ALTER TABLE Racun
    ADD CONSTRAINT FK_Racun_Prodavac
    FOREIGN KEY (idProdavac) REFERENCES Prodavac(idProdavac) ON DELETE SET NULL;
    
    PRINT '   - Kreiran novi constraint sa ON DELETE SET NULL';
    PRINT '   ✓ FK_Racun_Prodavac ispravljeno!';
    PRINT '   NAPOMENA: Računi prodavca će OSTATI u bazi, ali bez idProdavac vrednosti.';
END
ELSE
BEGIN
    PRINT '   ! FK_Racun_Prodavac ne postoji.';
END
GO

-- ==================================================
-- 4. PROVERA - Prikaz svih FK constraint-a
-- ==================================================
PRINT '';
PRINT '===========================================';
PRINT 'FINALNA PROVERA - FK CONSTRAINT-I:';
PRINT '===========================================';
PRINT '';

-- FK constraint-i KA Prodavac tabeli (ko referencira Prodavac)
PRINT 'FK constraint-i ka Prodavac tabeli:';
SELECT 
    OBJECT_NAME(fk.parent_object_id) AS [Tabela],
    fk.name AS [Constraint],
    COL_NAME(fc.parent_object_id, fc.parent_column_id) AS [Kolona],
    delete_referential_action_desc AS [ON DELETE]
FROM sys.foreign_keys AS fk
INNER JOIN sys.foreign_key_columns AS fc ON fk.object_id = fc.constraint_object_id
WHERE fk.referenced_object_id = OBJECT_ID('Prodavac');

PRINT '';

-- FK constraint-i IZ Prodavac tabele (što Prodavac referencira)
PRINT 'FK constraint-i iz Prodavac tabele:';
SELECT 
    OBJECT_NAME(fk.referenced_object_id) AS [Referencirana_Tabela],
    fk.name AS [Constraint],
    COL_NAME(fc.parent_object_id, fc.parent_column_id) AS [Kolona],
    delete_referential_action_desc AS [ON DELETE]
FROM sys.foreign_keys AS fk
INNER JOIN sys.foreign_key_columns AS fc ON fk.object_id = fc.constraint_object_id
WHERE fk.parent_object_id = OBJECT_ID('Prodavac');

PRINT '';
PRINT '===========================================';
PRINT 'GOTOVO! Sada možeš slobodno brisati prodavca.';
PRINT '===========================================';
GO
