using Models;
using Broker;
using System.Collections.Generic;

namespace SistemskeOperacije
{
    // Read-Only operacija - ne koristi transakcije
    public class SOVratiSve : SOBase
    {
        private IDomainObject _input;
        public List<IDomainObject> Result { get; private set; }

        // Označi da je ovo read-only operacija
        protected override bool IsReadOnly => true;

        public SOVratiSve(IDomainObject input)
        {
            _input = input;
        }

        protected override void Execute()
        {
            Result = broker.VratiSve(_input);

            // Ako vraćamo listu Racun-a, dopuni svaku stavku sa pripadajućim StavkaRacuna
            if (_input is Models.Racun)
            {
                var racuni = Result.ConvertAll(r => (Models.Racun)r);
                for (int i = 0; i < racuni.Count; i++)
                {
                    var racun = racuni[i];
                    var stavke = broker.VratiSve(new Models.StavkaRacuna { IdRacun = racun.IdRacun });
                    racun.Stavke = stavke.ConvertAll(s => (Models.StavkaRacuna)s);
                    // update the list entry
                    Result[i] = racun;
                }
            }
        }
    }
}
