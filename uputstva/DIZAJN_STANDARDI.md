# Standardi dizajna formi

## Pregled

Sve forme u aplikaciji sada koriste uniforman dizajn i konzistentna imena kontrola.

---

## Imenovanje kontrola

### Prefiksi za tipove kontrola

| Tip kontrole | Prefiks | Primer |
|--------------|---------|--------|
| Button | `btn` | `btnLogin`, `btnPrikazi`, `btnSacuvaj` |
| TextBox | `txt` | `txtEmail`, `txtPassword`, `txtFilter1` |
| ComboBox | `cmb` | `cmbEntitet`, `cmbKupac`, `cmbOprema` |
| Label | `lbl` | `lblTitle`, `lblEmail`, `lblFilter1` |
| DataGridView | `dgv` | `dgvData`, `dgvStavke` |
| Panel | `panel` | `panelInputs`, `panelDetails`, `panelUkupno` |
| GroupBox | `groupBox` | `groupBoxFilter`, `groupBoxStavke` |
| NumericUpDown | `num` | `numKolicina` |
| DateTimePicker | `dtp` | `dtpDatum` |

---

## Stil formi

### Boje

**Pozadina forme:**
- `BackColor = SystemColors.ControlDark`

**Dugmići:**
- Potvrda/Sačuvaj: `BackColor = Color.LightGreen`
- Prikaz: `BackColor = Color.LightBlue`
- Dodaj: `BackColor = Color.LightGreen` ili `Color.MediumSeaGreen`
- Brisanje/Otkaži: `BackColor = Color.LightCoral` ili `Color.Tomato`
- Ažuriranje: `BackColor = Color.LightBlue`

**Stil dugmića:**
- `FlatStyle = FlatStyle.Flat` (moderne forme)
- `FlatStyle = FlatStyle.Standard` (starije forme - opciono)

### Fontovi

**Naslovi:**
- `Font = new Font("Bahnschrift SemiBold", 16-18F, FontStyle.Bold)`
- `Font = new Font("Segoe UI", 16F, FontStyle.Bold)`

**Labele:**
- `Font = new Font("Bahnschrift", 11F, FontStyle.Regular)`
- `Font = new Font("Segoe UI", 10F, FontStyle.Bold)`

**Dugmići:**
- `Font = new Font("Bahnschrift", 11-12F, FontStyle.Bold)`
- `Font = new Font("Segoe UI", 12F, FontStyle.Bold)`

**Input polja:**
- `Font = new Font("Segoe UI", 10-11F)`

### Dimenzije

**Dugmići:**
- Standardni: `Size = new Size(100-150, 30-40)`
- Mali: `Size = new Size(80-100, 25-30)`

**TextBox:**
- Kratki: `Size = new Size(200, 25)`
- Srednji: `Size = new Size(260-300, 27-30)`
- Dugački: `Size = new Size(350-400, 30)`

---

## Specifične forme

### 1. Login.cs / Login.Designer.cs

**Kontrole:**
- `lblTitle` - Naslov forme ("Prijava sistema")
- `lblEmail` - Labela za email
- `txtEmail` - TextBox za email (sa PlaceholderText)
- `lblPassword` - Labela za lozinku
- `txtPassword` - TextBox za lozinku (PasswordChar = '●')
- `btnLogin` - Dugme za prijavu

**Specifičnosti:**
- `FormBorderStyle = FormBorderStyle.FixedDialog`
- `MaximizeBox = false`
- `StartPosition = FormStartPosition.CenterScreen`

### 2. Main.cs / Main.Designer.cs

**Kontrole:**
- `dgvData` - DataGridView za prikaz podataka
- `cmbEntitet` - ComboBox za izbor entiteta
- `btnPrikazi` - Dugme za prikaz podataka
- `btnDodaj` - Dugme za dodavanje novog zapisa
- `groupBoxFilter` - Grupa kontrola za filtriranje
  - `txtFilter1` - Prvi filter (ime, naziv, itd.)
  - `txtFilter2` / `cmbFilter2` - Drugi filter (dinamički - TextBox ili ComboBox)
  - `lblFilter1` - Labela za prvi filter
  - `lblFilter2` - Labela za drugi filter
  - `btnClearFilter` - Dugme za brisanje filtera

**Specifičnosti:**
- `StartPosition = FormStartPosition.CenterScreen`
- `FormBorderStyle = FormBorderStyle.Sizable`
- DataGridView: `SelectionMode = FullRowSelect`, `ReadOnly = true`

### 3. Add.cs / Add.Designer.cs

**Kontrole:**
- `lblTitle` - Naslov ("Dodaj novi zapis")
- `lblTipObjekta` - Labela za tip objekta
- `cmbTipObjekta` - ComboBox za izbor tipa
- `panelInputs` - Panel sa AutoScroll za dinamičke input kontrole
- `btnSacuvaj` - Dugme za čuvanje (zeleno)
- `btnOtkazi` - Dugme za otkazivanje (crveno)

**Specifičnosti:**
- `StartPosition = FormStartPosition.CenterParent`
- Panel: `AutoScroll = true`, `BorderStyle = FixedSingle`

### 4. Details.cs / Details.Designer.cs

**Kontrole:**
- `lblTitle` - Naslov ("Detalji")
- `panelDetails` - Panel za prikaz detalja
- `btnAzuriraj` - Dugme za ažuriranje (plavo)
- `btnObrisi` - Dugme za brisanje (crveno)

**Specifičnosti:**
- `StartPosition = FormStartPosition.CenterParent`

### 5. AddRacun.cs / AddRacun.Designer.cs

**Kontrole:**
- `lblTitle` - Naslov ("Kreiranje Računa")
- `lblKupac`, `cmbKupac` - Izbor kupca
- `lblDatum`, `dtpDatum` - Datum izdavanja
- `groupBoxStavke` - Grupa za stavke računa
  - `lblOprema`, `cmbOprema` - Izbor opreme
  - `lblKolicina`, `numKolicina` - Unos količine
  - `btnDodajStavku` - Dodavanje stavke (zeleno)
  - `btnObrisiStavku` - Brisanje stavke (crveno)
  - `dgvStavke` - Tabela sa stavkama
- `panelUkupno` - Panel sa finansijskim informacijama
  - `lblCenaStavki` - Cena stavki
  - `lblPopust` - Popust
  - `lblCenaSaPopustom` - Cena sa popustom
  - `lblPDV` - PDV iznos
  - `lblKonacanIznos` - Konačan iznos (bold)
- `btnSacuvaj` - Čuvanje računa
- `btnOtkazi` - Otkazivanje

### 6. DetailsRacun.cs / DetailsRacun.Designer.cs

**Kontrole:**
- `lblTitle` - Naslov ("Detalji Računa")
- `lblIdRacun` - Broj računa (bold)
- `lblDatum` - Datum izdavanja
- `lblProdavac` - Prodavac
- `lblKupac` - Kupac
- `groupBoxStavke` - Stavke računa
  - `dgvStavke` - Tabela stavki
- `panelUkupno` - Finansijske informacije
- `btnAzuriraj` - Ažuriranje računa (plavo)
- `btnObrisi` - Brisanje računa (crveno)
- `btnZatvori` - Zatvaranje forme (sivo)

---

## Doslednost u kodiranju

### Event handleri

**Stil imenovanja:**
- `kontrola_Dogadjaj` (npr. `btnLogin_Click`, `cmbEntitet_SelectedIndexChanged`)

**Primeri:**
```csharp
private void btnLogin_Click(object sender, EventArgs e)
private void cmbEntitet_SelectedIndexChanged(object sender, EventArgs e)
private void dgvData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
```

### Kreiranje dinamičkih kontrola

Pri kreiranju kontrola u kodu, koristi isti stil:

```csharp
Label lbl = new Label
{
    Text = "Naziv:",
    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
    AutoSize = true,
    Location = new Point(10, yPos)
};

TextBox txt = new TextBox
{
    Name = $"txt{propertyName}",
    Font = new Font("Segoe UI", 10F),
    Size = new Size(300, 27),
    Location = new Point(150, yPos)
};
```

---

## Izmene napravljene

### Login forma
- ✅ `button1` → `btnLogin`
- ✅ `textBox1` → `txtEmail`
- ✅ `textBox2` → `txtPassword`
- ✅ Dodati labele `lblTitle`, `lblEmail`, `lblPassword`
- ✅ Dodati PlaceholderText na TextBox kontrole
- ✅ Font: Bahnschrift za labele, Segoe UI za input polja

### Main forma
- ✅ `dataGridView1` → `dgvData`
- ✅ `comboBox1` → `cmbEntitet`
- ✅ `button1` → `btnPrikazi`
- ✅ FlatStyle dugmića
- ✅ DataGridView: `SelectionMode`, `ReadOnly`, `AutoSizeColumnsMode`
- ✅ Naslovna linija forme: "Sistem za upravljanje prodavnicom"

### Add forma
- ✅ Već ima konzistentna imena (dobro urađeno!)
- ✅ Uniforman stil dugmića

### Details forma
- ✅ Već ima konzistentna imena
- ✅ Uniforman stil

### AddRacun forma
- ✅ Već ima konzistentna imena i dobar dizajn
- ✅ Flat style dugmići sa specifičnim bojama

### DetailsRacun forma
- ✅ Već ima konzistentna imena
- ✅ Uniforman dizajn

---

## Smernice za buduće forme

Pri kreiranju novih formi, pridržavaj se sledećih pravila:

1. **Uvek koristi prefikse** za imena kontrola (`btn`, `txt`, `cmb`, itd.)
2. **Konzistentne boje** za dugmiće prema funkciji
3. **Flat style** za moderne forme
4. **Segoe UI** ili **Bahnschrift** fontovi
5. **StartPosition = CenterParent** ili **CenterScreen**
6. **PlaceholderText** za input polja gde ima smisla
7. **ReadOnly i SelectionMode** za DataGridView
8. **AutoScroll** za panele sa dinamičkim sadržajem
9. **Smislena imena** event handlera (`kontrola_Dogadjaj`)

---

## Provera

Sve forme su kompajlirane i testirane nakon izmena:
- ✅ Build uspešan (0 errora)
- ✅ Sve kontrole pravilno imenovane
- ✅ Event handleri ažurirani
- ✅ Uniforman dizajn kroz celu aplikaciju
