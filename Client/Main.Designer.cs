namespace Client
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dgvData = new DataGridView();
            cmbEntitet = new ComboBox();
            btnPrikazi = new Button();
            btnDodaj = new Button();
            groupBoxFilter = new GroupBox();
            txtFilter1 = new TextBox();
            txtFilter2 = new TextBox();
            cmbFilter2 = new ComboBox();
            cmbProdavac = new ComboBox();
            cmbKupac = new ComboBox();
            lblFilter1 = new Label();
            lblFilter2 = new Label();
            lblProdavac = new Label();
            lblKupac = new Label();
            btnClearFilter = new Button();
            panelHeader = new Panel();
            lblWelcome = new Label();
            lblTitle = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvData).BeginInit();
            groupBoxFilter.SuspendLayout();
            panelHeader.SuspendLayout();
            SuspendLayout();
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.FromArgb(44, 62, 80);
            panelHeader.Controls.Add(lblWelcome);
            panelHeader.Controls.Add(lblTitle);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new Size(1024, 80);
            panelHeader.TabIndex = 5;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 15);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(450, 32);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "📊 Sistem za upravljanje prodavnicom";
            // 
            // lblWelcome
            // 
            lblWelcome.AutoSize = true;
            lblWelcome.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblWelcome.ForeColor = Color.FromArgb(189, 195, 199);
            lblWelcome.Location = new Point(20, 50);
            lblWelcome.Name = "lblWelcome";
            lblWelcome.Size = new Size(150, 19);
            lblWelcome.TabIndex = 1;
            lblWelcome.Text = "Dobrodošli";
            // 
            // dgvData
            // 
            dgvData.AllowUserToAddRows = false;
            dgvData.AllowUserToDeleteRows = false;
            dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvData.BackgroundColor = Color.White;
            dgvData.BorderStyle = BorderStyle.None;
            dgvData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvData.Location = new Point(20, 245);
            dgvData.MultiSelect = false;
            dgvData.Name = "dgvData";
            dgvData.ReadOnly = true;
            dgvData.RowHeadersVisible = false;
            dgvData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvData.Size = new Size(984, 380);
            dgvData.TabIndex = 0;
            // 
            // cmbEntitet
            // 
            cmbEntitet.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEntitet.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            cmbEntitet.FormattingEnabled = true;
            cmbEntitet.Location = new Point(20, 645);
            cmbEntitet.Name = "cmbEntitet";
            cmbEntitet.Size = new Size(220, 28);
            cmbEntitet.TabIndex = 1;
            cmbEntitet.SelectedIndexChanged += cmbEntitet_SelectedIndexChanged;
            // 
            // btnPrikazi
            // 
            btnPrikazi.BackColor = Color.FromArgb(52, 152, 219);
            btnPrikazi.FlatAppearance.BorderSize = 0;
            btnPrikazi.FlatStyle = FlatStyle.Flat;
            btnPrikazi.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnPrikazi.ForeColor = Color.White;
            btnPrikazi.Location = new Point(260, 641);
            btnPrikazi.Name = "btnPrikazi";
            btnPrikazi.Size = new Size(150, 38);
            btnPrikazi.TabIndex = 2;
            btnPrikazi.Text = "🔍 Prikaži";
            btnPrikazi.UseVisualStyleBackColor = false;
            btnPrikazi.Click += button1_Click;
            // 
            // btnDodaj
            // 
            btnDodaj.BackColor = Color.FromArgb(46, 204, 113);
            btnDodaj.FlatAppearance.BorderSize = 0;
            btnDodaj.FlatStyle = FlatStyle.Flat;
            btnDodaj.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnDodaj.ForeColor = Color.White;
            btnDodaj.Location = new Point(430, 641);
            btnDodaj.Name = "btnDodaj";
            btnDodaj.Size = new Size(160, 38);
            btnDodaj.TabIndex = 3;
            btnDodaj.Text = "➕ Dodaj novi";
            btnDodaj.UseVisualStyleBackColor = false;
            btnDodaj.Click += btnDodaj_Click;
            // 
            // groupBoxFilter
            // 
            groupBoxFilter.BackColor = Color.White;
            groupBoxFilter.Controls.Add(btnClearFilter);
            groupBoxFilter.Controls.Add(lblKupac);
            groupBoxFilter.Controls.Add(lblProdavac);
            groupBoxFilter.Controls.Add(lblFilter2);
            groupBoxFilter.Controls.Add(lblFilter1);
            groupBoxFilter.Controls.Add(cmbKupac);
            groupBoxFilter.Controls.Add(cmbProdavac);
            groupBoxFilter.Controls.Add(cmbFilter2);
            groupBoxFilter.Controls.Add(txtFilter2);
            groupBoxFilter.Controls.Add(txtFilter1);
            groupBoxFilter.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBoxFilter.ForeColor = Color.FromArgb(52, 73, 94);
            groupBoxFilter.Location = new Point(20, 100);
            groupBoxFilter.Name = "groupBoxFilter";
            groupBoxFilter.Size = new Size(984, 125);
            groupBoxFilter.TabIndex = 4;
            groupBoxFilter.TabStop = false;
            groupBoxFilter.Text = "🔍 Filteri za pretragu";
            // 
            // txtFilter1
            // 
            txtFilter1.Font = new Font("Segoe UI", 10F);
            txtFilter1.Location = new Point(20, 55);
            txtFilter1.Name = "txtFilter1";
            txtFilter1.Size = new Size(220, 25);
            txtFilter1.TabIndex = 0;
            // 
            // txtFilter2
            // 
            txtFilter2.Font = new Font("Segoe UI", 10F);
            txtFilter2.Location = new Point(260, 55);
            txtFilter2.Name = "txtFilter2";
            txtFilter2.Size = new Size(220, 25);
            txtFilter2.TabIndex = 1;
            // 
            // cmbFilter2
            // 
            cmbFilter2.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFilter2.Font = new Font("Segoe UI", 10F);
            cmbFilter2.FormattingEnabled = true;
            cmbFilter2.Location = new Point(260, 55);
            cmbFilter2.Name = "cmbFilter2";
            cmbFilter2.Size = new Size(220, 25);
            cmbFilter2.TabIndex = 2;
            // 
            // cmbProdavac
            // 
            cmbProdavac.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProdavac.Font = new Font("Segoe UI", 10F);
            cmbProdavac.FormattingEnabled = true;
            cmbProdavac.Location = new Point(500, 55);
            cmbProdavac.Name = "cmbProdavac";
            cmbProdavac.Size = new Size(220, 25);
            cmbProdavac.TabIndex = 5;
            // 
            // cmbKupac
            // 
            cmbKupac.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbKupac.Font = new Font("Segoe UI", 10F);
            cmbKupac.FormattingEnabled = true;
            cmbKupac.Location = new Point(740, 55);
            cmbKupac.Name = "cmbKupac";
            cmbKupac.Size = new Size(220, 25);
            cmbKupac.TabIndex = 6;
            // 
            // lblFilter1
            // 
            lblFilter1.AutoSize = true;
            lblFilter1.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblFilter1.ForeColor = Color.FromArgb(52, 73, 94);
            lblFilter1.Location = new Point(20, 32);
            lblFilter1.Name = "lblFilter1";
            lblFilter1.Size = new Size(50, 17);
            lblFilter1.TabIndex = 2;
            lblFilter1.Text = "Filter 1:";
            // 
            // lblFilter2
            // 
            lblFilter2.AutoSize = true;
            lblFilter2.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblFilter2.ForeColor = Color.FromArgb(52, 73, 94);
            lblFilter2.Location = new Point(260, 32);
            lblFilter2.Name = "lblFilter2";
            lblFilter2.Size = new Size(50, 17);
            lblFilter2.TabIndex = 3;
            lblFilter2.Text = "Filter 2:";
            // 
            // lblProdavac
            // 
            lblProdavac.AutoSize = true;
            lblProdavac.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblProdavac.ForeColor = Color.FromArgb(52, 73, 94);
            lblProdavac.Location = new Point(500, 32);
            lblProdavac.Name = "lblProdavac";
            lblProdavac.Size = new Size(65, 17);
            lblProdavac.TabIndex = 7;
            lblProdavac.Text = "Prodavac:";
            // 
            // lblKupac
            // 
            lblKupac.AutoSize = true;
            lblKupac.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblKupac.ForeColor = Color.FromArgb(52, 73, 94);
            lblKupac.Location = new Point(740, 32);
            lblKupac.Name = "lblKupac";
            lblKupac.Size = new Size(48, 17);
            lblKupac.TabIndex = 8;
            lblKupac.Text = "Kupac:";
            // 
            // btnClearFilter
            // 
            btnClearFilter.BackColor = Color.FromArgb(231, 76, 60);
            btnClearFilter.FlatAppearance.BorderSize = 0;
            btnClearFilter.FlatStyle = FlatStyle.Flat;
            btnClearFilter.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnClearFilter.ForeColor = Color.White;
            btnClearFilter.Location = new Point(20, 88);
            btnClearFilter.Name = "btnClearFilter";
            btnClearFilter.Size = new Size(150, 30);
            btnClearFilter.TabIndex = 4;
            btnClearFilter.Text = "✖ Obriši filtere";
            btnClearFilter.UseVisualStyleBackColor = false;
            btnClearFilter.Click += btnClearFilter_Click;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(236, 240, 241);
            ClientSize = new Size(1024, 700);
            Controls.Add(panelHeader);
            Controls.Add(groupBoxFilter);
            Controls.Add(btnDodaj);
            Controls.Add(btnPrikazi);
            Controls.Add(cmbEntitet);
            Controls.Add(dgvData);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Main";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "📊 Sistem za upravljanje prodavnicom";
            Load += Main_Load;
            ((System.ComponentModel.ISupportInitialize)dgvData).EndInit();
            groupBoxFilter.ResumeLayout(false);
            groupBoxFilter.PerformLayout();
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelHeader;
        private Label lblTitle;
        private Label lblWelcome;
        private DataGridView dgvData;
        private ComboBox cmbEntitet;
        private Button btnPrikazi;
        private Button btnDodaj;
        private GroupBox groupBoxFilter;
        private TextBox txtFilter1;
        private TextBox txtFilter2;
        private ComboBox cmbFilter2;
        private ComboBox cmbProdavac;
        private ComboBox cmbKupac;
        private Label lblFilter1;
        private Label lblFilter2;
        private Label lblProdavac;
        private Label lblKupac;
        private Button btnClearFilter;
    }
}