# TEMPLATE METHOD PATTERN & GENERIC BROKER

## Datum implementacije: 9. februar 2026.

## Zahtevi
1. ✅ **Template metoda za sistemske operacije** - Implementiran Template Method Pattern
2. ✅ **Generički broker** - Broker ne zavisi od konkretnih tipova objekata
3. ✅ **Kontroler korisničkog interfejsa** - UIController za upravljanje formama
4. ✅ **Server sa konzolnim menijem** - Server se pokreće iz menija, ne automatski
5. ✅ **Admin ne sme da dodaje račune** - Kontrola pristupa na klijentskoj i serverskoj strani

---

## 1. TEMPLATE METHOD PATTERN

### IDomainObject interface (Models/IDomainObject.cs)

Proširili smo interfejs sa template metodama:

```csharp
public interface IDomainObject
{
    // Osnovna svojstva
    string TableName { get; }
    string WhereCondition { get; }
    string InsertValues { get; }
    string UpdateValues { get; }
    List<IDomainObject> GetList(SqlDataReader reader);
    
    // TEMPLATE METODE - objekat definiše custom logiku
    string GetCustomSelectQuery() => null;
    bool ExecuteCustomInsert(SqlConnection connection) => false;
    bool ExecuteCustomUpdate(SqlConnection connection) => false;
    bool ExecuteCustomDelete(SqlConnection connection) => false;
}
```

**Princip rada:**
- Metode vraćaju `null` ili `false` ako objekat koristi standardnu logiku
- Ako vrate vrednost, broker koristi tu prilagođenu logiku
- Objekat sam kontroliše svoju kompleksnost

### Implementacije u domain klasama:

#### Racun.cs
- **Custom SELECT**: JOIN sa Prodavac, Kupac, Firma, FizickoLice
- **Custom INSERT**: Transakcija za dodavanje računa i stavki
- **Custom UPDATE**: Transakcija za ažuriranje računa i stavki

#### Firma.cs i FizickoLice.cs
- **Custom SELECT**: JOIN sa Kupac tabelom
- **Custom INSERT**: Koristi stored procedure `DodajFirmu`/`DodajFizickoLice`
- **Custom UPDATE**: Koristi stored procedure `AzurirajFirmu`/`AzurirajFizickoLice`

#### Prodavac.cs
- **Custom SELECT**: Prikazuje skladišta koristeći XML PATH agregaciju

#### StavkaRacuna.cs
- **Custom SELECT**: Vraća kompletan query ako je IdRacun postavljen

#### Oprema.cs, Skladiste.cs, ProdSklad.cs, Kupac.cs
- Koriste standardne operacije - vraćaju `null`/`false`

---

## 2. GENERIČKI BROKER

### DatabaseBroker.cs (Broker/DatabaseBroker.cs)

**Potpuno generički** - ne sadrži `if (obj is TipX)` provere!

```csharp
public IDomainObject VratiJedan(IDomainObject obj)
{
    connection.Open();
    
    // Pokušaj custom query
    string query = obj.GetCustomSelectQuery();
    
    // Ako nema, koristi standardni
    if (string.IsNullOrEmpty(query))
    {
        query = $"SELECT * FROM {obj.TableName} WHERE {obj.WhereCondition}";
    }
    
    SqlCommand cmd = new SqlCommand(query, connection);
    using (SqlDataReader reader = cmd.ExecuteReader())
    {
        List<IDomainObject> lista = obj.GetList(reader);
        return lista.Count > 0 ? lista[0] : null;
    }
}
```

**Isti princip važi za:**
- `VratiSve(IDomainObject obj)`
- `Dodaj(IDomainObject obj)` - prvo pokušava `ExecuteCustomInsert()`
- `Azuriraj(IDomainObject obj)` - prvo pokušava `ExecuteCustomUpdate()`
- `Obrisi(IDomainObject obj)` - prvo pokušava `ExecuteCustomDelete()`

**Prednosti:**
- Broker ne zna ništa o konkretnim tipovima
- Dodavanje novog entiteta ne zahteva izmenu brokera
- Sva specifična logika je u samom objektu

---

## 3. KONTROLER KORISNIČKOG INTERFEJSA

### UIController.cs (Client/UIController.cs)

Statička klasa za upravljanje formama i kontrolu pristupa:

```csharp
public static class UIController
{
    // Provera admin prava
    public static bool IsAdmin() => Sesija.IsAdmin;
    
    // Kontrola pristupa za račune
    public static bool MoguceDodatiRacun()
    {
        if (IsAdmin())
        {
            MessageBox.Show("Administratori ne smeju da dodaju račune!", 
                           "Pristup zabranjen", 
                           MessageBoxButtons.OK, 
                           MessageBoxIcon.Warning);
            return false;
        }
        return true;
    }
    
    // Metode za otvaranje formi sa kontrolom pristupa
    public static void OtvoriDodajRacun()
    public static void OtvoriModifikujRacun(Racun racun)
    public static void OtvoriDetaljeRacuna(Racun racun)
    // ...
}
```

**Korišćenje:**
```csharp
// Umesto direktnog otvaranja forme
if (UIController.MoguceDodatiRacun())
{
    AddRacun forma = new AddRacun();
    forma.ShowDialog();
}

// Ili pojednostavljeno
UIController.OtvoriDodajRacun();
```

---

## 4. SERVER SA KONZOLNIM MENIJEM

### Program.cs (Server/Program.cs)

Server se više ne pokreće automatski. Korisnik bira iz menija:

```
╔══════════════════════════════════════════╗
║         SERVER MANAGEMENT MENI           ║
╚══════════════════════════════════════════╝

  1. Pokreni server
  2. Zaustavi server
  3. Status servera
  4. Izlaz

Izaberite opciju (1-4): 
```

**Funkcionlnost:**
- Server se pokreće u pozadinskoj niti kada korisnik izabere opciju 1
- Status prikazuje da li je server aktivan ili zaustavljen
- Server radi na portu 9000

---

## 5. ADMIN NE SME DA DODAJE RAČUNE

### Klijentska strana (Client/)

**UIController.cs:**
```csharp
public static bool MoguceDodatiRacun()
{
    if (IsAdmin())
    {
        MessageBox.Show("Administratori ne smeju da dodaju račune!", 
                       "Pristup zabranjen", ...);
        return false;
    }
    return true;
}
```

### Serverska strana (Server/Controller.cs)

**Dvostruka zaštita** - provera i na serveru:

```csharp
case Operation.DodajObjekat:
    if (objJson.Contains("\"TableName\":\"Racun\""))
    {
        Racun racun = JsonSerializer.Deserialize<Racun>(objJson, jsonOptions);
        
        // KONTROLA PRISTUPA
        if (racun.Prodavac != null && racun.Prodavac.Email == "admin@prodavnica.rs")
        {
            res.IsSuccessful = false;
            res.Message = "ZABRANJENA OPERACIJA: Administratori ne smeju da dodaju račune!";
            Console.WriteLine("[SECURITY] Admin pokušao da doda račun - ZABRANJEN PRISTUP!");
            return res;
        }
    }
```

**Ista zaštita postoji i za AzurirajObjekat!**

---

## ARHITEKTURNI PRINCIPI

### 1. Single Responsibility Principle
- Svaka klasa ima jednu odgovornost
- Broker: Izvršavanje SQL operacija
- Controller: Rutiranje zahteva
- UIController: Upravljanje UI i kontrola pristupa
- Domain objekti: Poslovna logika i SQL generisanje

### 2. Open/Closed Principle
- Broker je zatvoren za izmene, otvoren za proširenja
- Dodavanje novog entiteta ne zahteva izmenu brokera
- Nova funkcionalnost se dodaje kroz template metode u objektima

### 3. Dependency Inversion Principle
- Broker zavisi od interfejsa (IDomainObject), ne od konkretnih klasa
- Controller takođe radi sa interfejsom

### 4. Template Method Pattern
- Definisan algoritam u brokeru (VratiSve, Dodaj, Azuriraj, Obrisi)
- Objekti mogu redefinisati korake (GetCustomSelectQuery, ExecuteCustomInsert, ...)
- Broker poziva template metode u tačno definisanom redosledu

---

## KAKO DODATI NOVI ENTITET

1. Kreiraj klasu koja implementira `IDomainObject`
2. Implementiraj obavezna svojstva: `TableName`, `WhereCondition`, `InsertValues`, `UpdateValues`
3. Implementiraj `GetList(SqlDataReader reader)`
4. Ako treba custom logika, implementiraj template metode:
   - `GetCustomSelectQuery()` - za JOIN upite
   - `ExecuteCustomInsert()` - za transakcije ili stored procedures
   - `ExecuteCustomUpdate()` - za složena ažuriranja
   - `ExecuteCustomDelete()` - za cascade brisanje
5. Dodaj deserijalizaciju u Controller.cs (samo switch case)
6. **NE MENJAJ BROKER!** On je potpuno generički.

---

## TESTIRANJE

### Testiranje brokera:
```csharp
DatabaseBroker broker = new DatabaseBroker();

// Testiranje sa različitim entitetima
Prodavac p = new Prodavac { IdProdavac = 1 };
var prodavac = broker.VratiJedan(p);

Racun r = new Racun { IdRacun = 5 };
var racun = (Racun)broker.VratiJedan(r);
```

### Testiranje kontrole pristupa:
1. Prijavi se kao admin (admin@prodavnica.rs)
2. Pokušaj da dodaš račun - trebalo bi da bude blokirano
3. Prijavi se kao običan prodavac
4. Dodaj račun - trebalo bi da uspe

---

## ZAKLJUČAK

✅ Sistem je sada **potpuno generički**  
✅ Broker **ne zna** o konkretnim tipovima  
✅ Template Method Pattern omogućava **fleksibilnost**  
✅ Kontrola pristupa je **dvostruko zaštićena**  
✅ Server se pokreće kroz **korisnički meni**  
✅ Arhitektura prati **SOLID principe**

**Autor:** GitHub Copilot  
**Implementacija:** 9. februar 2026.
