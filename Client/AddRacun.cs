using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Models;
using Client.Client;
using System.Text.Json;

namespace Client
{
    public partial class AddRacun : Form
    {
        private List<StavkaRacuna> stavke = new List<StavkaRacuna>();
        private List<Kupac> sviKupci = new List<Kupac>();
        private List<Firma> sveFirme = new List<Firma>();
        private List<FizickoLice> svaFizickaLica = new List<FizickoLice>();
        private List<Oprema> svaOprema = new List<Oprema>();
        private const double PDV_STOPA = 0.20; // 20% PDV
        private Racun? existingRacun = null; // Za edit režim
        private bool isEditMode = false;
        private int? selektovanaStavkaIndex = null; // Za praćenje koja stavka se ažurira

        public AddRacun()
        {
            InitializeComponent();
            ApplyModernStyling();
            UcitajPodatke();

            // Event handler za klik na stavku
            dgvStavke.CellClick += dgvStavke_CellClick;
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

            btnDodajStavku.FlatAppearance.MouseOverBackColor = Color.FromArgb(39, 174, 96);
            btnDodajStavku.MouseEnter += (s, ev) => btnDodajStavku.BackColor = Color.FromArgb(39, 174, 96);
            btnDodajStavku.MouseLeave += (s, ev) => btnDodajStavku.BackColor = Color.FromArgb(46, 204, 113);

            btnObrisiStavku.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 57, 43);
            btnObrisiStavku.MouseEnter += (s, ev) => btnObrisiStavku.BackColor = Color.FromArgb(192, 57, 43);
            btnObrisiStavku.MouseLeave += (s, ev) => btnObrisiStavku.BackColor = Color.FromArgb(231, 76, 60);

            // Moderan styling za DataGridView
            dgvStavke.EnableHeadersVisualStyles = false;
            dgvStavke.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvStavke.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvStavke.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvStavke.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgvStavke.ColumnHeadersHeight = 35;
            dgvStavke.DefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
            dgvStavke.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvStavke.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            dgvStavke.RowTemplate.Height = 30;
        }

        // Konstruktor za edit režim
        public AddRacun(Racun racun) : this()
        {
            existingRacun = racun;
            isEditMode = true;
            this.Text = "Ažuriraj račun";
            btnSacuvaj.Text = "Ažuriraj";
            
            // Popuni formu sa postojećim podacima
            UcitajPostojeciRacun();
        }

        private void UcitajPostojeciRacun()
        {
            if (existingRacun == null) return;

            // Postavi datum
            dtpDatum.Value = existingRacun.DatumIzdavanja;

            // Pronađi i selektuj kupca
            for (int i = 0; i < cmbKupac.Items.Count; i++)
            {
                dynamic item = cmbKupac.Items[i];
                Kupac kupac = item.Value;
                if (kupac.IdKupac == existingRacun.Kupac.IdKupac)
                {
                    cmbKupac.SelectedIndex = i;
                    break;
                }
            }

            // Učitaj stavke računa
            if (existingRacun.Stavke != null && existingRacun.Stavke.Count > 0)
            {
                stavke = new List<StavkaRacuna>(existingRacun.Stavke);
                OsveziPrikazStavki();
                IzracunajUkupno();
            }
        }

        private void UcitajPodatke()
        {
            // Učitaj sve firme
            Request reqFirme = new Request { Operation = Operation.VratiSve, Data = new Firma() };
            Response resFirme = CommunicationHelper.Instance.SendRequest(reqFirme);
            
            if (resFirme.IsSuccessful && resFirme.Data != null)
            {
                string jsonData = resFirme.Data is JsonElement jsonElement ? jsonElement.GetRawText() : resFirme.Data.ToString();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                sveFirme = JsonSerializer.Deserialize<List<Firma>>(jsonData, options);
            }

            // Učitaj sva fizička lica
            Request reqFizicka = new Request { Operation = Operation.VratiSve, Data = new FizickoLice() };
            Response resFizicka = CommunicationHelper.Instance.SendRequest(reqFizicka);
            
            if (resFizicka.IsSuccessful && resFizicka.Data != null)
            {
                string jsonData = resFizicka.Data is JsonElement jsonElement ? jsonElement.GetRawText() : resFizicka.Data.ToString();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                svaFizickaLica = JsonSerializer.Deserialize<List<FizickoLice>>(jsonData, options);
            }

            // Popuni ComboBox za kupce
            cmbKupac.Items.Clear();
            cmbKupac.DisplayMember = "Display";
            cmbKupac.ValueMember = "Value";

            foreach (var firma in sveFirme)
            {
                cmbKupac.Items.Add(new { Display = $"Firma: {firma.Naziv}", Value = new Kupac { IdKupac = firma.IdKupac, Popust = firma.Popust } });
            }

            foreach (var fizicko in svaFizickaLica)
            {
                cmbKupac.Items.Add(new { Display = $"Fizičko lice: {fizicko.ImePrezime}", Value = new Kupac { IdKupac = fizicko.IdKupac, Popust = fizicko.Popust } });
            }

            // Učitaj svu opremu
            Request reqOprema = new Request { Operation = Operation.VratiSve, Data = new Oprema() };
            Response resOprema = CommunicationHelper.Instance.SendRequest(reqOprema);
            
            if (resOprema.IsSuccessful && resOprema.Data != null)
            {
                string jsonData = resOprema.Data is JsonElement jsonElement ? jsonElement.GetRawText() : resOprema.Data.ToString();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                svaOprema = JsonSerializer.Deserialize<List<Oprema>>(jsonData, options);
            }

            // Popuni ComboBox za opremu
            cmbOprema.Items.Clear();
            cmbOprema.DisplayMember = "Display";
            cmbOprema.ValueMember = "Value";

            foreach (var oprema in svaOprema)
            {
                cmbOprema.Items.Add(new { Display = $"{oprema.Ime} - {oprema.Cena:C2}", Value = oprema });
            }

            // Postavi datum na danas
            dtpDatum.Value = DateTime.Now;
        }

        private void btnDodajStavku_Click(object sender, EventArgs e)
        {
            if (cmbOprema.SelectedItem == null)
            {
                MessageBox.Show("Molim izaberite opremu!", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (numKolicina.Value <= 0)
            {
                MessageBox.Show("Količina mora biti veća od 0!", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dynamic selectedItem = cmbOprema.SelectedItem;
            Oprema oprema = selectedItem.Value;

            int kolicina = (int)numKolicina.Value;

            // Provjeri da li ažuriramo postojeću stavku ili dodajemo novu
            if (selektovanaStavkaIndex.HasValue)
            {
                // Ažuriranje postojeće stavke
                stavke[selektovanaStavkaIndex.Value].Kolicina = kolicina;
                stavke[selektovanaStavkaIndex.Value].Cena = oprema.Cena;
                stavke[selektovanaStavkaIndex.Value].Oprema = oprema;
                
                // Resetuj selekciju
                selektovanaStavkaIndex = null;
                btnDodajStavku.Text = "Dodaj stavku";
                btnDodajStavku.BackColor = Color.LightGreen;
            }
            else
            {
                // Dodavanje nove stavke
                StavkaRacuna stavka = new StavkaRacuna
                {
                    Kolicina = kolicina,
                    Cena = oprema.Cena,
                    Oprema = oprema
                };

                stavke.Add(stavka);
            }

            OsveziPrikazStavki();
            IzracunajUkupno();

            // Resetuj selekciju
            cmbOprema.SelectedIndex = -1;
            numKolicina.Value = 1;
        }

        private void OsveziPrikazStavki()
        {
            dgvStavke.DataSource = null;
            
            var prikazStavki = stavke.Select((s, index) => new
            {
                RB = index + 1,
                Oprema = s.Oprema.Ime,
                Kategorija = s.Oprema.Kategorija,
                CenaPoKomadu = s.Cena,
                Količina = s.Kolicina,
                Iznos = s.Iznos
            }).ToList();

            dgvStavke.DataSource = prikazStavki;
            FormatiranjKolona(dgvStavke);
        }

        private void IzracunajUkupno()
        {
            if (cmbKupac.SelectedItem == null)
            {
                return;
            }

            dynamic selectedKupac = cmbKupac.SelectedItem;
            Kupac kupac = selectedKupac.Value;

            // Suma svih stavki
            double cenaStavki = stavke.Sum(s => s.Iznos);
            
            // Popust
            double popust = cenaStavki * kupac.Popust;
            double cenaSaPopustom = cenaStavki - popust;
            
            // PDV
            double pdv = cenaSaPopustom * PDV_STOPA;
            
            // Konačan iznos
            double konacanIznos = cenaSaPopustom + pdv;

            lblCenaStavki.Text = $"Cena stavki: {cenaStavki:C2}";
            lblPopust.Text = $"Popust ({kupac.Popust:P0}): -{popust:C2}";
            lblCenaSaPopustom.Text = $"Cena sa popustom: {cenaSaPopustom:C2}";
            lblPDV.Text = $"PDV (20%): {pdv:C2}";
            lblKonacanIznos.Text = $"KONAČAN IZNOS: {konacanIznos:C2}";
        }

        private void dgvStavke_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < stavke.Count)
            {
                // Sačuvaj indeks selektovane stavke
                selektovanaStavkaIndex = e.RowIndex;
                
                // Uzmi stavku iz liste
                StavkaRacuna stavka = stavke[e.RowIndex];
                
                // Popuni ComboBox sa opremom iz stavke
                for (int i = 0; i < cmbOprema.Items.Count; i++)
                {
                    dynamic item = cmbOprema.Items[i];
                    Oprema oprema = item.Value;
                    if (oprema.IdOprema == stavka.Oprema.IdOprema)
                    {
                        cmbOprema.SelectedIndex = i;
                        break;
                    }
                }
                
                // Popuni količinu
                numKolicina.Value = stavka.Kolicina;
                
                // Promijeni tekst dugmeta i boju
                btnDodajStavku.Text = "Ažuriraj stavku";
                btnDodajStavku.BackColor = Color.LightBlue;
            }
        }

        private void btnObrisiStavku_Click(object sender, EventArgs e)
        {
            if (dgvStavke.SelectedRows.Count > 0)
            {
                int index = dgvStavke.SelectedRows[0].Index;
                stavke.RemoveAt(index);
                
                // Resetuj selekciju za ažuriranje
                selektovanaStavkaIndex = null;
                btnDodajStavku.Text = "Dodaj stavku";
                btnDodajStavku.BackColor = Color.LightGreen;
                cmbOprema.SelectedIndex = -1;
                numKolicina.Value = 1;
                
                // Ažuriraj RB stavke
                for (int i = 0; i < stavke.Count; i++)
                {
                    stavke[i].RbStavke = i + 1;
                }
                
                OsveziPrikazStavki();
                IzracunajUkupno();
            }
        }

        private void cmbKupac_SelectedIndexChanged(object sender, EventArgs e)
        {
            IzracunajUkupno();
        }

        private void btnSacuvaj_Click(object sender, EventArgs e)
        {
            // Admin ne može da dodaje račune jer nije registrovan prodavac
            if (Sesija.IsAdmin)
            {
                MessageBox.Show("Admin ne može da kreira/ažurira račune jer nije registrovan prodavac!", 
                    "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (cmbKupac.SelectedItem == null)
            {
                MessageBox.Show("Molim izaberite kupca!", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (stavke.Count == 0)
            {
                MessageBox.Show("Račun mora imati bar jednu stavku!", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                dynamic selectedKupac = cmbKupac.SelectedItem;
                Kupac kupac = selectedKupac.Value;

                // Izračunaj vrednosti
                double cenaStavki = stavke.Sum(s => s.Iznos);
                double cenaSaPopustom = cenaStavki * (1 - kupac.Popust);
                double pdv = cenaSaPopustom * PDV_STOPA;
                double konacanIznos = cenaSaPopustom + pdv;

                // Odredi prodavca: pri ažuriranju zadrži originalnog, pri kreiranju koristi ulogovanog
                Prodavac prodavac;
                if (isEditMode && existingRacun != null)
                {
                    // Pri ažuriranju zadrži originalnog prodavca
                    prodavac = existingRacun.Prodavac;
                }
                else
                {
                    // Pri kreiranju koristi ulogovanog prodavca
                    prodavac = new Prodavac { IdProdavac = Sesija.UlogovaniProdavac.IdProdavac };
                }

                Racun racun = new Racun
                {
                    DatumIzdavanja = dtpDatum.Value,
                    CenaStavke = cenaStavki,
                    CenaSaPopustom = cenaSaPopustom,
                    Pdv = pdv,
                    KonacanIznos = konacanIznos,
                    Prodavac = prodavac,
                    Kupac = kupac,
                    Stavke = stavke
                };

                // Ako je edit režim, dodaj ID računa
                if (isEditMode && existingRacun != null)
                {
                    racun.IdRacun = existingRacun.IdRacun;
                }

                // Pošalji na server
                Operation operation = isEditMode ? Operation.AzurirajObjekat : Operation.DodajObjekat;
                Request req = new Request { Operation = operation, Data = racun };
                Response res = CommunicationHelper.Instance.SendRequest(req);

                if (res.IsSuccessful)
                {
                    // Ako je kreiran novi račun, server vraća objekat sa popunjenim ID-evima.
                    if (!isEditMode && res.Data != null)
                    {
                        try
                        {
                            string jsonData = res.Data is System.Text.Json.JsonElement je ? je.GetRawText() : res.Data.ToString();
                            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            Racun created = JsonSerializer.Deserialize<Racun>(jsonData, options);

                            if (created != null && created.Kupac != null && created.Kupac.IdKupac != 0)
                            {
                                // Dodaj novog kupca u lokalne kolekcije i combobox ako već ne postoji
                                bool exists = false;
                                for (int i = 0; i < cmbKupac.Items.Count; i++)
                                {
                                    dynamic item = cmbKupac.Items[i];
                                    Kupac k = item.Value;
                                    if (k.IdKupac == created.Kupac.IdKupac)
                                    {
                                        exists = true;
                                        // Selektuj novog kupca
                                        cmbKupac.SelectedIndex = i;
                                        break;
                                    }
                                }

                                if (!exists)
                                {
                                    var newKupac = new Kupac { IdKupac = created.Kupac.IdKupac, Popust = created.Kupac.Popust };
                                    string display = string.IsNullOrEmpty(created.NazivKupca) ? $"Kupac #{newKupac.IdKupac}" : created.NazivKupca;
                                    cmbKupac.Items.Add(new { Display = display, Value = newKupac });
                                    cmbKupac.SelectedIndex = cmbKupac.Items.Count - 1;
                                }

                                // Takođe osveži lokalne liste (ako ih koristiš kasnije)
                                sviKupci.Add(created.Kupac);
                            }
                        }
                        catch { }
                    }

                    string poruka = isEditMode ? "Račun je uspešno ažuriran!" : "Račun je uspešno kreiran!";
                    MessageBox.Show(poruka, "Uspešno", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(res.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri čuvanju računa: {ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOtkazi_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
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
    }
}
