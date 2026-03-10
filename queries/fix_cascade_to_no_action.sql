-- Ispravka constraint-a: Promjena FK_FizickoLice_Kupac i FK_Firma_Kupac sa CASCADE na NO ACTION
-- Problem: CASCADE DELETE omogućava kaskadno brisanje što sprječava primjenu INSTEAD OF trigger-a
-- Rješenje: Promijeniti na NO ACTION + trigger za ručno upravljanje brisanjem

USE ProdavnicaDB;
GO

PRINT '=================================================';
PRINT 'PROMJENA CONSTRAINT-a ZA ZAŠTITU OD BRISANJA';
PRINT '=================================================';
PRINT '';

-- Prvo, obrišemo stare constraint-e
ALTER TABLE FizickoLice DROP CONSTRAINT FK_FizickoLice_Kupac;
ALTER TABLE Firma DROP CONSTRAINT FK_Firma_Kupac;
PRINT '✓ Obrisani stari constraint-i FK_FizickoLice_Kupac i FK_Firma_Kupac';
GO

-- Kreiramo nove constraint-e sa NO ACTION
ALTER TABLE FizickoLice
ADD CONSTRAINT FK_FizickoLice_Kupac 
FOREIGN KEY (idKupac) REFERENCES Kupac(idKupac) ON DELETE NO ACTION;

ALTER TABLE Firma
ADD CONSTRAINT FK_Firma_Kupac 
FOREIGN KEY (idKupac) REFERENCES Kupac(idKupac) ON DELETE NO ACTION;

PRINT '✓ Kreirani novi constraint-i sa NO ACTION';
GO

-- Kreiramo pogled koji pokazuje Kupce sa njihovim Racunima
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_KupacSaRacunima')
    DROP VIEW vw_KupacSaRacunima;
GO

CREATE VIEW vw_KupacSaRacunima AS
SELECT 
    k.idKupac,
    COUNT(r.idRacun) AS BrojRacuna
FROM Kupac k
LEFT JOIN Racun r ON k.idKupac = r.idKupac
GROUP BY k.idKupac;
GO

PRINT '✓ Kreiran pogled vw_KupacSaRacunima';
GO

-- Kreiramo trigger na Kupac tabeli da sprječava brisanje ako ima Racune
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'tr_Kupac_PreventDelete')
    DROP TRIGGER tr_Kupac_PreventDelete;
GO

CREATE TRIGGER tr_Kupac_PreventDelete
ON Kupac
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Proveravamo da li neki od obrisanih Kupaca imaju Racune
    DECLARE @KupacSaRacunima INT;
    
    SELECT @KupacSaRacunima = COUNT(*)
    FROM deleted d
    WHERE EXISTS (
        SELECT 1 FROM Racun r WHERE r.idKupac = d.idKupac
    );
    
    IF @KupacSaRacunima > 0
    BEGIN
        RAISERROR('Ne možete obrisati kupca jer ima povezane račune u sistemu! Prvo obrišite sve račune tog kupca.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    -- Ako je sve OK, brišemo Kupac (što će kaskadno obrisati FizickoLice/Firma jer ćemo prvo obrisati one)
    -- Prvo obrisemo FizickoLice i Firma zapise
    DELETE FROM FizickoLice WHERE idKupac IN (SELECT idKupac FROM deleted);
    DELETE FROM Firma WHERE idKupac IN (SELECT idKupac FROM deleted);
    
    -- Zatim brišemo Kupac zapise
    DELETE FROM Kupac WHERE idKupac IN (SELECT idKupac FROM deleted);
END
GO

PRINT '✓ Kreiran trigger tr_Kupac_PreventDelete';
GO

PRINT '';
PRINT '=================================================';
PRINT '✓ ZAŠTITA OD BRISANJA USPJEŠNO IMPLEMENTIRANA!';
PRINT '=================================================';
PRINT '';
PRINT 'Sada je nemoguće obrisati:';
PRINT '  ✓ Kupac ako ima Racune (NO ACTION constraint + trigger)';
PRINT '  ✓ Prodavac ako ima Racune (NO ACTION constraint)';
PRINT '  ✓ Oprema ako je u StavkaRacuna (NO ACTION constraint)';
PRINT '  ✓ Skladiste ako je u ProdSklad (NO ACTION constraint)';
PRINT '';
PRINT 'Kada obrisite Kupac koji nema Racune, automatski će se obrisati i FizickoLice/Firma.';
PRINT '';
