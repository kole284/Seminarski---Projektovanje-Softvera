# 🖼️ CLIENT Layer - Detaljno Objašnjenje

## 🎯 Uloga Client Layer-a

Client layer je **presentation tier** (Windows Forms aplikacija) koja:
1. Prikazuje UI korisniku
2. Validira user input
3. Komunicira sa serverom kroz TCP socket
4. Prikazuje rezultate u DataGridView-u

**Komponente:**
- `Login.cs` - Forma za autentifikaciju
- `Main.cs` - Glavna forma sa DataGridView + filteri
- `Add.cs` - Generička forma za dodavanje
- `Details.cs` - Generička forma za prikaz/izmenu
- `AddRacun.cs` - Specijalizovana forma za dodavanje računa
- `DetailsRacun.cs` - Specijalizovana forma za prikaz računa
- `CommunicationHelper.cs` - Singleton za TCP komunikaciju
- `Sesija.cs` - Session management (ulogovani prodavac)

---

## 🔐 Login.cs - Autentifikacija

### UI Elementi

```csharp
private Label lblTitle;         // "🔐 Prijava sistema"
private Label lblEmail;         // "Email:"
private TextBox txtEmail;       // Input polje za email
private Label lblPassword;      // "Lozinka:"
private TextBox txtPassword;    // Input polje za password (PasswordChar = '●')
private Button btnLogin;        // "✓ Prijavi se" dugme
```

### Constructor - UI Enhancements

```csharp
public Login()
{
    InitializeComponent();
    
    // 1. Hover efekat za login dugme
    btnLogin.MouseEnter += (s, ev) => 
        btnLogin.BackColor = Color.FromArgb(39, 174, 96);  // Tamnija nijansa
    btnLogin.MouseLeave += (s, ev) => 
        btnLogin.BackColor = Color.FromArgb(46, 204, 113);  // Originalna
    
    // 2. Enter key support - pritisni Enter za brzi login
    txtPassword.KeyPress += (s, ev) => {
        if (ev.KeyChar == (char)Keys.Enter) {
            button1_Click(btnLogin, EventArgs.Empty);
        }
    };
}
```

### button1_Click - Login Logic

```csharp
private void button1_Click(object sender, EventArgs e)
{
    try
    {
        // 1. Admin bypass
        if (txtEmail.Text.Trim().ToLower() == "admin" && txtPassword.Text == "admin")
        {
            Sesija.IsAdmin = true;
            Sesija.UlogovaniProdavac = new Prodavac
            {
                Ime = "Admin",
                Prezime = "User",
                Email = "admin"
            };
            
            this.Hide();
            Main main = new Main();
            main.ShowDialog();
            this.Close();
            return;
        }
        
        // 2. Normalan login
        Prodavac prodavac = new Prodavac
        {
            Email = txtEmail.Text.Trim(),
            Password = txtPassword.Text
        };
        
        // 3. Pošalji Request serveru
        Request req = new Request
        {
            Operation = Operation.Login,
            Data = prodavac
        };
        
        Response res = CommunicationHelper.Instance.SendRequest(req);
        
        // 4. Proveri odgovor
        if (res.IsSuccessful)
        {
            // 5. Deserialize prodavca iz JSON-a
            Prodavac ulogovani = JsonSerializer.Deserialize<Prodavac>(
                res.Data.ToString()
            );
            
            // 6. Sačuvaj u sesiju
            Sesija.UlogovaniProdavac = ulogovani;
            Sesija.IsAdmin = false;
            
            // 7. Otvori Main formu
            this.Hide();
            Main main = new Main();
            main.ShowDialog();
            this.Close();
        }
        else
        {
            // 8. Login failed
            MessageBox.Show(res.Message, "Greška", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Greška pri povezivanju: {ex.Message}", 
            "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
```

### Flow Dijagram

```
[USER] Unese email + password
   ↓
[btnLogin.Click]
   ↓
Admin check? → Da → Postavi Sesija.IsAdmin = true → Open Main
           ↓ Ne
   ↓
Kreiraj Prodavac objekat
   ↓
CommunicationHelper.SendRequest(Operation.Login, prodavac)
   ↓ [TCP Socket]
[SERVER] Controller.HandleLogin()
   ↓
DatabaseBroker.VratiJedan(prodavac)
   ↓ SQL: SELECT * WHERE email='...' AND password='...'
   ↓
Response { IsSuccessful = true/false, Data = Prodavac }
   ↓ [TCP Socket]
[CLIENT] Deserialize Response
   ↓
Success? → Da → Deserialize Prodavac → Sesija.UlogovaniProdavac → Open Main
         ↓ Ne → MessageBox.Show("Pogrešan email ili lozinka")
```

---

## 🏠 Main.cs - Centralna Forma

### UI Elementi

```csharp
// Filter grupa
private GroupBox groupBoxFilter;
private TextBox txtFilter1;         // Prvi filter (dinamički label)
private TextBox txtFilter2;         // Drugi filter
private ComboBox cmbFilter2;        // Za kategoriju opreme
private ComboBox cmbProdavac;       // Filter po prodavcu (Računi)
private ComboBox cmbKupac;          // Filter po kupcu (Računi)
private Label lblFilter1, lblFilter2, lblProdavac, lblKupac;
private Button btnClearFilter;      // Obriši sve filtere

// Prikaz podataka
private DataGridView dgvData;       // Tabela sa rezultatima

// Akcije
private ComboBox cmbEntitet;        // Izbor tipa (Prodavci, Firme, ...)
private Button btnPrikazi;          // Prikaži sa filterima
private Button btnDodaj;            // Dodaj novi entitet
```

### Main_Load - Inicijalizacija

```csharp
private void Main_Load(object sender, EventArgs e)
{
    // 1. Popuni ComboBox sa tipovima entiteta
    cmbEntitet.Items.Add("Prodavci");
    cmbEntitet.Items.Add("Firme");
    cmbEntitet.Items.Add("Fizička lica");
    cmbEntitet.Items.Add("Oprema");
    cmbEntitet.Items.Add("Skladišta");
    cmbEntitet.Items.Add("Računi");
    cmbEntitet.SelectedIndex = 0;
    
    // 2. DataGridView styling - moderan izgled
    dgvData.BackgroundColor = Color.White;
    dgvData.BorderStyle = BorderStyle.None;
    dgvData.EnableHeadersVisualStyles = false;
    
    // Header styling
    dgvData.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
    dgvData.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
    dgvData.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
    
    // Selection color
    dgvData.DefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
    dgvData.DefaultCellStyle.SelectionForeColor = Color.White;
    
    // Alternativne boje redova
    dgvData.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(236, 240, 241);
    
    // 3. Tooltips za user guidance
    ToolTip toolTip = new ToolTip();
    toolTip.SetToolTip(cmbEntitet, "Izaberite tip podataka koje želite da vidite");
    toolTip.SetToolTip(btnPrikazi, "Prikaži podatke sa primenjenim filterima");
    toolTip.SetToolTip(btnDodaj, "Dodaj novi unos za izabrani tip podataka");
    toolTip.SetToolTip(btnClearFilter, "Obriši sve filtere i prikaži sve podatke");
    toolTip.SetToolTip(dgvData, "Dvaput kliknite na red za prikaz detalja");
    
    // 4. Placeholder text za filter polja
    txtFilter1.Text = "Unesite termin za pretragu...";
    txtFilter1.ForeColor = Color.Gray;
    txtFilter1.GotFocus += (s, ev) => {
        if (txtFilter1.Text == "Unesite termin za pretragu...") {
            txtFilter1.Text = "";
            txtFilter1.ForeColor = Color.Black;
        }
    };
    txtFilter1.LostFocus += (s, ev) => {
        if (string.IsNullOrWhiteSpace(txtFilter1.Text)) {
            txtFilter1.Text = "Unesite termin za pretragu...";
            txtFilter1.ForeColor = Color.Gray;
        }
    };
    
    // Isto za txtFilter2...
    
    // 5. Hover efekti na dugmići
    btnPrikazi.MouseEnter += (s, ev) => 
        btnPrikazi.BackColor = Color.FromArgb(41, 128, 185);
    btnPrikazi.MouseLeave += (s, ev) => 
        btnPrikazi.BackColor = Color.FromArgb(52, 152, 219);
    
    btnDodaj.MouseEnter += (s, ev) => 
        btnDodaj.BackColor = Color.FromArgb(39, 174, 96);
    btnDodaj.MouseLeave += (s, ev) => 
        btnDodaj.BackColor = Color.FromArgb(46, 204, 113);
    
    btnClearFilter.MouseEnter += (s, ev) => 
        btnClearFilter.BackColor = Color.FromArgb(192, 57, 43);
    btnClearFilter.MouseLeave += (s, ev) => 
        btnClearFilter.BackColor = Color.FromArgb(231, 76, 60);
    
    // 6. Double-click event za Details
    dgvData.CellDoubleClick += dgvData_CellDoubleClick;
    
    // 7. Inicijalizuj filtere
    UpdateFilterLabels();
}
```

### button1_Click (Prikaži) - Glavna Logika

```csharp
private void button1_Click(object sender, EventArgs e)
{
    string izbor = cmbEntitet.SelectedItem.ToString();
    
    IDomainObject model = null;
    
    // Helper funkcija za validaciju filtera (ignoriše placeholder text)
    bool IsValidFilter(string text) => 
        !string.IsNullOrWhiteSpace(text) && 
        text != "Unesite termin za pretragu..." && 
        text != "Dodatni filter...";
    
    switch (izbor)
    {
        case "Firme":
            model = new Firma();
            if (IsValidFilter(txtFilter1.Text))
                ((Firma)model).Naziv = txtFilter1.Text;
            if (IsValidFilter(txtFilter2.Text))
                ((Firma)model).Pib = txtFilter2.Text;
            break;
        
        case "Fizička lica":
            model = new FizickoLice();
            if (IsValidFilter(txtFilter1.Text))
                ((FizickoLice)model).ImePrezime = txtFilter1.Text;
            if (IsValidFilter(txtFilter2.Text))
            {
                // Smart detection: @ = email, else = telefon
                if (txtFilter2.Text.Contains("@"))
                    ((FizickoLice)model).Email = txtFilter2.Text;
                else
                    ((FizickoLice)model).Telefon = txtFilter2.Text;
            }
            break;
        
        case "Oprema":
            model = new Oprema();
            if (IsValidFilter(txtFilter1.Text))
                ((Oprema)model).Ime = txtFilter1.Text;
            // ComboBox za kategoriju
            if (cmbFilter2.SelectedItem != null && cmbFilter2.SelectedIndex > 0)
                ((Oprema)model).Kategorija = cmbFilter2.SelectedItem.ToString();
            break;
        
        case "Prodavci":
            model = new Prodavac();
            if (IsValidFilter(txtFilter1.Text))
                ((Prodavac)model).ImePrezime = txtFilter1.Text;
            if (IsValidFilter(txtFilter2.Text))
                ((Prodavac)model).Email = txtFilter2.Text;
            break;
        
        case "Skladišta":
            model = new Skladiste();
            if (IsValidFilter(txtFilter1.Text))
                ((Skladiste)model).Naziv = txtFilter1.Text;
            if (IsValidFilter(txtFilter2.Text))
                ((Skladiste)model).Adresa = txtFilter2.Text;
            break;
        
        case "Računi":
            model = new Racun();
            
            // Filter 1: ID Računa
            if (IsValidFilter(txtFilter1.Text) && int.TryParse(txtFilter1.Text, out int idRacun))
                ((Racun)model).IdRacun = idRacun;
            
            // Filter 2: Datum (YYYY / YYYY-MM / YYYY-MM-DD)
            if (IsValidFilter(txtFilter2.Text))
                ((Racun)model).DatumFilter = txtFilter2.Text;
            
            // Filter 3: Prodavac (ComboBox)
            if (cmbProdavac.SelectedItem != null && cmbProdavac.SelectedIndex > 0)
            {
                var prodavac = cmbProdavac.SelectedItem as Prodavac;
                if (prodavac != null)
                    ((Racun)model).Prodavac = new Prodavac { IdProdavac = prodavac.IdProdavac };
            }
            
            // Filter 4: Kupac (ComboBox)
            if (cmbKupac.SelectedItem != null && cmbKupac.SelectedIndex > 0)
            {
                var kupac = cmbKupac.SelectedItem as dynamic;
                if (kupac != null && kupac.IdKupac > 0)
                    ((Racun)model).Kupac = new Kupac { IdKupac = kupac.IdKupac };
            }
            break;
    }
    
    if (model != null)
    {
        // Pošalji Request serveru
        Request req = new Request { Operation = Operation.VratiSve, Data = model };
        Response res = CommunicationHelper.Instance.SendRequest(req);
        
        if (res.IsSuccessful && res.Data != null)
        {
            // Deserialize različito zavisno od tipa
            string jsonData = res.Data.ToString();
            
            switch (izbor)
            {
                case "Prodavci":
                    var prodavci = JsonSerializer.Deserialize<List<Prodavac>>(jsonData);
                    dgvData.DataSource = prodavci;
                    break;
                
                case "Firme":
                    var firme = JsonSerializer.Deserialize<List<Firma>>(jsonData);
                    dgvData.DataSource = firme;
                    break;
                
                case "Fizička lica":
                    var fizicka = JsonSerializer.Deserialize<List<FizickoLice>>(jsonData);
                    dgvData.DataSource = fizicka;
                    break;
                
                case "Oprema":
                    var oprema = JsonSerializer.Deserialize<List<Oprema>>(jsonData);
                    dgvData.DataSource = oprema;
                    break;
                
                case "Skladišta":
                    var skladista = JsonSerializer.Deserialize<List<Skladiste>>(jsonData);
                    dgvData.DataSource = skladista;
                    break;
                
                case "Računi":
                    var racuni = JsonSerializer.Deserialize<List<Racun>>(jsonData);
                    dgvData.DataSource = racuni;
                    break;
            }
        }
        else
        {
            MessageBox.Show(res.Message, "Greška", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
```

### UpdateFilterLabels() - Dinamički Filter UI

```csharp
private void UpdateFilterLabels()
{
    string izbor = cmbEntitet.SelectedItem.ToString();
    
    // Sakrij sve filter kontrole prvo
    txtFilter2.Visible = false;
    cmbFilter2.Visible = false;
    lblFilter2.Visible = false;
    cmbProdavac.Visible = false;
    cmbKupac.Visible = false;
    lblProdavac.Visible = false;
    lblKupac.Visible = false;
    
    switch (izbor)
    {
        case "Firme":
            lblFilter1.Text = "Naziv:";
            lblFilter2.Text = "PIB:";
            txtFilter2.Visible = true;
            lblFilter2.Visible = true;
            break;
        
        case "Fizička lica":
            lblFilter1.Text = "Ime i Prezime:";
            lblFilter2.Text = "Email ili Telefon:";  // Smart filter!
            txtFilter2.Visible = true;
            lblFilter2.Visible = true;
            break;
        
        case "Oprema":
            lblFilter1.Text = "Naziv:";
            lblFilter2.Text = "Kategorija:";
            cmbFilter2.Visible = true;
            lblFilter2.Visible = true;
            
            // Popuni ComboBox sa kategorijama
            cmbFilter2.Items.Clear();
            cmbFilter2.Items.Add("--Sve kategorije--");
            cmbFilter2.Items.Add("Laptop");
            cmbFilter2.Items.Add("Desktop");
            cmbFilter2.Items.Add("Telefon");
            cmbFilter2.Items.Add("Tablet");
            cmbFilter2.Items.Add("Ostalo");
            cmbFilter2.SelectedIndex = 0;
            break;
        
        case "Prodavci":
            lblFilter1.Text = "Ime i Prezime:";
            lblFilter2.Text = "Email:";
            txtFilter2.Visible = true;
            lblFilter2.Visible = true;
            break;
        
        case "Skladišta":
            lblFilter1.Text = "Naziv:";
            lblFilter2.Text = "Adresa:";
            txtFilter2.Visible = true;
            lblFilter2.Visible = true;
            break;
        
        case "Računi":
            lblFilter1.Text = "ID Računa:";
            lblFilter2.Text = "Datum (YYYY / YYYY-MM / YYYY-MM-DD):";
            txtFilter2.Visible = true;
            lblFilter2.Visible = true;
            
            // Prikaži i ComboBox filtere
            cmbProdavac.Visible = true;
            cmbKupac.Visible = true;
            lblProdavac.Visible = true;
            lblKupac.Visible = true;
            
            // Učitaj prodavce i kupce
            UcitajProdavce();
            UcitajKupce();
            break;
    }
}
```

### UcitajProdavce() & UcitajKupce()

```csharp
private void UcitajProdavce()
{
    Request req = new Request { 
        Operation = Operation.VratiSve, 
        Data = new Prodavac() 
    };
    Response res = CommunicationHelper.Instance.SendRequest(req);
    
    if (res.IsSuccessful)
    {
        var prodavci = JsonSerializer.Deserialize<List<Prodavac>>(res.Data.ToString());
        
        cmbProdavac.Items.Clear();
        cmbProdavac.Items.Add("--Svi prodavci--");
        
        foreach (var p in prodavci)
        {
            cmbProdavac.Items.Add(p);  // Uses ToString() → ImePrezime
        }
        
        cmbProdavac.DisplayMember = "ImePrezime";
        cmbProdavac.ValueMember = "IdProdavac";
        cmbProdavac.SelectedIndex = 0;
    }
}

private void UcitajKupce()
{
    // Učitaj i Firme i Fizička lica
    // ...slično kao UcitajProdavce()
}
```

### dgvData_CellDoubleClick - Otvori Details

```csharp
private void dgvData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
{
    if (e.RowIndex < 0) return;  // Kliknuto na header
    
    string izbor = cmbEntitet.SelectedItem.ToString();
    
    if (izbor == "Računi")
    {
        // Specijalizovana forma za račune
        Racun racun = dgvData.SelectedRows[0].DataBoundItem as Racun;
        DetailsRacun details = new DetailsRacun(racun);
        details.ShowDialog();
    }
    else
    {
        // Generičkaforma za ostale entitete
        IDomainObject obj = dgvData.SelectedRows[0].DataBoundItem as IDomainObject;
        Details details = new Details(obj, izbor);
        details.ShowDialog();
    }
    
    // Refresh nakon zatvaranja
    button1_Click(btnPrikazi, EventArgs.Empty);
}
```

---

## ➕ Add.cs - Generička Forma za Dodavanje

### Dinamički UI Generation

```csharp
public Add(string tipEntiteta)
{
    InitializeComponent();
    this.tipEntiteta = tipEntiteta;
    
    switch (tipEntiteta)
    {
        case "Prodavci":
            GenerisiPoljaProdavac();
            break;
        case "Firme":
            GenerisiPoljaFirma();
            break;
        // ... ostali tipovi
    }
}
```

### GenerisiPoljaProdavac()

```csharp
private void GenerisiPoljaProdavac()
{
    // Ime
    Label lblIme = new Label { Text = "Ime:", Location = new Point(20, 20) };
    TextBox txtIme = new TextBox { Name = "txtIme", Location = new Point(150, 20) };
    
    // Prezime
    Label lblPrezime = new Label { Text = "Prezime:", Location = new Point(20, 60) };
    TextBox txtPrezime = new TextBox { Name = "txtPrezime", Location = new Point(150, 60) };
    
    // Email
    Label lblEmail = new Label { Text = "Email:", Location = new Point(20, 100) };
    TextBox txtEmail = new TextBox { Name = "txtEmail", Location = new Point(150, 100) };
    
    // Telefon
    Label lblTelefon = new Label { Text = "Telefon:", Location = new Point(20, 140) };
    TextBox txtTelefon = new TextBox { Name = "txtTelefon", Location = new Point(150, 140) };
    
    // Password
    Label lblPassword = new Label { Text = "Lozinka:", Location = new Point(20, 180) };
    TextBox txtPassword = new TextBox { 
        Name = "txtPassword", 
        Location = new Point(150, 180),
        PasswordChar = '●'
    };
    
    // Skladišta (CheckedListBox za Many-to-Many)
    Label lblSkladista = new Label { Text = "Skladišta:", Location = new Point(20, 220) };
    CheckedListBox clbSkladista = new CheckedListBox { 
        Name = "clbSkladista", 
        Location = new Point(150, 220),
        Size = new Size(200, 100)
    };
    
    // Učitaj skladišta
    UcitajSkladista(clbSkladista);
    
    // Dodaj kontrole na formu
    this.Controls.Add(lblIme);
    this.Controls.Add(txtIme);
    // ... ostale kontrole
}
```

### btnSacuvaj_Click

```csharp
private void btnSacuvaj_Click(object sender, EventArgs e)
{
    try
    {
        IDomainObject obj = null;
        
        switch (tipEntiteta)
        {
            case "Prodavci":
                Prodavac p = new Prodavac
                {
                    Ime = this.Controls["txtIme"].Text,
                    Prezime = this.Controls["txtPrezime"].Text,
                    Email = this.Controls["txtEmail"].Text,
                    Telefon = this.Controls["txtTelefon"].Text,
                    Password = this.Controls["txtPassword"].Text
                };
                
                obj = p;
                
                // Dodaj Prodavca
                Request req1 = new Request { Operation = Operation.Dodaj, Data = p };
                Response res1 = CommunicationHelper.Instance.SendRequest(req1);
                
                if (!res1.IsSuccessful)
                {
                    MessageBox.Show(res1.Message);
                    return;
                }
                
                // Uzmi ID novododatog prodavca
                Request reqGet = new Request { 
                    Operation = Operation.VratiSve, 
                    Data = new Prodavac { Email = p.Email } 
                };
                Response resGet = CommunicationHelper.Instance.SendRequest(reqGet);
                var prodavci = JsonSerializer.Deserialize<List<Prodavac>>(resGet.Data.ToString());
                int idProdavac = prodavci[0].IdProdavac;
                
                // Dodaj ProdSklad zapise za checkirana skladišta
                CheckedListBox clb = this.Controls["clbSkladista"] as CheckedListBox;
                foreach (Skladiste s in clb.CheckedItems)
                {
                    ProdSklad ps = new ProdSklad {
                        IdProdavac = idProdavac,
                        IdSkladiste = s.IdSkladiste
                    };
                    
                    Request reqPS = new Request { Operation = Operation.Dodaj, Data = ps };
                    CommunicationHelper.Instance.SendRequest(reqPS);
                }
                
                break;
            
            // ... ostali tipovi
        }
        
        MessageBox.Show("Uspešno sačuvano!");
        this.DialogResult = DialogResult.OK;
        this.Close();
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Greška: {ex.Message}");
    }
}
```

---

## 🧾 AddRacun.cs - Specijalizovana Forma za Račune

### UI Komponente

```csharp
private ComboBox cmbTipKupca;       // "Firma" ili "Fizičko lice"
private ComboBox cmbKupac;          // Lista firmi ili fizičkih lica
private DataGridView dgvStavke;     // Tabela stavki
private ComboBox cmbOprema;         // Izbor opreme
private NumericUpDown nudKolicina;  // Količina
private Button btnDodajStavku;      // Dodaj stavku u listu
private Label lblUkupno;            // Ukupan iznos
```

### Flow Dodavanja Računa

```
1. Izaberi Tip Kupca (Firma/Fizičko lice)
   ↓
2. Izaberi konkretnog Kupca iz ComboBox-a
   ↓
3. Dodaj stavke:
   - Izaberi Opremu
   - Unesi Količinu
   - Klikni "Dodaj stavku"
   - Stavka se dodaje u dgvStavke (ne u bazu još!)
   ↓
4. Automatski izračun:
   - Cena stavke = Količina * Cena opreme
   - Ukupno = Σ(Cena stavke)
   - PDV (20%) = Ukupno * 0.20
   - Konačan iznos = Ukupno + PDV
   ↓
5. Klikni "Sačuvaj račun"
   → Pošalji jedan Request sa Racun + List<StavkaRacuna>
```

### btnSacuvaj_Click

```csharp
private void btnSacuvaj_Click(object sender, EventArgs e)
{
    try
    {
        // 1. Kreiraj Racun objekat
        Racun racun = new Racun
        {
            DatumIzdavanja = DateTime.Now,
            Prodavac = new Prodavac { IdProdavac = Sesija.UlogovaniProdavac.IdProdavac },
            Kupac = new Kupac { IdKupac = izabraniKupacId },
            CenaStavke = ukupnaCenaStavki,
            Pdv = pdv,
            CenaSaPopustom = popust,
            KonacanIznos = konacanIznos,
            Stavke = new List<StavkaRacuna>()
        };
        
        // 2. Dodaj stavke iz DataGridView-a
        foreach (DataGridViewRow row in dgvStavke.Rows)
        {
            if (row.IsNewRow) continue;
            
            StavkaRacuna stavka = new StavkaRacuna
            {
                IdOprema = (int)row.Cells["IdOprema"].Value,
                Kolicina = (int)row.Cells["Kolicina"].Value,
                Cena = Convert.ToDouble(row.Cells["Cena"].Value)
            };
            
            racun.Stavke.Add(stavka);
        }
        
        // 3. Pošalji kao jednu transakciju
        Request req = new Request
        {
            Operation = Operation.DodajRacun,  // Specijalizovana operacija!
            Data = racun
        };
        
        Response res = CommunicationHelper.Instance.SendRequest(req);
        
        if (res.IsSuccessful)
        {
            MessageBox.Show("Račun uspešno kreiran!");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        else
        {
            MessageBox.Show(res.Message, "Greška");
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Greška: {ex.Message}");
    }
}
```

---

## 🔗 CommunicationHelper.cs - Singleton TCP Client

### Singleton Pattern

```csharp
public class CommunicationHelper
{
    private static CommunicationHelper _instance;
    private TcpClient client;
    private NetworkStream stream;
    private BinaryFormatter formatter;
    
    // Private constructor
    private CommunicationHelper()
    {
        formatter = new BinaryFormatter();
        Connect();
    }
    
    // Public accessor
    public static CommunicationHelper Instance
    {
        get
        {
            if (_instance == null)
                _instance = new CommunicationHelper();
            return _instance;
        }
    }
    
    private void Connect()
    {
        try
        {
            client = new TcpClient("127.0.0.1", 5000);
            stream = client.GetStream();
            Console.WriteLine("Konektovan na server.");
        }
        catch (Exception ex)
        {
            throw new Exception($"Greška pri povezivanju na server: {ex.Message}");
        }
    }
    
    public Response SendRequest(Request req)
    {
        try
        {
            // 1. Serialize Request → byte[]
            formatter.Serialize(stream, req);
            
            // 2. Primi Response kao byte[]
            Response res = (Response)formatter.Deserialize(stream);
            
            return res;
        }
        catch (Exception ex)
        {
            return new Response
            {
                IsSuccessful = false,
                Message = $"Greška u komunikaciji: {ex.Message}"
            };
        }
    }
    
    public void Disconnect()
    {
        stream?.Close();
        client?.Close();
    }
}
```

### Kako se koristi svugde

```csharp
// U Login.cs
Response res = CommunicationHelper.Instance.SendRequest(req);

// U Main.cs
Response res = CommunicationHelper.Instance.SendRequest(req);

// U Add.cs
Response res = CommunicationHelper.Instance.SendRequest(req);
```

**Prednost:** Jedna TCP konekcija se reuse-uje kroz celu aplikaciju.

---

## 🔐 Sesija.cs - Session Management

```csharp
public static class Sesija
{
    public static Prodavac UlogovaniProdavac { get; set; }
    public static bool IsAdmin { get; set; }
}
```

### Kako se koristi

```csharp
// U Login.cs - postavi nakon uspešnog logina
Sesija.UlogovaniProdavac = prodavac;
Sesija.IsAdmin = false;

// U AddRacun.cs - koristi trenutnog prodavca
racun.Prodavac = new Prodavac { 
    IdProdavac = Sesija.UlogovaniProdavac.IdProdavac 
};

// U Main.cs - prikaži ime u title-u
this.Text = $"Sistem - {Sesija.UlogovaniProdavac.ImePrezime}";
```

---

## 📝 Best Practices Summary

### UI Thread Safety
```csharp
// Ako bi komunikacija bila async:
this.Invoke((MethodInvoker)delegate {
    dgvData.DataSource = rezultat;
});
```

### Input Validation
```csharp
if (string.IsNullOrWhiteSpace(txtEmail.Text))
{
    MessageBox.Show("Email je obavezan!");
    txtEmail.Focus();
    return;
}

if (!txtEmail.Text.Contains("@"))
{
    MessageBox.Show("Nevalidan email format!");
    return;
}
```

### Error Handling
```csharp
try {
    Response res = CommunicationHelper.Instance.SendRequest(req);
    if (res.IsSuccessful) {
        // Success path
    } else {
        MessageBox.Show(res.Message);
    }
} catch (Exception ex) {
    MessageBox.Show($"Greška: {ex.Message}");
}
```

### DataGridView Formatting
```csharp
// Sakri ID kolone
dgvData.Columns["IdProdavac"].Visible = false;

// Promeni header text
dgvData.Columns["ImePrezime"].HeaderText = "Ime i Prezime";

// Auto-resize
dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
```

---

## 🎯 Zaključak

Client layer implementira **moderan WinForms UI** sa:

✅ **Smart Filtering** - dinamički UI koji se prilagođava entity tipu  
✅ **Placeholder Text** - user-friendly input polja  
✅ **Hover Effects** - moderna interaktivnost  
✅ **Tooltips** - kontekstualna pomoć  
✅ **DataGridView Styling** - profesionalan izgled tabele  
✅ **Singleton Communication** - jedna TCP konekcija  
✅ **Session Management** - pamćenje ulogovanog korisnika  
✅ **Error Handling** - graceful user experience  

Client je **user-facing layer** koji omogućava intuitivnu interakciju sa sistemom!
