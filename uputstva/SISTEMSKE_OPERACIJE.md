# Sistemske Operacije - Dokumentacija

## Pregled operacija

Sistem podržava sve navedene sistemske operacije kroz univerzalne CRUD operacije:

### ✅ Implementirane operacije

| Kod | Naziv | Operation Enum | Opis |
|-----|-------|---------------|------|
| SK1 | Kreiranje računa | `DodajObjekat` | Kreiranje novog računa sa stavkama |
| SK2 | Pretraživanje računa | `VratiSve`, `VratiJedan` | Pretraga svih računa ili pojedinačnog po ID-u |
| SK3 | Promena računa | `AzurirajObjekat` | Ažuriranje postojećeg računa |
| SK4 | Kreiranje kupca | `DodajObjekat` | Kreiranje Firme ili Fizičkog lica |
| SK5 | Pretraživanje kupca | `VratiSve`, `VratiJedan` | Pretraga svih kupaca ili pojedinačnog po ID-u |
| SK6 | Promena kupca | `AzurirajObjekat` | Ažuriranje podataka kupca (Firma/Fizičko lice) |
| SK7 | Brisanje kupca | `ObrisiObjekat` | Brisanje kupca iz sistema (CASCADE briše i povezane zapise) |
| SK16 | Prijava prodavca | `PrijaviProdavac` | Login autentikacija sa case-sensitive provjerom |
| SK26 | Ubacivanje skladišta | `DodajObjekat` | Kreiranje novog skladišta |

---

## Univerzalne operacije

Sistem koristi **6 univerzalnih operacija** koje rade sa svim entitetima:

### 1. **PrijaviProdavac** (SK16)
**Namjena:** Autentikacija prodavca  
**Request:**
```csharp
new Request {
    Operation = Operation.PrijaviProdavac,
    Data = new Prodavac { Email = "email@domen.com", Password = "sifra123" }
}
```
**Response:** Objekat Prodavac ili poruka o grešci  
**Napomena:** Poređenje email-a i lozinke je **case-sensitive** (koristi `COLLATE Latin1_General_CS_AS`)

---

### 2. **VratiSve** (SK2, SK5)
**Namjena:** Pretraga svih zapisa određenog tipa  
**Request:**
```csharp
new Request {
    Operation = Operation.VratiSve,
    Data = new Racun() // ili Firma(), FizickoLice(), Prodavac(), Oprema(), Skladiste()
}
```
**Response:** Lista objekata  
**Napomena:** Za Račune JOIN-uje sa Prodavac i Kupac tabelama

---

### 3. **VratiJedan** (SK2, SK5)
**Namjena:** Pretraga pojedinačnog zapisa po ID-u  
**Request:**
```csharp
new Request {
    Operation = Operation.VratiJedan,
    Data = new Firma { IdKupac = 5 } // Koristi WhereCondition iz modela
}
```
**Response:** Jedan objekat ili null ako ne postoji

---

### 4. **DodajObjekat** (SK1, SK4, SK26)
**Namjena:** Kreiranje novog zapisa  
**Request:**
```csharp
new Request {
    Operation = Operation.DodajObjekat,
    Data = new Firma { 
        Naziv = "Nova Firma DOO", 
        Popust = 0.15, 
        Pib = "123456789",
        Adresa = "Adresa 123",
        Partnerstvo = true
    }
}
```
**Response:** Poruka o uspjehu  
**Napomena:** 
- Za Račune kreira i stavke u transakciji
- Za Firmu/Fizičko lice koristi stored procedures

---

### 5. **AzurirajObjekat** (SK3, SK6)
**Namjena:** Ažuriranje postojećeg zapisa  
**Request:**
```csharp
new Request {
    Operation = Operation.AzurirajObjekat,
    Data = new FizickoLice { 
        IdKupac = 3,
        ImePrezime = "Novo Ime",
        Email = "novo@email.com",
        Telefon = "065123456",
        Popust = 0.10,
        LoyaltyClan = true
    }
}
```
**Response:** Poruka o uspjehu  
**Napomena:** Koristi `WhereCondition` i `UpdateValues` iz modela

---

### 6. **ObrisiObjekat** (SK7)
**Namjena:** Brisanje zapisa iz baze  
**Request:**
```csharp
new Request {
    Operation = Operation.ObrisiObjekat,
    Data = new Firma { IdKupac = 5 }
}
```
**Response:** Poruka o uspjehu  
**Napomena:** 
- CASCADE DELETE automatski briše povezane zapise
- Koristi `WhereCondition` iz modela

---

## Arhitektura

### Client → Server komunikacija

```
Client (Forms)
    ↓
CommunicationHelper (Singleton)
    ↓ TCP/JSON
Server (ClientHandler)
    ↓
Controller (HandleRequest)
    ↓
DatabaseBroker (CRUD metode)
    ↓
SQL Server Database
```

### Komponente sistema

#### 1. **Models** (IDomainObject)
Svaki model implementira:
- `TableName` - Ime SQL tabele
- `WhereCondition` - SQL WHERE uslovi (npr. `idKupac = 5`)
- `InsertValues` - Vrednosti za INSERT
- `UpdateValues` - SET vrednosti za UPDATE
- `GetList(SqlDataReader)` - Mapiranje iz readera u objekte

#### 2. **DatabaseBroker**
Univerzalne metode:
- `VratiJedan(IDomainObject)` - SELECT WHERE WhereCondition
- `VratiSve(IDomainObject)` - SELECT * (sa specijalnim JOIN-ovima za Račune)
- `Dodaj(IDomainObject)` - INSERT (transakcije za Račune, SP za Kupce)
- `Azuriraj(IDomainObject)` - UPDATE WHERE WhereCondition
- `Obrisi(IDomainObject)` - DELETE WHERE WhereCondition

#### 3. **Controller**
Prepoznaje tip objekta iz JSON-a:
- Deserijalizuje u odgovarajući tip (Firma, FizickoLice, Racun...)
- Poziva odgovarajuću metodu DatabaseBroker-a
- Vraća Response sa rezultatom

#### 4. **Client UI**
- **Login.cs** - SK16 (Prijava)
- **Add.cs** - SK1, SK4, SK26 (Kreiranje)
- **Main.cs** - SK2, SK5 (Pretraga svih)
- **Details.cs** - SK2, SK5 (Prikaz jednog), SK6 (Ažuriranje), SK7 (Brisanje)
- **AddRacun.cs** - SK1 (Kreiranje računa sa stavkama)
- **DetailsRacun.cs** - SK2 (Prikaz računa)

---

## Primjeri korišćenja

### Kreiranje kupca (SK4)

```csharp
// Firma
Request req = new Request {
    Operation = Operation.DodajObjekat,
    Data = new Firma {
        Naziv = "Tech Solutions DOO",
        Popust = 0.15,
        Pib = "123456789",
        Adresa = "Bulevar Kralja Aleksandra 123",
        Partnerstvo = true
    }
};
Response res = CommunicationHelper.Instance.SendRequest(req);

// Fizičko lice
Request req = new Request {
    Operation = Operation.DodajObjekat,
    Data = new FizickoLice {
        ImePrezime = "Marko Marković",
        Popust = 0.05,
        Email = "marko@email.com",
        Telefon = "0641234567",
        LoyaltyClan = true
    }
};
```

### Pretraga kupaca (SK5)

```csharp
// Sve firme
Request req = new Request {
    Operation = Operation.VratiSve,
    Data = new Firma()
};
Response res = CommunicationHelper.Instance.SendRequest(req);
List<Firma> firme = JsonSerializer.Deserialize<List<Firma>>(res.Data.ToString());

// Jedno fizičko lice po ID-u
Request req = new Request {
    Operation = Operation.VratiJedan,
    Data = new FizickoLice { IdKupac = 3 }
};
```

### Brisanje kupca (SK7)

```csharp
Request req = new Request {
    Operation = Operation.ObrisiObjekat,
    Data = new Firma { IdKupac = 5 }
};
Response res = CommunicationHelper.Instance.SendRequest(req);
if (res.IsSuccessful) {
    MessageBox.Show("Kupac je uspešno obrisan!");
}
```

### Kreiranje računa (SK1)

```csharp
Racun noviRacun = new Racun {
    DatumIzdavanja = DateTime.Now,
    IdProdavac = Sesija.UlogovaniProdavac.IdProdavac,
    IdKupac = odabraniKupac.IdKupac,
    Stavke = new List<StavkaRacuna> {
        new StavkaRacuna {
            RbStavke = 1,
            Kolicina = 2,
            DatumKupovine = DateTime.Now,
            IdOprema = 10,
            Cena = 1000.00,
            Iznos = 2000.00,
            Oprema = odabranaOprema
        }
    }
};

Request req = new Request {
    Operation = Operation.DodajObjekat,
    Data = noviRacun
};
Response res = CommunicationHelper.Instance.SendRequest(req);
```

---

## Baza podataka

### Tabele

- **Kupac** (idKupac, popust) - bazna tabela
- **Firma** (idKupac PK+FK, naziv, pib, adresa, partnerstvo)
- **FizickoLice** (idKupac PK+FK, imePrezime, email, telefon, loyaltyClan)
- **Prodavac** (idProdavac, imePrezime, email, telefon, password, idSkladiste)
- **Racun** (idRacun, datumIzdavanja, konacanIznos, pdv, cenaSaPopustom, cenaStavke, idProdavac FK, idKupac FK)
- **StavkaRacuna** (idRacun FK, rbStavke, kolicina, datumKupovine, iznos, cena, idOprema FK)
- **Oprema** (idOprema, ime, kategorija, cena)
- **Skladiste** (idSkladiste, adresa)

### Stored Procedures

- **DodajFirmu** - Kreira Kupac + Firma u transakciji
- **DodajFizickoLice** - Kreira Kupac + FizickoLice u transakciji
- **AzurirajFirmu** - Ažurira oba zapisa
- **AzurirajFizickoLice** - Ažurira oba zapisa

### CASCADE DELETE

Kada se obriše Kupac (Firma ili Fizičko lice):
- Automatski se brišu svi Računi tog kupca
- Automatski se brišu sve Stavke povezanih računa

---

## Sigurnosne napomene

1. **Case-sensitive login** - Email i lozinka se provjeravaju sa velikim i malim slovima
2. **SQL Injection zaštita** - Koristi se SqlCommand (ne string concatenation)
3. **Transakcije** - Računi se kreiraju sa stavkama u ACID transakciji
4. **Validacija** - Client i Server provjeravaju podatke
5. **Cascade Delete** - Brisanje kupca briše sve njegove račune

---

## Proširenja

Sistem je dizajniran da je lako dodati nove operacije:

1. Dodaj novu vrijednost u `Operation` enum
2. Implementiraj case granu u `Controller.HandleRequest()`
3. Koristi postojeće DatabaseBroker metode ili dodaj nove po potrebi
4. Kreiraj UI komponentu u Client projektu

**Univerzalan pristup** znači da nove entitete možeš dodati bez izmjene postojećeg koda - samo implementiraj `IDomainObject` interface!
