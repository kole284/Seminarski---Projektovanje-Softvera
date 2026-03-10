namespace Client
{
    partial class DetailsRacun
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
            this.lblIdRacun = new System.Windows.Forms.Label();
            this.lblDatum = new System.Windows.Forms.Label();
            this.lblProdavac = new System.Windows.Forms.Label();
            this.lblKupac = new System.Windows.Forms.Label();
            this.groupBoxStavke = new System.Windows.Forms.GroupBox();
            this.dgvStavke = new System.Windows.Forms.DataGridView();
            this.panelUkupno = new System.Windows.Forms.Panel();
            this.lblCenaStavki = new System.Windows.Forms.Label();
            this.lblCenaSaPopustom = new System.Windows.Forms.Label();
            this.lblPDV = new System.Windows.Forms.Label();
            this.lblKonacanIznos = new System.Windows.Forms.Label();
            this.btnZatvori = new System.Windows.Forms.Button();
            this.btnAzuriraj = new System.Windows.Forms.Button();
            this.btnObrisi = new System.Windows.Forms.Button();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.panelInfo = new System.Windows.Forms.Panel();
            this.groupBoxStavke.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStavke)).BeginInit();
            this.panelUkupno.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.panelInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(740, 70);
            this.panelHeader.TabIndex = 10;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 18);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(230, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "🧾 Detalji Računa";
            // 
            // panelInfo
            // 
            this.panelInfo.BackColor = System.Drawing.Color.White;
            this.panelInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelInfo.Controls.Add(this.lblIdRacun);
            this.panelInfo.Controls.Add(this.lblDatum);
            this.panelInfo.Controls.Add(this.lblProdavac);
            this.panelInfo.Controls.Add(this.lblKupac);
            this.panelInfo.Location = new System.Drawing.Point(25, 90);
            this.panelInfo.Name = "panelInfo";
            this.panelInfo.Size = new System.Drawing.Size(690, 100);
            this.panelInfo.TabIndex = 11;
            // 
            // lblIdRacun
            // 
            this.lblIdRacun.AutoSize = true;
            this.lblIdRacun.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblIdRacun.ForeColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.lblIdRacun.Location = new System.Drawing.Point(15, 12);
            this.lblIdRacun.Name = "lblIdRacun";
            this.lblIdRacun.Size = new System.Drawing.Size(107, 21);
            this.lblIdRacun.TabIndex = 1;
            this.lblIdRacun.Text = "Račun broj: 0";
            // 
            // lblDatum
            // 
            this.lblDatum.AutoSize = true;
            this.lblDatum.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDatum.ForeColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.lblDatum.Location = new System.Drawing.Point(15, 40);
            this.lblDatum.Name = "lblDatum";
            this.lblDatum.Size = new System.Drawing.Size(55, 19);
            this.lblDatum.TabIndex = 2;
            this.lblDatum.Text = "Datum:";
            // 
            // lblProdavac
            // 
            this.lblProdavac.AutoSize = true;
            this.lblProdavac.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblProdavac.ForeColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.lblProdavac.Location = new System.Drawing.Point(15, 62);
            this.lblProdavac.Name = "lblProdavac";
            this.lblProdavac.Size = new System.Drawing.Size(73, 19);
            this.lblProdavac.TabIndex = 3;
            this.lblProdavac.Text = "Prodavac:";
            // 
            // lblKupac
            // 
            this.lblKupac.AutoSize = true;
            this.lblKupac.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblKupac.ForeColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.lblKupac.Location = new System.Drawing.Point(350, 62);
            this.lblKupac.Name = "lblKupac";
            this.lblKupac.Size = new System.Drawing.Size(54, 19);
            this.lblKupac.TabIndex = 4;
            this.lblKupac.Text = "Kupac:";
            // 
            // groupBoxStavke
            // 
            this.groupBoxStavke.BackColor = System.Drawing.Color.White;
            this.groupBoxStavke.Controls.Add(this.dgvStavke);
            this.groupBoxStavke.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.groupBoxStavke.ForeColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.groupBoxStavke.Location = new System.Drawing.Point(25, 205);
            this.groupBoxStavke.Name = "groupBoxStavke";
            this.groupBoxStavke.Size = new System.Drawing.Size(690, 300);
            this.groupBoxStavke.TabIndex = 5;
            this.groupBoxStavke.TabStop = false;
            this.groupBoxStavke.Text = "🛒 Stavke Računa";
            // 
            // dgvStavke
            // 
            this.dgvStavke.AllowUserToAddRows = false;
            this.dgvStavke.AllowUserToDeleteRows = false;
            this.dgvStavke.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvStavke.BackgroundColor = System.Drawing.Color.White;
            this.dgvStavke.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvStavke.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStavke.Location = new System.Drawing.Point(20, 35);
            this.dgvStavke.Name = "dgvStavke";
            this.dgvStavke.ReadOnly = true;
            this.dgvStavke.RowHeadersVisible = false;
            this.dgvStavke.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStavke.Size = new System.Drawing.Size(650, 250);
            this.dgvStavke.TabIndex = 0;
            // 
            // panelUkupno
            // 
            this.panelUkupno.BackColor = System.Drawing.Color.White;
            this.panelUkupno.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelUkupno.Controls.Add(this.lblCenaStavki);
            this.panelUkupno.Controls.Add(this.lblCenaSaPopustom);
            this.panelUkupno.Controls.Add(this.lblPDV);
            this.panelUkupno.Controls.Add(this.lblKonacanIznos);
            this.panelUkupno.Location = new System.Drawing.Point(25, 520);
            this.panelUkupno.Name = "panelUkupno";
            this.panelUkupno.Size = new System.Drawing.Size(690, 110);
            this.panelUkupno.TabIndex = 6;
            // 
            // lblCenaStavki
            // 
            this.lblCenaStavki.AutoSize = true;
            this.lblCenaStavki.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblCenaStavki.ForeColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.lblCenaStavki.Location = new System.Drawing.Point(430, 12);
            this.lblCenaStavki.Name = "lblCenaStavki";
            this.lblCenaStavki.Size = new System.Drawing.Size(122, 20);
            this.lblCenaStavki.TabIndex = 0;
            this.lblCenaStavki.Text = "Cena stavki: 0.00";
            // 
            // lblCenaSaPopustom
            // 
            this.lblCenaSaPopustom.AutoSize = true;
            this.lblCenaSaPopustom.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblCenaSaPopustom.ForeColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.lblCenaSaPopustom.Location = new System.Drawing.Point(430, 37);
            this.lblCenaSaPopustom.Name = "lblCenaSaPopustom";
            this.lblCenaSaPopustom.Size = new System.Drawing.Size(188, 20);
            this.lblCenaSaPopustom.TabIndex = 1;
            this.lblCenaSaPopustom.Text = "Cena sa popustom: 0.00";
            // 
            // lblPDV
            // 
            this.lblPDV.AutoSize = true;
            this.lblPDV.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblPDV.ForeColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.lblPDV.Location = new System.Drawing.Point(430, 62);
            this.lblPDV.Name = "lblPDV";
            this.lblPDV.Size = new System.Drawing.Size(118, 20);
            this.lblPDV.TabIndex = 2;
            this.lblPDV.Text = "PDV (20%): 0.00";
            // 
            // lblKonacanIznos
            // 
            this.lblKonacanIznos.AutoSize = true;
            this.lblKonacanIznos.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblKonacanIznos.ForeColor = System.Drawing.Color.FromArgb(39, 174, 96);
            this.lblKonacanIznos.Location = new System.Drawing.Point(425, 82);
            this.lblKonacanIznos.Name = "lblKonacanIznos";
            this.lblKonacanIznos.Size = new System.Drawing.Size(250, 25);
            this.lblKonacanIznos.TabIndex = 3;
            this.lblKonacanIznos.Text = "💰 KONAČAN IZNOS: 0.00";
            // 
            // btnZatvori
            // 
            this.btnZatvori.BackColor = System.Drawing.Color.FromArgb(149, 165, 166);
            this.btnZatvori.FlatAppearance.BorderSize = 0;
            this.btnZatvori.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZatvori.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnZatvori.ForeColor = System.Drawing.Color.White;
            this.btnZatvori.Location = new System.Drawing.Point(585, 645);
            this.btnZatvori.Name = "btnZatvori";
            this.btnZatvori.Size = new System.Drawing.Size(130, 45);
            this.btnZatvori.TabIndex = 7;
            this.btnZatvori.Text = "✖ Zatvori";
            this.btnZatvori.UseVisualStyleBackColor = false;
            this.btnZatvori.Click += new System.EventHandler(this.btnZatvori_Click);
            // 
            // btnAzuriraj
            // 
            this.btnAzuriraj.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.btnAzuriraj.FlatAppearance.BorderSize = 0;
            this.btnAzuriraj.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAzuriraj.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnAzuriraj.ForeColor = System.Drawing.Color.White;
            this.btnAzuriraj.Location = new System.Drawing.Point(25, 645);
            this.btnAzuriraj.Name = "btnAzuriraj";
            this.btnAzuriraj.Size = new System.Drawing.Size(170, 45);
            this.btnAzuriraj.TabIndex = 8;
            this.btnAzuriraj.Text = "✏️ Ažuriraj";
            this.btnAzuriraj.UseVisualStyleBackColor = false;
            this.btnAzuriraj.Click += new System.EventHandler(this.btnAzuriraj_Click);
            // 
            // btnObrisi
            // 
            this.btnObrisi.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            this.btnObrisi.FlatAppearance.BorderSize = 0;
            this.btnObrisi.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnObrisi.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnObrisi.ForeColor = System.Drawing.Color.White;
            this.btnObrisi.Location = new System.Drawing.Point(210, 645);
            this.btnObrisi.Name = "btnObrisi";
            this.btnObrisi.Size = new System.Drawing.Size(170, 45);
            this.btnObrisi.TabIndex = 9;
            this.btnObrisi.Text = "🗑️ Obriši";
            this.btnObrisi.UseVisualStyleBackColor = false;
            this.btnObrisi.Click += new System.EventHandler(this.btnObrisi_Click);
            // 
            // DetailsRacun
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(236, 240, 241);
            this.ClientSize = new System.Drawing.Size(740, 710);
            this.Controls.Add(this.btnObrisi);
            this.Controls.Add(this.btnAzuriraj);
            this.Controls.Add(this.btnZatvori);
            this.Controls.Add(this.panelUkupno);
            this.Controls.Add(this.groupBoxStavke);
            this.Controls.Add(this.panelInfo);
            this.Controls.Add(this.panelHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "DetailsRacun";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "🧾 Detalji Računa";
            this.groupBoxStavke.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStavke)).EndInit();
            this.panelUkupno.ResumeLayout(false);
            this.panelUkupno.PerformLayout();
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelInfo.ResumeLayout(false);
            this.panelInfo.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Panel panelInfo;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblIdRacun;
        private System.Windows.Forms.Label lblDatum;
        private System.Windows.Forms.Label lblProdavac;
        private System.Windows.Forms.Label lblKupac;
        private System.Windows.Forms.GroupBox groupBoxStavke;
        private System.Windows.Forms.DataGridView dgvStavke;
        private System.Windows.Forms.Panel panelUkupno;
        private System.Windows.Forms.Label lblCenaStavki;
        private System.Windows.Forms.Label lblCenaSaPopustom;
        private System.Windows.Forms.Label lblPDV;
        private System.Windows.Forms.Label lblKonacanIznos;
        private System.Windows.Forms.Button btnZatvori;
        private System.Windows.Forms.Button btnAzuriraj;
        private System.Windows.Forms.Button btnObrisi;
    }
}
