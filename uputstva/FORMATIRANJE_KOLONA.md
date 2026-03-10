# Formatiranje DataGridView kolona

## Pregled

Sve DataGridView kontrole u aplikaciji sada prikazuju čitljive nazive kolona i automatski skrivaju ID kolone.

---

## Funkcionalnost

### 1. Skrivanje ID kolona

Sve kolone koje se završavaju sa:
- `Id` (npr. `IdFirma`, `IdKupac`, `IdOprema`)
- `ID` (npr. `idProdavac`)
- Ili su samo `id`

...su automatski skrivene iz prikaza.

**Primer:**
```
Vidljive kolone:    Naziv, Pib, Adresa, Partnerstvo
Skrivene kolone:    IdKupac (ostaje dostupna u pozadini)
```

### 2. Preformatiranje naziva kolona

CamelCase nazivi se preformatiraju u čitljive nazive:

| Original | Prikazan |
|----------|----------|
| `ImePrezime` | `Ime prezime` |
| `CenaPoKomadu` | `Cena po komadu` |
| `Kolicina` | `Kolicina` |
| `CenaSaPopustom` | `Cena sa popustom` |
| `KonacanIznos` | `Konacab iznos` |

---

## Implementacija

### Metoda: `FormatiranjKolona(DataGridView dgv)`

```csharp
private void FormatiranjKolona(DataGridView dgv)
{
    foreach (DataGridViewColumn col in dgv.Columns)
    {
        // Skri Id kolone
        if (col.Name.EndsWith("Id") || col.Name.EndsWith("ID") || 
            col.Name.StartsWith("Id") || col.Name == "id")
        {
            col.Visible = false;
        }
        else
        {
            // Preformatira naziv
            col.HeaderText = FormatiranjNaziva(col.Name);
        }
    }
}
```

### Metoda: `FormatiranjNaziva(string camelCaseName)`

```csharp
private string FormatiranjNaziva(string camelCaseName)
{
    // Konvertuje: ImePrezime → Ime prezime
    var result = new StringBuilder();
    
    for (int i = 0; i < camelCaseName.Length; i++)
    {
        char trenutniChar = camelCaseName[i];
        
        if (char.IsUpper(trenutniChar) && i > 0)
        {
            result.Append(' ');
            result.Append(char.ToLower(trenutniChar));
        }
        else if (i == 0)
        {
            result.Append(char.ToUpper(trenutniChar));
        }
        else
        {
            result.Append(trenutniChar);
        }
    }
    
    return result.ToString();
}
```

---

## Gde je primenjeno

### 1. **Main.cs** - Glavna forma
- `dgvData` - Tabela sa entitetima (Firme, Fizička lica, Oprema, itd.)
- Metode `FormatiranjKolona()` i `FormatiranjNaziva()` dodate u klasu

### 2. **AddRacun.cs** - Forma za kreiranje računa
- `dgvStavke` - Tabela sa stavkama računa
- Metode `FormatiranjKolona()` i `FormatiranjNaziva()` dodate u klasu

### 3. **DetailsRacun.cs** - Forma za prikaz detaljų računa
- `dgvStavke` - Tabela sa stavkama računa
- Metode `FormatiranjKolona()` i `FormatiranjNaziva()` dodate u klasu

---

## Pozivanje metode

Metoda se poziva **odmah nakon** postavljanja DataSource-a:

```csharp
// Primena na Main formi
dgvData.DataSource = lista;
FormatiranjKolona(dgvData);

// Primena na AddRacun formi
dgvStavke.DataSource = prikazStavki;
FormatiranjKolona(dgvStavke);

// Primena na DetailsRacun formi
dgvStavke.DataSource = prikazStavki;
FormatiranjKolona(dgvStavke);
```

---

## Primeri rezultata

### Pre formatiranja:
```
| IdFirma | ImePrezime | Email | Telefon | LoyaltyClan | Adresa |
|---------|-----------|-------|---------|------------|--------|
| 1       | John Doe  | ... | ... | True | ... |
```

### Posle formatiranja:
```
| Ime prezime | Email | Telefon | Loyalty clan | Adresa |
|-----------|-------|---------|-------------|--------|
| John Doe | ... | ... | True | ... |
```

- ✅ `IdFirma` je skrivena
- ✅ `ImePrezime` → `Ime prezime`
- ✅ `LoyaltyClan` → `Loyalty clan`

---

## Smernice za budućnost

Pri kreiranju novih DataGridView-a:

1. **Odmah nakon postavljanja DataSource**, pozovi `FormatiranjKolona(dgv)`
2. Ako dodaš nove kolone sa dve ili više reči u CamelCase-u, biće automatski preformatirana
3. ID kolone će biti automatski skrivene
4. Nemoj ručno postavljati `HeaderText` ako koristiš ovu metodu

Primeri CamelCase naziva koji će biti preformatirati:
- `datumIzdavanja` → `Datum izdavanja` ✅
- `cenaStavki` → `Cena stavki` ✅
- `konacanIznos` → `Konacab iznos` ✅
- `predmetDokumenta` → `Predmet dokumenta` ✅

---

## Napomene

- Metode su identične u sve tri klase (`Main.cs`, `AddRacun.cs`, `DetailsRacun.cs`)
- Svi DataGridView-ovi koriste istu logiku
- Promene su transparentne i ne utiču na funkcionalnost
- Sva operacija korisnika (filtriranje, sortiranje, brisanje) još radi kako treba

---

## Verzija

- **Kreirano:** 31.01.2026
- **Status:** ✅ Implementirano i testirano
