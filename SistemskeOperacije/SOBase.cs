using Broker;
using System;

namespace SistemskeOperacije
{
    public abstract class SOBase
    {
        protected DatabaseBroker broker;

        // Flag koji označava da li operacija menja podatke (INSERT/UPDATE/DELETE)
        // ili samo čita (SELECT). Read-only operacije NE koriste transakcije.
        protected virtual bool IsReadOnly => false;

        public SOBase()
        {
            broker = new DatabaseBroker();
        }

        // Template Method Pattern - definisan tok izvršavanja operacije
        public void ExecuteTemplate()
        {
            try
            {
                broker.OpenConnection();

                // Samo write operacije koriste transakcije
                if (!IsReadOnly)
                    broker.BeginTransaction();

                Execute();

                if (!IsReadOnly)
                    broker.Commit();
            }
            catch (Exception)
            {
                if (!IsReadOnly)
                {
                    try
                    {
                        broker.Rollback();
                    }
                    catch
                    {
                        // Ignoriši grešku rollback-a (već završena transakcija)
                    }
                }
                throw;
            }
            finally
            {
                broker.CloseConnection();
            }
        }

        // Abstraktna metoda koju svaka SO mora implementirati
        // Ovo je "hook" metoda iz Template Method paterna
        protected abstract void Execute();
    }
}
