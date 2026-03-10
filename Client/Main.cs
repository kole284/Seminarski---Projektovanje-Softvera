using Client.Client;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using System.Linq;

namespace Client
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string izbor = cmbEntitet.SelectedItem.ToString();

            IDomainObject model = null;
            
            // Helper funkcija za proveru da li je filter validan (ne placeholder text)
            bool IsValidFilter(string text) => !string.IsNullOrWhiteSpace(text) && 
                                                text != "Unesite termin za pretragu..." && 
                                                text != "Dodatni filter...";
            
            switch (izbor)
            {
                case "Firme": 
                    model = new Firma(); 
                    // Postavi filtere ako su uneseni
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
                        // Ako sadrži @, to je email, inače telefon
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
                    // Koristi ComboBox za kategoriju
                    if (cmbFilter2.SelectedItem != null && cmbFilter2.SelectedIndex > 0)
                        ((Oprema)model).Kategorija = cmbFilter2.SelectedItem.ToString();
                    Console.WriteLine($"[CLIENT DEBUG] Oprema filter - Ime: '{((Oprema)model).Ime}', Kategorija: '{((Oprema)model).Kategorija}', SelectedIndex: {cmbFilter2.SelectedIndex}");
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
                    
                    // Filter 2: Datum izdavanja (YYYY / YYYY-MM / YYYY-MM-DD)
                    if (IsValidFilter(txtFilter2.Text))
                        ((Racun)model).DatumFilter = txtFilter2.Text;
                    
                    // Filtriraj po prodavcu
                    if (cmbProdavac.SelectedItem != null && cmbProdavac.SelectedIndex > 0)
                    {
                        var prodavac = cmbProdavac.SelectedItem as Prodavac;
                        if (prodavac != null)
                            ((Racun)model).Prodavac = new Prodavac { IdProdavac = prodavac.IdProdavac };
                    }
                    
                    // Filtriraj po kupcu
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
                Request req = new Request { Operation = Operation.VratiSve, Data = model };
                Response res = CommunicationHelper.Instance.SendRequest(req);

                if (res.IsSuccessful && res.Data != null)
                {
                    // res.Data je JsonElement, moramo uzeti sirovi JSON tekst
                    string jsonData;
                    if (res.Data is JsonElement jsonElement)
                    {
                        jsonData = jsonElement.GetRawText();
                    }
                    else
                    {
                        jsonData = res.Data.ToString();
                    }

                    // VA�NO: Koristi opcije za ignorisanje velikih/malih slova
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                    try
                    {
                        if (izbor == "Prodavci")
                        {
                            List<Prodavac> lista = JsonSerializer.Deserialize<List<Prodavac>>(jsonData, options);
                            dgvData.DataSource = lista;
                            FormatiranjKolona(dgvData);
                        }
                        else if (izbor == "Firme")
                        {
                            List<Firma> lista = JsonSerializer.Deserialize<List<Firma>>(jsonData, options);
                            dgvData.DataSource = lista;
                            FormatiranjKolona(dgvData);
                        }
                        else if (izbor == "Fizička lica")
                        {
                            List<FizickoLice> lista = JsonSerializer.Deserialize<List<FizickoLice>>(jsonData, options);
                            dgvData.DataSource = lista;
                            FormatiranjKolona(dgvData);
                        }
                        else if (izbor == "Oprema")
                        {
                            List<Oprema> lista = JsonSerializer.Deserialize<List<Oprema>>(jsonData, options);
                            Console.WriteLine($"[CLIENT DEBUG] Primljeno {lista?.Count ?? 0} opreme iz servera");
                            if (lista != null && lista.Count > 0)
                            {
                                foreach (var o in lista)
                                    Console.WriteLine($"  - {o.Ime}, {o.Kategorija}, {o.Cena}");
                            }
                            dgvData.DataSource = lista;
                            FormatiranjKolona(dgvData);
                        }
                        else if (izbor == "Skladišta")
                        {
                            List<Skladiste> lista = JsonSerializer.Deserialize<List<Skladiste>>(jsonData, options);
                            dgvData.DataSource = lista;
                            FormatiranjKolona(dgvData);
                        }
                        else if (izbor == "Računi")
                        {
                            List<Racun> lista = JsonSerializer.Deserialize<List<Racun>>(jsonData, options);
                            dgvData.DataSource = lista;
                            FormatiranjKolona(dgvData);
                        }

                        dgvData.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Gre�ka pri deserijalizaciji:\\n{ex.Message}\\n\\nJSON: {jsonData}");
                    }
                }
                else
                {
                    MessageBox.Show(res.Message);
                }
            }
        }

       

        private void Main_Load(object sender, EventArgs e)
        {
            // Svi korisnici vide sve opcije
            cmbEntitet.Items.Add("Prodavci");
            cmbEntitet.Items.Add("Firme");
            cmbEntitet.Items.Add("Fizička lica");
            cmbEntitet.Items.Add("Oprema");
            cmbEntitet.Items.Add("Skladišta");
            cmbEntitet.Items.Add("Računi");

            cmbEntitet.SelectedIndex = 0; // Postavi na prvu opciju po defaultu

            // Postavi welcome poruku u header
            lblWelcome.Text = UIController.PorukaDobroDosli();

            // Podešavanja za DataGridView - moderniji izgled
            dgvData.AutoGenerateColumns = true;
            dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvData.ReadOnly = true;
            dgvData.AllowUserToAddRows = false;
            dgvData.BackgroundColor = Color.White;
            dgvData.BorderStyle = BorderStyle.None;
            dgvData.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvData.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvData.EnableHeadersVisualStyles = false;
            dgvData.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvData.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvData.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvData.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 73, 94);
            dgvData.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgvData.ColumnHeadersHeight = 40;
            dgvData.DefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
            dgvData.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvData.DefaultCellStyle.BackColor = Color.White;
            dgvData.DefaultCellStyle.ForeColor = Color.FromArgb(44, 62, 80);
            dgvData.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvData.DefaultCellStyle.Padding = new Padding(5, 2, 5, 2);
            dgvData.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            dgvData.RowTemplate.Height = 35;

            // Dodaj event za double-click na red
            dgvData.CellDoubleClick += dgvData_CellDoubleClick;

            // Tooltips za user-friendly pomoć
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(cmbEntitet, "Izaberite tip podataka koje želite da vidite");
            toolTip.SetToolTip(btnPrikazi, "Prikaži podatke sa primenjenim filterima");
            toolTip.SetToolTip(btnDodaj, "Dodaj novi unos za izabrani tip podataka");
            toolTip.SetToolTip(btnClearFilter, "Obriši sve filtere i prikaži sve podatke");
            toolTip.SetToolTip(dgvData, "Dvaput kliknite na red za prikaz detalja");

            // Placeholder text za filter polja
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

            txtFilter2.Text = "Dodatni filter...";
            txtFilter2.ForeColor = Color.Gray;
            txtFilter2.GotFocus += (s, ev) => {
                if (txtFilter2.Text == "Dodatni filter...") {
                    txtFilter2.Text = "";
                    txtFilter2.ForeColor = Color.Black;
                }
            };
            txtFilter2.LostFocus += (s, ev) => {
                if (string.IsNullOrWhiteSpace(txtFilter2.Text)) {
                    txtFilter2.Text = "Dodatni filter...";
                    txtFilter2.ForeColor = Color.Gray;
                }
            };

            // Inicijalizuj filtere
            UpdateFilterLabels();

            // Hover efekti za dugmad sa zaobljenim ivicama
            ApplyModernButtonStyle(btnPrikazi, Color.FromArgb(52, 152, 219), Color.FromArgb(41, 128, 185));
            ApplyModernButtonStyle(btnDodaj, Color.FromArgb(46, 204, 113), Color.FromArgb(39, 174, 96));
            ApplyModernButtonStyle(btnClearFilter, Color.FromArgb(231, 76, 60), Color.FromArgb(192, 57, 43));
        }

        private void ApplyModernButtonStyle(Button btn, Color normalColor, Color hoverColor)
        {
            btn.FlatAppearance.MouseOverBackColor = hoverColor;
            btn.MouseEnter += (s, ev) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, ev) => btn.BackColor = normalColor;
        }

        private void cmbEntitet_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFilterLabels();
        }

        private void UpdateFilterLabels()
        {
            string izbor = cmbEntitet.SelectedItem?.ToString();
            
            // Ocisti filtere kad se promeni tip
            txtFilter1.Clear();
            txtFilter2.Clear();
            cmbFilter2.Items.Clear();
            cmbFilter2.Text = string.Empty;
            cmbProdavac.Items.Clear();
            cmbKupac.Items.Clear();
            
            // Sakrij sve dodatne filtere po defaultu
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
                    cmbFilter2.Visible = false;
                    lblFilter2.Visible = true;
                    break;
                case "Fizička lica":
                    lblFilter1.Text = "Ime i prezime:";
                    lblFilter2.Text = "Email ili Telefon:";
                    txtFilter2.Visible = true;
                    cmbFilter2.Visible = false;
                    lblFilter2.Visible = true;
                    break;
                case "Oprema":
                    lblFilter1.Text = "Ime:";
                    lblFilter2.Text = "Kategorija:";
                    // Popuni ComboBox sa kategorijama
                    txtFilter2.Visible = false;
                    cmbFilter2.Visible = true;
                    cmbFilter2.DropDownStyle = ComboBoxStyle.DropDownList; // Samo selekcija
                    cmbFilter2.Items.Add("-- Sve kategorije --");
                    cmbFilter2.Items.Add("Laptopovi");
                    cmbFilter2.Items.Add("Desktop računari");
                    cmbFilter2.Items.Add("Monitori");
                    cmbFilter2.Items.Add("Tastature");
                    cmbFilter2.Items.Add("Miševi");
                    cmbFilter2.Items.Add("Slušalice");
                    cmbFilter2.Items.Add("Kamere");
                    cmbFilter2.Items.Add("Printeri");
                    cmbFilter2.Items.Add("Skeneri");
                    cmbFilter2.Items.Add("Mrežna oprema");
                    cmbFilter2.Items.Add("Eksterni hard diskovi");
                    cmbFilter2.Items.Add("USB fleš diskovi");
                    cmbFilter2.Items.Add("Grafičke kartice");
                    cmbFilter2.Items.Add("RAM memorija");
                    cmbFilter2.Items.Add("Procesori");
                    cmbFilter2.Items.Add("Matične ploče");
                    cmbFilter2.Items.Add("Napajanja");
                    cmbFilter2.Items.Add("Kucišta");
                    cmbFilter2.Items.Add("Ostalo");
                    cmbFilter2.SelectedIndex = 0; // Izaberi "Sve kategorije"
                    lblFilter2.Visible = true;
                    break;
                case "Prodavci":
                    lblFilter1.Text = "Ime i prezime:";
                    lblFilter2.Text = "Email:";
                    txtFilter2.Visible = true;
                    cmbFilter2.Visible = false;
                    lblFilter2.Visible = true;
                    break;
                case "Skladišta":
                    lblFilter1.Text = "Naziv:";
                    lblFilter2.Text = "Adresa:";
                    txtFilter2.Visible = true;
                    cmbFilter2.Visible = false;
                    lblFilter2.Visible = true;
                    break;
                case "Računi":
                    lblFilter1.Text = "ID Računa:";
                    lblFilter2.Text = "Datum (YYYY / YYYY-MM / YYYY-MM-DD):";
                    txtFilter2.Visible = true;
                    cmbFilter2.Visible = false;
                    lblFilter2.Visible = true;
                    
                    // Prikazi filtere za prodavca i kupca
                    cmbProdavac.Visible = true;
                    cmbKupac.Visible = true;
                    lblProdavac.Visible = true;
                    lblKupac.Visible = true;
                    
                    // Ucitaj prodavce
                    UcitajProdavce();
                    
                    // Ucitaj kupce
                    UcitajKupce();
                    break;
                default:
                    lblFilter1.Text = "Filter 1:";
                    lblFilter2.Text = "Filter 2:";
                    break;
            }
        }

        private void btnClearFilter_Click(object sender, EventArgs e)
        {
            txtFilter1.Clear();
            txtFilter2.Clear();
            cmbFilter2.Text = string.Empty;
            if (cmbFilter2.Items.Count > 0)
                cmbFilter2.SelectedIndex = 0; // Reset na "Sve kategorije" ako postoji
            
            // Resetuj i nove ComboBox-ove za prodavca i kupca
            if (cmbProdavac.Items.Count > 0)
                cmbProdavac.SelectedIndex = 0;
            if (cmbKupac.Items.Count > 0)
                cmbKupac.SelectedIndex = 0;
            
            // Automatski prika�i sve podatke bez filtera
            button1_Click(sender, e);
        }

        private void dgvData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Proveri da li je kliknut validan red (ne header)
            if (e.RowIndex >= 0)
            {
                // Uzmi odabrani objekat iz reda
                var selectedObject = dgvData.Rows[e.RowIndex].DataBoundItem;
                
                if (selectedObject != null)
                {
                    // Ako je Racun, otvori specijalnu formu
                    if (selectedObject is Racun racun)
                    {
                        DetailsRacun detailsRacunForm = new DetailsRacun(racun);
                        if (detailsRacunForm.ShowDialog() == DialogResult.OK)
                        {
                            // Osveži prikaz nakon ažuriranja ili brisanja
                            button1_Click(sender, e);
                        }
                    }
                    // Ako je KupacView, konvertuj u Firma ili FizickoLice
                    else if (selectedObject is KupacView kupacView)
                    {
                        IDomainObject domainObject = null;
                        if (kupacView.Tip == "Firma")
                        {
                            domainObject = new Firma
                            {
                                IdKupac = kupacView.IdKupac,
                                Naziv = kupacView.Naziv,
                                Pib = kupacView.Pib,
                                Adresa = kupacView.Adresa,
                                Popust = kupacView.Popust,
                                Partnerstvo = kupacView.Partnerstvo ?? false
                            };
                        }
                        else if (kupacView.Tip == "Fizicko lice")
                        {
                            domainObject = new FizickoLice
                            {
                                IdKupac = kupacView.IdKupac,
                                ImePrezime = kupacView.ImePrezime,
                                Email = kupacView.Email,
                                Telefon = kupacView.Telefon,
                                Popust = kupacView.Popust,
                                LoyaltyClan = kupacView.LoyaltyClan ?? false
                            };
                        }
                        
                        if (domainObject != null)
                        {
                            Details detailsForm = new Details(domainObject);
                            if (detailsForm.ShowDialog() == DialogResult.OK)
                            {
                                // Osvezi prikaz ako je doslo do izmene
                                button1_Click(sender, e);
                            }
                        }
                    }
                    else
                    {
                        // Standardno ponasanje za druge tipove
                        Details detailsForm = new Details(selectedObject);
                        if (detailsForm.ShowDialog() == DialogResult.OK)
                        {
                            // Osvezi prikaz ako je doslo do izmene
                            button1_Click(sender, e);
                        }
                    }
                }
            }
        }

        private void btnDodaj_Click(object sender, EventArgs e)
        {
            // Proveri da li je izabran model iz ComboBox-a
            string izbor = cmbEntitet.SelectedItem?.ToString();
            
            if (string.IsNullOrEmpty(izbor))
            {
                MessageBox.Show("Molimo izaberite model iz liste.", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // Admin ne može dodavati račune jer nije registrovan prodavac
            if (Sesija.IsAdmin && izbor == "Računi")
            {
                MessageBox.Show("Admin ne može da kreira račune jer nije registrovan prodavac!", 
                    "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            // Obični prodavci mogu dodavati samo račune
            if (!Sesija.IsAdmin && izbor != "Računi")
            {
                MessageBox.Show("Nemate dozvolu za dodavanje. Možete samo kreirati račune.", "Nemate pristup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // Otvori odgovarajucu formu na osnovu izabranog modela
            switch (izbor)
            {
                case "Računi":
                    AddRacun addRacunForm = new AddRacun();
                    if (addRacunForm.ShowDialog() == DialogResult.OK)
                    {
                        button1_Click(sender, e); // Osve�i prikaz
                    }
                    break;
                    
                case "Firme":
                    Add addFirmaForm = new Add();
                    addFirmaForm.Text = "Dodaj Firmu";
                    addFirmaForm.Tag = "Firma"; // Postavi tag da forma zna �ta kreira
                    if (addFirmaForm.ShowDialog() == DialogResult.OK)
                    {
                        button1_Click(sender, e);
                    }
                    break;
                    
                case "Fizička lica":
                    Add addFizickoLiceForm = new Add();
                    addFizickoLiceForm.Text = "Dodaj Fizicko lice";
                    addFizickoLiceForm.Tag = "FizickoLice";
                    if (addFizickoLiceForm.ShowDialog() == DialogResult.OK)
                    {
                        button1_Click(sender, e);
                    }
                    break;
                    
                case "Oprema":
                    Add addOpremaForm = new Add();
                    addOpremaForm.Text = "Dodaj Opremu";
                    addOpremaForm.Tag = "Oprema";
                    if (addOpremaForm.ShowDialog() == DialogResult.OK)
                    {
                        button1_Click(sender, e);
                    }
                    break;
                    
                case "Prodavci":
                    Add addProdavacForm = new Add();
                    addProdavacForm.Text = "Dodaj Prodavca";
                    addProdavacForm.Tag = "Prodavac";
                    if (addProdavacForm.ShowDialog() == DialogResult.OK)
                    {
                        button1_Click(sender, e);
                    }
                    break;
                    
                case "Skladišta":
                    Add addSkladisteForm = new Add();
                    addSkladisteForm.Text = "Dodaj Skladi�te";
                    addSkladisteForm.Tag = "Skladiste";
                    if (addSkladisteForm.ShowDialog() == DialogResult.OK)
                    {
                        button1_Click(sender, e);
                    }
                    break;
                    
                default:
                    MessageBox.Show($"Forma za dodavanje '{izbor}' jo� nije implementirana.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        /// <summary>
        /// Formatira kolone DataGridView-a:
        /// - Skriva sve kolone sa Id ili id u nazivu
        /// - Preformatira nazive sa CamelCase na "Readable Format"
        /// - Pomera Popust kolonu na pretposlednju poziciju
        /// </summary>
        private void FormatiranjKolona(DataGridView dgv)
        {
            // Skri sve Id kolone i preformatiraj nazive
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                // Skri kolone koje se završavaju sa Id ili počinju sa Id
                if (col.Name.EndsWith("Id") || col.Name.EndsWith("ID") || 
                    col.Name.StartsWith("Id") || col.Name == "id")
                {
                    col.Visible = false;
                }
                else
                {
                    // Preformatira naziv kolone sa CamelCase na "Readable Format"
                    col.HeaderText = FormatiranjNaziva(col.Name);
                }
            }

            // Pomeri Popust kolonu na pretposlednju poziciju koristeći DisplayIndex
            DataGridViewColumn popustKolona = null;
            int totalColumns = dgv.Columns.Count;
            
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (col.Name == "Popust")
                {
                    popustKolona = col;
                    break;
                }
            }

            if (popustKolona != null && totalColumns > 2)
            {
                // Postavi DisplayIndex na pretposlednju poziciju (Count - 2)
                popustKolona.DisplayIndex = totalColumns - 2;
            }

            // Formatiranje numeričkih kolona za Računi: dve decimale
            try
            {
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    if (col.Name == "KonacanIznos" || col.Name == "Pdv" || col.Name == "CenaSaPopustom" || col.Name == "CenaStavke")
                    {
                        col.DefaultCellStyle.Format = "N2"; // npr. 1234.56
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Konvertuje CamelCase naziv u čitljiv format
        /// Primeri: ImePrezime -> Ime i Prezime, cenaStavki -> Cena stavki
        /// </summary>
        private string FormatiranjNaziva(string camelCaseName)
        {
            if (string.IsNullOrEmpty(camelCaseName))
                return camelCaseName;

            var result = new StringBuilder();
            bool prvaRec = true;

            for (int i = 0; i < camelCaseName.Length; i++)
            {
                char trenutniChar = camelCaseName[i];

                // Ako je trenutni karakter velika slova i nije prvi karakter
                if (char.IsUpper(trenutniChar) && i > 0)
                {
                    result.Append(' ');
                    result.Append(char.ToLower(trenutniChar));
                }
                else if (i == 0)
                {
                    // Prvi karakter uvek velika slova
                    result.Append(char.ToUpper(trenutniChar));
                }
                else
                {
                    result.Append(trenutniChar);
                }
            }

            return result.ToString();
        }

        private void UcitajProdavce()
        {
            try
            {
                Request req = new Request { Operation = Operation.VratiSve, Data = new Prodavac() };
                Response res = CommunicationHelper.Instance.SendRequest(req);

                if (res.IsSuccessful && res.Data != null)
                {
                    string jsonData;
                    if (res.Data is JsonElement jsonElement)
                    {
                        jsonData = jsonElement.GetRawText();
                    }
                    else
                    {
                        jsonData = res.Data.ToString();
                    }

                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    List<Prodavac> prodavci = JsonSerializer.Deserialize<List<Prodavac>>(jsonData, options);

                    cmbProdavac.Items.Clear();
                    cmbProdavac.Items.Add("-- Svi prodavci --");
                    
                    if (prodavci != null)
                    {
                        foreach (var prodavac in prodavci)
                        {
                            cmbProdavac.Items.Add(prodavac);
                        }
                    }
                    
                    cmbProdavac.DisplayMember = "ImePrezime";
                    cmbProdavac.ValueMember = "IdProdavac";
                    cmbProdavac.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju prodavaca: {ex.Message}");
            }
        }

        private void UcitajKupce()
        {
            try
            {
                // Ucitaj firme
                Request reqFirme = new Request { Operation = Operation.VratiSve, Data = new Firma() };
                Response resFirme = CommunicationHelper.Instance.SendRequest(reqFirme);

                // Ucitaj fizicka lica
                Request reqFizicka = new Request { Operation = Operation.VratiSve, Data = new FizickoLice() };
                Response resFizicka = CommunicationHelper.Instance.SendRequest(reqFizicka);

                cmbKupac.Items.Clear();
                cmbKupac.Items.Add("-- Svi kupci --");

                if (resFirme.IsSuccessful && resFirme.Data != null)
                {
                    string jsonData;
                    if (resFirme.Data is JsonElement jsonElement)
                    {
                        jsonData = jsonElement.GetRawText();
                    }
                    else
                    {
                        jsonData = resFirme.Data.ToString();
                    }

                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    List<Firma> firme = JsonSerializer.Deserialize<List<Firma>>(jsonData, options);

                    if (firme != null)
                    {
                        foreach (var firma in firme)
                        {
                            // Kreiraj anonimni objekat sa IdKupac i display text
                            var displayItem = new { IdKupac = firma.IdKupac, Display = $"{firma.Naziv} (Firma)" };
                            cmbKupac.Items.Add(displayItem);
                        }
                    }
                }

                if (resFizicka.IsSuccessful && resFizicka.Data != null)
                {
                    string jsonData;
                    if (resFizicka.Data is JsonElement jsonElement)
                    {
                        jsonData = jsonElement.GetRawText();
                    }
                    else
                    {
                        jsonData = resFizicka.Data.ToString();
                    }

                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    List<FizickoLice> fizickaLica = JsonSerializer.Deserialize<List<FizickoLice>>(jsonData, options);

                    if (fizickaLica != null)
                    {
                        foreach (var fizicko in fizickaLica)
                        {
                            var displayItem = new { IdKupac = fizicko.IdKupac, Display = $"{fizicko.ImePrezime} (Fizičko lice)" };
                            cmbKupac.Items.Add(displayItem);
                        }
                    }
                }

                cmbKupac.DisplayMember = "Display";
                cmbKupac.ValueMember = "IdKupac";
                cmbKupac.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju kupaca: {ex.Message}");
            }
        }
    }
}
