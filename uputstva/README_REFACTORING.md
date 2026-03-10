# Refaktorisanje Kupac, Firma i FizickoLice tabela

## Pregled izmena

Sistem je refaktorisan tako da:
- **Kupac** tabela sadrži samo: `idKupac` (PK), `popust`
- **Firma** tabela: `idFirma` (PK), `idKupac` (FK), `naziv`, `pib`, `adresa`, `partnerstvo`
- **FizickoLice** tabela: `idFizickoLice` (PK), `idKupac` (FK), `naziv`, `imePrezime`, `email`, `telefon`, `loyaltyClan`

## Koraci za ažuriranje baze podataka

### 1. Izvršite glavnu refaktorisanje skriptu
```sql
-- Pokreni: refactor_kupac_tables.sql
```
Ova skripta će:
- Kreirati tabele `Firma` i `FizickoLice`
- Migrirati postojeće podatke iz `Kupac` tabele
- Obrisati stare kolone iz `Kupac` tabele

### 2. Kreirajte stored procedures
```sql
-- Pokreni: create_stored_procedures.sql
```
Ova skripta će kreirati:
- `DodajFirmu` - dodaje Kupac zapis i Firma zapis u transakciji
- `DodajFizickoLice` - dodaje Kupac zapis i FizickoLice zapis u transakciji
- `AzurirajFirmu` - ažurira oba zapisa
- `AzurirajFizickoLice` - ažurira oba zapisa

### 3. (Opciono) Dodajte test podatke
```sql
-- Pokreni: insert_test_data.sql
```

## Izmene u kodu

### Models
- **Firma.cs**: Dodato `IdFirma`, `Popust`, `TableName => "Firma"`
- **FizickoLice.cs**: Dodato `IdFizickoLice`, `Popust`, `TableName => "FizickoLice"`

### Broker
- **DatabaseBroker.cs**: 
  - `Dodaj()` metoda koristi stored procedures za Firma/FizickoLice
  - `Azuriraj()` metoda koristi stored procedures za Firma/FizickoLice
  - `VratiSve()` metoda radi direktno sa tabelama

### Server
- **Controller.cs**: Ažurirano prepoznavanje tipova po `TableName` umesto po poljima

### Client
- **Main.cs**: Dodato "Firme" i "Fizička lica" u ComboBox, ažurirano deserijalizaciju
- **Add.cs**: Ažurirano kreiranje objekata sa `IdFirma`/`IdFizickoLice` i `Popust` poljem
- **Details.cs**: Ažurirano prikazivanje svih polja

## Testiranje

1. **Rebuild solution** (Build → Rebuild Solution)
2. **Pokreni Server i Client**
3. **Test dodavanje**:
   - Izaberi "Kupac - Firma", unesi podatke, sačuvaj
   - Izaberi "Kupac - Fizičko lice", unesi podatke, sačuvaj
4. **Test prikaz**:
   - Izaberi "Firme" iz dropdown-a, klikni "Prikaži"
   - Izaberi "Fizička lica" iz dropdown-a, klikni "Prikaži"
5. **Test ažuriranje**:
   - Double-click na red da otvoriš Details
   - Klikni "Ažuriraj", izmeni podatke, sačuvaj

## Struktura baze nakon refaktorisanja

```
Kupac                 Firma                    FizickoLice
------------          -----------------        -------------------
idKupac (PK)          idFirma (PK)             idFizickoLice (PK)
popust                idKupac (FK)             idKupac (FK)
                      naziv                    naziv
                      pib                      imePrezime
                      adresa                   email
                      partnerstvo              telefon
                                              loyaltyClan
```

## Napomene

- Stored procedures koriste transakcije za integritet podataka
- Cascade delete je omogućen na Foreign Key vezama
- Popust se čuva u `Kupac` tabeli jer je zajednički za oba tipa
- `naziv` je duplikiran u `Firma` i `FizickoLice` tabelama za lakši pristup
