using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Models
{
    [Serializable]
    public class Kupac : IDomainObject
    {
        public int IdKupac { get; set; }
        public double Popust { get; set; } // Dodato prema UML dijagramu

        [Browsable(false)]
        public string TableName => "Kupac";

        [Browsable(false)]
        public string InsertValues => $"{Popust.ToString().Replace(',', '.')}";

        [Browsable(false)]
        public string UpdateValues => $"popust={Popust.ToString().Replace(',', '.')}";

        [Browsable(false)]
        public string WhereCondition => $"idKupac = {IdKupac}";

        public List<IDomainObject> GetList(SqlDataReader reader)
        {
            List<IDomainObject> rezultati = new List<IDomainObject>();
            while (reader.Read())
            {
                rezultati.Add(new Kupac
                {
                    // Koristi mala slova ako su ti tako nazivi u bazi (kao na tvojoj prošloj slici)
                    IdKupac = (int)reader["idKupac"],
                    Popust = Convert.ToDouble(reader["popust"])
                });
            }
            return rezultati;
        }

        // Standardni operacije - nema custom logike
        public string GetCustomSelectQuery() => null;
        public bool ExecuteCustomInsert(SqlConnection connection, SqlTransaction? transaction) => false;
        public bool ExecuteCustomUpdate(SqlConnection connection, SqlTransaction? transaction) => false;
        public bool ExecuteCustomDelete(SqlConnection connection, SqlTransaction? transaction) => false;
    }
}