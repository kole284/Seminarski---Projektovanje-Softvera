# 🖥️ SERVER Layer - Detaljno Objašnjenje

## 🎯 Uloga Server Layer-a

Server layer je **business logic tier** koji:
1. Prima TCP konekcije od klijenata
2. Deserijalizuje Request objekte
3. Izvršava business operacije kroz DatabaseBroker
4. Vraća Response objekte klijentima

**Komponente:**
- `Server.cs` - TCP Listener i Thread management
- `ClientHandler.cs` - Komunikacija sa pojedinačnim klijentom
- `Controller.cs` - Business logika i orchestration

---

## 🌐 Server.cs - TCP Server

### Uloga

Main entry point aplikacije. Pokreće TCP listener i kreira thread za svakog klijenta.

### Kompletan Kod

```csharp
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public class Server
    {
        private TcpListener listener;
        
        public void Start()
        {
            // 1. Kreiraj TCP listener na portu 5000
            listener = new TcpListener(IPAddress.Any, 5000);
            listener.Start();
            
            Console.WriteLine("Server je pokrenut na portu 5000...");
            Console.WriteLine("Čekam klijente...\n");
            
            // 2. Beskonačna petlja - prihvati klijente
            while (true)
            {
                try
                {
                    // 3. Blokiraj dok ne stigne klijent
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine($"Klijent konektovan: {client.Client.RemoteEndPoint}");
                    
                    // 4. Kreiraj handler za tog klijenta
                    ClientHandler handler = new ClientHandler(client);
                    
                    // 5. Startuj novi thread za komunikaciju
                    Thread clientThread = new Thread(handler.Handle);
                    clientThread.Start();
                    
                    Console.WriteLine("Thread startovan za klijenta.\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Greška pri prihvatanju klijenta: {ex.Message}");
                }
            }
        }
        
        public void Stop()
        {
            listener?.Stop();
            Console.WriteLine("Server zaustavljen.");
        }
    }
}
```

### Program.cs - Entry Point

```csharp
using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();
            
            // Program će biti blokiran u Start() metodi
            // Može se dodati graceful shutdown sa Console.ReadLine()
        }
    }
}
```

### Kako Radi?

```
1. [START] Server.Start() → TcpListener.Start()
   ↓
2. [LISTEN] listener.AcceptTcpClient() - čeka klijenta (blocking call)
   ↓
3. [CLIENT CONNECTS] Klijent se konektuje na 127.0.0.1:5000
   ↓
4. [ACCEPT] listener vraća TcpClient objekat
   ↓
5. [THREAD] Kreira novi thread sa ClientHandler(client)
   ↓
6. [LOOP] Vraća se na korak 2 (prihvata sledećeg klijenta)
```

### Multi-Threading Model

```
Server Process
│
├─ Thread 1: Main (AcceptTcpClient loop)
│
├─ Thread 2: ClientHandler za Klijenta A
│   └─ Deserialize Request → Controller → Serialize Response
│
├─ Thread 3: ClientHandler za Klijenta B
│   └─ Deserialize Request → Controller → Serialize Response
│
└─ Thread 4: ClientHandler za Klijenta C
    └─ ...
```

**Prednosti:**
- Svaki klijent ima svoj thread - nezavisna komunikacija
- Blokiranje jednog klijenta ne utiče na druge
- Podrška za više istovremenih klijenata

**Napomena:** Za production bi se koristio ThreadPool ili async/await.

---

## 🔌 ClientHandler.cs - Managed Client Connection

### Uloga

Upravlja komunikacijom sa **jednim klijentom**:
1. Prima Request objekte
2. Prosledi Controller-u
3. Vrati Response klijentu

### Kompletan Kod

```csharp
using Models;
using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server
{
    public class ClientHandler
    {
        private TcpClient client;
        private NetworkStream stream;
        private BinaryFormatter formatter;
        
        public ClientHandler(TcpClient client)
        {
            this.client = client;
            this.stream = client.GetStream();
            this.formatter = new BinaryFormatter();
        }
        
        public void Handle()
        {
            try
            {
                Console.WriteLine("ClientHandler: Započinjem komunikaciju...");
                
                // Beskonačna petlja - prima request-e dok je konekcija aktivna
                while (true)
                {
                    // 1. Primi Request od klijenta (BLOCKING)
                    Request req = ReceiveRequest();
                    
                    if (req == null)
                    {
                        Console.WriteLine("Klijent je zatvorio konekciju.");
                        break;
                    }
                    
                    Console.WriteLine($"Primljen request: {req.Operation}");
                    
                    // 2. Prosledi Controller-u i dobij Response
                    Response res = Controller.HandleRequest(req);
                    
                    // 3. Pošalji Response klijentu
                    SendResponse(res);
                    
                    Console.WriteLine($"Response poslat: {res.IsSuccessful}\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška u ClientHandler: {ex.Message}");
            }
            finally
            {
                // Cleanup
                stream?.Close();
                client?.Close();
                Console.WriteLine("Klijent diskonektovan.\n");
            }
        }
        
        private Request ReceiveRequest()
        {
            try
            {
                // Deserialize Request objekat iz stream-a
                object obj = formatter.Deserialize(stream);
                return obj as Request;
            }
            catch (IOException)
            {
                // Klijent je zatvorio konekciju
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška pri primanju request-a: {ex.Message}");
                return null;
            }
        }
        
        private void SendResponse(Response res)
        {
            try
            {
                // Serialize Response objekat u stream
                formatter.Serialize(stream, res);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška pri slanju response-a: {ex.Message}");
            }
        }
    }
}
```

### Serijalizacija

**BinaryFormatter** pretvara objekte u binarne podatke:

```
Request objekat    →  BinaryFormatter  →  byte[] stream  →  NetworkStream
                      [SERIALIZE]                             [SEND]

Response objekat   ←  BinaryFormatter  ←  byte[] stream  ←  NetworkStream
                      [DESERIALIZE]                           [RECEIVE]
```

### Communication Flow

```
[CLIENT]
   ↓ TCP Socket
[ClientHandler.ReceiveRequest()]
   ↓ Deserialize
[Request { Operation, Data }]
   ↓ Pass to
[Controller.HandleRequest(req)]
   ↓ Business Logic + DB
[Response { IsSuccessful, Data, Message }]
   ↓ Serialize
[ClientHandler.SendResponse()]
   ↓ TCP Socket
[CLIENT]
```

### Blocking Behavior

```csharp
while (true) {
    Request req = ReceiveRequest();  // ⏸️ BLOKIRA dok klijent ne pošalje
    Response res = Controller.HandleRequest(req);
    SendResponse(res);
}
```

Svaki klijent ima svoj thread, tako da blokiranje ne utiče na druge klijente.

---

## 🎮 Controller.cs - Business Logic Orchestrator

### Uloga

**Centralni dispatcher** koji:
1. Prima Request sa Operation + Data
2. Poziva odgovarajuću handler metodu
3. Komunicira sa DatabaseBroker-om
4. Vraća Response sa rezultatom

### Struktura

```csharp
using Broker;
using Models;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Server
{
    public class Controller
    {
        private static DatabaseBroker broker = new DatabaseBroker();
        
        public static Response HandleRequest(Request req)
        {
            try
            {
                switch (req.Operation)
                {
                    case Operation.Login:
                        return HandleLogin(req.Data);
                    
                    case Operation.VratiSve:
                        return HandleVratiSve(req.Data);
                    
                    case Operation.VratiJedan:
                        return HandleVratiJedan(req.Data);
                    
                    case Operation.Dodaj:
                        return HandleDodaj(req.Data);
                    
                    case Operation.Izmeni:
                        return HandleIzmeni(req.Data);
                    
                    case Operation.Obrisi:
                        return HandleObrisi(req.Data);
                    
                    case Operation.DodajRacun:
                        return HandleDodajRacun(req.Data);
                    
                    case Operation.IzmeniRacun:
                        return HandleIzmeniRacun(req.Data);
                    
                    default:
                        return new Response {
                            IsSuccessful = false,
                            Message = "Nepoznata operacija."
                        };
                }
            }
            catch (Exception ex)
            {
                return new Response {
                    IsSuccessful = false,
                    Message = $"Server greška: {ex.Message}\n{ex.StackTrace}"
                };
            }
        }
        
        // ... handler metode ...
    }
}
```

### HandleLogin(object data)

```csharp
private static Response HandleLogin(object data)
{
    try
    {
        // 1. Deserialize JSON → Prodavac
        string json = data.ToString();
        Prodavac prodavac = JsonSerializer.Deserialize<Prodavac>(json);
        
        // 2. Query baze za login
        IDomainObject result = broker.VratiJedan(prodavac);
        
        if (result != null)
        {
            // 3. Login uspešan - vrati Prodavca
            string resultJson = JsonSerializer.Serialize(result);
            return new Response {
                IsSuccessful = true,
                Data = resultJson,
                Message = "Login uspešan."
            };
        }
        else
        {
            // 4. Nevalidni kredencijali
            return new Response {
                IsSuccessful = false,
                Message = "Pogrešan email ili lozinka."
            };
        }
    }
    catch (Exception ex)
    {
        return new Response {
            IsSuccessful = false,
            Message = ex.Message
        };
    }
}
```

**Flow:**
```
Request { Operation.Login, Data = { Email: "john@mail.com", Password: "123" } }
   ↓
Deserialize → Prodavac objekat
   ↓
broker.VratiJedan(prodavac)
   ↓ SQL: SELECT * FROM Prodavac WHERE email='john@mail.com' AND password='123'
   ↓
Result? → Da: Serialize Prodavac → Response success
       → Ne: Response failed
```

### HandleVratiSve(object data)

```csharp
private static Response HandleVratiSve(object data)
{
    try
    {
        // 1. Deserialize dinamički - ne znamo tip unapred
        string json = data.ToString();
        IDomainObject obj = DeserializeDynamically(json);
        
        // 2. Query baze
        List<IDomainObject> results = broker.VratiSve(obj);
        
        // 3. Serialize rezultate
        string resultJson = JsonSerializer.Serialize(results);
        
        return new Response {
            IsSuccessful = true,
            Data = resultJson,
            Message = $"Pronađeno {results.Count} zapisa."
        };
    }
    catch (Exception ex)
    {
        return new Response {
            IsSuccessful = false,
            Message = ex.Message
        };
    }
}
```

**DeserializeDynamically() - Polymorphic Deserialization:**

```csharp
private static IDomainObject DeserializeDynamically(string json)
{
    // Prvo deserialize u JsonElement da vidimo tip
    JsonElement element = JsonSerializer.Deserialize<JsonElement>(json);
    
    // Pogledaj koja polja postoje da odredimo tip
    if (element.TryGetProperty("IdProdavac", out _))
        return JsonSerializer.Deserialize<Prodavac>(json);
    else if (element.TryGetProperty("IdRacun", out _))
        return JsonSerializer.Deserialize<Racun>(json);
    else if (element.TryGetProperty("Naziv", out _) && element.TryGetProperty("Pib", out _))
        return JsonSerializer.Deserialize<Firma>(json);
    // ... ostali tipovi
    
    throw new Exception("Nepoznat tip objekta.");
}
```

**Alternativno rešenje:** Client može poslati tip kao metadata:
```csharp
Request req = new Request {
    Operation = Operation.VratiSve,
    Data = new { Type = "Racun", Filter = racun }
};
```

### HandleVratiJedan(object data)

Isti pattern kao `HandleVratiSve`, ali poziva `broker.VratiJedan()`.

### HandleDodaj(object data)

```csharp
private static Response HandleDodaj(object data)
{
    try
    {
        IDomainObject obj = DeserializeDynamically(data.ToString());
        broker.Dodaj(obj);
        
        return new Response {
            IsSuccessful = true,
            Message = "Uspešno dodato."
        };
    }
    catch (Exception ex)
    {
        return new Response {
            IsSuccessful = false,
            Message = ex.Message
        };
    }
}
```

### HandleIzmeni(object data)

```csharp
private static Response HandleIzmeni(object data)
{
    try
    {
        IDomainObject obj = DeserializeDynamically(data.ToString());
        broker.Izmeni(obj);
        
        return new Response {
            IsSuccessful = true,
            Message = "Uspešno izmenjeno."
        };
    }
    catch (Exception ex)
    {
        return new Response {
            IsSuccessful = false,
            Message = ex.Message
        };
    }
}
```

### HandleObrisi(object data)

```csharp
private static Response HandleObrisi(object data)
{
    try
    {
        IDomainObject obj = DeserializeDynamically(data.ToString());
        broker.Obrisi(obj);
        
        return new Response {
            IsSuccessful = true,
            Message = "Uspešno obrisano."
        };
    }
    catch (Exception ex)
    {
        return new Response {
            IsSuccessful = false,
            Message = ex.Message
        };
    }
}
```

---

## 🧾 Specijalizovane Operacije: Računi

### HandleDodajRacun(object data)

**Složena transakciona operacija:**

```csharp
private static Response HandleDodajRacun(object data)
{
    try
    {
        // 1. Deserialize Racun sa Stavkama
        string json = data.ToString();
        Racun racun = JsonSerializer.Deserialize<Racun>(json);
        
        // 2. Započni transakciju
        broker.ZapocniTransakciju();
        
        try
        {
            // 3. Dodaj račun (INSERT INTO Racun)
            broker.Dodaj(racun);
            
            // 4. Uzmi SCOPE_IDENTITY() - ID novog računa
            int idRacun = GetLastInsertedId();
            
            // 5. Dodaj sve stavke
            foreach (StavkaRacuna stavka in racun.Stavke)
            {
                stavka.IdRacun = idRacun;
                broker.Dodaj(stavka);
            }
            
            // 6. Commit - sve je uspelo
            broker.Commit();
            
            return new Response {
                IsSuccessful = true,
                Message = "Račun uspešno dodat."
            };
        }
        catch
        {
            // 7. Rollback ako bilo šta failuje
            broker.Rollback();
            throw;
        }
    }
    catch (Exception ex)
    {
        return new Response {
            IsSuccessful = false,
            Message = ex.Message
        };
    }
}
```

**GetLastInsertedId():**

```csharp
private static int GetLastInsertedId()
{
    string sql = "SELECT SCOPE_IDENTITY()";
    SqlCommand cmd = new SqlCommand(sql, broker.Connection, broker.Transaction);
    return Convert.ToInt32(cmd.ExecuteScalar());
}
```

**Flow:**
```
1. Primi Racun { Prodavac=1, Kupac=2, Stavke=[...] }
   ↓
2. BEGIN TRANSACTION
   ↓
3. INSERT INTO Racun (datumIzdavanja, ..., idProdavac, idKupac) VALUES (...)
   → SCOPE_IDENTITY() → idRacun = 15
   ↓
4. INSERT INTO StavkaRacuna (idRacun, idOprema, kolicina, cena) VALUES (15, 3, 2, 1000)
5. INSERT INTO StavkaRacuna (idRacun, idOprema, kolicina, cena) VALUES (15, 5, 1, 500)
   ↓
6. COMMIT TRANSACTION
   ↓
7. Response { IsSuccessful = true }
```

**Ako failuje korak 5:**
```
4. ✅ StavkaRacuna #1 inserted
5. ❌ Greška (npr. idOprema ne postoji)
   ↓
ROLLBACK TRANSACTION  ← Sve se poništava!
   ↓
Response { IsSuccessful = false, Message = "FK constraint violation" }
```

### HandleIzmeniRacun(object data)

Slično kao `HandleDodajRacun`, ali:
1. `UPDATE Racun SET ... WHERE idRacun = X`
2. `DELETE FROM StavkaRacuna WHERE idRacun = X`
3. `INSERT` nove stavke

---

## 🔐 Handling Many-to-Many (Prodavac ↔ Skladiste)

### HandleDodajProdavacSaSkladistima(object data)

```csharp
private static Response HandleDodajProdavacSaSkladistima(object data)
{
    try
    {
        var obj = JsonSerializer.Deserialize<ProdavacWithSkladista>(data.ToString());
        
        broker.ZapocniTransakciju();
        
        try
        {
            // 1. Dodaj Prodavca
            broker.Dodaj(obj.Prodavac);
            int idProdavac = GetLastInsertedId();
            
            // 2. Dodaj ProdSklad zapise
            foreach (int idSkladiste in obj.SkladistaIds)
            {
                ProdSklad ps = new ProdSklad {
                    IdProdavac = idProdavac,
                    IdSkladiste = idSkladiste
                };
                broker.Dodaj(ps);
            }
            
            broker.Commit();
            
            return new Response {
                IsSuccessful = true,
                Message = "Prodavac sa skladištima dodat."
            };
        }
        catch
        {
            broker.Rollback();
            throw;
        }
    }
    catch (Exception ex)
    {
        return new Response {
            IsSuccessful = false,
            Message = ex.Message
        };
    }
}
```

---

## 🛡️ Error Handling Strategy

### Try-Catch na Tri Nivoa

```
1. [Controller.HandleRequest] - Outer catch
   │
   ├─→ 2. [HandleLogin] - Operation-specific catch
   │      │
   │      └─→ 3. [broker.VratiJedan] - DB exception
   │
   └─→ Response { IsSuccessful = false, Message = "..." }
```

### Exception Types

```csharp
catch (SqlException sqlEx) {
    // Database errors (constraint violations, timeouts)
    return new Response {
        IsSuccessful = false,
        Message = $"Greška u bazi: {sqlEx.Message}"
    };
}
catch (JsonException jsonEx) {
    // Deserialization errors
    return new Response {
        IsSuccessful = false,
        Message = "Neispravan format podataka."
    };
}
catch (Exception ex) {
    // General errors
    return new Response {
        IsSuccessful = false,
        Message = $"Greška: {ex.Message}"
    };
}
```

---

## 📊 Logging Strategy

### Console Output

```csharp
Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Klijent konektovan");
Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Request: {req.Operation}");
Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Response: {res.IsSuccessful}");
```

### Production Logging

Za production bi se koristio logger framework (Serilog, NLog):

```csharp
_logger.Information("Klijent konektovan: {EndPoint}", client.RemoteEndPoint);
_logger.Warning("Login failed za: {Email}", prodavac.Email);
_logger.Error(ex, "Greška pri obradi request-a");
```

---

## 🔧 Configuration

### Connection String

Trenutno hardcoded u DatabaseBroker:
```csharp
"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SeminarskiDB;Integrated Security=True"
```

**Best Practice:** Koristiti `appsettings.json` ili environment variables:

```json
{
  "ConnectionStrings": {
    "Default": "Data Source=...;Initial Catalog=...;"
  },
  "Server": {
    "Port": 5000,
    "MaxClients": 100
  }
}
```

```csharp
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

string connectionString = config.GetConnectionString("Default");
int port = config.GetValue<int>("Server:Port");
```

---

## 🚀 Performance Considerations

### 1. Thread Pool vs Manual Threads

**Trenutno:**
```csharp
Thread clientThread = new Thread(handler.Handle);
clientThread.Start();
```

**Production:**
```csharp
ThreadPool.QueueUserWorkItem(_ => handler.Handle());
```

Ili async/await:
```csharp
Task.Run(() => handler.Handle());
```

### 2. DatabaseBroker Instance per Request

**Trenutno:** Static instance u Controller-u - **NOT THREAD SAFE!**

**Trebalo bi:**
```csharp
public static Response HandleRequest(Request req) {
    using (DatabaseBroker broker = new DatabaseBroker()) {
        // ... operacije ...
    }  // Auto-dispose connection
}
```

### 3. Connection Pooling

ADO.NET automatski koristi connection pooling, ali bi trebalo:
- Zatvoriti konekcije nakon korišćenja
- Koristiti `using` statements

---

## 📝 Best Practices - Summary

✅ **Use Transactions** za multi-table operacije  
✅ **Handle Exceptions** na svim nivoima  
✅ **Close Connections** nakon korišćenja  
✅ **Thread Safety** - nova DatabaseBroker instanca per request  
✅ **Logging** - loguj sve bitne operacije  
✅ **Configuration** - externalize connection strings  
✅ **Async I/O** - koristiti async/await za network operacije  

---

## 🎯 Zaključak

Server layer implementira robusnu **multi-threaded client-server arhitekturu** sa:

✅ **TCP Communication** - BinaryFormatter serijalizacija  
✅ **Thread per Client** - izolacija klijenata  
✅ **Controller Pattern** - centralni dispatcher za operacije  
✅ **Transakcije** - atomične operacije za složene use-case-ove  
✅ **Error Handling** - graceful degradation  

Server je **centralni hub** koji orkestrira sve business operacije između klijenata i baze podataka!
