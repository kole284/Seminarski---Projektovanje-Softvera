using System;
using System.Collections.Generic;

namespace Models
{
    public class ProdavacView
    {
        public int IdProdavac { get; set; }
        public string ImePrezime { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string Skladista { get; set; } // Lista skladišta odvojena zarezima
    }
}
