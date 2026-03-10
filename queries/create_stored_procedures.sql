-- Stored procedure za dodavanje Firme
-- Prvo kreira Kupac zapis, zatim Firma zapis

USE ProdavnicaDB;
GO

-- SP za dodavanje Firme
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'DodajFirmu')
    DROP PROCEDURE DodajFirmu;
GO

CREATE PROCEDURE DodajFirmu
    @naziv VARCHAR(100),
    @popust FLOAT,
    @pib VARCHAR(20),
    @adresa VARCHAR(200),
    @partnerstvo BIT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @idKupac INT;
    
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Prvo kreiramo Kupac zapis
        INSERT INTO Kupac (popust)
        VALUES (@popust);
        
        SET @idKupac = SCOPE_IDENTITY();
        
        -- Zatim kreiramo Firma zapis sa istim ID-em
        INSERT INTO Firma (idKupac, naziv, pib, adresa, partnerstvo)
        VALUES (@idKupac, @naziv, @pib, @adresa, @partnerstvo);
        
        COMMIT TRANSACTION;
        SELECT @idKupac AS IdKupac;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- SP za dodavanje FizickoLice
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'DodajFizickoLice')
    DROP PROCEDURE DodajFizickoLice;
GO

CREATE PROCEDURE DodajFizickoLice
    @popust FLOAT,
    @imePrezime VARCHAR(100),
    @email VARCHAR(100),
    @telefon VARCHAR(20),
    @loyaltyClan BIT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @idKupac INT;
    
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Prvo kreiramo Kupac zapis
        INSERT INTO Kupac (popust)
        VALUES (@popust);
        
        SET @idKupac = SCOPE_IDENTITY();
        
        -- Zatim kreiramo FizickoLice zapis sa istim ID-em
        INSERT INTO FizickoLice (idKupac, imePrezime, email, telefon, loyaltyClan)
        VALUES (@idKupac, @imePrezime, @email, @telefon, @loyaltyClan);
        
        COMMIT TRANSACTION;
        SELECT @idKupac AS IdKupac;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- SP za ažuriranje Firme
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'AzurirajFirmu')
    DROP PROCEDURE AzurirajFirmu;
GO

CREATE PROCEDURE AzurirajFirmu
    @idKupac INT,
    @naziv VARCHAR(100),
    @popust FLOAT,
    @pib VARCHAR(20),
    @adresa VARCHAR(200),
    @partnerstvo BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Ažuriramo Kupac zapis
        UPDATE Kupac
        SET popust = @popust
        WHERE idKupac = @idKupac;
        
        -- Ažuriramo Firma zapis
        UPDATE Firma
        SET naziv = @naziv, pib = @pib, adresa = @adresa, partnerstvo = @partnerstvo
        WHERE idKupac = @idKupac;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- SP za ažuriranje FizickoLice
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'AzurirajFizickoLice')
    DROP PROCEDURE AzurirajFizickoLice;
GO

CREATE PROCEDURE AzurirajFizickoLice
    @idFizickoLice INT,
    @idKupac INT,
    @popust FLOAT,
    @imePrezime VARCHAR(100),
    @email VARCHAR(100),
    @telefon VARCHAR(20),
    @loyaltyClan BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Ažuriramo Kupac zapis
        UPDATE Kupac
        SET popust = @popust
        WHERE idKupac = @idKupac;
        
        -- Ažuriramo FizickoLice zapis
        UPDATE FizickoLice
        SET imePrezime = @imePrezime, email = @email, telefon = @telefon, loyaltyClan = @loyaltyClan
        WHERE idKupac = @idKupac;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

PRINT 'Stored procedures su kreirane uspešno.';
