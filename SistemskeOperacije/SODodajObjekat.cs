using Models;
using Broker;

namespace SistemskeOperacije
{
    // Write operacija - koristi transakcije (INSERT)
    public class SODodajObjekat : SOBase
    {
        private IDomainObject _input;
        public bool Success { get; private set; }
        public IDomainObject Result { get; private set; }

        public SODodajObjekat(IDomainObject input)
        {
            _input = input;
        }

        protected override void Execute()
        {
            Success = broker.Dodaj(_input);
            if (Success)
            {
                // broker.Dodaj should populate identity fields on the _input object
                Result = _input;
            }
        }
    }
}
