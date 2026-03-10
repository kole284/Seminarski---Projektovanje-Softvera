using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Models;
using Client.Client;

namespace Client
{
    public partial class Add : Form
    {
        private Dictionary<string, Control> inputControls = new Dictionary<string, Control>();
        private IDomainObject? existingObject = null;
        private bool isEditMode = false;
        private List<Skladiste> svaSkladista = new List<Skladiste>();
        private List<ProdSklad> prodavacSkladista = new List<ProdSklad>();

        public Add()
        {
            InitializeComponent();
            ApplyModernStyling();
            PopuniComboBox();
            UcitajSkladista();
        }

        private void ApplyModernStyling()
        {
            // Hover efekti za dugmad
            btnSacuvaj.FlatAppearance.MouseOverBackColor = Color.FromArgb(39, 174, 96);
            btnSacuvaj.MouseEnter += (s, ev) => btnSacuvaj.BackColor = Color.FromArgb(39, 174, 96);
            btnSacuvaj.MouseLeave += (s, ev) => btnSacuvaj.BackColor = Color.FromArgb(46, 204, 113);

            btnOtkazi.FlatAppearance.MouseOverBackColor = Color.FromArgb(127, 140, 141);
            btnOtkazi.MouseEnter += (s, ev) => btnOtkazi.BackColor = Color.FromArgb(127, 140, 141);
            btnOtkazi.MouseLeave += (s, ev) => btnOtkazi.BackColor = Color.FromArgb(149, 165, 166);
        }

        // Konstruktor za Edit mode
        public Add(IDomainObject objectToEdit) : this()
        {
            existingObject = objectToEdit;
            isEditMode = true;
            lblTitle.Text = "Ažuriraj podatke";
            btnSacuvaj.Text = "Ažuriraj";
            
            // Postavi odgovarajući tip u ComboBox i onemogući ga
            if (objectToEdit is Prodavac)
            {
                cmbTipObjekta.SelectedItem = "Prodavac";
            }
            else if (objectToEdit is Firma)
            {
                cmbTipObjekta.SelectedItem = "Kupac - Firma";
            }
            else if (objectToEdit is FizickoLice)
            {
                cmbTipObjekta.SelectedItem = "Kupac - Fizičko lice";
            }
            else if (objectToEdit is Kupac)
            {
                cmbTipObjekta.SelectedItem = "Kupac - Firma";
            }
            else if (objectToEdit is Oprema)
            {
                cmbTipObjekta.SelectedItem = "Oprema";
            }
            else if (objectToEdit is Skladiste)
            {
                cmbTipObjekta.SelectedItem = "Skladište";
            }
            
            cmbTipObjekta.Enabled = false; // Ne dozvoljavaj promenu tipa
            PopuniPostojeceVrednosti();
        }
        
        // Override OnLoad da automatski postavi tip ako je prosleđen preko Tag-a
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            // Ako je Tag postavljen (iz Main forme), automatski izaberi taj tip
            if (!isEditMode && this.Tag != null && this.Tag is string tag)
            {
                // Sakrij ComboBox jer je tip već određen
                cmbTipObjekta.Visible = false;
                lblTipObjekta.Visible = false;
                
                switch (tag)
                {
                    case "Firma":
                        cmbTipObjekta.SelectedItem = "Kupac - Firma";
                        break;
                    case "FizickoLice":
                        cmbTipObjekta.SelectedItem = "Kupac - Fizičko lice";
                        break;
                    case "Oprema":
                        cmbTipObjekta.SelectedItem = "Oprema";
                        break;
                    case "Prodavac":
                        cmbTipObjekta.SelectedItem = "Prodavac";
                        break;
                    case "Skladiste":
                        cmbTipObjekta.SelectedItem = "Skladište";
                        break;
                }
                
                // Onemogući promenu tipa
                cmbTipObjekta.Enabled = false;
            }
        }

        private void PopuniComboBox()
        {
            cmbTipObjekta.Items.Add("Prodavac");
            cmbTipObjekta.Items.Add("Kupac - Firma");
            cmbTipObjekta.Items.Add("Kupac - Fizičko lice");
            cmbTipObjekta.Items.Add("Oprema");
            cmbTipObjekta.Items.Add("Skladište");
            cmbTipObjekta.Items.Add("Račun");
        }

        private void PopuniPostojeceVrednosti()
        {
            if (existingObject == null) return;

            if (existingObject is Prodavac prodavac)
            {
                SetTextBoxValue("ImePrezime", prodavac.ImePrezime);
                SetTextBoxValue("Email", prodavac.Email);
                SetTextBoxValue("Telefon", prodavac.Telefon);
                SetTextBoxValue("Password", prodavac.Password);
                
                // Učitaj skladišta za ovog prodavca
                // Ensure controls for Prodavac (including the CheckedListBox for Skladista)
                // are created before attempting to check items.
                if (!inputControls.ContainsKey("Skladista"))
                {
                    cmbTipObjekta_SelectedIndexChanged(this, EventArgs.Empty);
                }
                UcitajProdavacSkladista(prodavac.IdProdavac);
            }
            else if (existingObject is Firma firma)
            {
                SetTextBoxValue("Naziv", firma.Naziv);
                SetNumericValue("Popust", (decimal)firma.Popust);
                SetTextBoxValue("Pib", firma.Pib);
                SetTextBoxValue("AdresaFirme", firma.Adresa);
                SetCheckBoxValue("Partnerstvo", firma.Partnerstvo);
            }
            else if (existingObject is FizickoLice fizickoLice)
            {
                SetTextBoxValue("ImePrezimeFizicko", fizickoLice.ImePrezime);
                SetTextBoxValue("EmailFizicko", fizickoLice.Email);
                SetTextBoxValue("TelefonFizicko", fizickoLice.Telefon);                
                SetNumericValue("Popust", (decimal)fizickoLice.Popust);

                SetCheckBoxValue("LoyaltyClan", fizickoLice.LoyaltyClan);
            }
            else if (existingObject is Oprema oprema)
            {
                SetTextBoxValue("Ime", oprema.Ime);
                SetComboBoxValue("Kategorija", oprema.Kategorija);
                SetNumericValue("Cena", (decimal)oprema.Cena);
            }
            else if (existingObject is Skladiste skladiste)
            {
                SetTextBoxValue("Naziv", skladiste.Naziv);
                SetTextBoxValue("Adresa", skladiste.Adresa);
            }
        }

        private void cmbTipObjekta_SelectedIndexChanged(object sender, EventArgs e)
        {
            panelInputs.Controls.Clear();
            inputControls.Clear();

            string? izabrano = cmbTipObjekta.SelectedItem?.ToString();
            int yPosition = 10;

            switch (izabrano)
            {
                case "Prodavac":
                    KreirajProdavacKontrole(ref yPosition);
                    break;
                case "Kupac - Firma":
                    KreirajFirmaKontrole(ref yPosition);
                    break;
                case "Kupac - Fizičko lice":
                    KreirajFizickoLiceKontrole(ref yPosition);
                    break;
                case "Oprema":
                    KreirajOpremaKontrole(ref yPosition);
                    break;
                case "Skladište":
                    KreirajSkladisteKontrole(ref yPosition);
                    break;
                case "Račun":
                    // Za račun koristimo posebnu formu
                    MessageBox.Show("Za kreiranje računa koristite dugme 'Kreiraj Račun' na glavnom ekranu.", "Informacija", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        private void KreirajProdavacKontrole(ref int yPosition)
        {
            DodajTextBox("Ime i Prezime:", "ImePrezime", ref yPosition);
            DodajTextBox("Email:", "Email", ref yPosition);
            DodajTextBox("Telefon:", "Telefon", ref yPosition);
            
            // Prikaži password polje samo:
            // 1) Pri dodavanju novog prodavca (existingObject == null)
            // 2) Kada prodavac uređuje svoj profil
            bool prikaziPassword = existingObject == null || 
                                   (existingObject is Prodavac p && Sesija.UlogovaniProdavac != null && p.IdProdavac == Sesija.UlogovaniProdavac.IdProdavac);
            
            if (prikaziPassword)
            {
                DodajTextBox("Password:", "Password", ref yPosition, true);
            }
            
            DodajCheckedListBox("Skladišta:", "Skladista", ref yPosition, svaSkladista);
        }

        private void KreirajFirmaKontrole(ref int yPosition)
        {
            DodajTextBox("Naziv:", "Naziv", ref yPosition);
            DodajNumericUpDown("Popust (0-1):", "Popust", ref yPosition, 0, 1, 0.01m);
            DodajTextBox("PIB:", "Pib", ref yPosition);
            DodajTextBox("Adresa:", "AdresaFirme", ref yPosition);
            DodajCheckBox("Partnerstvo:", "Partnerstvo", ref yPosition);
        }

        private void KreirajFizickoLiceKontrole(ref int yPosition)
        {
            DodajTextBox("Ime i Prezime:", "ImePrezimeFizicko", ref yPosition);
            DodajTextBox("Email:", "EmailFizicko", ref yPosition);
            DodajTextBox("Telefon:", "TelefonFizicko", ref yPosition);            
            DodajNumericUpDown("Popust (0-1):", "Popust", ref yPosition, 0, 1, 0.01m);
            DodajCheckBox("Loyalty Clan:", "LoyaltyClan", ref yPosition);
        }

        private void KreirajOpremaKontrole(ref int yPosition)
        {
            DodajTextBox("Ime:", "Ime", ref yPosition);
            DodajComboBox("Kategorija:", "Kategorija", ref yPosition, GetKategorijeOpreme());
            DodajNumericUpDown("Cena:", "Cena", ref yPosition, 0, 1000000, 1);
        }
        
        private List<string> GetKategorijeOpreme()
        {
            List<string> kategorije = new List<string>();
            foreach (KategorijaOpreme kat in Enum.GetValues(typeof(KategorijaOpreme)))
            {
                kategorije.Add(kat.GetDescription());
            }
            return kategorije;
        }

        /// <summary>
        /// Validira PIB - mora biti tačno 9 cifara
        /// </summary>
        private bool ValidirajPib(string pib)
        {
            if (string.IsNullOrWhiteSpace(pib))
            {
                MessageBox.Show("PIB je obavezan!", "Validaciona greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Ukloni sve razmake
            pib = pib.Trim();

            // Proveri da li ima tačno 9 cifara
            if (pib.Length != 9)
            {
                MessageBox.Show("PIB mora imati tačno 9 cifara!", "Validaciona greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Proveri da li su sve karakteri cifre
            if (!pib.All(char.IsDigit))
            {
                MessageBox.Show("PIB može sadržati samo cifre!", "Validaciona greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validira broj telefona - mora biti 10 cifara (format: 06XXXXXXXX)
        /// </summary>
        private bool ValidirajTelefon(string telefon)
        {
            if (string.IsNullOrWhiteSpace(telefon))
            {
                MessageBox.Show("Broj telefona je obavezan!", "Validaciona greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Ukloni sve razmake, crtice i zagrade
            telefon = telefon.Trim().Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

            // Ako počinje sa +381, pretvori u 0
            if (telefon.StartsWith("+381"))
            {
                telefon = "0" + telefon.Substring(4);
            }

            // Proveri da li počinje sa 0
            if (!telefon.StartsWith("0"))
            {
                MessageBox.Show("Broj telefona mora počinjati sa 0 (npr. 0641234567)!", "Validaciona greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Proveri da li ima tačno 10 cifara
            if (telefon.Length != 10)
            {
                MessageBox.Show("Broj telefona mora imati tačno 10 cifara (format: 06XXXXXXXX)!", "Validaciona greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Proveri da li su sve karakteri cifre
            if (!telefon.All(char.IsDigit))
            {
                MessageBox.Show("Broj telefona može sadržati samo cifre!", "Validaciona greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void KreirajSkladisteKontrole(ref int yPosition)
        {
            DodajTextBox("Naziv:", "Naziv", ref yPosition);
            DodajTextBox("Adresa:", "Adresa", ref yPosition);
        }

        private void DodajTextBox(string labelText, string controlName, ref int yPosition, bool isPassword = false)
        {
            Label lbl = new Label
            {
                Text = labelText,
                Location = new Point(10, yPosition),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = true
            };
            panelInputs.Controls.Add(lbl);

            TextBox txt = new TextBox
            {
                Name = controlName,
                Location = new Point(200, yPosition - 3),
                Width = 300,
                Font = new Font("Segoe UI", 10F)
            };

            if (isPassword)
            {
                txt.PasswordChar = '*';
            }

            panelInputs.Controls.Add(txt);
            inputControls[controlName] = txt;

            yPosition += 40;
        }

        private void DodajNumericUpDown(string labelText, string controlName, ref int yPosition, decimal minimum = 0, decimal maximum = 100000, decimal increment = 1)
        {
            Label lbl = new Label
            {
                Text = labelText,
                Location = new Point(10, yPosition),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = true
            };
            panelInputs.Controls.Add(lbl);

            NumericUpDown num = new NumericUpDown
            {
                Name = controlName,
                Location = new Point(200, yPosition - 3),
                Width = 300,
                Font = new Font("Segoe UI", 10F),
                Minimum = minimum,
                Maximum = maximum,
                DecimalPlaces = (increment < 1) ? 2 : 0,
                Increment = increment
            };

            panelInputs.Controls.Add(num);
            inputControls[controlName] = num;

            yPosition += 40;
        }

        private void DodajCheckBox(string labelText, string controlName, ref int yPosition)
        {
            Label lbl = new Label
            {
                Text = labelText,
                Location = new Point(10, yPosition),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = true
            };
            panelInputs.Controls.Add(lbl);

            CheckBox chk = new CheckBox
            {
                Name = controlName,
                Location = new Point(200, yPosition - 3),
                Width = 300,
                Font = new Font("Segoe UI", 10F)
            };

            panelInputs.Controls.Add(chk);
            inputControls[controlName] = chk;

            yPosition += 40;
        }
        
        private void DodajComboBox(string labelText, string controlName, ref int yPosition, List<string> items)
        {
            Label lbl = new Label
            {
                Text = labelText,
                Location = new Point(10, yPosition),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = true
            };
            panelInputs.Controls.Add(lbl);

            ComboBox cmb = new ComboBox
            {
                Name = controlName,
                Location = new Point(200, yPosition - 3),
                Width = 300,
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            
            foreach (string item in items)
            {
                cmb.Items.Add(item);
            }
            
            if (cmb.Items.Count > 0)
                cmb.SelectedIndex = 0;

            panelInputs.Controls.Add(cmb);
            inputControls[controlName] = cmb;

            yPosition += 40;
        }

        private void btnSacuvaj_Click(object sender, EventArgs e)
        {
            try
            {
                string? izabrano = cmbTipObjekta.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(izabrano))
                {
                    MessageBox.Show("Morate izabrati tip objekta!", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                IDomainObject? objekat = null;

                switch (izabrano)
                {
                    case "Prodavac":
                        // Validacija telefona
                        string telefonProdavac = GetTextBoxValue("Telefon");
                        if (!ValidirajTelefon(telefonProdavac))
                            return;

                        string passwordValue = GetTextBoxValue("Password");
                        objekat = new Prodavac
                        {
                            IdProdavac = isEditMode && existingObject is Prodavac p ? p.IdProdavac : 0,
                            ImePrezime = GetTextBoxValue("ImePrezime"),
                            Email = GetTextBoxValue("Email"),
                            Telefon = telefonProdavac.Trim().Replace(" ", "").Replace("-", ""),
                            // Ako je password prazan (polje nije vidljivo), zadrži staru lozinku
                            Password = !string.IsNullOrEmpty(passwordValue) ? passwordValue : (isEditMode && existingObject is Prodavac pOld ? pOld.Password : "")
                        };
                        
                        // Sačuvaj prodavca prvo
                        Operation operation = isEditMode ? Operation.AzurirajObjekat : Operation.DodajObjekat;
                        Request reqProdavac = new Request { Operation = operation, Data = objekat };
                        Response resProdavac = CommunicationHelper.Instance.SendRequest(reqProdavac);

                        if (resProdavac.IsSuccessful)
                        {
                            // Dobij ID prodavca (novi ili postojeći)
                            int idProdavac = ((Prodavac)objekat).IdProdavac;
                            
                            // Ako je novi prodavac, ID će biti vraćen u poruci ili trebamo da ga učitamo
                            if (!isEditMode)
                            {
                                // Za novi prodavac, učitaj ga ponovo da dobiješ ID
                                Prodavac tempProdavac = new Prodavac { Email = ((Prodavac)objekat).Email, Password = ((Prodavac)objekat).Password };
                                Request reqGet = new Request { Operation = Operation.VratiJedan, Data = tempProdavac };
                                Response resGet = CommunicationHelper.Instance.SendRequest(reqGet);
                                
                                if (resGet.IsSuccessful && resGet.Data != null)
                                {
                                    string jsonData = resGet.Data is System.Text.Json.JsonElement jsonElement ? jsonElement.GetRawText() : resGet.Data.ToString();
                                    var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                                    var ucitanProdavac = System.Text.Json.JsonSerializer.Deserialize<Prodavac>(jsonData, options);
                                    if (ucitanProdavac != null)
                                        idProdavac = ucitanProdavac.IdProdavac;
                                }
                            }
                            
                            // Ažuriraj skladišta
                            AzurirajProdavacSkladista(idProdavac);
                            
                            string poruka = isEditMode ? "Prodavac je uspešno ažuriran!" : "Prodavac je uspešno sačuvan!";
                            MessageBox.Show(poruka, "Uspešno", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                            return;
                        }
                        else
                        {
                            MessageBox.Show(resProdavac.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    case "Kupac - Firma":
                        // Validacija PIB-a
                        string pib = GetTextBoxValue("Pib");
                        if (!ValidirajPib(pib))
                            return;

                        // Validacija popusta - mora biti čekirano partnerstvo ako postoji popust
                        double popustFirma = (double)GetNumericValue("Popust");
                        bool partnerstvoFirma = GetCheckBoxValue("Partnerstvo");
                        
                        if (popustFirma > 0 && !partnerstvoFirma)
                        {
                            MessageBox.Show("Ne možete dodeliti popust bez partnerstva! Molimo čekirajte 'Partnerstvo' polje.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        objekat = new Firma
                        {
                            IdKupac = isEditMode && existingObject is Firma f ? f.IdKupac : 0,
                            Naziv = GetTextBoxValue("Naziv"),
                            Popust = popustFirma,
                            Pib = pib.Trim(),
                            Adresa = GetTextBoxValue("AdresaFirme"),
                            Partnerstvo = partnerstvoFirma
                        };
                        break;
                    case "Kupac - Fizičko lice":
                        // Validacija telefona
                        string telefonFizicko = GetTextBoxValue("TelefonFizicko");
                        if (!ValidirajTelefon(telefonFizicko))
                            return;

                        // Validacija popusta - mora biti čekiran loyalty clan ako postoji popust
                        double popustFizicko = (double)GetNumericValue("Popust");
                        bool loyaltyClanFizicko = GetCheckBoxValue("LoyaltyClan");
                        
                        if (popustFizicko > 0 && !loyaltyClanFizicko)
                        {
                            MessageBox.Show("Ne možete dodeliti popust bez loyalty člana! Molimo čekirajte 'Loyalty Član' polje.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        objekat = new FizickoLice
                        {
                            IdKupac = isEditMode && existingObject is FizickoLice fl ? fl.IdKupac : 0,
                            Popust = popustFizicko,
                            ImePrezime = GetTextBoxValue("ImePrezimeFizicko"),
                            Email = GetTextBoxValue("EmailFizicko"),
                            Telefon = telefonFizicko.Trim().Replace(" ", "").Replace("-", ""),
                            LoyaltyClan = loyaltyClanFizicko
                        };
                        break;
                    case "Oprema":
                        objekat = new Oprema
                        {
                            IdOprema = isEditMode && existingObject is Oprema o ? o.IdOprema : 0,
                            Ime = GetTextBoxValue("Ime"),
                            Kategorija = GetComboBoxValue("Kategorija"),
                            KategorijaEnum = KategorijaOpremeExtensions.FromString(GetComboBoxValue("Kategorija")),
                            Cena = (double)GetNumericValue("Cena")
                        };
                        break;
                    case "Skladište":
                        objekat = new Skladiste
                        {
                            IdSkladiste = isEditMode && existingObject is Skladiste s ? s.IdSkladiste : 0,
                            Naziv = GetTextBoxValue("Naziv"),
                            Adresa = GetTextBoxValue("Adresa")
                        };
                        break;
                }

                if (objekat != null)
                {
                    // Pošalji zahtev serveru za dodavanje ili ažuriranje
                    Operation operation = isEditMode ? Operation.AzurirajObjekat : Operation.DodajObjekat;
                    Request req = new Request { Operation = operation, Data = objekat };
                    Response res = CommunicationHelper.Instance.SendRequest(req);

                    if (res.IsSuccessful)
                    {
                        string poruka = isEditMode ? "Objekat je uspešno ažuriran!" : "Objekat je uspešno sačuvan!";
                        MessageBox.Show(poruka, "Uspešno", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(res.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri čuvanju: {ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetTextBoxValue(string controlName)
        {
            if (inputControls.ContainsKey(controlName) && inputControls[controlName] is TextBox txt)
            {
                return txt.Text;
            }
            return string.Empty;
        }

        private decimal GetNumericValue(string controlName)
        {
            if (inputControls.ContainsKey(controlName) && inputControls[controlName] is NumericUpDown num)
            {
                return num.Value;
            }
            return 0;
        }

        private void SetTextBoxValue(string controlName, string value)
        {
            if (inputControls.ContainsKey(controlName) && inputControls[controlName] is TextBox txt)
            {
                txt.Text = value ?? string.Empty;
            }
        }

        private void SetNumericValue(string controlName, decimal value)
        {
            if (inputControls.ContainsKey(controlName) && inputControls[controlName] is NumericUpDown num)
            {
                num.Value = value;
            }
        }

        private bool GetCheckBoxValue(string controlName)
        {
            if (inputControls.ContainsKey(controlName) && inputControls[controlName] is CheckBox chk)
            {
                return chk.Checked;
            }
            return false;
        }

        private void SetCheckBoxValue(string controlName, bool value)
        {
            if (inputControls.ContainsKey(controlName) && inputControls[controlName] is CheckBox chk)
            {
                chk.Checked = value;
            }
        }
        
        private string GetComboBoxValue(string controlName)
        {
            if (inputControls.ContainsKey(controlName) && inputControls[controlName] is ComboBox cmb)
            {
                return cmb.SelectedItem?.ToString() ?? string.Empty;
            }
            return string.Empty;
        }
        
        private void SetComboBoxValue(string controlName, string value)
        {
            if (inputControls.ContainsKey(controlName) && inputControls[controlName] is ComboBox cmb)
            {
                int index = cmb.Items.IndexOf(value);
                if (index >= 0)
                    cmb.SelectedIndex = index;
            }
        }

        private void btnOtkazi_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void UcitajSkladista()
        {
            try
            {
                Request req = new Request { Operation = Operation.VratiSve, Data = new Skladiste() };
                Response res = CommunicationHelper.Instance.SendRequest(req);

                if (res.IsSuccessful && res.Data != null)
                {
                    string jsonData = res.Data is System.Text.Json.JsonElement jsonElement ? jsonElement.GetRawText() : res.Data.ToString();
                    var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    svaSkladista = System.Text.Json.JsonSerializer.Deserialize<List<Skladiste>>(jsonData, options) ?? new List<Skladiste>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju skladišta: {ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UcitajProdavacSkladista(int idProdavac)
        {
            try
            {
                Request req = new Request { Operation = Operation.VratiSve, Data = new ProdSklad { IdProdavac = idProdavac } };
                Response res = CommunicationHelper.Instance.SendRequest(req);

                if (res.IsSuccessful && res.Data != null)
                {
                    string jsonData = res.Data is System.Text.Json.JsonElement jsonElement ? jsonElement.GetRawText() : res.Data.ToString();
                    var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    prodavacSkladista = System.Text.Json.JsonSerializer.Deserialize<List<ProdSklad>>(jsonData, options) ?? new List<ProdSklad>();
                    
                    // Čekiraj odgovarajuća skladišta u CheckedListBox-u
                    if (inputControls.ContainsKey("Skladista") && inputControls["Skladista"] is CheckedListBox chklst)
                    {
                        for (int i = 0; i < chklst.Items.Count; i++)
                        {
                            dynamic item = chklst.Items[i];
                            Skladiste skladiste = item.Value;
                            
                            bool jeCekirano = prodavacSkladista.Exists(ps => ps.IdSkladiste == skladiste.IdSkladiste);
                            chklst.SetItemChecked(i, jeCekirano);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju skladišta prodavca: {ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AzurirajProdavacSkladista(int idProdavac)
        {
            try
            {
                // Prvo obriši sve postojeće ProdSklad zapise za ovog prodavca
                if (isEditMode)
                {
                    foreach (var ps in prodavacSkladista)
                    {
                        Request reqDelete = new Request { Operation = Operation.ObrisiObjekat, Data = ps };
                        CommunicationHelper.Instance.SendRequest(reqDelete);
                    }
                }

                // Dodaj nove zapise za čekirana skladišta
                if (inputControls.ContainsKey("Skladista") && inputControls["Skladista"] is CheckedListBox chklst)
                {
                    for (int i = 0; i < chklst.CheckedItems.Count; i++)
                    {
                        dynamic item = chklst.CheckedItems[i];
                        Skladiste skladiste = item.Value;

                        ProdSklad ps = new ProdSklad
                        {
                            IdProdavac = idProdavac,
                            IdSkladiste = skladiste.IdSkladiste
                        };

                        Request reqAdd = new Request { Operation = Operation.DodajObjekat, Data = ps };
                        CommunicationHelper.Instance.SendRequest(reqAdd);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri ažuriranju skladišta: {ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DodajCheckedListBox(string labelText, string controlName, ref int yPosition, List<Skladiste> skladista)
        {
            Label lbl = new Label
            {
                Text = labelText,
                Location = new Point(10, yPosition),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = true
            };
            panelInputs.Controls.Add(lbl);

            CheckedListBox chklst = new CheckedListBox
            {
                Name = controlName,
                Location = new Point(200, yPosition - 3),
                Width = 300,
                Height = 100,
                Font = new Font("Segoe UI", 10F),
                CheckOnClick = true
            };

            // Popuni listu skladištima
            foreach (var s in skladista)
            {
                chklst.Items.Add(new { Display = $"{s.Naziv} - {s.Adresa}", Value = s }, false);
            }

            chklst.DisplayMember = "Display";
            chklst.ValueMember = "Value";

            panelInputs.Controls.Add(chklst);
            inputControls[controlName] = chklst;

            yPosition += 120;
        }
    }
}
