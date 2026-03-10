-- FINALNA ISPRAVKA: Trigger-i direktno na FizickoLice i Firma tabelama
-- Problem: DatabaseBroker briše direktno iz FizickoLice/Firma, ne kroz Kupac
-- Rješenje: INSTEAD OF DELETE trigger na svakoj tabeli koja provjerava parent Kupac

USE ProdavnicaDB;
GO

PRINT '================================================';
PRINT 'PRIMJENA DIREKTNIH TRIGGER-A NA FIZICKOLICE/FIRMA';
PRINT '================================================';
GO

-- Prvo obrišemo stari trigger sa Kupac
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'tr_Kupac_PreventDelete')
    DROP TRIGGER tr_Kupac_PreventDelete;
GO

PRINT 'Obrisan stari trigger sa Kupac tabele';
GO

-- Kreiramo trigger na FizickoLice koji sprječava brisanje ako Kupac ima Racune
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'tr_FizickoLice_PreventDelete')
    DROP TRIGGER tr_FizickoLice_PreventDelete;
GO

CREATE TRIGGER tr_FizickoLice_PreventDelete
ON FizickoLice
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @KupacSaRacunima INT;
    
    -- Proveravamo da li neki od obrisanih FizickoLice imaju parent Kupac sa Racunima
    SELECT @KupacSaRacunima = COUNT(*)
    FROM deleted d
    WHERE EXISTS (
        SELECT 1 FROM Racun r WHERE r.idKupac = d.idKupac
    );
    
    IF @KupacSaRacunima > 0
    BEGIN
        RAISERROR('Ne možete obrisati fizičko lice jer ima povezane račune u sistemu!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    -- Ako je sve OK, brišemo FizickoLice, pa onda parent Kupac
    DELETE FROM FizickoLice WHERE idKupac IN (SELECT idKupac FROM deleted);
    DELETE FROM Kupac WHERE idKupac IN (SELECT idKupac FROM deleted);
END
GO

PRINT '✓ Kreiran trigger tr_FizickoLice_PreventDelete';
GO

-- Kreiramo trigger na Firma koji sprječava brisanje ako Kupac ima Racune
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'tr_Firma_PreventDelete')
    DROP TRIGGER tr_Firma_PreventDelete;
GO

CREATE TRIGGER tr_Firma_PreventDelete
ON Firma
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @KupacSaRacunima INT;
    
    -- Proveravamo da li neki od obrisanih Firma imaju parent Kupac sa Racunima
    SELECT @KupacSaRacunima = COUNT(*)
    FROM deleted d
    WHERE EXISTS (
        SELECT 1 FROM Racun r WHERE r.idKupac = d.idKupac
    );
    
    IF @KupacSaRacunima > 0
    BEGIN
        RAISERROR('Ne možete obrisati firmu jer ima povezane račune u sistemu!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    -- Ako je sve OK, brišemo Firma, pa onda parent Kupac
    DELETE FROM Firma WHERE idKupac IN (SELECT idKupac FROM deleted);
    DELETE FROM Kupac WHERE idKupac IN (SELECT idKupac FROM deleted);
END
GO

PRINT '✓ Kreiran trigger tr_Firma_PreventDelete';
GO

PRINT '';
PRINT '================================================';
PRINT '✓ ZAŠTITA USPJEŠNO PRIMJENJENA!';
PRINT '================================================';
PRINT '';
PRINT 'Sada je nemoguće obrisati:';
PRINT '  ✓ FizickoLice ako Kupac ima Racune (TRIGGER)';
PRINT '  ✓ Firma ako Kupac ima Racune (TRIGGER)';
PRINT '  ✓ Kupac ako ima Racune (FK NO ACTION)';
PRINT '  ✓ Prodavac ako ima Racune (FK NO ACTION)';
PRINT '  ✓ Oprema ako je u StavkaRacuna (FK NO ACTION)';
PRINT '';
