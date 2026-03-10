using Models;

namespace Client
{
    public static class Sesija
    {
        public static Prodavac UlogovaniProdavac { get; set; }
        public static int ProdavacId => UlogovaniProdavac?.IdProdavac ?? 0;
        public static bool IsAdmin { get; set; } = false;
    }
}