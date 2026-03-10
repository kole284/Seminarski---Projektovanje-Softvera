namespace Client
{
    partial class Add
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
            lblTitle = new Label();
            lblTipObjekta = new Label();
            cmbTipObjekta = new ComboBox();
            panelInputs = new Panel();
            btnSacuvaj = new Button();
            btnOtkazi = new Button();
            panelHeader = new Panel();
            panelHeader.SuspendLayout();
            SuspendLayout();
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.FromArgb(41, 128, 185);
            panelHeader.Controls.Add(lblTitle);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new Size(950, 70);
            panelHeader.TabIndex = 6;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 18);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(240, 32);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "➕ Dodaj novi zapis";
            // 
            // lblTipObjekta
            // 
            lblTipObjekta.AutoSize = true;
            lblTipObjekta.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            lblTipObjekta.ForeColor = Color.FromArgb(52, 73, 94);
            lblTipObjekta.Location = new Point(25, 90);
            lblTipObjekta.Name = "lblTipObjekta";
            lblTipObjekta.Size = new Size(92, 20);
            lblTipObjekta.TabIndex = 1;
            lblTipObjekta.Text = "Tip objekta:";
            // 
            // cmbTipObjekta
            // 
            cmbTipObjekta.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTipObjekta.Font = new Font("Segoe UI", 11F);
            cmbTipObjekta.FormattingEnabled = true;
            cmbTipObjekta.Location = new Point(125, 87);
            cmbTipObjekta.Name = "cmbTipObjekta";
            cmbTipObjekta.Size = new Size(300, 28);
            cmbTipObjekta.TabIndex = 2;
            cmbTipObjekta.SelectedIndexChanged += cmbTipObjekta_SelectedIndexChanged;
            // 
            // panelInputs
            // 
            panelInputs.AutoScroll = true;
            panelInputs.BackColor = Color.White;
            panelInputs.BorderStyle = BorderStyle.FixedSingle;
            panelInputs.Location = new Point(25, 130);
            panelInputs.Margin = new Padding(3, 4, 3, 4);
            panelInputs.Name = "panelInputs";
            panelInputs.Size = new Size(900, 420);
            panelInputs.TabIndex = 3;
            // 
            // btnSacuvaj
            // 
            btnSacuvaj.BackColor = Color.FromArgb(46, 204, 113);
            btnSacuvaj.FlatAppearance.BorderSize = 0;
            btnSacuvaj.FlatStyle = FlatStyle.Flat;
            btnSacuvaj.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnSacuvaj.ForeColor = Color.White;
            btnSacuvaj.Location = new Point(675, 565);
            btnSacuvaj.Margin = new Padding(3, 4, 3, 4);
            btnSacuvaj.Name = "btnSacuvaj";
            btnSacuvaj.Size = new Size(120, 45);
            btnSacuvaj.TabIndex = 4;
            btnSacuvaj.Text = "✓ Sačuvaj";
            btnSacuvaj.UseVisualStyleBackColor = false;
            btnSacuvaj.Click += btnSacuvaj_Click;
            // 
            // btnOtkazi
            // 
            btnOtkazi.BackColor = Color.FromArgb(149, 165, 166);
            btnOtkazi.FlatAppearance.BorderSize = 0;
            btnOtkazi.FlatStyle = FlatStyle.Flat;
            btnOtkazi.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnOtkazi.ForeColor = Color.White;
            btnOtkazi.Location = new Point(805, 565);
            btnOtkazi.Margin = new Padding(3, 4, 3, 4);
            btnOtkazi.Name = "btnOtkazi";
            btnOtkazi.Size = new Size(120, 45);
            btnOtkazi.TabIndex = 5;
            btnOtkazi.Text = "✖ Otkaži";
            btnOtkazi.UseVisualStyleBackColor = false;
            btnOtkazi.Click += btnOtkazi_Click;
            // 
            // Add
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(236, 240, 241);
            ClientSize = new Size(950, 630);
            Controls.Add(btnOtkazi);
            Controls.Add(btnSacuvaj);
            Controls.Add(panelInputs);
            Controls.Add(cmbTipObjekta);
            Controls.Add(lblTipObjekta);
            Controls.Add(panelHeader);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            Name = "Add";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "➕ Dodaj novi";
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panelHeader;
        private Label lblTitle;
        private Label lblTipObjekta;
        private ComboBox cmbTipObjekta;
        private Panel panelInputs;
        private Button btnSacuvaj;
        private Button btnOtkazi;
    }
}