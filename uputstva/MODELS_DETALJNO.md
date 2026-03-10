# 📦 MODELS Layer - Detaljno Objašnjenje

## 🎯 Uloga Models Layer-a

Models layer je **shared library** koji koriste svi ostali projekti (Client, Server, Broker). Sadrži:
1. Domain modele (entities)
2. Komunikacione modele (Request/Response)
3. Enumeracije (Operation, KategorijaOpreme)
4. Interfejse (IDomainObject)

## 🔌 IDomainObject Interface - Template Method Contract

```csharp
public interface IDomainObject
{
    // 1. Metadata za generičke SQL operacije
    string TableName { get; }
    
    // 2. SQL generatori
    string InsertValues { get; }
    string UpdateValues { get; }
    string WhereCondition { get; }
    
    // 3. Custom query za kompleksne SELECT-e sa JOIN-ovima
    string GetCustomSelectQuery();
    
    // 4. Mapping SqlDataReader → List<Domain>
    List<IDomainObject> GetList(SqlDataReader reader);
}
```

### Kako DatabaseBroker koristi IDomainObject

```csharp
public void Dodaj(IDomainObject obj) {
    string sql = $"INSERT INTO {obj.TableName} VALUES ({obj.InsertValues})";
    SqlCommand cmd = new SqlCommand(sql, connection);
    cmd.ExecuteNonQuery();
}

public List<IDomainObject> VratiSve(IDomainObject obj) {
    string query = obj.GetCustomSelectQuery();
    string where = obj.WhereCondition;
    if (!string.IsNullOrEmpty(where))
        query += $" WHERE {where}";
    
    SqlCommand cmd = new SqlCommand(query, connection);
    SqlDataReader reader = cmd.ExecuteReader();
    List<IDomainObject> result = obj.GetList(reader);
    return result;
}
```

**Ključna prednost:** Dodavanje novog entiteta ne zahteva promenu DatabaseBroker-a!

---

## 📋 Domain Modeli - Detaljno

### 1. Prodavac.cs

**Uloga:** Prodavci koji izdaju račune i upravljaju skladištima.

```csharp
public class Prodavac : IDomainObject
{
    // Properties
    public int IdProdavac { get; set; }
    public string Ime { get; set; }
    public string Prezime { get; set; }
    public string Email { get; set; }
    public string Telefon { get; set; }
    public string Password { get; set; }
    
    // Computed property za prikaz
    [Browsable(false)]
    public string ImePrezime => $"{Ime} {Prezime}";
    
    // Many-to-Many sa Skladiste preko ProdSklad
    [Browsable(false)]
    public string Skladista { get; set; }  // Comma-separated lista skladišta
    
    // IDomainObject implementacija
    public string TableName => "Prodavac";
    
    public string InsertValues => 
        $"'{Ime}', '{Prezime}', '{Email}', '{Telefon}', '{Password}'";
    
    public string UpdateValues => 
        $"ime='{Ime}', prezime='{Prezime}', email='{Email}', " +
        $"telefon='{Telefon}', password='{Password}'";
    
    public string WhereCondition {
        get {
            // Za login - mora biti i email i password
            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password))
                return $"p.email='{Email}' AND p.password='{Password}'";
            
            // Za search - ako ima ImePrezime ili Email (bez passworda)
            List<string> conditions = new List<string>();
            if (!string.IsNullOrEmpty(Ime) || !string.IsNullOrEmpty(Prezime))
                conditions.Add($"CONCAT(p.ime, ' ', p.prezime) LIKE '%{ImePrezime}%'");
            if (!string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Password))
                conditions.Add($"p.email LIKE '%{Email}%'");
            
            return conditions.Count > 0 ? string.Join(" AND ", conditions) : "";
        }
    }
    
    public string GetCustomSelectQuery() {
        // Complex query: JOIN sa ProdSklad i Skladiste, agregacija sa STUFF
        return @"
            SELECT 
                p.idProdavac, p.ime, p.prezime, p.email, p.telefon, p.password,
                ISNULL(
                    STUFF((
                        SELECT ', ' + s.naziv
                        FROM ProdSklad ps
                        JOIN Skladiste s ON ps.idSkladiste = s.idSkladiste
                        WHERE ps.idProdavac = p.idProdavac
                        FOR XML PATH(''), TYPE
                    ).value('.', 'NVARCHAR(MAX)'), 1, 2, ''),
                    ''
                ) AS skladista
            FROM Prodavac p";
    }
    
    public List<IDomainObject> GetList(SqlDataReader reader) {
        List<IDomainObject> list = new List<IDomainObject>();
        while (reader.Read()) {
            list.Add(new Prodavac {
                IdProdavac = (int)reader["idProdavac"],
                Ime = reader["ime"]?.ToString() ?? "",
                Prezime = reader["prezime"]?.ToString() ?? "",
                Email = reader["email"]?.ToString() ?? "",
                Telefon = reader["telefon"]?.ToString() ?? "",
                Password = reader["password"]?.ToString() ?? "",
                Skladista = reader["skladista"]?.ToString() ?? ""
            });
        }
        return list;
    }
}
```

**Veze:**
- **1:N** sa Racun (idProdavac FK)
- **M:N** sa Skladiste preko ProdSklad

**Posebnosti:**
- `STUFF` + `FOR XML PATH` za agregaciju skladišta u CSV string
- `ISNULL` wrapper da izbegne NULL vrednost
- Razlikovanje login (Email+Password) od search (samo Email)

---

### 2. Kupac.cs (Apstraktna klasa)

**Uloga:** Bazna klasa za Firma i FizickoLice (Table-Per-Type nasledjivanje).

```csharp
[Serializable]
public class Kupac : IDomainObject
{
    public int IdKupac { get; set; }
    
    // Ove metode implementiraju naslednici
    public virtual string TableName => "Kupac";
    public virtual string InsertValues => "";
    public virtual string UpdateValues => "";
    public virtual string WhereCondition => "";
    public virtual string GetCustomSelectQuery() => "";
    public virtual List<IDomainObject> GetList(SqlDataReader reader) => null;
}
```

**Razlog za apstrakciju:** Kupac može biti Firma ili FizickoLice, obe dele IdKupac.

---

### 3. Firma.cs : Kupac

**Uloga:** Pravna lica koja kupuju opremu.

```csharp
public class Firma : Kupac
{
    public string Naziv { get; set; }
    public string Pib { get; set; }
    public string Adresa { get; set; }
    
    public override string TableName => "Firma";
    
    public override string InsertValues => 
        $"'{Naziv}', '{Pib}', '{Adresa}'";
    
    public override string UpdateValues => 
        $"naziv='{Naziv}', pib='{Pib}', adresa='{Adresa}'";
    
    public override string WhereCondition {
        get {
            if (IdKupac > 0)
                return $"f.idKupac = {IdKupac}";
            
            List<string> conditions = new List<string>();
            if (!string.IsNullOrWhiteSpace(Naziv))
                conditions.Add($"f.naziv LIKE '%{Naziv}%'");
            if (!string.IsNullOrWhiteSpace(Pib))
                conditions.Add($"f.pib LIKE '%{Pib}%'");
            
            return conditions.Count > 0 ? string.Join(" AND ", conditions) : "";
        }
    }
    
    public override string GetCustomSelectQuery() {
        // JOIN sa baznom Kupac tabelom (iako trenutno Kupac nema dodatnih kolona)
        return @"
            SELECT f.idKupac, f.naziv, f.pib, f.adresa
            FROM Firma f
            JOIN Kupac k ON f.idKupac = k.idKupac";
    }
    
    public override List<IDomainObject> GetList(SqlDataReader reader) {
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
}
```

**Veze:**
- **1:1** sa Kupac (idKupac PK + FK)
- **1:N** sa Racun preko Kupac.idKupac

---

### 4. FizickoLice.cs : Kupac

**Uloga:** Fizička lica koja kupuju opremu.

```csharp
public class FizickoLice : Kupac
{
    public string ImePrezime { get; set; }
    public string Email { get; set; }
    public string Telefon { get; set; }
    
    public override string TableName => "FizickoLice";
    
    public override string InsertValues => 
        $"'{ImePrezime}', '{Email}', '{Telefon}'";
    
    public override string UpdateValues => 
        $"imePrezime='{ImePrezime}', email='{Email}', telefon='{Telefon}'";
    
    public override string WhereCondition {
        get {
            if (IdKupac > 0)
                return $"fl.idKupac = {IdKupac}";
            
            List<string> conditions = new List<string>();
            if (!string.IsNullOrWhiteSpace(ImePrezime))
                conditions.Add($"fl.imePrezime LIKE '%{ImePrezime}%'");
            if (!string.IsNullOrWhiteSpace(Email))
                conditions.Add($"fl.email LIKE '%{Email}%'");
            if (!string.IsNullOrWhiteSpace(Telefon))
                conditions.Add($"fl.telefon LIKE '%{Telefon}%'");
            
            return conditions.Count > 0 ? string.Join(" AND ", conditions) : "";
        }
    }
    
    public override string GetCustomSelectQuery() {
        return @"
            SELECT fl.idKupac, fl.imePrezime, fl.email, fl.telefon
            FROM FizickoLice fl
            JOIN Kupac k ON fl.idKupac = k.idKupac";
    }
    
    public override List<IDomainObject> GetList(SqlDataReader reader) {
        List<IDomainObject> list = new List<IDomainObject>();
        while (reader.Read()) {
            list.Add(new FizickoLice {
                IdKupac = (int)reader["idKupac"],
                ImePrezime = reader["imePrezime"]?.ToString(),
                Email = reader["email"]?.ToString(),
                Telefon = reader["telefon"]?.ToString()
            });
        }
        return list;
    }
}
```

**Veze:** isto kao Firma

**Smart filter:** Telefon filter podržava i email (ako sadrži @) i telefon.

---

### 5. Racun.cs

**Uloga:** Fakture/Računi koje izdaju prodavci kupcima.

```csharp
public class Racun : IDomainObject
{
    // Constructor inicijalizuje nested objekte
    public Racun() {
        Prodavac = new Prodavac();
        Kupac = new Kupac();
        Stavke = new List<StavkaRacuna>();
    }
    
    // Basic properties
    public int IdRacun { get; set; }
    public DateTime DatumIzdavanja { get; set; }
    public double KonacanIznos { get; set; }
    public double Pdv { get; set; }
    public double CenaSaPopustom { get; set; }
    public double CenaStavke { get; set; }
    
    // Foreign key objekti
    [Browsable(false)]
    public Prodavac Prodavac { get; set; }
    
    [Browsable(false)]
    public Kupac Kupac { get; set; }
    
    [Browsable(false)]
    public string NazivKupca { get; set; }  // Computed - Firma.naziv ili FL.imePrezime
    
    // Computed properties za DataGridView
    [DisplayName("Prodavac")]
    public string ImeProdavca => Prodavac?.ImePrezime ?? "";
    
    [DisplayName("Kupac")]
    public string ImeKupca => NazivKupca ?? "";
    
    // 1:N veza sa StavkaRacuna
    [Browsable(false)]
    public List<StavkaRacuna> Stavke { get; set; }
    
    // Filter property za fleksibilnu pretragu po datumu
    [Browsable(false)]
    public string? DatumFilter { get; set; }
    
    public string TableName => "Racun";
    
    public string InsertValues => 
        $"'{DatumIzdavanja:yyyy-MM-dd}', " +
        $"{KonacanIznos.ToString().Replace(',', '.')}, " +
        $"{Pdv.ToString().Replace(',', '.')}, " +
        $"{CenaSaPopustom.ToString().Replace(',', '.')}, " +
        $"{CenaStavke.ToString().Replace(',', '.')}, " +
        $"{Prodavac.IdProdavac}, {Kupac.IdKupac}";
    
    public string UpdateValues => 
        $"datumIzdavanja='{DatumIzdavanja:yyyy-MM-dd}', " +
        $"konacanIznos={KonacanIznos.ToString().Replace(',', '.')}, " +
        $"pdv={Pdv.ToString().Replace(',', '.')}, " +
        $"cenaSaPopustom={CenaSaPopustom.ToString().Replace(',', '.')}, " +
        $"cenaStavke={CenaStavke.ToString().Replace(',', '.')}, " +
        $"idProdavac={Prodavac.IdProdavac}, " +
        $"idKupac={Kupac.IdKupac}";
    
    public string WhereCondition {
        get {
            if (IdRacun > 0)
                return $"r.idRacun = {IdRacun}";
            
            List<string> conditions = new List<string>();
            
            if (Prodavac != null && Prodavac.IdProdavac > 0)
                conditions.Add($"r.idProdavac = {Prodavac.IdProdavac}");
            
            if (Kupac != null && Kupac.IdKupac > 0)
                conditions.Add($"r.idKupac = {Kupac.IdKupac}");
            
            // Fleksibilna pretraga po datumu
            if (!string.IsNullOrWhiteSpace(DatumFilter)) {
                if (DatumFilter.Length == 4)  // YYYY
                    conditions.Add($"YEAR(r.datumIzdavanja) = {DatumFilter}");
                else if (DatumFilter.Length == 7)  // YYYY-MM
                    conditions.Add($"FORMAT(r.datumIzdavanja, 'yyyy-MM') = '{DatumFilter}'");
                else  // YYYY-MM-DD
                    conditions.Add($"CAST(r.datumIzdavanja AS DATE) = '{DatumFilter}'");
            }
            
            return conditions.Count > 0 ? string.Join(" AND ", conditions) : "";
        }
    }
    
    public string GetCustomSelectQuery() {
        // Union Firma + FizickoLice za naziv kupca
        return @"
            SELECT 
                r.idRacun, r.datumIzdavanja, r.konacanIznos, 
                r.pdv, r.cenaSaPopustom, r.cenaStavke,
                r.idProdavac, r.idKupac,
                CONCAT(p.ime, ' ', p.prezime) AS imeProdavca,
                ISNULL(f.naziv, fl.imePrezime) AS nazivKupca
            FROM Racun r
            JOIN Prodavac p ON r.idProdavac = p.idProdavac
            JOIN Kupac k ON r.idKupac = k.idKupac
            LEFT JOIN Firma f ON k.idKupac = f.idKupac
            LEFT JOIN FizickoLice fl ON k.idKupac = fl.idKupac";
    }
    
    public List<IDomainObject> GetList(SqlDataReader reader) {
        List<IDomainObject> list = new List<IDomainObject>();
        while (reader.Read()) {
            list.Add(new Racun {
                IdRacun = (int)reader["idRacun"],
                DatumIzdavanja = (DateTime)reader["datumIzdavanja"],
                KonacanIznos = Convert.ToDouble(reader["konacanIznos"]),
                Pdv = Convert.ToDouble(reader["pdv"]),
                CenaSaPopustom = Convert.ToDouble(reader["cenaSaPopustom"]),
                CenaStavke = Convert.ToDouble(reader["cenaStavke"]),
                Prodavac = new Prodavac { 
                    IdProdavac = (int)reader["idProdavac"],
                    ImePrezime = reader["imeProdavca"]?.ToString()
                },
                Kupac = new Kupac { 
                    IdKupac = (int)reader["idKupac"] 
                },
                NazivKupca = reader["nazivKupca"]?.ToString()
            });
        }
        return list;
    }
}
```

**Veze:**
- **N:1** sa Prodavac (idProdavac FK)
- **N:1** sa Kupac (idKupac FK)
- **1:N** sa StavkaRacuna (idRacun FK)

**Posebnosti:**
- `ISNULL(f.naziv, fl.imePrezime)` - smart select između Firma i FizickoLice
- `DatumFilter` omogućava pretragu po godini/mesecu/danu
- `Replace(',', '.')` - fiksira decimalni separator za SQL

---

### 6. StavkaRacuna.cs

**Uloga:** Line items na računu (Oprema + količina).

```csharp
public class StavkaRacuna : IDomainObject
{
    public int IdStavkaRacuna { get; set; }
    public int IdRacun { get; set; }      // FK → Racun
    public int IdOprema { get; set; }      // FK → Oprema
    public int Kolicina { get; set; }
    public double Cena { get; set; }       // Ukupna cena (Kolicina * JedinicnaCena)
    
    // Nested objekat za prikaz
    [Browsable(false)]
    public Oprema Oprema { get; set; }
    
    // Computed property
    [DisplayName("Naziv opreme")]
    public string NazivOpreme => Oprema?.Ime ?? "";
    
    public string TableName => "StavkaRacuna";
    
    public string InsertValues => 
        $"{IdRacun}, {IdOprema}, {Kolicina}, {Cena.ToString().Replace(',', '.')}";
    
    public string UpdateValues => 
        $"idRacun={IdRacun}, idOprema={IdOprema}, kolicina={Kolicina}, " +
        $"cena={Cena.ToString().Replace(',', '.')}";
    
    public string WhereCondition => 
        IdRacun > 0 ? $"sr.idRacun = {IdRacun}" : "";
    
    public string GetCustomSelectQuery() {
        return @"
            SELECT 
                sr.idStavkaRacuna, sr.idRacun, sr.idOprema, 
                sr.kolicina, sr.cena,
                o.ime AS imeOpreme
            FROM StavkaRacuna sr
            JOIN Oprema o ON sr.idOprema = o.idOprema";
    }
    
    public List<IDomainObject> GetList(SqlDataReader reader) {
        List<IDomainObject> list = new List<IDomainObject>();
        while (reader.Read()) {
            list.Add(new StavkaRacuna {
                IdStavkaRacuna = (int)reader["idStavkaRacuna"],
                IdRacun = (int)reader["idRacun"],
                IdOprema = (int)reader["idOprema"],
                Kolicina = (int)reader["kolicina"],
                Cena = Convert.ToDouble(reader["cena"]),
                Oprema = new Oprema { 
                    Ime = reader["imeOpreme"]?.ToString() 
                }
            });
        }
        return list;
    }
}
```

**Veze:**
- **N:1** sa Racun
- **N:1** sa Oprema

---

### 7. Oprema.cs

**Uloga:** Proizvodi koje prodajemo.

```csharp
public class Oprema : IDomainObject
{
    public int IdOprema { get; set; }
    public string Ime { get; set; }
    public string Kategorija { get; set; }  // Enum-like (Laptop, Telefon, etc.)
    public double Cena { get; set; }
    
    public string TableName => "Oprema";
    
    public string InsertValues => 
        $"'{Ime}', '{Kategorija}', {Cena.ToString().Replace(',', '.')}";
    
    public string UpdateValues => 
        $"ime='{Ime}', kategorija='{Kategorija}', " +
        $"cena={Cena.ToString().Replace(',', '.')}";
    
    public string WhereCondition {
        get {
            if (IdOprema > 0)
                return $"o.idOprema = {IdOprema}";
            
            List<string> conditions = new List<string>();
            if (!string.IsNullOrWhiteSpace(Ime))
                conditions.Add($"o.ime LIKE '%{Ime}%'");
            if (!string.IsNullOrWhiteSpace(Kategorija))
                conditions.Add($"o.kategorija = '{Kategorija}'");
            
            return conditions.Count > 0 ? string.Join(" AND ", conditions) : "";
        }
    }
    
    public string GetCustomSelectQuery() {
        return "SELECT o.idOprema, o.ime, o.kategorija, o.cena FROM Oprema o";
    }
    
    public List<IDomainObject> GetList(SqlDataReader reader) {
        List<IDomainObject> list = new List<IDomainObject>();
        while (reader.Read()) {
            list.Add(new Oprema {
                IdOprema = (int)reader["idOprema"],
                Ime = reader["ime"]?.ToString(),
                Kategorija = reader["kategorija"]?.ToString(),
                Cena = Convert.ToDouble(reader["cena"])
            });
        }
        return list;
    }
}
```

**Veze:**
- **1:N** sa StavkaRacuna

---

### 8. Skladiste.cs

**Uloga:** Skladišta gde se čuva oprema.

```csharp
public class Skladiste : IDomainObject
{
    public int IdSkladiste { get; set; }
    public string Naziv { get; set; }
    public string Adresa { get; set; }
    
    public string TableName => "Skladiste";
    
    public string InsertValues => $"'{Naziv}', '{Adresa}'";
    
    public string UpdateValues => $"naziv='{Naziv}', adresa='{Adresa}'";
    
    public string WhereCondition {
        get {
            if (IdSkladiste > 0)
                return $"s.idSkladiste = {IdSkladiste}";
            
            List<string> conditions = new List<string>();
            if (!string.IsNullOrWhiteSpace(Naziv))
                conditions.Add($"s.naziv LIKE '%{Naziv}%'");
            if (!string.IsNullOrWhiteSpace(Adresa))
                conditions.Add($"s.adresa LIKE '%{Adresa}%'");
            
            return conditions.Count > 0 ? string.Join(" AND ", conditions) : "";
        }
    }
    
    public string GetCustomSelectQuery() {
        return "SELECT s.idSkladiste, s.naziv, s.adresa FROM Skladiste s";
    }
    
    public List<IDomainObject> GetList(SqlDataReader reader) {
        List<IDomainObject> list = new List<IDomainObject>();
        while (reader.Read()) {
            list.Add(new Skladiste {
                IdSkladiste = (int)reader["idSkladiste"],
                Naziv = reader["naziv"]?.ToString(),
                Adresa = reader["adresa"]?.ToString()
            });
        }
        return list;
    }
}
```

**Veze:**
- **M:N** sa Prodavac preko ProdSklad

---

### 9. ProdSklad.cs (Junction Table)

**Uloga:** Many-to-Many veza između Prodavac i Skladiste.

```csharp
public class ProdSklad : IDomainObject
{
    public int IdProdavac { get; set; }
    public int IdSkladiste { get; set; }
    
    public string TableName => "ProdSklad";
    
    public string InsertValues => $"{IdProdavac}, {IdSkladiste}";
    
    public string UpdateValues => "";  // Obično se ne update-uje junction table
    
    public string WhereCondition => 
        IdProdavac > 0 ? $"ps.idProdavac = {IdProdavac}" : "";
    
    public string GetCustomSelectQuery() {
        return @"
            SELECT ps.idProdavac, ps.idSkladiste, s.naziv
            FROM ProdSklad ps
            JOIN Skladiste s ON ps.idSkladiste = s.idSkladiste";
    }
    
    public List<IDomainObject> GetList(SqlDataReader reader) {
        List<IDomainObject> list = new List<IDomainObject>();
        while (reader.Read()) {
            list.Add(new ProdSklad {
                IdProdavac = (int)reader["idProdavac"],
                IdSkladiste = (int)reader["idSkladiste"]
            });
        }
        return list;
    }
}
```

---

## 📨 Komunikacioni Modeli

### Request.cs

```csharp
[Serializable]
public class Request
{
    public Operation Operation { get; set; }
    public object Data { get; set; }
}
```

**Kako se koristi:**
```csharp
Request req = new Request {
    Operation = Operation.VratiSve,
    Data = new Racun { DatumFilter = "2026-02" }
};
```

### Response.cs

```csharp
[Serializable]
public class Response
{
    public bool IsSuccessful { get; set; }
    public object Data { get; set; }
    public string Message { get; set; }
}
```

**Kako se koristi:**
```csharp
if (response.IsSuccessful) {
    List<Racun> racuni = JsonSerializer.Deserialize<List<Racun>>(
        response.Data.ToString()
    );
}
```

### Operation.cs (Enum)

```csharp
[Serializable]
public enum Operation
{
    Login,
    VratiSve,
    VratiJedan,
    Dodaj,
    Izmeni,
    Obrisi,
    DodajRacun,
    IzmeniRacun
}
```

---

## 🎨 Atributi za UI Binding

### [Browsable(false)]
Sakriva property iz DataGridView automatskog generisanja kolona.

```csharp
[Browsable(false)]
public Prodavac Prodavac { get; set; }  // Neće se prikazati
```

### [DisplayName("Text")]
Prikazuje custom naziv kolone u DataGridView.

```csharp
[DisplayName("Prodavac")]
public string ImeProdavca => Prodavac?.ImePrezime ?? "";
```

---

## ✅ Best Practices u Models Layer-u

1. **Null-safety:** Uvek koristiti `?.` i `?? ""` za null checks
2. **Decimal separator:** `Replace(',', '.')` za SQL compatibility
3. **Date format:** `{datum:yyyy-MM-dd}` za SQL DATE
4. **LIKE queries:** `'%{value}%'` za partial match
5. **Constructor inicijalizacija:** Inicijalizuj nested objekte u constructor-u
6. **Browsable attributi:** Sakrij kompleksne objekte od DataGridView-a
7. **Empty conditions:** Vraćaj `""` umesto `"1=1"` kada nema filtera

---

## 🔗 Veze između Modela (ER Dijagram)

```
Prodavac ──────────┐
  │                │
  │ M:N            │ 1:N
  │ (ProdSklad)    │
  │                ↓
Skladiste        Racun
                   │ N:1
                   ├─────→ Kupac ←──┬── Firma
                   │                └── FizickoLice
                   │
                   │ 1:N
                   ↓
              StavkaRacuna
                   │ N:1
                   ↓
                 Oprema
```

Ova dokumentacija omogućava potpuno razumevanje kako svaki model funkcioniše i kako su svi međusobno povezani!
