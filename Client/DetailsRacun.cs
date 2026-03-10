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
    public partial class DetailsRacun : Form
    {
        private Racun racun;

        public DetailsRacun(Racun selectedRacun)
        {
            InitializeComponent();
            ApplyModernStyling();
            racun = selectedRacun;
            UcitajDetalje();

            // Kontrola pristupa
            if (Sesija.IsAdmin)
            {
                // Admin ne može ažurirati račune jer nije registrovan prodavac
                btnAzuriraj.Visible = false;
                // Admin može brisati sve račune
                btnObrisi.Visible = true;
            }
            else if (!Sesija.IsAdmin)
            {
                // Ako račun nije kreirao ulogovani prodavac, sakri dugmad
                if (racun.Prodavac?.IdProdavac != Sesija.ProdavacId)
                {
                    btnAzuriraj.Visible = false;
                    btnObrisi.Visible = false;
                }
                else
                {
                    // Može azurirati svoj račun, ali ne može da briše
                    btnObrisi.Visible = false;
                }
            }
        }

        private void ApplyModernStyling()
        {
            // Hover efekti za dugmad
            btnAzuriraj.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 128, 185);
            btnAzuriraj.MouseEnter += (s, ev) => btnAzuriraj.BackColor = Color.FromArgb(41, 128, 185);
            btnAzuriraj.MouseLeave += (s, ev) => btnAzuriraj.BackColor = Color.FromArgb(52, 152, 219);

            btnObrisi.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 57, 43);
            btnObrisi.MouseEnter += (s, ev) => btnObrisi.BackColor = Color.FromArgb(192, 57, 43);
            btnObrisi.MouseLeave += (s, ev) => btnObrisi.BackColor = Color.FromArgb(231, 76, 60);

            btnZatvori.FlatAppearance.MouseOverBackColor = Color.FromArgb(127, 140, 141);
            btnZatvori.MouseEnter += (s, ev) => btnZatvori.BackColor = Color.FromArgb(127, 140, 141);
            btnZatvori.MouseLeave += (s, ev) => btnZatvori.BackColor = Color.FromArgb(149, 165, 166);

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

        private void UcitajDetalje()
        {
            lblIdRacun.Text = $"Račun broj: {racun.IdRacun}";
            lblDatum.Text = $"Datum: {racun.DatumIzdavanja:dd.MM.yyyy}";
            lblProdavac.Text = $"Prodavac: {racun.ImeProdavca}";
            lblKupac.Text = $"Kupac: {racun.ImeKupca}";

            // Učitaj stavke računa
            UcitajStavke();

            // Prikaži sume
            lblCenaStavki.Text = $"Cena stavki: {racun.CenaStavke:C2}";
            lblCenaSaPopustom.Text = $"Cena sa popustom: {racun.CenaSaPopustom:C2}";
            lblPDV.Text = $"PDV (20%): {racun.Pdv:C2}";
            lblKonacanIznos.Text = $"KONAČAN IZNOS: {racun.KonacanIznos:C2}";
        }

        private void UcitajStavke()
        {
            // Ako račun nema učitane stavke, učitaj ih sa servera
            if (racun.Stavke == null || racun.Stavke.Count == 0)
            {
                try
                {
                    // Učitaj stavke sa servera
                    Request req = new Request
                    {
                        Operation = Operation.VratiSve,
                        Data = new StavkaRacuna { IdRacun = racun.IdRacun }
                    };

                    Response res = CommunicationHelper.Instance.SendRequest(req);
                    
                    if (res.IsSuccessful && res.Data != null)
                    {
                        string jsonData = res.Data is JsonElement jsonElement ? jsonElement.GetRawText() : res.Data.ToString();
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        racun.Stavke = JsonSerializer.Deserialize<List<StavkaRacuna>>(jsonData, options) ?? new List<StavkaRacuna>();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Greška pri učitavanju stavki: {ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Prikaži stavke
            if (racun.Stavke != null && racun.Stavke.Count > 0)
            {
                var prikazStavki = racun.Stavke.Select((s, index) => new
                {
                    RB = index + 1,
                    Oprema = s.Oprema?.Ime ?? "N/A",
                    Kategorija = s.Oprema?.Kategorija ?? "N/A",
                    CenaPoKomadu = s.Cena,
                    Količina = s.Kolicina,
                    Iznos = s.Iznos
                }).ToList();

                dgvStavke.DataSource = prikazStavki;
                FormatiranjKolona(dgvStavke);
            }
        }

        private void btnZatvori_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAzuriraj_Click(object sender, EventArgs e)
        {
            // Admin ne može ažurirati račune jer nije registrovan prodavac
            if (Sesija.IsAdmin)
            {
                MessageBox.Show("Admin ne može da ažurira račune jer nije registrovan prodavac!", 
                    "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Provera dozvola
            if (!Sesija.IsAdmin)
            {
                if (racun.Prodavac?.IdProdavac != Sesija.ProdavacId)
                {
                    MessageBox.Show("Nemate dozvolu da menjate račune drugih prodavaca.", "Nemate pristup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Otvori AddRacun formu u Edit režimu
            AddRacun editForm = new AddRacun(racun);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                // Osveži podatke
                UcitajDetalje();
                MessageBox.Show("Račun je uspešno ažuriran!", "Uspeh", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
        }

        private void btnObrisi_Click(object sender, EventArgs e)
        {
            // Provera dozvola - samo admin može brisati račune
            if (!Sesija.IsAdmin)
            {
                MessageBox.Show("Nemate dozvolu za brisanje računa. Samo administrator može brisati račune.", "Nemate pristup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Da li ste sigurni da želite da obrišete račun broj {racun.IdRacun}?\n\n" +
                $"UPOZORENJE: Ova akcija će obrisati račun i sve njegove stavke!",
                "Potvrda brisanja",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Kreiramo Request sa ObrisiObjekat operacijom
                    Request req = new Request
                    {
                        Operation = Operation.ObrisiObjekat,
                        Data = new Racun { IdRacun = racun.IdRacun }
                    };

                    // Šaljemo preko CommunicationHelper-a
                    Response res = CommunicationHelper.Instance.SendRequest(req);

                    if (res.IsSuccessful)
                    {
                        MessageBox.Show("Račun je uspešno obrisan!", "Uspeh", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        // Zatvori Details formu sa OK rezultatom da bi Main znao da osveži podatke
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
                    MessageBox.Show("Greška pri komunikaciji sa serverom: " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
