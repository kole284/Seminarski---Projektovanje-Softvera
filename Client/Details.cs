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
    public partial class Details : Form
    {
        private object? currentObject;

        public Details()
        {
            InitializeComponent();
            ApplyModernStyling();
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

            btnPromeniSifru.FlatAppearance.MouseOverBackColor = Color.FromArgb(211, 84, 0);
            btnPromeniSifru.MouseEnter += (s, ev) => btnPromeniSifru.BackColor = Color.FromArgb(211, 84, 0);
            btnPromeniSifru.MouseLeave += (s, ev) => btnPromeniSifru.BackColor = Color.FromArgb(230, 126, 34);
        }

        public Details(object selectedObject) : this()
        {
            currentObject = selectedObject;
            PrikaziDetalje(selectedObject);
            
            // Ako je Prodavac
            if (currentObject is Prodavac prodavac)
            {
                // Dugme za promenu lozinke se prikazuje SAMO za trenutno ulogovanog prodavca
                if (Sesija.UlogovaniProdavac != null && prodavac.IdProdavac == Sesija.UlogovaniProdavac.IdProdavac)
                {
                    btnPromeniSifru.Visible = true;
                }
                else
                {
                    btnPromeniSifru.Visible = false;
                }
                
                // Kontrola pristupa
                if (!Sesija.IsAdmin)
                {
                    if (prodavac.IdProdavac != Sesija.ProdavacId)
                    {
                        btnAzuriraj.Visible = false;
                        btnObrisi.Visible = false;
                    }
                    else
                    {
                        // Može azurirati svoj profil i menjati lozinku, ali ne može da se obriše
                        btnObrisi.Visible = false;
                    }
                }
            }
            else
            {
                btnPromeniSifru.Visible = false;
                
                // Kontrola pristupa za ostale objekte
                if (!Sesija.IsAdmin)
                {
                    // Obični korisnici ne mogu menjati/brisati firme, fizička lica, opremu, skladišta
                    btnAzuriraj.Visible = false;
                    btnObrisi.Visible = false;
                }
            }
        }

        private void PrikaziDetalje(object obj)
        {
            panelDetails.Controls.Clear();
            int yPosition = 10;

            if (obj is Prodavac prodavac)
            {
                lblTitle.Text = "Detalji prodavca";
                DodajPolje("ID Prodavac:", prodavac.IdProdavac.ToString(), ref yPosition);
                DodajPolje("Ime i Prezime:", prodavac.ImePrezime, ref yPosition);
                DodajPolje("Email:", prodavac.Email, ref yPosition);
                DodajPolje("Telefon:", prodavac.Telefon, ref yPosition);
                DodajPolje("Skladišta:", !string.IsNullOrWhiteSpace(prodavac.Skladista) ? prodavac.Skladista : "Nema dodeljenih skladišta", ref yPosition);
            }
            else if (obj is Firma firma)
            {
                lblTitle.Text = "Detalji firme";
                DodajPolje("ID Kupac:", firma.IdKupac.ToString(), ref yPosition);
                DodajPolje("Naziv:", firma.Naziv, ref yPosition);
                DodajPolje("Popust:", firma.Popust.ToString("P2"), ref yPosition);
                DodajPolje("PIB:", firma.Pib, ref yPosition);
                DodajPolje("Adresa:", firma.Adresa, ref yPosition);
                DodajPolje("Partnerstvo:", firma.Partnerstvo ? "Da" : "Ne", ref yPosition);
            }
            else if (obj is FizickoLice fizickoLice)
            {
                lblTitle.Text = "Detalji fizičkog lica";
                DodajPolje("ID Kupac:", fizickoLice.IdKupac.ToString(), ref yPosition);
                DodajPolje("Popust:", fizickoLice.Popust.ToString("P2"), ref yPosition);
                DodajPolje("Ime i Prezime:", fizickoLice.ImePrezime, ref yPosition);
                DodajPolje("Email:", fizickoLice.Email, ref yPosition);
                DodajPolje("Telefon:", fizickoLice.Telefon, ref yPosition);
                DodajPolje("Loyalty Clan:", fizickoLice.LoyaltyClan ? "Da" : "Ne", ref yPosition);
            }
            else if (obj is Oprema oprema)
            {
                lblTitle.Text = "Detalji opreme";
                DodajPolje("ID Oprema:", oprema.IdOprema.ToString(), ref yPosition);
                DodajPolje("Naziv Opreme:", oprema.Ime, ref yPosition);
                DodajPolje("Kategorija:", oprema.Kategorija, ref yPosition);
                DodajPolje("Cena:", oprema.Cena.ToString("C2"), ref yPosition);
            }
            else if (obj is Skladiste skladiste)
            {
                lblTitle.Text = "Detalji skladišta";
                DodajPolje("ID Skladište:", skladiste.IdSkladiste.ToString(), ref yPosition);
                DodajPolje("Adresa:", skladiste.Adresa, ref yPosition);
            }
            else if (obj is Racun racun)
            {
                // Za račune koristimo specijalnu DetailsRacun formu
                DetailsRacun detailsRacunForm = new DetailsRacun(racun);
                detailsRacunForm.ShowDialog();
                return; // Ne prikazuj u ovoj formi
            }
            else
            {
                lblTitle.Text = "Detalji";
                Label lblError = new Label
                {
                    Text = "Nepoznat tip objekta",
                    Location = new Point(10, yPosition),
                    AutoSize = true,
                    ForeColor = Color.Red
                };
                panelDetails.Controls.Add(lblError);
            }
        }

        private void DodajPolje(string naziv, string vrednost, ref int yPosition)
        {
            // Label za naziv polja
            Label lblNaziv = new Label
            {
                Text = naziv,
                Location = new Point(10, yPosition),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = true,
                Width = 200
            };
            panelDetails.Controls.Add(lblNaziv);

            // Label za vrednost
            Label lblVrednost = new Label
            {
                Text = vrednost ?? "N/A",
                Location = new Point(220, yPosition),
                Font = new Font("Segoe UI", 10F),
                AutoSize = true
            };
            panelDetails.Controls.Add(lblVrednost);

            yPosition += 35;
        }

        private void btnAzuriraj_Click(object sender, EventArgs e)
        {
            if (currentObject == null)
            {
                MessageBox.Show("Nema objekta za ažuriranje.", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Provera dozvola
            if (!Sesija.IsAdmin)
            {
                if (currentObject is Prodavac prodavac)
                {
                    if (prodavac.IdProdavac != Sesija.ProdavacId)
                    {
                        MessageBox.Show("Nemate dozvolu da menjate podatke drugih prodavaca.", "Nemate pristup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Nemate dozvolu za ovu operaciju.", "Nemate pristup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Ne dozvoljavaj izmenu Računa kroz ovaj interfejs (može biti slozheniji)
            if (currentObject is Racun)
            {
                MessageBox.Show("Računi se ne mogu direktno menjati.", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Otvori Add formu u Edit mode-u
            if (currentObject is IDomainObject domainObject)
            {
                Add editForm = new Add(domainObject);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    // Zatvori Details formu sa OK rezultatom da bi Main znao da osvezhi podatke
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        private void btnObrisi_Click(object sender, EventArgs e)
        {
            if (currentObject == null)
            {
                MessageBox.Show("Nema objekta za brisanje.", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Provera dozvola - samo admin može brisati
            if (!Sesija.IsAdmin)
            {
                MessageBox.Show("Nemate dozvolu za brisanje. Samo administrator može brisati podatke.", "Nemate pristup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Ne dozvoljavaj brisanje Računa kroz ovaj interfejs
            if (currentObject is Racun)
            {
                MessageBox.Show("Računi se ne mogu direktno brisati.", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Potvrda brisanja
            string tipObjekta = currentObject.GetType().Name;
            string nazivObjekta = "";
            
            if (currentObject is Firma firma)
                nazivObjekta = firma.Naziv;
            else if (currentObject is FizickoLice fizickoLice)
                nazivObjekta = fizickoLice.ImePrezime;
            else if (currentObject is Prodavac prodavac)
                nazivObjekta = prodavac.ImePrezime;
            else if (currentObject is Oprema oprema)
                nazivObjekta = oprema.Ime;
            else if (currentObject is Skladiste skladiste)
                nazivObjekta = skladiste.Adresa;

            var result = MessageBox.Show(
                $"Da li ste sigurni da želite da obrišete {tipObjekta}: {nazivObjekta}?",
                "Potvrda brisanja",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Kreiramo Request sa ObrisiObjekat operacijom
                    Request req = new Request
                    {
                        Operation = Operation.ObrisiObjekat,
                        Data = currentObject
                    };

                    // Šaljemo preko CommunicationHelper-a
                    Response res = CommunicationHelper.Instance.SendRequest(req);

                    if (res.IsSuccessful)
                    {
                        MessageBox.Show(res.Message, "Uspešno", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
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

        private void btnPromeniSifru_Click(object sender, EventArgs e)
        {
            if (currentObject is Prodavac prodavac)
            {
                PromeniSifru promeniSifruForm = new PromeniSifru();
                if (promeniSifruForm.ShowDialog() == DialogResult.OK)
                {
                    // Ažuriraj prikaz ako je potrebno
                    MessageBox.Show("Šifra je uspešno promenjena!", "Uspeh", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
