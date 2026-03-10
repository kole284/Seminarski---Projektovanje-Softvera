using System;
using System.Drawing;
using System.Windows.Forms;
using Models;
using Client.Client;

namespace Client
{
    public partial class PromeniSifru : Form
    {
        public PromeniSifru()
        {
            InitializeComponent();
            ApplyModernStyling();
        }

        private void ApplyModernStyling()
        {
            // Hover efekti za dugmad
            if (btnPromeni != null)
            {
                btnPromeni.MouseEnter += (s, ev) => btnPromeni.BackColor = Color.FromArgb(39, 174, 96);
                btnPromeni.MouseLeave += (s, ev) => btnPromeni.BackColor = Color.FromArgb(46, 204, 113);
            }

            if (btnOtkazi != null)
            {
                btnOtkazi.MouseEnter += (s, ev) => btnOtkazi.BackColor = Color.FromArgb(192, 57, 43);
                btnOtkazi.MouseLeave += (s, ev) => btnOtkazi.BackColor = Color.FromArgb(231, 76, 60);
            }
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblStaraSifra = new Label();
            this.txtStaraSifra = new TextBox();
            this.lblNovaSifra = new Label();
            this.txtNovaSifra = new TextBox();
            this.lblPotvrdaSifre = new Label();
            this.txtPotvrdaSifre = new TextBox();
            this.btnPromeni = new Button();
            this.btnOtkazi = new Button();
            this.panelHeader = new Panel();
            this.panelHeader.SuspendLayout();

            this.SuspendLayout();

            // panelHeader
            this.panelHeader.BackColor = Color.FromArgb(230, 126, 34);
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Dock = DockStyle.Top;
            this.panelHeader.Location = new Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new Size(500, 70);
            this.panelHeader.TabIndex = 9;

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.White;
            this.lblTitle.Location = new Point(130, 18);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(240, 32);
            this.lblTitle.Text = "🔑 Promena Šifre";

            // lblStaraSifra
            this.lblStaraSifra.AutoSize = true;
            this.lblStaraSifra.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            this.lblStaraSifra.ForeColor = Color.FromArgb(52, 73, 94);
            this.lblStaraSifra.Location = new Point(40, 95);
            this.lblStaraSifra.Name = "lblStaraSifra";
            this.lblStaraSifra.Size = new Size(80, 20);
            this.lblStaraSifra.Text = "Stara šifra:";

            // txtStaraSifra
            this.txtStaraSifra.Font = new Font("Segoe UI", 11F);
            this.txtStaraSifra.Location = new Point(190, 92);
            this.txtStaraSifra.Name = "txtStaraSifra";
            this.txtStaraSifra.PasswordChar = '●';
            this.txtStaraSifra.PlaceholderText = "Unesite staru šifru";
            this.txtStaraSifra.Size = new Size(270, 27);
            this.txtStaraSifra.TabIndex = 0;

            // lblNovaSifra
            this.lblNovaSifra.AutoSize = true;
            this.lblNovaSifra.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            this.lblNovaSifra.ForeColor = Color.FromArgb(52, 73, 94);
            this.lblNovaSifra.Location = new Point(40, 135);
            this.lblNovaSifra.Name = "lblNovaSifra";
            this.lblNovaSifra.Size = new Size(85, 20);
            this.lblNovaSifra.Text = "Nova šifra:";

            // txtNovaSifra
            this.txtNovaSifra.Font = new Font("Segoe UI", 11F);
            this.txtNovaSifra.Location = new Point(190, 132);
            this.txtNovaSifra.Name = "txtNovaSifra";
            this.txtNovaSifra.PasswordChar = '●';
            this.txtNovaSifra.PlaceholderText = "Minimum 4 karaktera";
            this.txtNovaSifra.Size = new Size(270, 27);
            this.txtNovaSifra.TabIndex = 1;

            // lblPotvrdaSifre
            this.lblPotvrdaSifre.AutoSize = true;
            this.lblPotvrdaSifre.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            this.lblPotvrdaSifre.ForeColor = Color.FromArgb(52, 73, 94);
            this.lblPotvrdaSifre.Location = new Point(40, 175);
            this.lblPotvrdaSifre.Name = "lblPotvrdaSifre";
            this.lblPotvrdaSifre.Size = new Size(95, 20);
            this.lblPotvrdaSifre.Text = "Potvrdi šifru:";

            // txtPotvrdaSifre
            this.txtPotvrdaSifre.Font = new Font("Segoe UI", 11F);
            this.txtPotvrdaSifre.Location = new Point(190, 172);
            this.txtPotvrdaSifre.Name = "txtPotvrdaSifre";
            this.txtPotvrdaSifre.PasswordChar = '●';
            this.txtPotvrdaSifre.PlaceholderText = "Ponovite novu šifru";
            this.txtPotvrdaSifre.Size = new Size(270, 27);
            this.txtPotvrdaSifre.TabIndex = 2;

            // btnPromeni
            this.btnPromeni.BackColor = Color.FromArgb(46, 204, 113);
            this.btnPromeni.FlatAppearance.BorderSize = 0;
            this.btnPromeni.FlatStyle = FlatStyle.Flat;
            this.btnPromeni.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.btnPromeni.ForeColor = Color.White;
            this.btnPromeni.Location = new Point(160, 225);
            this.btnPromeni.Name = "btnPromeni";
            this.btnPromeni.Size = new Size(140, 45);
            this.btnPromeni.TabIndex = 3;
            this.btnPromeni.Text = "✓ Promeni";
            this.btnPromeni.UseVisualStyleBackColor = false;
            this.btnPromeni.Click += new EventHandler(this.btnPromeni_Click);

            // btnOtkazi
            this.btnOtkazi.BackColor = Color.FromArgb(149, 165, 166);
            this.btnOtkazi.FlatAppearance.BorderSize = 0;
            this.btnOtkazi.FlatStyle = FlatStyle.Flat;
            this.btnOtkazi.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.btnOtkazi.ForeColor = Color.White;
            this.btnOtkazi.Location = new Point(310, 225);
            this.btnOtkazi.Name = "btnOtkazi";
            this.btnOtkazi.Size = new Size(140, 45);
            this.btnOtkazi.TabIndex = 4;
            this.btnOtkazi.Text = "✖ Otkaži";
            this.btnOtkazi.UseVisualStyleBackColor = false;
            this.btnOtkazi.Click += new EventHandler(this.btnOtkazi_Click);

            // PromeniSifru
            this.BackColor = Color.White;
            this.ClientSize = new Size(500, 295);
            this.Controls.Add(this.btnOtkazi);
            this.Controls.Add(this.btnPromeni);
            this.Controls.Add(this.txtPotvrdaSifre);
            this.Controls.Add(this.lblPotvrdaSifre);
            this.Controls.Add(this.txtNovaSifra);
            this.Controls.Add(this.lblNovaSifra);
            this.Controls.Add(this.txtStaraSifra);
            this.Controls.Add(this.lblStaraSifra);
            this.Controls.Add(this.panelHeader);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "PromeniSifru";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "🔑 Promena Šifre";
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Panel? panelHeader;
        private Label? lblTitle;
        private Label? lblStaraSifra;
        private TextBox? txtStaraSifra;
        private Label? lblNovaSifra;
        private TextBox? txtNovaSifra;
        private Label? lblPotvrdaSifre;
        private TextBox? txtPotvrdaSifre;
        private Button? btnPromeni;
        private Button? btnOtkazi;

        private void btnPromeni_Click(object? sender, EventArgs e)
        {
            // Validacija
            if (txtStaraSifra == null || string.IsNullOrWhiteSpace(txtStaraSifra.Text))
            {
                MessageBox.Show("Molim unesite staru šifru!", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtNovaSifra == null || string.IsNullOrWhiteSpace(txtNovaSifra.Text))
            {
                MessageBox.Show("Molim unesite novu šifru!", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtNovaSifra.Text.Length < 4)
            {
                MessageBox.Show("Nova šifra mora imati najmanje 4 karaktera!", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtPotvrdaSifre == null || txtNovaSifra.Text != txtPotvrdaSifre.Text)
            {
                MessageBox.Show("Nova šifra i potvrda šifre se ne poklapaju!", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Proveri staru šifru
            if (txtStaraSifra.Text != Sesija.UlogovaniProdavac.Password)
            {
                MessageBox.Show("Stara šifra nije tačna!", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Ažuriraj prodavca sa novom šifrom
                Prodavac izmenjeniProdavac = new Prodavac
                {
                    IdProdavac = Sesija.UlogovaniProdavac.IdProdavac,
                    Ime = Sesija.UlogovaniProdavac.Ime,
                    Prezime = Sesija.UlogovaniProdavac.Prezime,
                    Email = Sesija.UlogovaniProdavac.Email,
                    Telefon = Sesija.UlogovaniProdavac.Telefon,
                    Password = txtNovaSifra.Text
                };

                Request req = new Request { Operation = Operation.AzurirajObjekat, Data = izmenjeniProdavac };
                Response res = CommunicationHelper.Instance.SendRequest(req);

                if (res.IsSuccessful)
                {
                    // Ažuriraj sesiju sa novom šifrom
                    Sesija.UlogovaniProdavac.Password = txtNovaSifra.Text;
                    
                    MessageBox.Show("Šifra je uspešno promenjena!", "Uspeh", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show($"Greška pri promeni šifre: {ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOtkazi_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
