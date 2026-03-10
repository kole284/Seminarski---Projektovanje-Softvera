using Microsoft.Data.SqlClient;
using Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Broker
{
    /// <summary>
    /// DatabaseBroker - posrednik za komunikaciju sa bazom podataka
    /// Koristi se od strane Sistemskih Operacija (SO) za izvršavanje SQL upita
    /// </summary>
    public class DatabaseBroker
    {
        private SqlTransaction? transaction;
        private SqlConnection connection;
        private string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ProdavnicaDB;Integrated Security=True;";

        public DatabaseBroker()
        {
            connection = new SqlConnection(connString);
            transaction = null;
        }

        #region Connection Management

        public void OpenConnection()
        {
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();
        }

        public void CloseConnection()
        {
            if (connection.State != System.Data.ConnectionState.Closed)
                connection.Close();
        }

        #endregion

        #region Transaction Management

        public void BeginTransaction()
        {
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();
            transaction = connection.BeginTransaction();
        }

        public void Commit()
        {
            transaction?.Commit();
            transaction = null;
        }

        public void Rollback()
        {
            if (transaction != null && connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (InvalidOperationException)
                {
                    // Transakcija je već završena ili nije validna za rollback
                }
                catch (Exception)
                {
                    // Logujte ili ignorišite druge greške
                }
                finally
                {
                    transaction.Dispose();
                    transaction = null;
                }
            }
            else
            {
                transaction = null;
            }
        }

        #endregion

        public SqlCommand CreateCommand()
        {
            return new SqlCommand("", connection, transaction);
        }

        #region Read Operations (SELECT)

        /// <summary>
        /// Vraća jedan objekat iz baze na osnovu WHERE uslova
        /// </summary>
        public IDomainObject VratiJedan(IDomainObject obj)
        {
            string query = obj.GetCustomSelectQuery();

            if (!string.IsNullOrEmpty(query))
            {
                string queryUpper = query.ToUpper();
                int lastFromIndex = queryUpper.LastIndexOf(" FROM ");
                bool hasMainWhere = lastFromIndex > 0 && queryUpper.IndexOf(" WHERE ", lastFromIndex) > lastFromIndex;

                if (!hasMainWhere && !string.IsNullOrWhiteSpace(obj.WhereCondition))
                {
                    query += " WHERE " + obj.WhereCondition;
                }
            }
            else
            {
                query = $"SELECT * FROM {obj.TableName} WHERE {obj.WhereCondition}";
            }

            SqlCommand cmd = new SqlCommand(query, connection, transaction);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                List<IDomainObject> lista = obj.GetList(reader);
                return lista.Count > 0 ? lista[0] : null;
            }
        }

        /// <summary>
        /// Vraća sve objekte iz baze (sa opcionalnim WHERE uslovom za filtriranje)
        /// </summary>
        public List<IDomainObject> VratiSve(IDomainObject obj)
        {
            List<IDomainObject> rezultati = new List<IDomainObject>();
            string query = obj.GetCustomSelectQuery();

            if (!string.IsNullOrEmpty(query))
            {
                string queryUpper = query.ToUpper();
                int lastFromIndex = queryUpper.LastIndexOf(" FROM ");
                bool hasMainWhere = lastFromIndex > 0 && queryUpper.IndexOf(" WHERE ", lastFromIndex) > lastFromIndex;

                if (!hasMainWhere && !string.IsNullOrWhiteSpace(obj.WhereCondition))
                {
                    query += " WHERE " + obj.WhereCondition;
                }
            }
            else
            {
                query = $"SELECT * FROM {obj.TableName}";

                if (!string.IsNullOrWhiteSpace(obj.WhereCondition))
                {
                    query += " WHERE " + obj.WhereCondition;
                }
            }

            SqlCommand cmd = new SqlCommand(query, connection, transaction);

            // Log SQL query for debugging
            try
            {
                Console.WriteLine($"[SQL] VratiSve query for {obj.TableName}: {query}");
            }
            catch { }

            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    rezultati = obj.GetList(reader);
                }
                try
                {
                    Console.WriteLine($"[SQL] VratiSve returned {rezultati.Count} rows for {obj.TableName}");
                }
                catch { }
            }
            catch (Exception ex)
            {
                try { Console.WriteLine($"[SQL ERROR] VratiSve for {obj.TableName}: {ex.Message}"); } catch { }
                throw;
            }
            return rezultati;
        }

        #endregion

        #region Write Operations (INSERT, UPDATE, DELETE)

        /// <summary>
        /// Dodaje objekat u bazu (INSERT)
        /// Koristi transakciju koju upravlja SOBase
        /// </summary>
        public bool Dodaj(IDomainObject obj)
        {
            if (obj.ExecuteCustomInsert(connection, transaction))
            {
                return true;
            }

            string query = $"INSERT INTO {obj.TableName} VALUES ({obj.InsertValues})";
            SqlCommand cmd = new SqlCommand(query, connection, transaction);

            int affectedRows = cmd.ExecuteNonQuery();
            return affectedRows > 0;
        }

        /// <summary>
        /// Ažurira objekat u bazi (UPDATE)
        /// Koristi transakciju koju upravlja SOBase
        /// </summary>
        public bool Azuriraj(IDomainObject obj)
        {
            if (obj.ExecuteCustomUpdate(connection, transaction))
            {
                return true;
            }

            // Some domain objects build WHERE conditions using table aliases
            // (e.g. "p.idProdavac = 1") which are valid for SELECT queries
            // that include the alias, but cause "could not be bound" errors
            // for plain UPDATE/DELETE statements. Strip single-letter aliases
            // like "p.", "f.", "r." from the WHERE condition here.
            string where = obj.WhereCondition ?? "";
            if (!string.IsNullOrWhiteSpace(where))
            {
                where = Regex.Replace(where, "\\b[a-zA-Z]\\.", "");
            }

            string query = $"UPDATE {obj.TableName} SET {obj.UpdateValues} WHERE {where}";
            SqlCommand cmd = new SqlCommand(query, connection, transaction);

            int affectedRows = cmd.ExecuteNonQuery();
            return affectedRows > 0;
        }

        /// <summary>
        /// Briše objekat iz baze (DELETE)
        /// Koristi transakciju koju upravlja SOBase
        /// </summary>
        public bool Obrisi(IDomainObject obj)
        {
            Console.WriteLine($"[DEBUG DatabaseBroker.Obrisi] Pokušaj brisanja: Tip={obj.GetType().Name}, TableName={obj.TableName}, WhereCondition={obj.WhereCondition}");

            // connection.Open() i connection.Close() nisu potrebni, SOBase upravlja konekcijom
            if (obj.ExecuteCustomDelete(connection, transaction))
            {
                Console.WriteLine($"[DEBUG DatabaseBroker.Obrisi] ExecuteCustomDelete vratio TRUE za {obj.GetType().Name}");
                return true;
            }

            Console.WriteLine($"[DEBUG DatabaseBroker.Obrisi] ExecuteCustomDelete vratio FALSE, izvršavam standardni DELETE za {obj.GetType().Name}");

            string where = obj.WhereCondition ?? "";
            if (!string.IsNullOrWhiteSpace(where))
            {
                where = Regex.Replace(where, "\\b[a-zA-Z]\\.", "");
            }

            string query = $"DELETE FROM {obj.TableName} WHERE {where}";
            SqlCommand cmd = new SqlCommand(query, connection, transaction);

            int affectedRows = cmd.ExecuteNonQuery();
            Console.WriteLine($"[DEBUG DatabaseBroker.Obrisi] Standardni DELETE: query='{query}', affectedRows={affectedRows}");
            return affectedRows > 0;
        }

        #endregion
    }
}