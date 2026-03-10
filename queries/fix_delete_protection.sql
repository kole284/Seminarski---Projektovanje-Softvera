-- Ispravka za sprečavanje brisanja zaštićenih zapisa
-- Problem: Korisnici su mogli brisati FizickoLice/Firma direktno čak i ako imaju Racune
-- Rješenje: Dodati CHECK constraint koji sprječava brisanje ako Kupac ima Racune

USE ProdavnicaDB;
GO

PRINT '=================================================';
PRINT 'DODAVANJE ZAŠTITE OD BRISANJA';
PRINT '=================================================';
GO

-- Kreiramo pogled koji kombinuje sve podatke za lak pristup
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_KupacSaRacunima')
    DROP VIEW vw_KupacSaRacunima;
GO

CREATE VIEW vw_KupacSaRacunima AS
SELECT 
    k.idKupac,
    COUNT(r.idRacun) AS BrojRacuna,
    CASE WHEN COUNT(r.idRacun) > 0 THEN 1 ELSE 0 END AS ImaRacune
FROM Kupac k
LEFT JOIN Racun r ON k.idKupac = r.idKupac
GROUP BY k.idKupac;
GO

PRINT 'Kreiran pogled vw_KupacSaRacunima - kombinuje Kupac sa brojem Racuna';
GO

-- Kreiramo trigger na FizickoLice tabeli da sprječava brisanje ako Kupac ima Racune
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'tr_FizickoLice_PreventDelete')
    DROP TRIGGER tr_FizickoLice_PreventDelete;
GO

CREATE TRIGGER tr_FizickoLice_PreventDelete
ON FizickoLice
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Proveravamo da li neki od obrisanih FizickoLice imaju parent Kupac sa Racunima
    IF EXISTS (
        SELECT 1 FROM deleted d
        JOIN vw_KupacSaRacunima k ON d.idKupac = k.idKupac
        WHERE k.ImaRacune = 1
    )
    BEGIN
        RAISERROR('Ne možete obrisati fizičko lice jer ima povezane račune u sistemu!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    -- Ako je sve OK, brišemo i FizickoLice i parent Kupac
    DELETE FROM FizickoLice WHERE idKupac IN (SELECT idKupac FROM deleted);
    DELETE FROM Kupac WHERE idKupac IN (SELECT idKupac FROM deleted);
END
GO

PRINT 'Kreiran trigger tr_FizickoLice_PreventDelete - sprječava brisanje FizickoLice sa Racunima';
GO

-- Kreiramo trigger na Firma tabeli da sprječava brisanje ako Kupac ima Racune
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'tr_Firma_PreventDelete')
    DROP TRIGGER tr_Firma_PreventDelete;
GO

CREATE TRIGGER tr_Firma_PreventDelete
ON Firma
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Proveravamo da li neki od obrisanih Firma imaju parent Kupac sa Racunima
    IF EXISTS (
        SELECT 1 FROM deleted d
        JOIN vw_KupacSaRacunima k ON d.idKupac = k.idKupac
        WHERE k.ImaRacune = 1
    )
    BEGIN
        RAISERROR('Ne možete obrisati firmu jer ima povezane račune u sistemu!', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    -- Ako je sve OK, brišemo i Firma i parent Kupac
    DELETE FROM Firma WHERE idKupac IN (SELECT idKupac FROM deleted);
    DELETE FROM Kupac WHERE idKupac IN (SELECT idKupac FROM deleted);
END
GO

PRINT 'Kreiran trigger tr_Firma_PreventDelete - sprječava brisanje Firma sa Racunima';
GO

PRINT '';
PRINT '=================================================';
PRINT '✓ ZAŠTITA OD BRISANJA USPJEŠNO INSTALIRANA!';
PRINT '=================================================';
PRINT '';
PRINT 'Sada je nemoguće obrisati:';
PRINT '  ✓ FizickoLice ako Kupac ima Racune';
PRINT '  ✓ Firma ako Kupac ima Racune';
PRINT '  ✓ Kupac ako ima Racune (FK_Racun_Kupac)';
PRINT '  ✓ Prodavac ako ima Racune (FK_Racun_Prodavac)';
PRINT '  ✓ Oprema ako je u StavkaRacuna (FK_StavkaRacuna_Oprema)';
PRINT '';
