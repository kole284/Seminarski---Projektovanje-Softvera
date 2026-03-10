using Models;
using Broker;

namespace SistemskeOperacije
{
    // Write operacija - koristi transakcije (UPDATE)
    public class SOAzurirajObjekat : SOBase
    {
        private IDomainObject _input;
        public bool Success { get; private set; }

        public SOAzurirajObjekat(IDomainObject input)
        {
            _input = input;
        }

        protected override void Execute()
        {
            Success = broker.Azuriraj(_input);
        }
    }
}
