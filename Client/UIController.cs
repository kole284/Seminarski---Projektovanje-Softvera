using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Models;

namespace Client
{
    // Kontroler korisničkog interfejsa - upravlja svim formama i kontrolama pristupa
    public static class UIController
    {
        // Provera da li je trenutni korisnik administrator
        public static bool IsAdmin()
        {
            return Sesija.IsAdmin;
        }

        // Provera da li trenutni korisnik ima pravo da dodaje račune
        public static bool MoguceDodatiRacun()
        {
            if (IsAdmin())
            {
                MessageBox.Show("Administratori ne smeju da dodaju račune!", 
                               "Pristup zabranjen", 
                               MessageBoxButtons.OK, 
                               MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        // Provera da li trenutni korisnik ima pravo da modifikuje račune
        public static bool MoguceModifikovatiRacun()
        {
            if (IsAdmin())
            {
                MessageBox.Show("Administratori ne smeju da modifikuju račune!", 
                               "Pristup zabranjen", 
                               MessageBoxButtons.OK, 
                               MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        // Otvori formu za dodavanje računa (sa kontrolom pristupa)
        public static void OtvoriDodajRacun()
        {
            if (!MoguceDodatiRacun())
                return;

            AddRacun forma = new AddRacun();
            forma.ShowDialog();
        }

        // Otvori formu za modifikaciju računa (sa kontrolom pristupa)
        public static void OtvoriModifikujRacun(Racun racun)
        {
            if (!MoguceModifikovatiRacun())
                return;

            AddRacun forma = new AddRacun(racun);
            forma.ShowDialog();
        }

        // Otvori formu za detalje računa (samo čitanje)
        public static void OtvoriDetaljeRacuna(Racun racun)
        {
            DetailsRacun forma = new DetailsRacun(racun);
            forma.ShowDialog();
        }

        // Otvori formu za dodavanje prodavca
        public static void OtvoriDodajProdavca()
        {
            Add forma = new Add();
            forma.ShowDialog();
        }

        // Otvori formu za modifikaciju prodavca
        public static void OtvoriModifikujProdavca(Prodavac prodavac)
        {
            Add forma = new Add(prodavac);
            forma.ShowDialog();
        }

        // Otvori formu za detalje prodavca
        public static void OtvoriDetaljeProdavca(Prodavac prodavac)
        {
            Details forma = new Details(prodavac);
            forma.ShowDialog();
        }

        // Provera da li trenutni korisnik ima administratorska prava
        public static void ProveraAdminPristupa(Action akcija, string nazivOperacije)
        {
            if (!IsAdmin())
            {
                MessageBox.Show($"Nemate pravo da izvršite operaciju: {nazivOperacije}", 
                               "Pristup zabranjen", 
                               MessageBoxButtons.OK, 
                               MessageBoxIcon.Warning);
                return;
            }

            akcija.Invoke();
        }

        // Poruka dobrodošlice za ulogovanog korisnika
        public static string PorukaDobroDosli()
        {
            if (Sesija.UlogovaniProdavac != null)
            {
                string rola = IsAdmin() ? "Administrator" : "Prodavac";
                return $"Dobrodošli, {Sesija.UlogovaniProdavac.ImePrezime} ({rola})";
            }
            return "Dobrodošli";
        }
    }
}
