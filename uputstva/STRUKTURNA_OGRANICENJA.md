# Strukturna Ograničenja (Foreign Keys i Constraints)

## Pregled

Baza podataka sada ima implementirana strukturna ograničenja koja sprečavaju neispravne operacije brisanja i održavaju integritet podataka. Sistem koristi dvije vrste ograničenja:

### 1. **RESTRICT Ograničenja** (Sprečavaju brisanje)
Ako postojeReference na objekat koji želite obrisati, brisanje je **zabranjeno**.

### 2. **CASCADE Ograničenja** (Automatsko brisanje)
Ako obrišete objekat, sve reference na njega se **automatski brišu**.

---

## Ograničenja po Objektu

### **Kupac** 🚫 (RESTRICT)
- **Ne može se obrisati ako:** Ima registrovane Račune
- **Poruka:** "Nije moguće obrisati kupca jer ima registrovane račune. Prvo obrišite sve račune za ovog kupca."
- **Šta se automatski briše kada se obriše Kupac:**
  - ✅ Firma (ako je firma)
  - ✅ FizickoLice (ako je fizičko lice)

### **Prodavac** 🚫 (RESTRICT)
- **Ne može se obrisati ako:** Ima registrovane Račune
- **Poruka:** "Nije moguće obrisati prodavca jer ima registrovane račune. Prvo obrišite sve račune ovog prodavca."

### **Oprema** 🚫 (RESTRICT)
- **Ne može se obrisati ako:** Je korišćena u StavkaRacuna (bilo kom računu)
- **Poruka:** "Nije moguće obrisati opremu jer je već zabeležena u stavkama računa."

### **Skladište** 🚫 (RESTRICT)
- **Ne može se obrisati ako:** Je registrovano u ProdSklad
- **Poruka:** "Nije moguće obrisati skladište jer je već u upotrebi."

### **Račun** ✅ (CASCADE)
- **Može se obrisati:** Bez ograničenja
- **Šta se automatski briše:**
  - ✅ Sve StavkaRacuna povezane sa ovim računom

### **Firma** ✅ (CASCADE)
- **Automatski se briše kada:** Se obriše Kupac kojem pripada

### **FizickoLice** ✅ (CASCADE)
- **Automatski se briše kada:** Se obriše Kupac kojem pripada

---

## Validacijska Ograničenja (CHECK)

Dodatna ograničenja za validaciju vrednosti:

- **Kupac.popust** ≥ 0 (diskont ne može biti negativan)
- **Oprema.cena** > 0 (cena mora biti pozitivna)
- **StavkaRacuna.kolicina** > 0 (količina mora biti pozitivna)
- **StavkaRacuna.cena** > 0 (cena stavke mora biti pozitivna)

---

## Instalacija Ograničenja

### Korak 1: Pokreni SQL skriptu

```sql
-- U SQL Server Management Studio ili Azure Data Studio:
-- Otvori fajl: create_constraints.sql
-- Izaberi bazu: ProdavnicaDB
-- Pokreni (F5)
```

### Korak 2: Rebuild Solution
```
Build → Rebuild Solution
```

### Korak 3: Testiraj u Aplikaciji

1. **Test RESTRICT ograničenja:**
   - Otvori aplikaciju
   - Odaberi Kupca koji ima Račune
   - Pokušaj da ga obrišeš
   - Trebalo bi da vidiš poruku: "Nije moguće obrisati kupca jer ima registrovane račune..."

2. **Test CASCADE ograničenja:**
   - Otvori aplikaciju
   - Obrši Račun
   - Sve stavke tog računa se automatski brišu (bez greške)

---

## Ponašanje programa

### Kada korisnik pokušaji obrisati objekat sa referencama:

```
❌ Greška - Nije moguće obrisati kupca jer ima registrovane račune.
   Prvo obrišite sve račune za ovog kupca.
```

**Korisnik vidi jasnu poruku sa instrukcijama šta trebam da uradi.**

### Kada korisnik obriše objekat bez referencija:

```
✅ Objekat je uspešno obrisan.
```

---

## Redosled Brisanja (Preporuka)

Ako trebate obrisati kompletan kupca:

1. **Prvo:** Pronađi sve njegove Račune
2. **Drugo:** Obrši sve StavkaRacuna za te Račune
3. **Treće:** Obrši Račune
4. **Četvrto:** Obrši Kupca
   - Firma/FizickoLice će se automatski obrisati

---

## Grafički Prikaz Ograničenja

```
Kupac (PK: idKupac)
  ├─→ RESTRICT ← Racun (NE MOŽE se obrisati ako ima račune)
  └─→ CASCADE → Firma / FizickoLice (AUTOMATSKI brišu se)

Prodavac (PK: idProdavac)
  ├─→ RESTRICT ← Racun (NE MOŽE se obrisati ako ima račune)

Oprema (PK: idOprema)
  ├─→ RESTRICT ← StavkaRacuna (NE MOŽE se obrisati ako je u stavka)

Skladiste (PK: idSkladiste)
  ├─→ RESTRICT ← ProdSklad (NE MOŽE se obrisati ako je u ProdSklad)

Racun (PK: idRacun)
  ├─→ CASCADE → StavkaRacuna (AUTOMATSKI brišu se stavke)
```

---

## Provereni Scenariji

| Scenario | Rezultat | Poruka |
|----------|----------|--------|
| Obrišite kupca sa računima | ❌ Greška | "Nije moguće obrisati kupca jer ima registrovane račune..." |
| Obrišite prodavca sa računima | ❌ Greška | "Nije moguće obrisati prodavca jer ima registrovane račune..." |
| Obrišite opremu koja je u stavki | ❌ Greška | "Nije moguće obrisati opremu jer je već zabeležena..." |
| Obrišite račun bez ograničenja | ✅ OK | Sve stavke automatski brišu se |
| Obrišite kupca bez računa | ✅ OK | Firma/FizickoLice se automatski briše |

---

## Tehnički Detalji

- **Error Code 547:** SQL Server koristi error kod 547 za Foreign Key RESTRICT conflicts
- **Hvatanje grešaka:** DatabaseBroker.cs → Obrisi() metoda detektuje ove greške i prikazuje korisničke poruke
- **Kompatibilnost:** Svi sistemski objekti (Kupac, Prodavac, Oprema, Skladiste, itd.) su pokriti
