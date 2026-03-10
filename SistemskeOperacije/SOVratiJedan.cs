using Models;
using Broker;

namespace SistemskeOperacije
{
    // Read-Only operacija - ne koristi transakcije
    public class SOVratiJedan : SOBase
    {
        private IDomainObject _input;
        public IDomainObject Result { get; private set; }

        // Označi da je ovo read-only operacija
        protected override bool IsReadOnly => true;

        public SOVratiJedan(IDomainObject input)
        {
            _input = input;
        }

        protected override void Execute()
        {
            Result = broker.VratiJedan(_input);

            // Ako je vraćen Racun, dopuni ga sa pripadajućim stavkama
            if (Result is Models.Racun racun)
            {
                var stavke = broker.VratiSve(new Models.StavkaRacuna { IdRacun = racun.IdRacun });
                // Konvertuj List<IDomainObject> u List<StavkaRacuna>
                racun.Stavke = stavke.ConvertAll(s => (Models.StavkaRacuna)s);
            }
        }
    }
}
