# 🏗️ Arhitektura Sistema - Kompletna Dokumentacija

## 📋 Pregled Sistema

Sistem za upravljanje prodavnicom je **3-tier client-server aplikacija** napisana u C# .NET 10.0 sa SQL Server bazom podataka.

```
┌─────────────────────────────────────────────────────────────┐
│                        CLIENT TIER                          │
│  (WinForms - Prezentacioni sloj)                           │
│  - Login.cs, Main.cs, Add.cs, Details.cs                   │
│  - AddRacun.cs, DetailsRacun.cs                            │
└─────────────┬───────────────────────────────────────────────┘
              │ TCP/IP Socket Communication
              │ (Serijalizovani Request/Response objekti)
              ↓
┌─────────────────────────────────────────────────────────────┐
│                        SERVER TIER                          │
│  (Business Logic Layer)                                     │
│  - Server.cs (TCP Listener)                                │
│  - ClientHandler.cs (Thread po klijentu)                   │
│  - Controller.cs (Operacije i validacija)                  │
└─────────────┬───────────────────────────────────────────────┘
              │ Direktni pozivi metoda
              ↓
┌─────────────────────────────────────────────────────────────┐
│                        BROKER TIER                          │
│  (Data Access Layer)                                        │
│  - DatabaseBroker.cs (Template Method Pattern)             │
│  - Generički CRUD operacije                                │
└─────────────┬───────────────────────────────────────────────┘
              │ ADO.NET (SqlConnection, SqlCommand)
              ↓
┌─────────────────────────────────────────────────────────────┐
│                     SQL SERVER DATABASE                     │
│  Tabele: Prodavac, Kupac, Firma, FizickoLice,             │
│          Racun, StavkaRacuna, Oprema, Skladiste            │
└─────────────────────────────────────────────────────────────┘
```

## 🔄 Komunikacioni Protokol

### Request-Response Pattern

```csharp
// Request objekat (Client → Server)
public class Request {
    public Operation Operation { get; set; }  // Tip operacije
    public object Data { get; set; }          // Payload data
}

// Response objekat (Server → Client)
public class Response {
    public bool IsSuccessful { get; set; }    // Da li je uspelo
    public object Data { get; set; }          // Rezultat
    public string Message { get; set; }       // Poruka (greška ili success)
}
```

### Tip Operacija

```csharp
public enum Operation {
    Login,           // Autentifikacija prodavca
    VratiSve,        // SELECT sve zapise sa filterima
    VratiJedan,      // SELECT jedan zapis po ID-u
    Dodaj,           // INSERT novi zapis
    Izmeni,          // UPDATE postojeći zapis
    Obrisi,          // DELETE zapis
    // Specijalizovane operacije
    DodajRacun,      // Transakciona operacija za dodavanje računa sa stavkama
    IzmeniRacun      // Transakciona operacija za izmenu računa
}
```

## 🎯 MODELS Layer (Models.csproj)

### Shared Domain Models

Svi modeli implementiraju `IDomainObject` interfejs za **Template Method Pattern**.

```csharp
public interface IDomainObject {
    string TableName { get; }           // Naziv tabele u bazi
    string InsertValues { get; }        // VALUES za INSERT
    string UpdateValues { get; }        // SET klauzula za UPDATE
    string WhereCondition { get; }      // WHERE za SELECT
    string GetCustomSelectQuery();      // Custom JOIN query
    List<IDomainObject> GetList(SqlDataReader reader); // Mapiranje rezultata
}
```

### Domain Model Hijerarhija

```
IDomainObject (interface)
    ├── Prodavac          // Prodavci koji izdaju račune
    ├── Skladiste         // Skladišta opreme
    ├── Oprema            // Proizvodi za prodaju
    ├── KategorijaOpreme  // Kategorije proizvoda (enum sa atributima)
    ├── Kupac (apstraktna)
    │   ├── Firma         // Pravna lica
    │   └── FizickoLice   // Fizička lica
    ├── Racun             // Fakture/Računi
    ├── StavkaRacuna      // Stavke na računu (line items)
    └── ProdSklad         // Many-to-many veza Prodavac-Skladiste
```

### Request/Response Modeli

```csharp
public class Request {
    public Operation Operation { get; set; }
    public object Data { get; set; }
}

public class Response {
    public bool IsSuccessful { get; set; }
    public object Data { get; set; }
    public string Message { get; set; }
}
```

### View Modeli (Samo za prikaz)

```csharp
public class ProdavacView    // Prodavac sa concat skladišta
public class KupacView       // Unified view firma + fiz.lice
```

## 🗄️ BROKER Layer (Broker.csproj)

### DatabaseBroker - Generički Data Access

**Template Method Pattern** - apstraktne operacije se specijalizuju kroz domain objekte.

```csharp
public class DatabaseBroker {
    private SqlConnection connection;
    private SqlTransaction transaction;
    
    // Template metode
    public List<IDomainObject> VratiSve(IDomainObject obj);
    public IDomainObject VratiJedan(IDomainObject obj);
    public void Dodaj(IDomainObject obj);
    public void Izmeni(IDomainObject obj);
    public void Obrisi(IDomainObject obj);
    
    // Transakcije
    public void ZapocniTransakciju();
    public void Commit();
    public void Rollback();
}
```

**Ključne karakteristike:**
1. **Type-agnostic** - radi sa bilo kojim IDomainObject
2. **Custom queries** - podržava kompleksne JOIN upite
3. **Filter sistem** - WHERE klauzule generisane iz WhereCondition property-ja
4. **Transakcije** - podrška za višestruke operacije u jednoj transakciji

## 🖥️ SERVER Layer (Server.csproj)

### Server.cs - TCP Listener

```csharp
public class Server {
    private TcpListener listener;
    
    public void Start() {
        listener = new TcpListener(IPAddress.Any, 5000);
        listener.Start();
        
        while (true) {
            TcpClient client = listener.AcceptTcpClient();
            ClientHandler handler = new ClientHandler(client);
            Thread thread = new Thread(handler.Handle);
            thread.Start();  // Novi thread za svakog klijenta
        }
    }
}
```

### ClientHandler.cs - Managed Client Connection

```csharp
public class ClientHandler {
    private TcpClient client;
    private NetworkStream stream;
    
    public void Handle() {
        while (true) {
            // 1. Primi Request od klijenta
            Request req = DeserializeRequest(stream);
            
            // 2. Prosledi Controller-u
            Response res = Controller.HandleRequest(req);
            
            // 3. Vrati Response klijentu
            SerializeResponse(res, stream);
        }
    }
}
```

### Controller.cs - Business Logic

```csharp
public class Controller {
    private DatabaseBroker broker = new DatabaseBroker();
    
    public Response HandleRequest(Request req) {
        try {
            switch (req.Operation) {
                case Operation.Login:
                    return HandleLogin(req.Data);
                case Operation.VratiSve:
                    return HandleVratiSve(req.Data);
                case Operation.Dodaj:
                    return HandleDodaj(req.Data);
                case Operation.DodajRacun:
                    return HandleDodajRacun(req.Data);
                // ... ostale operacije
            }
        } catch (Exception ex) {
            return new Response { 
                IsSuccessful = false, 
                Message = ex.Message 
            };
        }
    }
}
```

**Ključne odgovornosti:**
1. Validacija business pravila
2. Koordinacija transakcija
3. Error handling
4. Logika specifična za operacije (npr. DodajRacun sa stavkama)

## 🖼️ CLIENT Layer (Client.csproj)

### Forme

#### 1. Login.cs
**Uloga:** Autentifikacija prodavaca

```csharp
private void button1_Click() {
    Prodavac p = new Prodavac { 
        Email = txtEmail.Text, 
        Password = txtPassword.Text 
    };
    Request req = new Request { 
        Operation = Operation.Login, 
        Data = p 
    };
    Response res = CommunicationHelper.Instance.SendRequest(req);
    
    if (res.IsSuccessful) {
        Sesija.UlogovaniProdavac = DeserializeFromJson(res.Data);
        Main main = new Main();
        main.Show();
    }
}
```

#### 2. Main.cs
**Uloga:** Glavni ekran sa DataGridView prikaz svih entiteta

**Funkcionalnosti:**
- ComboBox za izbor entiteta (Prodavci, Firme, Fizička lica, Oprema, Skladišta, Računi)
- Filter sistem (txtFilter1, txtFilter2, cmbProdavac, cmbKupac)
- Prikaz podataka u DataGridView
- Navigacija na Add/Details forme

**Filter logika:**
```csharp
private void button1_Click() {
    string izbor = cmbEntitet.SelectedItem.ToString();
    IDomainObject model = null;
    
    switch (izbor) {
        case "Računi":
            model = new Racun();
            if (IsValidFilter(txtFilter1.Text))
                ((Racun)model).IdRacun = int.Parse(txtFilter1.Text);
            if (IsValidFilter(txtFilter2.Text))
                ((Racun)model).DatumFilter = txtFilter2.Text;
            // + Prodavac i Kupac filtri iz ComboBox-eva
            break;
    }
    
    Request req = new Request { Operation = Operation.VratiSve, Data = model };
    Response res = CommunicationHelper.Instance.SendRequest(req);
    dgvData.DataSource = DeserializeList(res.Data);
}
```

#### 3. Add.cs
**Uloga:** Dodavanje novih entiteta (generička forma)

**Dinamički kontrole:**
- Automatski prikazuje polja na osnovu izabranog entiteta
- Checkbox liste za Prodavac-Skladište many-to-many vezu

#### 4. Details.cs
**Uloga:** Prikaz i izmena postojećih entiteta

#### 5. AddRacun.cs / DetailsRacun.cs
**Uloga:** Specijalizovane forme za kompleksnu operaciju dodavanja računa

**Workflow:**
1. Izbor kupca (Firma ili Fizičko lice)
2. Dodavanje stavki u listu (Oprema + količina)
3. Automatski izračun PDV, popusta, ukupne cene
4. Slanje jednog transakcionalnog request-a sa račun + sve stavke

### CommunicationHelper.cs - Singleton Pattern

```csharp
public class CommunicationHelper {
    private static CommunicationHelper _instance;
    private TcpClient client;
    private NetworkStream stream;
    
    public static CommunicationHelper Instance {
        get {
            if (_instance == null)
                _instance = new CommunicationHelper();
            return _instance;
        }
    }
    
    public Response SendRequest(Request req) {
        // Serialize request → Send → Receive → Deserialize response
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, req);
        Response res = (Response)formatter.Deserialize(stream);
        return res;
    }
}
```

**Konfiguracija:**
```csharp
public void Connect() {
    client = new TcpClient("127.0.0.1", 5000);
    stream = client.GetStream();
}
```

## 🔐 Sesija (Session Management)

```csharp
public static class Sesija {
    public static Prodavac UlogovaniProdavac { get; set; }
    public static bool IsAdmin { get; set; }
}
```

Cuva stanje trenutno ulogovanog korisnika kroz celu aplikaciju.

## 🗂️ Dizajn Paterni Korišćeni

### 1. **Template Method Pattern**
- `IDomainObject` interfejs definiše contract
- `DatabaseBroker` ima template metode koje koriste domain object properties
- Svaki domain model specijalizuje ponašanje kroz properties

### 2. **Singleton Pattern**
- `CommunicationHelper` - jedna instanca TCP konekcije kroz ceo klijent

### 3. **Repository Pattern**
- `DatabaseBroker` deluje kao generic repository za sve entitete

### 4. **Data Transfer Object (DTO)**
- `Request` i `Response` za serijalizaciju preko mreže

### 5. **Three-Tier Architecture**
- Separation of Concerns: Presentation → Business Logic → Data Access

## 📊 Baza Podataka - Table Per Type Nasledjivanje

```sql
-- Bazna tabela
Kupac (idKupac PK)

-- Specijalizovane tabele
Firma (idKupac PK,FK → Kupac, naziv, pib, adresa)
FizickoLice (idKupac PK,FK → Kupac, imePrezime, email, telefon)
```

**Razlog:** Firma i FizickoLice dele idKupac ali imaju različita polja.

## 🔄 Tipičan Request Flow

```
1. [CLIENT] User klikne "Prikaži" na Main formi
   ↓
2. [CLIENT] Kreira Racun objekat sa filter properties
   ↓
3. [CLIENT] Kreira Request { Operation.VratiSve, Data = racun }
   ↓
4. [CLIENT] CommunicationHelper.SendRequest(req) - serialize preko TCP
   ↓
5. [SERVER] ClientHandler primi request
   ↓
6. [SERVER] Controller.HandleRequest(req)
   ↓
7. [SERVER] Controller.HandleVratiSve(racun)
   ↓
8. [BROKER] DatabaseBroker.VratiSve(racun)
   ↓
9. [BROKER] Generise SQL:
   - query = racun.GetCustomSelectQuery() 
   - where = racun.WhereCondition
   - Finalni SQL: query + " WHERE " + where
   ↓
10. [DATABASE] Izvrši SQL query
   ↓
11. [BROKER] racun.GetList(reader) - mapira SqlDataReader → List<Racun>
   ↓
12. [SERVER] Return Response { IsSuccessful = true, Data = racuni }
   ↓
13. [CLIENT] CommunicationHelper prima response
   ↓
14. [CLIENT] Deserializuje List<Racun> iz response.Data
   ↓
15. [CLIENT] dgvData.DataSource = racuni
   ↓
16. [CLIENT] User vidi podatke u DataGridView
```

## 🚀 Startovanje Sistema

1. **Start SQL Server** i pokreni `recreate_database_full.sql`
2. **Start Server.exe** - sluša na portu 5000
3. **Start Client.exe** - konektuje se na localhost:5000
4. **Login** sa admin/admin ili email/password prodavca

## 📁 Struktura Projekta

```
Seminarski/
├── Models/              // Shared domain models
│   ├── IDomainObject.cs
│   ├── Prodavac.cs, Kupac.cs, Firma.cs, FizickoLice.cs
│   ├── Racun.cs, StavkaRacuna.cs
│   ├── Oprema.cs, Skladiste.cs, ProdSklad.cs
│   ├── Request.cs, Response.cs, Operation.cs
│   └── View modeli (ProdavacView, KupacView)
│
├── Broker/              // Data Access Layer
│   └── DatabaseBroker.cs
│
├── Server/              // Business Logic + TCP Server
│   ├── Server.cs
│   ├── ClientHandler.cs
│   └── Controller.cs
│
├── Client/              // WinForms UI
│   ├── Login.cs, Main.cs
│   ├── Add.cs, Details.cs
│   ├── AddRacun.cs, DetailsRacun.cs
│   ├── CommunicationHelper.cs
│   └── Sesija.cs
│
├── queries/             // SQL skripte
│   └── recreate_database_full.sql
│
└── uputstva/            // Dokumentacija
    └── (ova datoteka)
```

## 🔧 Tehnologije

- **Framework:** .NET 10.0
- **UI:** Windows Forms
- **Database:** SQL Server (ADO.NET)
- **Network:** TCP/IP Sockets
- **Serialization:** BinaryFormatter / JSON (System.Text.Json)
- **Pattern:** Template Method, Singleton, Repository, 3-Tier

## ⚡ Performance Optimizacije

1. **Custom JOIN queries** umesto N+1 problema
2. **Multi-threading** - svaki klijent u svom thread-u
3. **Connection pooling** (ADO.NET default)
4. **Transakcije** za atomičke operacije (npr. Racun + StavkaRacuna)

## 🛡️ Error Handling

- Try-catch na svim slojevima
- Response.IsSuccessful flag za status
- Response.Message za user-friendly poruke
- Transaction rollback pri greškama

## 📝 Zaključak

Sistem je dizajniran po industry best practices:
- **Loose coupling** između slojeva
- **High cohesion** unutar svake komponente
- **Reusable** - Template Method omogućava lako dodavanje novih entiteta
- **Maintainable** - jasna separacija odgovornosti
- **Scalable** - multi-threaded server, može podržati mnogo klijenata
