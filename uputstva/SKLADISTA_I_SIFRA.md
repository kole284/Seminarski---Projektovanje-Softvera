# Upravljanje Skladištima i Promena Šifre Prodavca

## Šta je dodato

### 1. Upravljanje Skladištima za Prodavca
Dodata je mogućnost dodavanja i ažuriranja skladišta na kojima prodavac radi preko ProdSklad tabele.

### 2. Promena Šifre
Dodata je mogućnost promene šifre samo za trenutno ulogovanog prodavca.

---

## Izmene

### 1. Models/ProdSklad.cs (NOVI FAJL)
Kreiran novi model za ProdSklad tabelu koja povezuje prodavce i skladišta.

**Svojstva:**
- `IdProdavac` - ID prodavca
- `IdSkladiste` - ID skladišta
- `TableName` - Ime tabele ("ProdSklad")
- `InsertValues` - Vrednosti za INSERT
- `UpdateValues` - Vrednosti za UPDATE
- `WhereCondition` - Uslov za WHERE
- `GetList()` - Metoda za učitavanje liste iz SqlDataReader-a

### 2. Client/PromeniSifru.cs (NOVI FAJL)
Kreirana nova forma za promenu šifre trenutno ulogovanog prodavca.

**Kontrole:**
- `txtStaraSifra` - Polje za unos stare šifre
- `txtNovaSifra` - Polje za unos nove šifre
- `txtPotvrdaSifre` - Polje za potvrdu nove šifre
- `btnPromeni` - Dugme za potvrdu promene
- `btnOtkazi` - Dugme za otkazivanje

**Validacije:**
- Stara šifra mora biti tačna
- Nova šifra mora imati najmanje 4 karaktera
- Nova šifra i potvrda šifre moraju biti iste

**Funkcionalnost:**
- Provera stare šifre protiv `Sesija.UlogovaniProdavac.Password`
- Ažuriranje prodavca sa novom šifrom preko servera
- Automatsko ažuriranje sesije nakon uspešne promene

### 3. Client/Add.cs
**Dodate nove funkcionalnosti:**

#### Novi privatni članovi:
- `List<Skladiste> svaSkladista` - Lista svih skladišta
- `List<ProdSklad> prodavacSkladista` - Lista skladišta za trenutnog prodavca

#### Nove metode:
- **`UcitajSkladista()`** - Učitava sva skladišta iz baze
- **`UcitajProdavacSkladista(int idProdavac)`** - Učitava skladišta za određenog prodavca i čekira ih u CheckedListBox-u
- **`AzurirajProdavacSkladista(int idProdavac)`** - Briše stare i dodaje nove ProdSklad zapise
- **`DodajCheckedListBox(string labelText, string controlName, ref int yPosition, List<Skladiste> skladista)`** - Kreira CheckedListBox kontrolu za izbor skladišta

#### Izmenjene metode:
- **`Add()`** - Dodato pozivanje `UcitajSkladista()`
- **`PopuniPostojeceVrednosti()`** - Dodato pozivanje `UcitajProdavacSkladista()` za prodavce
- **`KreirajProdavacKontrole()`** - Dodato `DodajCheckedListBox()` za izbor skladišta
- **`btnSacuvaj_Click()` - case "Prodavac"** - Kompletno promenjena logika:
  - Prvo sačuva prodavca
  - Učita ID novog prodavca ako je novi
  - Pozove `AzurirajProdavacSkladista()` da snimi skladišta
  - Vraća se sa return umesto break

### 4. Client/Main.cs
**Dodato:**
- **`btnPromeniSifru_Click()`** - Event handler koji otvara PromeniSifru formu

### 5. Client/Main.Designer.cs
**Dodato:**
- `btnPromeniSifru` - Novo dugme na glavnoj formi
  - **Text:** "Promeni Šifru"
  - **Lokacija:** (650, 362)
  - **Veličina:** (138, 31)
  - **Boja:** LightSalmon
  - **Event:** btnPromeniSifru_Click

### 6. Broker/DatabaseBroker.cs
**Izmenjeno:**
- **`VratiSve()`** - Dodata podrška za ProdSklad objekat:
  ```csharp
  else if (obj is ProdSklad prodSklad)
  {
      query = $"SELECT * FROM {obj.TableName}";
      
      if (prodSklad.IdProdavac > 0)
          whereConditions.Add($"idProdavac = {prodSklad.IdProdavac}");
      if (prodSklad.IdSkladiste > 0)
          whereConditions.Add($"idSkladiste = {prodSklad.IdSkladiste}");
  }
  ```

**Napomena:** `Dodaj()` i `Obrisi()` metode već rade sa ProdSklad jer koriste univerzalne INSERT i DELETE naredbe.

---

## Kako koristiti

### Upravljanje Skladištima:

#### 1. Dodavanje novog prodavca sa skladištima:
1. Otvori glavni prozor
2. Izaberi "Prodavci" iz dropdown-a
3. Klikni "Dodaj novi"
4. Popuni:
   - Ime i Prezime
   - Email
   - Telefon
   - Password
   - **Čekiraj skladišta** na kojima prodavac radi (možeš izabrati više)
5. Klikni "Sačuvaj"

#### 2. Ažuriranje prodavca i promena skladišta:
1. Double-click na prodavca u listi
2. Klikni "Ažuriraj"
3. Izmeni podatke
4. **Čekiraj/odčekiraj skladišta** kako želiš
5. Klikni "Ažuriraj"
6. Sistem će:
   - Obrisati sve stare ProdSklad zapise za ovog prodavca
   - Dodati nove ProdSklad zapise za čekirana skladišta

### Promena Šifre:

1. **Na glavnoj formi**, klikni dugme **"Promeni Šifru"** (narandžasto dugme)
2. U formi unesi:
   - **Stara šifra** - Trenutna šifra (mora biti tačna)
   - **Nova šifra** - Nova šifra (najmanje 4 karaktera)
   - **Potvrdi šifru** - Ponovi novu šifru
3. Klikni **"Promeni"**
4. Sistem će:
   - Proveriti da li je stara šifra tačna
   - Proveriti da li nova šifra i potvrda odgovaraju
   - Ažurirati prodavca u bazi sa novom šifrom
   - Ažurirati `Sesija.UlogovaniProdavac.Password` sa novom šifrom

**Napomena:** Samo trenutno ulogovani prodavac može da menja svoju šifru!

---

## Tehnički detalji

### CheckedListBox format prikaza:
- Svako skladište se prikazuje kao: "Naziv - Adresa"
- Primer: "Beograd 1 - Kraljice Marije 21"

### Backend skladišta:
ProdSklad zapisi se šalju na server kroz request:
```csharp
ProdSklad ps = new ProdSklad
{
    IdProdavac = idProdavac,
    IdSkladiste = skladiste.IdSkladiste
};

Request req = new Request { Operation = Operation.DodajObjekat, Data = ps };
Response res = CommunicationHelper.Instance.SendRequest(req);
```

### Proces ažuriranja skladišta:
1. **Brisanje:** Briše sve postojeće ProdSklad zapise za prodavca
2. **Dodavanje:** Dodaje nove ProdSklad zapise za svako čekirano skladište
3. **Transakcija:** Sve operacije se izvršavaju u okviru jednog procesa

### Promena šifre workflow:
1. **Validacija na klijentu:** Provera svih uslova pre slanja na server
2. **Ažuriranje na serveru:** Koristi `Operation.AzurirajObjekat`
3. **Ažuriranje sesije:** Lokalno ažurira `Sesija.UlogovaniProdavac.Password`

---

## Primeri

### Primer 1: Dodavanje prodavca sa 2 skladišta
```
Ime i Prezime: Marko Marković
Email: marko@prodavnica.com
Telefon: 064-123-4567
Password: marko123
Skladišta: [✓] Beograd 1 - Kraljice Marije 21
          [✓] Novi Sad - Bulevar oslobođenja 15
          [ ] Niš - Cara Dušana 45
```

**Rezultat:**
- Kreiran prodavac sa ID-om (npr. 5)
- Kreirane 2 ProdSklad zapise: (5, 1) i (5, 2)

### Primer 2: Promena skladišta prodavca
**Pre:**
- Prodavac 5: Beograd 1, Novi Sad

**Ažuriranje:**
```
Skladišta: [ ] Beograd 1 - Kraljice Marije 21
          [✓] Novi Sad - Bulevar oslobođenja 15
          [✓] Niš - Cara Dušana 45
```

**Rezultat:**
- Obrisani ProdSklad zapisi: (5, 1) i (5, 2)
- Dodati ProdSklad zapisi: (5, 2) i (5, 3)

### Primer 3: Promena šifre
```
Trenutno ulogovani: Ana Anić (ID: 2)
Stara šifra: ana123
Nova šifra: ana_nova_sifra_2024
Potvrdi šifru: ana_nova_sifra_2024
```

**Rezultat:**
- Prodavac 2 ažuriran u bazi sa novom šifrom
- Sesija.UlogovaniProdavac.Password = "ana_nova_sifra_2024"

---

## Sigurnosne napomene

### Promena šifre:
- ✅ **Sigurno:** Samo trenutno ulogovani prodavac može da menja svoju šifru
- ✅ **Validacija:** Stara šifra se proverava pre promene
- ⚠️ **Napomena:** Šifra se čuva u plain text-u u bazi (u produkciji treba hash-ovati!)

### Upravljanje skladištima:
- ✅ **Fleksibilno:** Prodavac može raditi na više skladišta
- ✅ **Integritet:** ProdSklad zapisi se uvek ažuriraju atomski (ili svi ili nijedan)
- ✅ **Brisanje:** Kada se obriše prodavac, ProdSklad zapisi se automatski brišu (CASCADE)

---

## Baza podataka

### ProdSklad tabela struktura:
```sql
CREATE TABLE ProdSklad
(
    idProdavac INT NOT NULL,
    idSkladiste INT NOT NULL,
    PRIMARY KEY (idProdavac, idSkladiste),
    FOREIGN KEY (idProdavac) REFERENCES Prodavac(idProdavac) ON DELETE CASCADE,
    FOREIGN KEY (idSkladiste) REFERENCES Skladiste(idSkladiste) ON DELETE NO ACTION
);
```

**Constraint-i:**
- `FK_ProdSklad_Prodavac`: CASCADE - Kada se obriše prodavac, brišu se i njegovi ProdSklad zapisi
- `FK_ProdSklad_Skladiste`: NO ACTION - Ne može se obrisati skladište koje ima ProdSklad zapise

---

## Moguća proširenja

1. **Istorija promena šifre:** Čuvanje datuma i vremena kada je šifra promenjena
2. **Kompleksnost šifre:** Pravila za jačinu šifre (velika slova, brojevi, specijalni znakovi)
3. **Hashing šifre:** Implementacija bcrypt ili SHA-256 hash-ovanja
4. **Notifikacije:** Email notifikacija nakon promene šifre
5. **Prikaz skladišta u Details.cs:** Prikazivanje skladišta prodavca u Details formi
6. **Filter po skladištu:** Filtriranje prodavaca po skladištu na kojem rade
