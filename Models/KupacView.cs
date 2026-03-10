using System;
using System.ComponentModel;

namespace Models
{
    [Serializable]
    public class KupacView
    {
        public int IdKupac { get; set; }
        public double Popust { get; set; }
        public string Tip { get; set; } // "Firma" ili "Fizičko lice"
        public string Naziv { get; set; }
        
        // Firma polja
        public string Pib { get; set; }
        public string Adresa { get; set; }
        
        [Browsable(false)]
        public bool? Partnerstvo { get; set; }
        
        public string PartnerstvoPrikaz => Partnerstvo.HasValue ? (Partnerstvo.Value ? "Da" : "Ne") : "";
        
        // Fizičko lice polja
        public string ImePrezime { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        
        [Browsable(false)]
        public bool? LoyaltyClan { get; set; }
        
        public string LoyaltyClanPrikaz => LoyaltyClan.HasValue ? (LoyaltyClan.Value ? "Da" : "Ne") : "";
    }
}
