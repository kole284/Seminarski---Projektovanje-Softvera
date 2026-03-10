# 🗄️ DATABASE BROKER - Template Method Pattern Implementacija

## 🎯 Uloga DatabaseBroker-a

DatabaseBroker je **generic data access layer** koji koristi **Template Method Pattern** za rad sa bilo kojim domain objektom koji implementira `IDomainObject` interfejs.

**Ključne prednosti:**
- **Type-agnostic** - isti kod radi za Prodavac, Racun, Oprema, etc.
- **DRY princip** - nema dupliranja koda za CRUD operacije
- **Single Responsibility** - samo data access, bez business logike
- **Transakcije** - podrška za složene multi-table operacije

---

## 📦 Klasa DatabaseBroker

```csharp
using Microsoft.Data.SqlClient;
using Models;
using System;
using System.Collections.Generic;

namespace Broker
{
    public class DatabaseBroker
    {
        private SqlConnection connection;
        private SqlTransaction transaction;
        
        // Constructor - otvara konekciju ka bazi
        public DatabaseBroker()
        {
            connection = new SqlConnection(
                @"Data Source=(localdb)\MSSQLLocalDB;
                  Initial Catalog=SeminarskiDB;
                  Integrated Security=True"
            );
            connection.Open();
        }
        
        // ... metode ...
    }
}
```

### Connection String Breakdown

```
Data Source=(localdb)\MSSQLLocalDB  → LocalDB SQL Server instanca
Initial Catalog=SeminarskiDB        → Ime baze podataka
Integrated Security=True            → Windows Authentication
```

---

## 🔧 Template Method Pattern - Kako Funkcioniše

### Koncept

Template Method definiše **skeleton algoritma** u baznoj klasi ali dozvoljava subklasama da override-uju specifične korake.

U našem slučaju:
- **Template metode** su u `DatabaseBroker` (npr. `VratiSve`, `Dodaj`)
- **Specijalizovani koraci** dolaze iz `IDomainObject` properties

### Dijagram Flow-a

```
[CLIENT]
   ↓ poziva
[DatabaseBroker.VratiSve(IDomainObject obj)]
   ↓
   1. query = obj.GetCustomSelectQuery()  ← Domain-specific
   2. where = obj.WhereCondition           ← Domain-specific
   3. SQL = query + " WHERE " + where
   4. Execute SQL
   5. result = obj.GetList(reader)         ← Domain-specific
   ↓
[CLIENT]
```

---

## 🛠️ Metode DatabaseBroker-a

### 1. VratiSve(IDomainObject obj)

**Svrha:** SELECT sve zapise sa optional filterima.

```csharp
public List<IDomainObject> VratiSve(IDomainObject obj)
{
    // 1. Uzmi custom SELECT query od domain objekta
    string query = obj.GetCustomSelectQuery();
    
    // 2. Proveri da li query već ima WHERE (npr. u subquery-ju)
    string queryUpper = query.ToUpper();
    int lastFromIndex = queryUpper.LastIndexOf(" FROM ");
    bool hasMainWhere = lastFromIndex > 0 && 
                        queryUpper.IndexOf(" WHERE ", lastFromIndex) > lastFromIndex;
    
    // 3. Dodaj WHERE klauzulu ako postoje filteri
    string whereCondition = obj.WhereCondition;
    if (!string.IsNullOrEmpty(whereCondition))
    {
        if (hasMainWhere)
            query += $" AND ({whereCondition})";  // Već ima WHERE
        else
            query += $" WHERE {whereCondition}";   // Nema WHERE
    }
    
    // 4. Izvrši SQL query
    SqlCommand cmd = new SqlCommand(query, connection, transaction);
    SqlDataReader reader = cmd.ExecuteReader();
    
    // 5. Mapiranje SqlDataReader → List<Domain> korišćenjem obj.GetList()
    List<IDomainObject> result = obj.GetList(reader);
    reader.Close();
    
    return result;
}
```

#### Primer Korišćenja

```csharp
// Tražimo sve Prodavce sa slovom "a" u imenu
Prodavac filter = new Prodavac { ImePrezime = "a" };
List<IDomainObject> prodavci = broker.VratiSve(filter);

// Template Method poziva:
// - filter.GetCustomSelectQuery() → SELECT p.*, skladista FROM Prodavac p ...
// - filter.WhereCondition → "CONCAT(p.ime, ' ', p.prezime) LIKE '%a%'"
// - Finalni SQL: SELECT... FROM Prodavac p WHERE CONCAT(...) LIKE '%a%'
// - filter.GetList(reader) → mapira SqlDataReader u List<Prodavac>
```

#### Složen Primer - Racun sa Filterima

```csharp
Racun filter = new Racun {
    DatumFilter = "2026-02",                      // Svi računi iz februara 2026
    Prodavac = new Prodavac { IdProdavac = 1 },   // Od prodavca #1
    Kupac = new Kupac { IdKupac = 3 }             // Za kupca #3
};

List<IDomainObject> racuni = broker.VratiSve(filter);

// Template Method poziva:
// - filter.GetCustomSelectQuery() → 
//     SELECT r.*, CONCAT(p.ime, ' ', p.prezime), ISNULL(f.naziv, fl.imePrezime)
//     FROM Racun r
//     JOIN Prodavac p ON r.idProdavac = p.idProdavac
//     JOIN Kupac k ON r.idKupac = k.idKupac
//     LEFT JOIN Firma f ON k.idKupac = f.idKupac
//     LEFT JOIN FizickoLice fl ON k.idKupac = fl.idKupac
//
// - filter.WhereCondition →
//     "r.idProdavac = 1 AND r.idKupac = 3 AND FORMAT(r.datumIzdavanja, 'yyyy-MM') = '2026-02'"
//
// - Finalni SQL: (joined query) WHERE (conditions)
```

### Why LastIndexOf(" FROM ")?

**Problem:** Prodavac ima STUFF subquery koji sadrži WHERE:
```sql
SELECT 
    p.*,
    STUFF((
        SELECT ', ' + s.naziv
        FROM ProdSklad ps
        WHERE ps.idProdavac = p.idProdavac  ← WHERE u subquery-ju!
        ...
    ), ...) AS skladista
FROM Prodavac p
```

**Rešenje:** Proveravamo WHERE samo **nakon poslednjeg FROM** (glavni query):
```csharp
int lastFromIndex = queryUpper.LastIndexOf(" FROM ");
bool hasMainWhere = queryUpper.IndexOf(" WHERE ", lastFromIndex) > lastFromIndex;
```

---

### 2. VratiJedan(IDomainObject obj)

**Svrha:** SELECT jedan zapis po ID-u (ili drugom unique kriterijumu).

```csharp
public IDomainObject VratiJedan(IDomainObject obj)
{
    string query = obj.GetCustomSelectQuery();
    
    // Logika za WHERE ista kao u VratiSve
    string queryUpper = query.ToUpper();
    int lastFromIndex = queryUpper.LastIndexOf(" FROM ");
    bool hasMainWhere = lastFromIndex > 0 && 
                        queryUpper.IndexOf(" WHERE ", lastFromIndex) > lastFromIndex;
    
    string whereCondition = obj.WhereCondition;
    if (!string.IsNullOrEmpty(whereCondition))
    {
        if (hasMainWhere)
            query += $" AND ({whereCondition})";
        else
            query += $" WHERE {whereCondition}";
    }
    
    SqlCommand cmd = new SqlCommand(query, connection, transaction);
    SqlDataReader reader = cmd.ExecuteReader();
    
    List<IDomainObject> list = obj.GetList(reader);
    reader.Close();
    
    // Vrati prvi (i jedini) rezultat ili null
    return list.Count > 0 ? list[0] : null;
}
```

#### Primer

```csharp
// Tražimo Prodavca sa email=john@mail.com i password=123
Prodavac loginData = new Prodavac { 
    Email = "john@mail.com", 
    Password = "123" 
};

IDomainObject result = broker.VratiJedan(loginData);

// Template Method poziva:
// - loginData.WhereCondition → "p.email='john@mail.com' AND p.password='123'"
// - result će biti taj Prodavac ili null ako ne postoji
```

---

### 3. Dodaj(IDomainObject obj)

**Svrha:** INSERT novog zapisa.

```csharp
public void Dodaj(IDomainObject obj)
{
    // 1. Generiši INSERT statement
    string sql = $"INSERT INTO {obj.TableName} VALUES ({obj.InsertValues})";
    
    // 2. Izvrši
    SqlCommand cmd = new SqlCommand(sql, connection, transaction);
    cmd.ExecuteNonQuery();
}
```

#### Generisani SQL Primer

```csharp
Firma firma = new Firma {
    Naziv = "Tech Corp",
    Pib = "123456789",
    Adresa = "Beograd 123"
};
broker.Dodaj(firma);

// Template Method poziva:
// - firma.TableName → "Firma"
// - firma.InsertValues → "'Tech Corp', '123456789', 'Beograd 123'"
// - SQL: INSERT INTO Firma VALUES ('Tech Corp', '123456789', 'Beograd 123')
```

**Napomena:** ID se generiše automatski (IDENTITY kolona u SQL Server-u).

---

### 4. Izmeni(IDomainObject obj)

**Svrha:** UPDATE postojećeg zapisa.

```csharp
public void Izmeni(IDomainObject obj)
{
    // 1. Generiši UPDATE statement
    string sql = $"UPDATE {obj.TableName} SET {obj.UpdateValues} WHERE {obj.WhereCondition}";
    
    // 2. Izvrši
    SqlCommand cmd = new SqlCommand(sql, connection, transaction);
    cmd.ExecuteNonQuery();
}
```

#### Generisani SQL Primer

```csharp
Prodavac prodavac = new Prodavac {
    IdProdavac = 5,
    Ime = "Marko",
    Prezime = "Markovic",
    Email = "marko@mail.com",
    Telefon = "063123456",
    Password = "newpass"
};
broker.Izmeni(prodavac);

// Template Method poziva:
// - prodavac.TableName → "Prodavac"
// - prodavac.UpdateValues → "ime='Marko', prezime='Markovic', ..."
// - prodavac.WhereCondition → "p.idProdavac = 5" (jer IdProdavac > 0)
// - SQL: UPDATE Prodavac SET ime='Marko', ... WHERE p.idProdavac = 5
```

**Važno:** WhereCondition **mora** vratiti unique identifier (npr. ID).

---

### 5. Obrisi(IDomainObject obj)

**Svrha:** DELETE zapisa.

```csharp
public void Obrisi(IDomainObject obj)
{
    // 1. Generiši DELETE statement
    string sql = $"DELETE FROM {obj.TableName} WHERE {obj.WhereCondition}";
    
    // 2. Izvrši
    SqlCommand cmd = new SqlCommand(sql, connection, transaction);
    cmd.ExecuteNonQuery();
}
```

#### Generisani SQL Primer

```csharp
Skladiste skladiste = new Skladiste { IdSkladiste = 10 };
broker.Obrisi(skladiste);

// Template Method poziva:
// - skladiste.TableName → "Skladiste"
// - skladiste.WhereCondition → "s.idSkladiste = 10"
// - SQL: DELETE FROM Skladiste WHERE s.idSkladiste = 10
```

**Zaštita od brisanja:** Baza ima ON DELETE NO ACTION constraints za foreign keys.

---

## 🔄 Transakcioni Metodi

### Problema: Atomičnost Operacija

Npr. dodavanje Računa sa StavkaRacuna:
1. INSERT INTO Racun
2. SELECT SCOPE_IDENTITY() (dobaviti novi ID)
3. INSERT INTO StavkaRacuna (svaka stavka)

**Šta ako korak 3 failuje?** → Ostaje nepotpun račun u bazi!

**Rešenje:** Transakcije

### ZapocniTransakciju()

```csharp
public void ZapocniTransakciju()
{
    transaction = connection.BeginTransaction();
}
```

### Commit()

```csharp
public void Commit()
{
    transaction?.Commit();
    transaction = null;
}
```

### Rollback()

```csharp
public void Rollback()
{
    transaction?.Rollback();
    transaction = null;
}
```

### Primer Korišćenja u Controller-u

```csharp
public Response HandleDodajRacun(object data) {
    try {
        Racun racun = DeserializeFromJson<Racun>(data);
        
        broker.ZapocniTransakciju();
        
        // 1. Dodaj račun
        broker.Dodaj(racun);
        
        // 2. Uzmi generated ID
        int idRacun = GetLastInsertedId();
        
        // 3. Dodaj sve stavke
        foreach (StavkaRacuna stavka in racun.Stavke) {
            stavka.IdRacun = idRacun;
            broker.Dodaj(stavka);
        }
        
        broker.Commit();  // Sve je uspelo ✅
        
        return new Response { IsSuccessful = true };
        
    } catch (Exception ex) {
        broker.Rollback();  // Vrati sve nazad ❌
        return new Response { 
            IsSuccessful = false, 
            Message = ex.Message 
        };
    }
}
```

**Napomena:** Svi SQL commands koriste `transaction` parametar:
```csharp
SqlCommand cmd = new SqlCommand(sql, connection, transaction);
```

Ako je `transaction != null`, operacija je deo transakcije.

---

## 🔍 Fleksibilnost WhereCondition Property-ja

### Empty Condition = Vrati Sve

```csharp
Prodavac filter = new Prodavac();  // Sve properties prazne
// filter.WhereCondition vraća ""
List<IDomainObject> sviProdavci = broker.VratiSve(filter);
// SQL: SELECT ... FROM Prodavac p (bez WHERE)
```

### Single Filter

```csharp
Firma filter = new Firma { Naziv = "Tech" };
// filter.WhereCondition vraća "f.naziv LIKE '%Tech%'"
```

### Multiple Filters (AND logika)

```csharp
FizickoLice filter = new FizickoLice { 
    ImePrezime = "Marko",
    Email = "@gmail.com"
};
// filter.WhereCondition vraća:
// "fl.imePrezime LIKE '%Marko%' AND fl.email LIKE '%@gmail.com%'"
```

### Dynamic Date Filter (Racun)

```csharp
// Po godini
Racun f1 = new Racun { DatumFilter = "2026" };
// WHERE: YEAR(r.datumIzdavanja) = 2026

// Po mesecu
Racun f2 = new Racun { DatumFilter = "2026-02" };
// WHERE: FORMAT(r.datumIzdavanja, 'yyyy-MM') = '2026-02'

// Po tačnom datumu
Racun f3 = new Racun { DatumFilter = "2026-02-10" };
// WHERE: CAST(r.datumIzdavanja AS DATE) = '2026-02-10'
```

Logika definisana u `Racun.WhereCondition`:
```csharp
if (DatumFilter.Length == 4)
    conditions.Add($"YEAR(r.datumIzdavanja) = {DatumFilter}");
else if (DatumFilter.Length == 7)
    conditions.Add($"FORMAT(r.datumIzdavanja, 'yyyy-MM') = '{DatumFilter}'");
else
    conditions.Add($"CAST(r.datumIzdavanja AS DATE) = '{DatumFilter}'");
```

---

## 📊 GetList() - Mapping SqlDataReader to Objects

### Problem

ADO.NET vraća `SqlDataReader` - forward-only stream redova.

### Rešenje

Svaki domain objekat ima `GetList(SqlDataReader reader)` metodu koja mapira:

```csharp
public List<IDomainObject> GetList(SqlDataReader reader) {
    List<IDomainObject> list = new List<IDomainObject>();
    
    while (reader.Read()) {
        list.Add(new Firma {
            IdKupac = (int)reader["idKupac"],
            Naziv = reader["naziv"]?.ToString(),
            Pib = reader["pib"]?.ToString(),
            Adresa = reader["adresa"]?.ToString()
        });
    }
    
    return list;
}
```

### Null Safety

Koristi `?.ToString()` za nullable kolone:
```csharp
Ime = reader["ime"]?.ToString() ?? ""
```

### Type Casting

```csharp
IdProdavac = (int)reader["idProdavac"]           // int kolona
Cena = Convert.ToDouble(reader["cena"])           // decimal/float → double
DatumIzdavanja = (DateTime)reader["datum"]        // datetime kolona
```

---

## 💡 Design Decisions i Trade-offs

### ✅ Prednosti Template Method Pattern-a

1. **Extensibility** - dodavanje novog entiteta ne zahteva promenu Broker-a
2. **Code Reuse** - isti kod za sve CRUD operacije
3. **Separation of Concerns** - SQL logika u domain modelima
4. **Type Safety** (donekle) - IDE autocomplete za domain properties

### ⚠️ Nedostaci / Ograničenja

1. **SQL Injection Risk** - string concatenation umesto parametrizovanih query-a
   - **Mitigacija:** Client-side validacija, Server-side sanitization
2. **Tight Coupling** na SQL Server sintaksu (STUFF, FORMAT, etc.)
3. **Reflection Alternative** - moglo bi koristiti reflection, ali više boilerplate-a
4. **No ORM Features** - nema lazy loading, change tracking, migrations

### Zašto Ne ORM (Entity Framework)?

Razlozi za custom DatabaseBroker:
1. **Learning Purpose** - dublji understanding SQL-a i ADO.NET-a
2. **Control** - tačna kontrola nad generisanim SQL-om
3. **Performance** - direktan SQL bez ORM overhead-a
4. **Custom Queries** - lako pisanje složenih JOIN-ova

---

## 🔒 Connection Management

### Connection Pooling

ADO.NET automatski koristi connection pooling:
```csharp
connection = new SqlConnection(connectionString);
connection.Open();  // Uzima konekciju iz pool-a
```

**Best Practice:** Zatvori connection nakon korišćenja:
```csharp
public void Close() {
    connection?.Close();
    connection?.Dispose();
}
```

### Thread Safety

**DatabaseBroker NIJE thread-safe!**

Server kreira **novu instancu** za svakog klijenta:
```csharp
public Response HandleRequest(Request req) {
    DatabaseBroker broker = new DatabaseBroker();  // Nova instanca!
    // ... operacije ...
    broker.Close();
}
```

---

## 📝 Best Practices

### 1. Uvek Koristi Transaction za Multiple Inserts

```csharp
broker.ZapocniTransakciju();
try {
    broker.Dodaj(racun);
    foreach (var stavka in stavke)
        broker.Dodaj(stavka);
    broker.Commit();
} catch {
    broker.Rollback();
    throw;
}
```

### 2. SQL Injection Prevention

**Loše:**
```csharp
string ime = txtIme.Text;  // User input: "'; DROP TABLE Prodavac; --"
// WHERE: p.ime = ''; DROP TABLE Prodavac; --'
```

**Dobro:**
```csharp
string ime = txtIme.Text.Replace("'", "''");  // Escape single quotes
```

### 3. Null Checks u GetList()

```csharp
Ime = reader["ime"]?.ToString() ?? "",
Cena = reader["cena"] != DBNull.Value ? Convert.ToDouble(reader["cena"]) : 0.0
```

### 4. Close() Nakon Operacija

U Server Controller-u:
```csharp
DatabaseBroker broker = new DatabaseBroker();
try {
    // ... operacije ...
} finally {
    broker.Close();
}
```

---

## 🚀 Zaključak

DatabaseBroker implementira elegantnu **Template Method Pattern** arhitekturu koja omogućava:

✅ **Generic CRUD** - radi sa bilo kojim IDomainObject  
✅ **Custom Queries** - složeni JOIN-ovi bez ORM limitacija  
✅ **Transakcije** - atomičke multi-table operacije  
✅ **Extensibility** - lako dodavanje novih entiteta  
✅ **Clean Architecture** - jasna separacija data access logike  

DatabaseBroker je **centralna komponenta** koja omogućava celom sistemu da funkcioniše bez dupliranja SQL koda!
