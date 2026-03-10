using Models;
using Broker;

namespace SistemskeOperacije
{
    // Write operacija - koristi transakcije (DELETE)
    public class SOObrisiObjekat : SOBase
    {
        private IDomainObject _input;
        public bool Success { get; private set; }

        public SOObrisiObjekat(IDomainObject input)
        {
            _input = input;
        }

        protected override void Execute()
        {
            Success = broker.Obrisi(_input);
        }
    }
}
