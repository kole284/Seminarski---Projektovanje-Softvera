namespace Client
{
    partial class Details
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
            panelDetails = new Panel();
            btnAzuriraj = new Button();
            btnObrisi = new Button();
            btnPromeniSifru = new Button();
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
            panelHeader.Size = new Size(840, 70);
            panelHeader.TabIndex = 5;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 18);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(200, 32);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "📋 Detalji zapisa";
            // 
            // panelDetails
            // 
            panelDetails.AutoScroll = true;
            panelDetails.BackColor = Color.White;
            panelDetails.BorderStyle = BorderStyle.None;
            panelDetails.Location = new Point(20, 90);
            panelDetails.Name = "panelDetails";
            panelDetails.Size = new Size(800, 360);
            panelDetails.TabIndex = 1;
            // 
            // btnAzuriraj
            // 
            btnAzuriraj.BackColor = Color.FromArgb(52, 152, 219);
            btnAzuriraj.FlatAppearance.BorderSize = 0;
            btnAzuriraj.FlatStyle = FlatStyle.Flat;
            btnAzuriraj.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnAzuriraj.ForeColor = Color.White;
            btnAzuriraj.Location = new Point(20, 465);
            btnAzuriraj.Name = "btnAzuriraj";
            btnAzuriraj.Size = new Size(180, 45);
            btnAzuriraj.TabIndex = 2;
            btnAzuriraj.Text = "✏️ Ažuriraj";
            btnAzuriraj.UseVisualStyleBackColor = false;
            btnAzuriraj.Click += btnAzuriraj_Click;
            // 
            // btnObrisi
            // 
            btnObrisi.BackColor = Color.FromArgb(231, 76, 60);
            btnObrisi.FlatAppearance.BorderSize = 0;
            btnObrisi.FlatStyle = FlatStyle.Flat;
            btnObrisi.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnObrisi.ForeColor = Color.White;
            btnObrisi.Location = new Point(215, 465);
            btnObrisi.Name = "btnObrisi";
            btnObrisi.Size = new Size(180, 45);
            btnObrisi.TabIndex = 3;
            btnObrisi.Text = "🗑️ Obriši";
            btnObrisi.UseVisualStyleBackColor = false;
            btnObrisi.Click += btnObrisi_Click;
            // 
            // btnPromeniSifru
            // 
            btnPromeniSifru.BackColor = Color.FromArgb(230, 126, 34);
            btnPromeniSifru.FlatAppearance.BorderSize = 0;
            btnPromeniSifru.FlatStyle = FlatStyle.Flat;
            btnPromeniSifru.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnPromeniSifru.ForeColor = Color.White;
            btnPromeniSifru.Location = new Point(410, 465);
            btnPromeniSifru.Name = "btnPromeniSifru";
            btnPromeniSifru.Size = new Size(180, 45);
            btnPromeniSifru.TabIndex = 4;
            btnPromeniSifru.Text = "🔑 Promeni šifru";
            btnPromeniSifru.UseVisualStyleBackColor = false;
            btnPromeniSifru.Visible = false;
            btnPromeniSifru.Click += btnPromeniSifru_Click;
            // 
            // Details
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(236, 240, 241);
            ClientSize = new Size(840, 530);
            Controls.Add(btnPromeniSifru);
            Controls.Add(btnObrisi);
            Controls.Add(btnAzuriraj);
            Controls.Add(panelDetails);
            Controls.Add(panelHeader);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "Details";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "📋 Detalji";
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelHeader;
        private Label lblTitle;
        private Panel panelDetails;
        private Button btnAzuriraj;
        private Button btnObrisi;
        private Button btnPromeniSifru;
    }
}