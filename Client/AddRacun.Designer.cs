namespace Client
{
    partial class AddRacun
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblKupac = new System.Windows.Forms.Label();
            this.cmbKupac = new System.Windows.Forms.ComboBox();
            this.lblDatum = new System.Windows.Forms.Label();
            this.dtpDatum = new System.Windows.Forms.DateTimePicker();
            this.groupBoxStavke = new System.Windows.Forms.GroupBox();
            this.lblOprema = new System.Windows.Forms.Label();
            this.cmbOprema = new System.Windows.Forms.ComboBox();
            this.lblKolicina = new System.Windows.Forms.Label();
            this.numKolicina = new System.Windows.Forms.NumericUpDown();
            this.btnDodajStavku = new System.Windows.Forms.Button();
            this.btnObrisiStavku = new System.Windows.Forms.Button();
            this.dgvStavke = new System.Windows.Forms.DataGridView();
            this.panelUkupno = new System.Windows.Forms.Panel();
            this.lblCenaStavki = new System.Windows.Forms.Label();
            this.lblPopust = new System.Windows.Forms.Label();
            this.lblCenaSaPopustom = new System.Windows.Forms.Label();
            this.lblPDV = new System.Windows.Forms.Label();
            this.lblKonacanIznos = new System.Windows.Forms.Label();
            this.btnSacuvaj = new System.Windows.Forms.Button();
            this.btnOtkazi = new System.Windows.Forms.Button();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.groupBoxStavke.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numKolicina)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStavke)).BeginInit();
            this.panelUkupno.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(800, 70);
            this.panelHeader.TabIndex = 9;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 18);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(240, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "🧾 Kreiranje Računa";
            // 
            // lblKupac
            // 
            this.lblKupac.AutoSize = true;
            this.lblKupac.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.lblKupac.ForeColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.lblKupac.Location = new System.Drawing.Point(25, 90);
            this.lblKupac.Name = "lblKupac";
            this.lblKupac.Size = new System.Drawing.Size(57, 20);
            this.lblKupac.TabIndex = 1;
            this.lblKupac.Text = "Kupac:";
            // 
            // cmbKupac
            // 
            this.cmbKupac.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbKupac.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cmbKupac.FormattingEnabled = true;
            this.cmbKupac.Location = new System.Drawing.Point(95, 87);
            this.cmbKupac.Name = "cmbKupac";
            this.cmbKupac.Size = new System.Drawing.Size(380, 28);
            this.cmbKupac.TabIndex = 2;
            this.cmbKupac.SelectedIndexChanged += new System.EventHandler(this.cmbKupac_SelectedIndexChanged);
            // 
            // lblDatum
            // 
            this.lblDatum.AutoSize = true;
            this.lblDatum.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.lblDatum.ForeColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.lblDatum.Location = new System.Drawing.Point(500, 90);
            this.lblDatum.Name = "lblDatum";
            this.lblDatum.Size = new System.Drawing.Size(58, 20);
            this.lblDatum.TabIndex = 3;
            this.lblDatum.Text = "Datum:";
            // 
            // dtpDatum
            // 
            this.dtpDatum.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.dtpDatum.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDatum.Location = new System.Drawing.Point(570, 87);
            this.dtpDatum.Name = "dtpDatum";
            this.dtpDatum.Size = new System.Drawing.Size(180, 27);
            this.dtpDatum.TabIndex = 4;
            // 
            // groupBoxStavke
            // 
            this.groupBoxStavke.BackColor = System.Drawing.Color.White;
            this.groupBoxStavke.Controls.Add(this.lblOprema);
            this.groupBoxStavke.Controls.Add(this.cmbOprema);
            this.groupBoxStavke.Controls.Add(this.lblKolicina);
            this.groupBoxStavke.Controls.Add(this.numKolicina);
            this.groupBoxStavke.Controls.Add(this.btnDodajStavku);
            this.groupBoxStavke.Controls.Add(this.btnObrisiStavku);
            this.groupBoxStavke.Controls.Add(this.dgvStavke);
            this.groupBoxStavke.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.groupBoxStavke.ForeColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.groupBoxStavke.Location = new System.Drawing.Point(25, 130);
            this.groupBoxStavke.Name = "groupBoxStavke";
            this.groupBoxStavke.Size = new System.Drawing.Size(750, 360);
            this.groupBoxStavke.TabIndex = 5;
            this.groupBoxStavke.TabStop = false;
            this.groupBoxStavke.Text = "🛒 Stavke Računa";
            // 
            // lblOprema
            // 
            this.lblOprema.AutoSize = true;
            this.lblOprema.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblOprema.ForeColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.lblOprema.Location = new System.Drawing.Point(20, 35);
            this.lblOprema.Name = "lblOprema";
            this.lblOprema.Size = new System.Drawing.Size(62, 19);
            this.lblOprema.TabIndex = 0;
            this.lblOprema.Text = "Oprema:";
            // 
            // cmbOprema
            // 
            this.cmbOprema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOprema.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbOprema.FormattingEnabled = true;
            this.cmbOprema.Location = new System.Drawing.Point(100, 32);
            this.cmbOprema.Name = "cmbOprema";
            this.cmbOprema.Size = new System.Drawing.Size(320, 25);
            this.cmbOprema.TabIndex = 1;
            // 
            // lblKolicina
            // 
            this.lblKolicina.AutoSize = true;
            this.lblKolicina.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblKolicina.ForeColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.lblKolicina.Location = new System.Drawing.Point(440, 35);
            this.lblKolicina.Name = "lblKolicina";
            this.lblKolicina.Size = new System.Drawing.Size(65, 19);
            this.lblKolicina.TabIndex = 2;
            this.lblKolicina.Text = "Količina:";
            // 
            // numKolicina
            // 
            this.numKolicina.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numKolicina.Location = new System.Drawing.Point(515, 33);
            this.numKolicina.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numKolicina.Name = "numKolicina";
            this.numKolicina.Size = new System.Drawing.Size(90, 25);
            this.numKolicina.TabIndex = 3;
            this.numKolicina.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // btnDodajStavku
            // 
            this.btnDodajStavku.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            this.btnDodajStavku.FlatAppearance.BorderSize = 0;
            this.btnDodajStavku.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDodajStavku.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDodajStavku.ForeColor = System.Drawing.Color.White;
            this.btnDodajStavku.Location = new System.Drawing.Point(625, 30);
            this.btnDodajStavku.Name = "btnDodajStavku";
            this.btnDodajStavku.Size = new System.Drawing.Size(110, 32);
            this.btnDodajStavku.TabIndex = 4;
            this.btnDodajStavku.Text = "➕ Dodaj";
            this.btnDodajStavku.UseVisualStyleBackColor = false;
            this.btnDodajStavku.Click += new System.EventHandler(this.btnDodajStavku_Click);
            // 
            // btnObrisiStavku
            // 
            this.btnObrisiStavku.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            this.btnObrisiStavku.FlatAppearance.BorderSize = 0;
            this.btnObrisiStavku.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnObrisiStavku.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnObrisiStavku.ForeColor = System.Drawing.Color.White;
            this.btnObrisiStavku.Location = new System.Drawing.Point(625, 315);
            this.btnObrisiStavku.Name = "btnObrisiStavku";
            this.btnObrisiStavku.Size = new System.Drawing.Size(110, 32);
            this.btnObrisiStavku.TabIndex = 6;
            this.btnObrisiStavku.Text = "🗑️ Obriši";
            this.btnObrisiStavku.UseVisualStyleBackColor = false;
            this.btnObrisiStavku.Click += new System.EventHandler(this.btnObrisiStavku_Click);
            // 
            // dgvStavke
            // 
            this.dgvStavke.AllowUserToAddRows = false;
            this.dgvStavke.AllowUserToDeleteRows = false;
            this.dgvStavke.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvStavke.BackgroundColor = System.Drawing.Color.White;
            this.dgvStavke.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvStavke.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStavke.Location = new System.Drawing.Point(20, 75);
            this.dgvStavke.Name = "dgvStavke";
            this.dgvStavke.ReadOnly = true;
            this.dgvStavke.RowHeadersVisible = false;
            this.dgvStavke.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStavke.Size = new System.Drawing.Size(585, 270);
            this.dgvStavke.TabIndex = 5;
            // 
            // panelUkupno
            // 
            this.panelUkupno.BackColor = System.Drawing.Color.White;
            this.panelUkupno.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelUkupno.Controls.Add(this.lblCenaStavki);
            this.panelUkupno.Controls.Add(this.lblPopust);
            this.panelUkupno.Controls.Add(this.lblCenaSaPopustom);
            this.panelUkupno.Controls.Add(this.lblPDV);
            this.panelUkupno.Controls.Add(this.lblKonacanIznos);
            this.panelUkupno.Location = new System.Drawing.Point(25, 505);
            this.panelUkupno.Name = "panelUkupno";
            this.panelUkupno.Size = new System.Drawing.Size(750, 130);
            this.panelUkupno.TabIndex = 6;
            // 
            // lblCenaStavki
            // 
            this.lblCenaStavki.AutoSize = true;
            this.lblCenaStavki.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblCenaStavki.ForeColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.lblCenaStavki.Location = new System.Drawing.Point(480, 12);
            this.lblCenaStavki.Name = "lblCenaStavki";
            this.lblCenaStavki.Size = new System.Drawing.Size(122, 20);
            this.lblCenaStavki.TabIndex = 0;
            this.lblCenaStavki.Text = "Cena stavki: 0.00";
            // 
            // lblPopust
            // 
            this.lblPopust.AutoSize = true;
            this.lblPopust.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblPopust.ForeColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.lblPopust.Location = new System.Drawing.Point(480, 37);
            this.lblPopust.Name = "lblPopust";
            this.lblPopust.Size = new System.Drawing.Size(119, 20);
            this.lblPopust.TabIndex = 1;
            this.lblPopust.Text = "Popust (0%): 0.00";
            // 
            // lblCenaSaPopustom
            // 
            this.lblCenaSaPopustom.AutoSize = true;
            this.lblCenaSaPopustom.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblCenaSaPopustom.ForeColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.lblCenaSaPopustom.Location = new System.Drawing.Point(480, 62);
            this.lblCenaSaPopustom.Name = "lblCenaSaPopustom";
            this.lblCenaSaPopustom.Size = new System.Drawing.Size(188, 20);
            this.lblCenaSaPopustom.TabIndex = 2;
            this.lblCenaSaPopustom.Text = "Cena sa popustom: 0.00";
            // 
            // lblPDV
            // 
            this.lblPDV.AutoSize = true;
            this.lblPDV.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblPDV.ForeColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.lblPDV.Location = new System.Drawing.Point(480, 87);
            this.lblPDV.Name = "lblPDV";
            this.lblPDV.Size = new System.Drawing.Size(118, 20);
            this.lblPDV.TabIndex = 3;
            this.lblPDV.Text = "PDV (20%): 0.00";
            // 
            // lblKonacanIznos
            // 
            this.lblKonacanIznos.AutoSize = true;
            this.lblKonacanIznos.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblKonacanIznos.ForeColor = System.Drawing.Color.FromArgb(39, 174, 96);
            this.lblKonacanIznos.Location = new System.Drawing.Point(475, 105);
            this.lblKonacanIznos.Name = "lblKonacanIznos";
            this.lblKonacanIznos.Size = new System.Drawing.Size(250, 25);
            this.lblKonacanIznos.TabIndex = 4;
            this.lblKonacanIznos.Text = "💰 KONAČAN IZNOS: 0.00";
            // 
            // btnSacuvaj
            // 
            this.btnSacuvaj.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            this.btnSacuvaj.FlatAppearance.BorderSize = 0;
            this.btnSacuvaj.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSacuvaj.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnSacuvaj.ForeColor = System.Drawing.Color.White;
            this.btnSacuvaj.Location = new System.Drawing.Point(530, 650);
            this.btnSacuvaj.Name = "btnSacuvaj";
            this.btnSacuvaj.Size = new System.Drawing.Size(120, 45);
            this.btnSacuvaj.TabIndex = 7;
            this.btnSacuvaj.Text = "✓ Sačuvaj";
            this.btnSacuvaj.UseVisualStyleBackColor = false;
            this.btnSacuvaj.Click += new System.EventHandler(this.btnSacuvaj_Click);
            // 
            // btnOtkazi
            // 
            this.btnOtkazi.BackColor = System.Drawing.Color.FromArgb(149, 165, 166);
            this.btnOtkazi.FlatAppearance.BorderSize = 0;
            this.btnOtkazi.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOtkazi.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnOtkazi.ForeColor = System.Drawing.Color.White;
            this.btnOtkazi.Location = new System.Drawing.Point(660, 650);
            this.btnOtkazi.Name = "btnOtkazi";
            this.btnOtkazi.Size = new System.Drawing.Size(115, 45);
            this.btnOtkazi.TabIndex = 8;
            this.btnOtkazi.Text = "✖ Otkaži";
            this.btnOtkazi.UseVisualStyleBackColor = false;
            this.btnOtkazi.Click += new System.EventHandler(this.btnOtkazi_Click);
            // 
            // AddRacun
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(236, 240, 241);
            this.ClientSize = new System.Drawing.Size(800, 710);
            this.Controls.Add(this.btnOtkazi);
            this.Controls.Add(this.btnSacuvaj);
            this.Controls.Add(this.panelUkupno);
            this.Controls.Add(this.groupBoxStavke);
            this.Controls.Add(this.dtpDatum);
            this.Controls.Add(this.lblDatum);
            this.Controls.Add(this.cmbKupac);
            this.Controls.Add(this.lblKupac);
            this.Controls.Add(this.panelHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "AddRacun";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "🧾 Kreiranje Računa";
            this.groupBoxStavke.ResumeLayout(false);
            this.groupBoxStavke.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numKolicina)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStavke)).EndInit();
            this.panelUkupno.ResumeLayout(false);
            this.panelUkupno.PerformLayout();
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblKupac;
        private System.Windows.Forms.ComboBox cmbKupac;
        private System.Windows.Forms.Label lblDatum;
        private System.Windows.Forms.DateTimePicker dtpDatum;
        private System.Windows.Forms.GroupBox groupBoxStavke;
        private System.Windows.Forms.Label lblOprema;
        private System.Windows.Forms.ComboBox cmbOprema;
        private System.Windows.Forms.Label lblKolicina;
        private System.Windows.Forms.NumericUpDown numKolicina;
        private System.Windows.Forms.Button btnDodajStavku;
        private System.Windows.Forms.Button btnObrisiStavku;
        private System.Windows.Forms.DataGridView dgvStavke;
        private System.Windows.Forms.Panel panelUkupno;
        private System.Windows.Forms.Label lblCenaStavki;
        private System.Windows.Forms.Label lblPopust;
        private System.Windows.Forms.Label lblCenaSaPopustom;
        private System.Windows.Forms.Label lblPDV;
        private System.Windows.Forms.Label lblKonacanIznos;
        private System.Windows.Forms.Button btnSacuvaj;
        private System.Windows.Forms.Button btnOtkazi;
    }
}
