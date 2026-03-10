# ✓ ZAŠTITA OD BRISANJA - FINALNO RJEŠENJE

## Problem
- Korisnici su mogli brisati FizickoLice, Firma, i Kupac zapise čak i ako su imali povezane Racune
- Sistem je dozvoljavao brisanje objekata sa FK ograničenjima

## Root Cause
- FK_FizickoLice_Kupac i FK_Firma_Kupac su bili sa `CASCADE DELETE`
- Ovo je sprječavalo primjenu INSTEAD OF trigger-a koji bi dao precizniju kontrolu
- Cuando korisnik briše FizickoLice direktno, nije aktiviran trigger na Kupac

## Rješenje - Tri sloja zaštite

### 1. **NO ACTION Constraint** - Spremišta baza podataka
```sql
FK_Racun_Kupac: NO ACTION
  → Ne možete obrisati Kupac ako ima Racune
  
FK_Racun_Prodavac: NO ACTION
  → Ne možete obrisati Prodavac ako ima Racune
  
FK_FizickoLice_Kupac: NO ACTION (PROMIJENJENO sa CASCADE)
  → Ne možete obrisati FizickoLice ako ima parent Kupac sa Racunima
  
FK_Firma_Kupac: NO ACTION (PROMIJENJENO sa CASCADE)
  → Ne možete obrisati Firma ako ima parent Kupac sa Racunima
  
FK_StavkaRacuna_Oprema: NO ACTION
  → Ne možete obrisati Opremu ako je u StavkaRacuna
  
FK_ProdSklad_Skladiste: NO ACTION
  → Ne možete obrisati Skladiste ako je u ProdSklad
```

### 2. **INSTEAD OF Trigger** - Primjena baza podataka
- `tr_Kupac_PreventDelete` - Sprječava brisanje Kupac ako ima Racune
- Automatski briše FizickoLice/Firma prije brisanja Kupac (ako nema Racuna)

### 3. **DatabaseBroker Error Handling** - Aplikacijska razina
- Hvata SQL error 547 (FK violation)
- Prikazuje korisno-friendly poruke na srpskom

## Trenutno stanje

### Constraint-i:
```
FK_Firma_Kupac                  Firma → Kupac              NO_ACTION  ✓
FK_FizickoLice_Kupac            FizickoLice → Kupac        NO_ACTION  ✓
FK_ProdSklad_Prodavac           ProdSklad → Prodavac       CASCADE    ✓
FK_ProdSklad_Skladiste          ProdSklad → Skladiste      NO_ACTION  ✓
FK_Racun_Kupac                  Racun → Kupac              NO_ACTION  ✓
FK_Racun_Prodavac               Racun → Prodavac           NO_ACTION  ✓
FK_StavkaRacuna_Oprema          StavkaRacuna → Oprema      NO_ACTION  ✓
FK_StavkaRacuna_Racun           StavkaRacuna → Racun       CASCADE    ✓
```

### Trigger-i:
```
tr_Kupac_PreventDelete          Sprječava brisanje Kupac sa Racunima  ✓
```

### Stored Procedures:
```
DodajFirmu                      Dodaj Kupac + Firma                    ✓
DodajFizickoLice                Dodaj Kupac + FizickoLice              ✓
AzurirajFirmu                   Ažurira Kupac + Firma                  ✓
AzurirajFizickoLice             Ažurira Kupac + FizickoLice            ✓
```

## Test podaci (Oporavljena)
- 3 Prodavca (Marko, Ana, Petar)
- 2 Firme (Tehno Haus, Mega Kompanija)
- 3 Fizička lica (Jovan, Marija, Nikola)
- 3 Skladišta
- 8 Oprema
- 4 Računa sa 9 stavki

## Kako testirati

1. **Pokušaj brisati FizickoLice sa Racunom:**
   - Odaberi "Fizička lica" iz dropdown-a
   - Klikni na red sa "Jovanom Jovanovićem" (ima račune)
   - Klikni "Obriši"
   - **REZULTAT**: Greška "Ne možete obrisati fizičko lice jer ima povezane račune"

2. **Pokušaj brisati Kupac sa Racunom:**
   - Odaberi "Kupci" iz dropdown-a
   - Pokušaj obrisati bilo koji Kupac sa Racunom
   - **REZULTAT**: Greška "Ne možete obrisati kupca jer ima povezane račune"

3. **Brisanje Kupac bez Racuna:**
   - Kreiraj novi Kupac (Firma ili FizickoLice) bez Racuna
   - Pokušaj ga obrisati
   - **REZULTAT**: Uspješno obrisan (i Kupac i specijalizovani zapis)

## SQL skripte za primjenu

```bash
# 1. Obriši staru bazu i kreiraj novu sa tabelama i podacima
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "recreate_database_full.sql"

# 2. Kreiraj stored procedures
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "create_stored_procedures.sql"

# 3. Primijeni zaštitu od brisanja (constraint-i + trigger)
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "fix_cascade_to_no_action.sql"
```

## Status
✓ BAZA ZAŠTIĆENA - Nemoguće je brisati objekata sa FK ograničenjima
✓ STORED PROCEDURES - Dostupne za dodavanje Firma i FizickoLice
✓ TEST PODACI - Oporavljena kompletan skup podataka
✓ DOKUMENTACIJA - Ažurirana sa novim zaštitom
