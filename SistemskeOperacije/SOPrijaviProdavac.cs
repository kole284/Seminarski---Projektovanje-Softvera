using Models;
using Broker;
using System;

namespace SistemskeOperacije
{
    // Read-Only operacija - ne koristi transakcije
    public class SOPrijaviProdavac : SOBase
    {
        private Prodavac _input;
        public Prodavac Result { get; private set; }

        // Označi da je ovo read-only operacija
        protected override bool IsReadOnly => true;

        public SOPrijaviProdavac(Prodavac input)
        {
            _input = input;
        }

        protected override void Execute()
        {
            Result = (Prodavac)broker.VratiJedan(_input);
        }
    }
}
