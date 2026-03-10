using System.ComponentModel;

namespace Models
{
    public enum KategorijaOpreme
    {
        [Description("Laptopovi")]
        Laptopovi,
        
        [Description("Desktop računari")]
        DesktopRacunari,
        
        [Description("Monitori")]
        Monitori,
        
        [Description("Tastature")]
        Tastature,
        
        [Description("Miševi")]
        Misevi,
        
        [Description("Slušalice")]
        Slusalice,
        
        [Description("Kamere")]
        Kamere,
        
        [Description("Printeri")]
        Printeri,
        
        [Description("Skeneri")]
        Skeneri,
        
        [Description("Mrežna oprema")]
        MreznaOprema,
        
        [Description("Eksterni hard diskovi")]
        EksterniHardDiskovi,
        
        [Description("USB fleš diskovi")]
        USBFlesDiskovi,
        
        [Description("Grafičke kartice")]
        GrafickeKartice,
        
        [Description("RAM memorija")]
        RAMMemorija,
        
        [Description("Procesori")]
        Procesori,
        
        [Description("Matičneploče")]
        MaticneOploce,
        
        [Description("Napajanja")]
        Napajanja,
        
        [Description("Kućišta")]
        Kucista,
        
        [Description("Ostalo")]
        Ostalo
    }
    
    public static class KategorijaOpremeExtensions
    {
        public static string GetDescription(this KategorijaOpreme kategorija)
        {
            var field = kategorija.GetType().GetField(kategorija.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute?.Description ?? kategorija.ToString();
        }
        
        public static KategorijaOpreme FromString(string value)
        {
            foreach (KategorijaOpreme kategorija in Enum.GetValues(typeof(KategorijaOpreme)))
            {
                if (kategorija.GetDescription().Equals(value, StringComparison.OrdinalIgnoreCase) ||
                    kategorija.ToString().Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    return kategorija;
                }
            }
            return KategorijaOpreme.Ostalo;
        }
    }
}
