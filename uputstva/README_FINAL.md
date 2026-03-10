# ISPRAVLJENA STRUKTURA - Table per Type Inheritance

## Šta je ispravljeno

**VAŽNO**: Firma i FizickoLice **NE TREBA** da imaju svoje ID-ove. One su specijalizacije Kupac tabele i koriste `idKupac` kao primarni ključ.

### Nova struktura baze

```
Kupac
-----
idKupac (PK)
popust

Firma                         FizickoLice
--------------------------    ---------------------------
idKupac (PK + FK → Kupac)    idKupac (PK + FK → Kupac)
naziv                         naziv
pib                           imePrezime
adresa                        email
partnerstvo                   telefon
                              loyaltyClan
```

## Koraci za ispravku

### 1. Pokreni SQL skripte po redosledu:

```sql
-- PRVO: Kreiraj ispravnu strukturu
-- Pokreni: create_correct_structure.sql
-- Ova skripta briše stare tabele i kreira nove sa ispravnom strukturom

-- DRUGO: Kreiraj stored procedures
-- Pokreni: create_stored_procedures.sql
-- SP koriste samo idKupac, bez IdFirma/IdFizickoLice

-- TREĆE: Dodaj test podatke (opciono)
-- Pokreni: insert_test_data.sql
```

### 2. Rebuild solution

**Build → Rebuild Solution**

### 3. Testiraj

1. **Dodavanje**:
   - Otvori Add formu
   - Izaberi "Kupac - Firma" ili "Kupac - Fizičko lice"
   - Popuni podatke i sačuvaj

2. **Prikaz svih kupaca**:
   - Iz ComboBox-a izaberi **"Kupci"**
   - Klikni "Prikaži"
   - Videćeš sve kupce (i firme i fizička lica) u jednom DataGridView-u
   - Kolona "Tip" pokazuje da li je Firma ili Fizičko lice

3. **Detalji i ažuriranje**:
   - Double-click na bilo koji red
   - Otvoriće se Details forma
   - Klikni "Ažuriraj" za izmenu

## Izmene u kodu

### Models
- **Firma.cs**: Samo `IdKupac` (bez IdFirma), `WhereCondition => idKupac = {IdKupac}`
- **FizickoLice.cs**: Samo `IdKupac` (bez IdFizickoLice), `WhereCondition => idKupac = {IdKupac}`
- **KupacView.cs**: Novi model za kombinovani prikaz svih kupaca

### Broker
- **DatabaseBroker.cs**: 
  - `VratiSve()` JOIN-uje Firma/FizickoLice sa Kupac tabelom da dobije popust
  - `Dodaj()` stored procedures vraćaju samo IdKupac
  - `Azuriraj()` stored procedures koriste samo idKupac

### Client
- **Main.cs**: 
  - Opcija "Kupci" prikazuje sve kupce zajedno
  - `PrikaziSveKupce()` kombinuje Firme i Fizička lica u KupacView
  - DataGridView event handler konvertuje KupacView u Firma/FizickoLice za Details
- **Add.cs**: Kreira objekte samo sa IdKupac (bez IdFirma/IdFizickoLice)
- **Details.cs**: Prikazuje samo IdKupac

## Prednosti ovog pristupa

✅ **Ispravna specijalizacija**: Firma i FizickoLice dele isti ID prostor sa Kupac  
✅ **Jedinstveni prikaz**: Svi kupci u jednom DataGridView-u  
✅ **Lako razlikovanje**: Kolona "Tip" pokazuje da li je Firma ili Fizičko lice  
✅ **Integritet podataka**: Cascade delete automatski briše specijalizaciju kad se obriše Kupac  
✅ **Jednostavno JOIN-ovanje**: Jednostavno je spojiti sa drugim tabelama preko idKupac

## Napomene

- **idKupac** je i primarni ključ (PK) i strani ključ (FK) u Firma i FizickoLice tabelama
- Jedan Kupac može biti **ili** Firma **ili** Fizičko lice, **ali ne oba**
- Stored procedures koriste transakcije za integritet
- Popust se čuva u Kupac tabeli jer je zajednički za oba tipa
