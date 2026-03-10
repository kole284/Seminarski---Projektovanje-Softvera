using Client.Client;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace Client
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            ApplyModernStyling();

            // Hover efekat za login dugme
            btnLogin.MouseEnter += (s, ev) => btnLogin.BackColor = Color.FromArgb(39, 174, 96);
            btnLogin.MouseLeave += (s, ev) => btnLogin.BackColor = Color.FromArgb(46, 204, 113);

            // Enter key za brži login
            txtPassword.KeyPress += (s, ev) => {
                if (ev.KeyChar == (char)Keys.Enter) {
                    button1_Click(btnLogin, EventArgs.Empty);
                    ev.Handled = true;
                }
            };

            txtEmail.KeyPress += (s, ev) => {
                if (ev.KeyChar == (char)Keys.Enter) {
                    txtPassword.Focus();
                    ev.Handled = true;
                }
            };
        }

        private void ApplyModernStyling()
        {
            // Dodaj padding za textbox-ove sa custom paint event
            txtEmail.Paint += PaintTextBoxBorder;
            txtPassword.Paint += PaintTextBoxBorder;

            // Dodaj focus efekte
            txtEmail.Enter += (s, e) => txtEmail.BackColor = Color.FromArgb(245, 250, 255);
            txtEmail.Leave += (s, e) => txtEmail.BackColor = Color.White;

            txtPassword.Enter += (s, e) => txtPassword.BackColor = Color.FromArgb(245, 250, 255);
            txtPassword.Leave += (s, e) => txtPassword.BackColor = Color.White;

            // Zaobljeni ivice na dugmetu (koristeći Region)
            btnLogin.Region = CreateRoundedRegion(btnLogin.Width, btnLogin.Height, 10);
        }

        private void PaintTextBoxBorder(object sender, PaintEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt != null)
            {
                // Iscrtaj border oko textbox-a
                using (Pen pen = new Pen(Color.FromArgb(189, 195, 199), 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, txt.Width - 1, txt.Height - 1);
                }
            }
        }

        private System.Drawing.Region CreateRoundedRegion(int width, int height, int radius)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(width - radius, 0, radius, radius, 270, 90);
            path.AddArc(width - radius, height - radius, radius, radius, 0, 90);
            path.AddArc(0, height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return new System.Drawing.Region(path);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Proveri admin pristup
                if (txtEmail.Text.Trim().ToLower() == "admin" && txtPassword.Text == "admin")
                {
                    Sesija.IsAdmin = true;
                    Sesija.UlogovaniProdavac = new Prodavac
                    {
                        IdProdavac = 0,
                        ImePrezime = "Administrator",
                        Email = "admin",
                        Password = "admin"
                    };

                    MessageBox.Show("Uspešna prijava! Dobrodošli, Administrator");

                    Main frmMain = new Main();
                    frmMain.ShowDialog();
                    return;
                }

                // 1. Pakujemo podatke iz polja u objekat
                Prodavac p = new Prodavac
                {
                    Email = txtEmail.Text,
                    Password = txtPassword.Text
                };

                // 2. Kreiramo zahtev
                Request req = new Request
                {
                    Operation = Operation.PrijaviProdavac,
                    Data = p
                };

                // 3. Šaljemo preko CommunicationHelper-a
                Response res = CommunicationHelper.Instance.SendRequest(req);

                if (res.IsSuccessful)
                {
                    // Moramo deserijalizovati Data jer server vraća JsonElement
                    Sesija.UlogovaniProdavac = JsonSerializer.Deserialize<Prodavac>(res.Data.ToString());
                    Sesija.IsAdmin = false;

                    MessageBox.Show($"Uspešna prijava! Dobrodošli, {Sesija.UlogovaniProdavac.ImePrezime}");

                    Main frmMain=new Main();
                    frmMain.ShowDialog();
                }
                else
                {
                    MessageBox.Show(res.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri komunikaciji sa serverom: " + ex.Message);
            }
        }
    }
}
