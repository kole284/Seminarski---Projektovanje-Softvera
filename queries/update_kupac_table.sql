-- SQL skripta za ažuriranje tabele Kupac da podržava Firme i Fizička lica
-- Dodajemo nove kolone i tip kupca

USE ProdavnicaDB;
GO

-- Dodajemo kolone za specijalizaciju Kupac tabele
ALTER TABLE Kupac
ADD 
    tipKupca VARCHAR(20) NULL,  -- 'Firma' ili 'FizickoLice'
    -- Polja za Firmu
    pib VARCHAR(20) NULL,
    adresa VARCHAR(200) NULL,
    partnerstvo BIT NULL,
    -- Polja za Fizičko lice
    imePrezime VARCHAR(100) NULL,
    email VARCHAR(100) NULL,
    telefon VARCHAR(20) NULL,
    loyaltyClan BIT NULL;
GO

-- Ažuriramo postojeće kupce da imaju default tip (ako postoje)
UPDATE Kupac
SET tipKupca = 'Firma'
WHERE tipKupca IS NULL;
GO

PRINT 'Tabela Kupac je uspešno ažurirana sa novim kolonama.';
