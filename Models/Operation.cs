using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public enum Operation
    {
        PrijaviProdavac,      // SK16 - Prijavi prodavca (LOGIN)
        VratiSve,             // SK2, SK5 - Pretraži sve (READ ALL)
        VratiJedan,           // SK2, SK5 - Pretraži jedan po ID-u (READ ONE)
        DodajObjekat,         // SK1, SK4, SK26 - Kreiranje (CREATE)
        AzurirajObjekat,      // SK3, SK6 - Promena (UPDATE)
        ObrisiObjekat         // SK7 - Brisanje (DELETE)
    }
}
