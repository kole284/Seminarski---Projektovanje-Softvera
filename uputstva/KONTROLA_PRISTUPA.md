# Kontrola Pristupa - Admin i Obični Korisnici

## Šta je dodato

Implementiran je sistem kontrole pristupa sa dva nivoa dozvola:
1. **Administrator** - Pun pristup svim funkcijama (email: "admin", šifra: "admin")
2. **Obični korisnici (Prodavci)** - Mogu videti sve podatke, ali ne mogu ih menjati/brisati; mogu kreirati račune

---

## Izmene

### 1. Client/Sesija.cs
**Dodato:**
- `public static bool IsAdmin { get; set; } = false;` - Svojstvo koje označava da li je korisnik administrator

### 2. Client/Login.cs
**Izmenjeno:**
- **`button1_Click()`** - Dodato prepoznavanje admin korisnika
  - Ako je `email == "admin"` i `password == "admin"`, postavlja `Sesija.IsAdmin = true`
  - Kreira admin Prodavac objekat sa `IdProdavac = 0` i `ImePrezime = "Administrator"`
  - Za obične korisnike postavlja `Sesija.IsAdmin = false`

### 3. Client/Main.cs
**Izmenjeno:**
- **`Main_Load()`** - Svi korisnici vide sve opcije u ComboBox-u (Prodavci, Firme, Fizička lica, Oprema, Skladišta, Računi)

- **`button1_Click()`** - Uklonjena filtracija podataka:
  - **Svi korisnici** (admin i obični prodavci) vide sve podatke bez filtera
  - Prodavci mogu pregledati sve prodavce, firme, fizička lica, opremu i račune

- **`btnDodaj_Click()`** - Dodavanje dozvola:
  - **Admin:** Može dodavati sve tipove zapisa
  - **Obični prodavci:** Mogu dodavati samo račune
  - Pokušaj dodavanja drugog tipa prikazuje poruku "Možete samo kreirati račune."

### 4. Client/Details.cs
**Dodato:**
- **Dugme `btnPromeniSifru`** - Prikazuje se samo kada se vidi Prodavac objekat
  - Event handler `btnPromeniSifru_Click()` otvara PromeniSifru formu

**Izmenjeno:**
- **Konstruktor `Details(object selectedObject)`:**
  - Ako je Prodavac:
    - Prikazuje dugme "Promeni Šifru"
    - Ako nije admin i nije svoj profil → Sakriva sve dugmad
    - Ako je svoj profil → Prikazuje "Ažuriraj" i "Promeni Šifru", sakriva "Obriši"
    - Ako je admin → Prikazuje sva dugmad
  - Ako nije Prodavac:
    - Sakriva dugme "Promeni Šifru"
    - Ako nije admin → Sakriva "Ažuriraj" i "Obriši"

- **`PrikaziDetalje()`** - Dodato prikazivanje skladišta za prodavca

### 5. Client/Details.Designer.cs
**Dodato:**
- `btnPromeniSifru` - Novo dugme na Details formi
  - **Location:** (324, 396)
  - **Size:** (150, 40)
  - **BackColor:** LightSalmon
  - **Text:** "Promeni Šifru"
  - **Visible:** false (prikazuje se samo za Prodavca)

### 6. Client/DetailsRacun.cs
**Izmenjeno:**
- **Konstruktor `DetailsRacun(Racun selectedRacun)`:**
  - Ako nije admin:
    - Ako račun nije kreirao ulogovani prodavac → Sakriva oba dugmeta
    - Ako je svoj račun → Sakriva samo `btnObrisi`

- **`btnAzuriraj_Click()`:**
  - Provera: Ako nije admin i pokušava ažurirati tuđi račun → Prikazuje poruku

- **`btnObrisi_Click()`:**
  - Provera: Samo admin može brisati račune → Prikazuje poruku

### 7. Models/Prodavac.cs
**Dodato:**
- `public string Skladista { get; set; }` - Svojstvo koje sadrži listu skladišta odvojenu zarezima
- `GetList()` - Ažurirano da učitava skladišta iz query-ja

### 8. Broker/DatabaseBroker.cs
**Izmenjeno:**
- **`VratiSve()`** - Query za Prodavac:
  - Koristi SQL WITH STUFF i FOR XML PATH da spoji skladišta u jedan string odvojen zarezima
  - Primer: "Beograd 1, Novi Sad, Beograd 2"

---

## Kako se prijaviti

### Admin prijava:
```
Email: admin
Šifra: admin
```
**Dozvole:**
- ✅ Vidi sve podatke u sistemu
- ✅ Može dodavati, ažurirati i brisati sve zapise
- ✅ Pun pristup svim funkcijama sistema

### Obična prijava (Prodavac):
```
Email: [email prodavca iz baze]
Šifra: [šifra prodavca iz baze]
```
**Dozvole:**
- ✅ Vidi SVE prodavce, firme, fizička lica, opremu, skladišta i račune
- ✅ Može pregledati sve podatke u read-only režimu
- ✅ Može ažurirati SAMO svoj profil (ime, email, telefon)
- ✅ Može promeniti SAMO svoju lozinku
- ✅ Može kreirati nove račune
- ❌ Ne može brisati ništa
- ❌ Ne može dodavati/ažurirati firme, fizička lica, opremu, skladišta, druge prodavce

---

## Primeri ponašanja

### Scenario 1: Obični prodavac pregledava sve prodavce
**Rezultat:** Vidi sve prodavce u DataGridView-u sa njihovim skladištima

### Scenario 2: Obični prodavac otvori svoj profil
**Rezultat:** 
- Vidi sve svoje podatke + skladišta na kojima radi
- Dugmad "Ažuriraj" i "Promeni Šifru" su vidljiva
- Dugme "Obriši" je sakriveno

### Scenario 3: Obični prodavac otvori tuđi profil
**Rezultat:**
- Vidi sve podatke prodavca + skladišta
- Sva dugmad su sakrivena (samo read-only prikaz)

### Scenario 4: Obični prodavac pokušava da doda novu opremu
**Rezultat:** Prikazuje se poruka "Nemate dozvolu za dodavanje. Možete samo kreirati račune."

### Scenario 5: Obični prodavac kreira novi račun
**Rezultat:** Otvara se AddRacun forma i može kreirati račun

### Scenario 6: Admin otvori bilo koji profil
**Rezultat:**
- Vidi sve podatke
- Sva dugmad su vidljiva (Ažuriraj, Obriši, Promeni Šifru za prodavca)
- Može sve da menja i briše

---

## Prikaz skladišta

### U DataGridView-u (Main forma):
- Kolona "Skladišta" prikazuje listu skladišta odvojenu zarezima
- Primer: "Beograd 1, Novi Sad, Beograd 2"
- Automatski formatiranje kolona sakriva ID kolone

### U Details formi:
- Polje "Skladišta:" prikazuje listu skladišta
- Ako prodavac nema dodeljenih skladišta, prikazuje "Nema dodeljenih skladišta"

### Ažuriranje skladišta:
- Admin može ažurirati skladišta kroz Add formu (Edit mode)
- Obični prodavci ne mogu menjati skladišta

---

## Primeri ponašanja

### Scenario 1: Obični prodavac pokušava da vidi sve prodavce
**Rezultat:** Vidi samo svoj profil u DataGridView-u (automatski filtrirano)

### Scenario 2: Obični prodavac otvori svoj profil
**Rezultat:** 
- Vidi sve svoje podatke
- Dugme "Ažuriraj" je vidljivo → Može menjati svoje podatke
- Dugme "Obriši" je sakriveno → Ne može se obrisati

### Scenario 3: Obični prodavac pokušava da promeni tuđi račun
**Rezultat:** Račun se neće prikazati u listi jer automatski filtrira samo svoje račune

### Scenario 4: Admin otvori bilo koji račun
**Rezultat:**
- Vidi sve podatke računa
- Oba dugmeta su vidljiva
- Može ažurirati i obrisati račun

### Scenario 5: Obični prodavac pokušava da doda novu opremu
**Rezultat:** Dugme "Dodaj novi" nije vidljivo na glavnoj formi

---

## Sigurnosne provere

### UI nivo (Client):
- Sakrivanje dugmadi za akcije koje korisnik ne sme izvršiti
- Svi korisnici vide sve podatke, ali samo admin može ih menjati/brisati
- Obični prodavci mogu ažurirati samo svoj profil i kreirati račune

### Aplikacijski nivo (Client):
- Dupla provera u event handler-ima (btnAzuriraj_Click, btnObrisi_Click, btnDodaj_Click)
- Poruke korisniku ako pokuša neautorizovanu akciju
- Provera vlasništva podataka pre izmene (samo svoj profil)

### Napomena o Server strani:
**Trenutno server NE proverava dozvole** - Sve provere su samo na klijentskoj strani. Za produkciono okruženje, trebalo bi dodati:
- Slanje `Sesija.ProdavacId` u svakom Request-u
- Server provera da li korisnik ima pravo da izvršava određenu operaciju
- Autentikacioni token umesto skladištenja šifre na klijentu

---

## Testiranje

### Test 1: Admin prijava
1. Pokreni aplikaciju
2. Uloguj se sa `admin` / `admin`
3. Proveri da vidiš sve opcije u ComboBox-u
4. Otvori "Prodavci" → Proveri da vidiš kolonu "Skladišta"
5. Otvori bilo kog prodavca → Proveri da vidiš skladišta u detaljima
6. Proveri da možeš dodavati, menjati i brisati bilo šta

### Test 2: Obična prijava
1. Pokreni aplikaciju
2. Uloguj se sa email-om i šifrom postojećeg prodavca
3. Proveri da vidiš sve opcije u ComboBox-u
4. Otvori "Prodavci" → Proveri da vidiš sve prodavce sa skladištima
5. Double-click na SVOJ profil → Proveri da su vidljivi "Ažuriraj" i "Promeni Šifru"
6. Double-click na TUĐI profil → Proveri da su sva dugmad sakrivena
7. Klikni "Dodaj novi" → Izaberi "Oprema" → Proveri da prikazuje poruku "Možete samo kreirati račune"
8. Klikni "Dodaj novi" → Izaberi "Računi" → Proveri da se otvara forma za račun

### Test 3: Izmena svog profila
1. Uloguj se kao obični prodavac
2. Otvori "Prodavci"
3. Double-click na svoj profil
4. Klikni "Ažuriraj" i promeni nešto (npr. telefon)
5. Proveri da se promene sačuvaju

### Test 4: Promena šifre iz Details forme
1. Uloguj se kao obični prodavac
2. Otvori "Prodavci"
3. Double-click na svoj profil
4. Klikni dugme "Promeni Šifru"
5. Unesi staru šifru, novu šifru i potvrdu
6. Izloguj se i ponovo se uloguj sa novom šifrom
7. Proveri da radi

### Test 5: Kreiranje računa
1. Uloguj se kao obični prodavac
2. Klikni "Dodaj novi"
3. Izaberi "Računi"
4. Kreiraj novi račun
5. Proveri da je račun uspešno kreiran

---

## Struktura dozvola

| Akcija | Admin | Obični Prodavac |
|--------|-------|----------------|
| Vidi sve prodavce | ✅ | ✅ |
| Vidi skladišta prodavaca | ✅ | ✅ |
| Vidi sve račune | ✅ | ✅ |
| Vidi firme | ✅ | ✅ |
| Vidi fizička lica | ✅ | ✅ |
| Vidi opremu | ✅ | ✅ |
| Vidi skladišta | ✅ | ✅ |
| Dodaj prodavca | ✅ | ❌ |
| Dodaj firmu | ✅ | ❌ |
| Dodaj fizičko lice | ✅ | ❌ |
| Dodaj opremu | ✅ | ❌ |
| Dodaj skladište | ✅ | ❌ |
| Dodaj račun | ✅ | ✅ |
| Ažuriraj svoj profil | ✅ | ✅ |
| Ažuriraj tuđi profil | ✅ | ❌ |
| Ažuriraj firmu/fiz.lice/opremu | ✅ | ❌ |
| Obriši bilo šta | ✅ | ❌ |
| Promeni svoju šifru | ✅ | ✅ |

---

### Napomena o Server strani:
**Trenutno server NE proverava dozvole** - Sve provere su samo na klijentskoj strani. Za produkciono okruženje, trebalo bi dodati:
- Slanje `Sesija.ProdavacId` u svakom Request-u
- Server provera da li korisnik ima pravo da izvršava određenu operaciju
- Autentikacioni token umesto skladištenja šifre na klijentu

---

## Testiranje

### Test 1: Admin prijava
1. Pokreni aplikaciju
2. Uloguj se sa `admin` / `admin`
3. Proveri da vidiš sve opcije u ComboBox-u
4. Proveri da možeš dodavati, menjati i brisati bilo šta

### Test 2: Obična prijava
1. Pokreni aplikaciju
2. Uloguj se sa email-om i šifrom postojećeg prodavca (npr. `marko.markovic@email.com` / `marko123`)
3. Proveri da vidiš samo "Prodavci" i "Računi" u ComboBox-u
4. Proveri da dugme "Dodaj novi" nije vidljivo
5. Otvori "Prodavci" → Proveri da vidiš samo svoj profil
6. Otvori "Računi" → Proveri da vidiš samo svoje račune

### Test 3: Izmena svog profila
1. Uloguj se kao obični prodavac
2. Otvori "Prodavci"
3. Double-click na svoj profil
4. Proveri da je "Ažuriraj" vidljivo, a "Obriši" sakriveno
5. Klikni "Ažuriraj" i promeni nešto (npr. telefon)
6. Proveri da se promene sačuvaju

### Test 4: Promena šifre
1. Uloguj se kao obični prodavac
2. Klikni dugme "Promeni Šifru"
3. Unesi staru šifru, novu šifru i potvrdu
4. Izloguj se i ponovo se uloguj sa novom šifrom
5. Proveri da radi

---

## Struktura dozvola

| Akcija | Admin | Obični Prodavac |
|--------|-------|----------------|
| Vidi sve prodavce | ✅ | ❌ (samo svoj profil) |
| Vidi sve račune | ✅ | ❌ (samo svoje račune) |
| Vidi firme | ✅ | ❌ |
| Vidi fizička lica | ✅ | ❌ |
| Vidi opremu | ✅ | ❌ |
| Vidi skladišta | ✅ | ❌ |
| Dodaj novi zapis | ✅ | ❌ |
| Ažuriraj svoj profil | ✅ | ✅ |
| Ažuriraj tuđi profil | ✅ | ❌ |
| Ažuriraj svoj račun | ✅ | ✅ |
| Ažuriraj tuđi račun | ✅ | ❌ |
| Obriši bilo šta | ✅ | ❌ |
| Promeni svoju šifru | ✅ | ✅ |

---

## Dodatna poboljšanja (opciono)

### Za produkciono okruženje:

1. **JWT Tokeni:**
   - Umesto skladištenja korisnika u `Sesija`, koristi JWT token
   - Server vraća token pri login-u
   - Svaki request šalje token u header-u

2. **Server-side provere:**
   - Server provera uloge pre izvršavanja bilo koje operacije
   - Logovanje svih pokušaja neautorizovanih akcija

3. **Različiti nivoi pristupa:**
   - Menadžer (vidi sve, ali ne može brisati)
   - Prodavac (trenutno ponašanje)
   - Read-only korisnik (samo čitanje)

4. **Audit log:**
   - Skladištenje svih akcija korisnika
   - Ko je šta promenio i kada

5. **Session timeout:**
   - Automatsko odjava nakon određenog perioda neaktivnosti
   - Provera validnosti sesije pri svakom request-u

---

## Važne napomene

⚠️ **Sigurnost:**
- Admin credentials (`admin` / `admin`) su hardkodovani u klijentu - **NE KORISTITI U PRODUKCIJI!**
- Sve provere dozvola su samo na klijentskoj strani - može se zaobići ako neko direktno šalje request-e serveru
- Za pravu sigurnost, implementiraj server-side provere i JWT autentikaciju

✅ **Korisničko iskustvo:**
- Korisnici vide samo ono što im je dozvoljeno
- Jasne poruke kada pokušaju neautorizovanu akciju
- Sakrivanje dugmadi umesto prikazivanja sa disabled stanjem (bolje UX)

📝 **Održavanje:**
- Sve provere dozvola su centralizovane kroz `Sesija.IsAdmin`
- Lako dodavanje novih uloga u budućnosti
- Dokumentovane sve izmene u ovom fajlu
