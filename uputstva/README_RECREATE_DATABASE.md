# Kreiranje Nove Baze Podataka - Kompletan Vodiči

## 📋 Šta Ova Skripta Radi

Skripta **`recreate_database_full.sql`** će:

1. ✅ **Obrisati** staru bazu `ProdavnicaDB` (ako postoji)
2. ✅ **Kreirati** novu bazu `ProdavnicaDB`
3. ✅ **Kreirati sve tabele** sa ispravnom strukturom
4. ✅ **Postaviti sva Foreign Key ograničenja** (RESTRICT i CASCADE)
5. ✅ **Postaviti sva CHECK ograničenja** (validacija vrednosti)
6. ✅ **Ubaciti test podatke** (3 prodavca, 5 kupaca, 3 skladišta, 8 proizvoda, 4 računa)

---

## 🚀 Kako Pokrenuti

### Korak 1: Otvori SQL Server Management Studio (SSMS)
- Konekcija: `(localdb)\MSSQLLocalDB`
- Database: `master` (ne odabiramo ProdavnicaDB jer će biti obrisan!)

### Korak 2: Otvori skriptu
```
File → Open → File
Odaberi: C:\Users\Nikola Kostic\source\repos\Seminarski\recreate_database_full.sql
```

### Korak 3: Pokreni skriptu
```
Pritisni: F5  (ili Execute dugme)
```

### Korak 4: Čekaj poruke
```
Trebalo bi da vidiš poruke:
- "Stara baza ProdavnicaDB je obrisana."
- "Nova baza ProdavnicaDB je kreirana."
- "Sve tabele su uspešno kreirane!"
- "Svi test podaci su uspešno ubačeni!"
```

---

## 📊 Šta Će Biti Kreirano

### Tabele (7 ukupno):

| Tabela | Redova | Napomena |
|--------|--------|----------|
| **Prodavac** | 3 | Marko, Ana, Petar |
| **Kupac** | 5 | Osnovna tabela |
| **Firma** | 2 | Specijalizacija Kupca (Tehno Haus, Mega Kompanija) |
| **FizickoLice** | 3 | Specijalizacija Kupca (Jovan, Marija, Nikola) |
| **Skladiste** | 3 | Beograd 1, Beograd 2, Novi Sad |
| **Oprema** | 8 | Laptopovi, Monitori, Miševi, itd. |
| **Racun** | 4 | 4 računa sa različitim stavkama |
| **StavkaRacuna** | 9 | Stavke u računima |
| **ProdSklad** | 6 | Veza Prodavac-Skladiste |

### Ograničenja:

**RESTRICT** (Sprečavaju brisanje):
- ✅ Racun → Prodavac (ne može obrisati prodavca sa računima)
- ✅ Racun → Kupac (ne može obrisati kupca sa računima)
- ✅ StavkaRacuna → Oprema (ne može obrisati opremu iz stavki)
- ✅ ProdSklad → Skladiste (ne može obrisati skladiste iz ProdSklad)

**CASCADE** (Automatsko brisanje):
- ✅ Firma → Kupac (briše firmu ako se obriše kupac)
- ✅ FizickoLice → Kupac (briše fizičko lice ako se obriše kupac)
- ✅ StavkaRacuna → Racun (briše stavke ako se obriše račun)
- ✅ ProdSklad → Prodavac (briše ProdSklad ako se obriše prodavac)

**CHECK** (Validacija vrednosti):
- ✅ Kupac.popust >= 0
- ✅ Oprema.cena > 0
- ✅ StavkaRacuna.kolicina > 0
- ✅ StavkaRacuna.cena > 0

---

## 🧪 Testiranje Ograničenja

Posle kreiranja baze, možeš testirati:

### Test 1: RESTRICT - Ne mogu obrisati kupca sa računima
```sql
-- Pokušaj da obrišeš kupca 1 (ima 2 računa)
DELETE FROM Kupac WHERE idKupac = 1;
-- Trebalo bi greška: Foreign Key constraint conflict
```

### Test 2: CASCADE - Brisanje računa briše stavke
```sql
-- Obrši Račun 1
DELETE FROM Racun WHERE idRacun = 1;
-- StavkaRacuna sa idRacun=1 će biti automatski obrisane
```

### Test 3: Validacija - Ne mogu ubaciti negativan popust
```sql
-- Pokušaj INSERT sa negativnim popustom
INSERT INTO Kupac (popust) VALUES (-0.10);
-- Trebalo bi greška: CHECK constraint violation
```

---

## ⚠️ VAŽNO

### Sve što će biti obrisano:
- ❌ **Stara baza** `ProdavnicaDB` (ako postoji)
- ❌ **SVE tabele** iz stare baze
- ❌ **SVE podaci** iz stare baze

### Što možeš da uradiš da sprečiš greške:

1. **Napraviti backup** (opciono):
   ```sql
   BACKUP DATABASE ProdavnicaDB TO DISK = 'C:\Backup\ProdavnicaDB.bak';
   ```

2. **Detaljno pročitati sve poruke** tokom izvršavanja

3. **Komplenut** skripta - ne prekinjavaj izvršavanje dok se ne završi

---

## ✅ Posle Kreiranja Baze

1. **Rebuild Solution** u Visual Studio
   ```
   Build → Rebuild Solution
   ```

2. **Testiraj aplikaciju**
   - Pokreni Server
   - Pokreni Client
   - Testiraj login, prikaz podataka, dodavanje, itd.

3. **Testiraj ograničenja**
   - Pokušaj obrisati kupca sa računima (trebalo bi da se ne može)
   - Pokušaj obrisati proizvod koji je u računu (trebalo bi da se ne može)

---

## 📝 Test Korisnici

### Prodavci (za login):

| Email | Lozinka | Ime |
|-------|---------|-----|
| marko@prodavnica.com | sifra123 | Marko Marković |
| ana@prodavnica.com | sifra123 | Ana Anić |
| petar@prodavnica.com | sifra123 | Petar Petrović |

---

## 🔧 Ako Nešto Pođe Naopako

### Ako skripta izbaci grešku:

1. **Proveri** da li je SQL Server pokrenut
2. **Proveri** konekciju (trebalo bi `(localdb)\MSSQLLocalDB`)
3. **Pokušaj opet** - ponekad je potrebno malo čekanja
4. **Ručno obriši** bazu (ako je zaglavljena):
   ```sql
   ALTER DATABASE ProdavnicaDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
   DROP DATABASE ProdavnicaDB;
   ```

### Ako nedostaju tabele:
- Uredi skripto manuelno u delu "CREATE TABLE"
- Ili pokreni samo deo koji je neuspešan

---

## 📧 Struktura Baze Pregled

```
┌─ Prodavac
│  ├─ idProdavac (PK)
│  ├─ ime, prezime, email (UNIQUE), telefon, password
│  └─ FK: ProdSklad
│
├─ Kupac
│  ├─ idKupac (PK)
│  ├─ popust (CHECK >= 0)
│  ├─ Firma (specijalizacija)
│  └─ FizickoLice (specijalizacija)
│
├─ Firma
│  ├─ idKupac (PK, FK → Kupac CASCADE)
│  ├─ naziv, pib, adresa, partnerstvo
│
├─ FizickoLice
│  ├─ idKupac (PK, FK → Kupac CASCADE)
│  ├─ imePrezime, email, telefon, loyaltyClan
│
├─ Skladiste
│  ├─ idSkladiste (PK)
│  ├─ naziv, adresa
│  └─ FK: ProdSklad (RESTRICT)
│
├─ Oprema
│  ├─ idOprema (PK)
│  ├─ ime, kategorija, cena (CHECK > 0)
│  └─ FK: StavkaRacuna (RESTRICT)
│
├─ Racun
│  ├─ idRacun (PK)
│  ├─ datumIzdavanja, konacanIznos, pdv, cenaSaPopustom, cenaStavke
│  ├─ FK: idProdavac → Prodavac (RESTRICT)
│  ├─ FK: idKupac → Kupac (RESTRICT)
│  └─ StavkaRacuna (CASCADE)
│
└─ StavkaRacuna
   ├─ PK: (idRacun, idOprema)
   ├─ kolicina (CHECK > 0), cena (CHECK > 0)
   ├─ FK: idRacun → Racun (CASCADE)
   └─ FK: idOprema → Oprema (RESTRICT)

ProdSklad (Prodavac ↔ Skladiste)
├─ PK: (idProdavac, idSkladiste)
├─ FK: idProdavac → Prodavac (CASCADE)
└─ FK: idSkladiste → Skladiste (RESTRICT)
```

---

## ✨ Rezultat

Posle izvršavanja ove skripte imaćeš:
- ✅ Potpuno novu bazu ProdavnicaDB
- ✅ Sva ograničenja pravilno postavljena
- ✅ Test podatke za razvoj i testiranje
- ✅ Gotov sistem za zaštitu integriteta podataka

**Sveće je? Pokreni aplikaciju!** 🚀
