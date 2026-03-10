using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Models
{
    [Serializable]
    public class ProdSklad : IDomainObject
    {
        public int IdProdavac { get; set; }
        public int IdSkladiste { get; set; }

        // Navigacione properties (opciono, za lak shi pristup)
        [Browsable(false)]
        public Prodavac? Prodavac { get; set; }
        
        [Browsable(false)]
        public Skladiste? Skladiste { get; set; }

        [Browsable(false)]
        public string TableName => "ProdSklad";

        [Browsable(false)]
        public string InsertValues => $"{IdProdavac}, {IdSkladiste}";

        [Browsable(false)]
        public string UpdateValues => $"idSkladiste = {IdSkladiste}";

        [Browsable(false)]
        public string WhereCondition
        {
            get
            {
                var conditions = new List<string>();
                if (IdProdavac > 0)
                    conditions.Add($"idProdavac = {IdProdavac}");
                if (IdSkladiste > 0)
                    conditions.Add($"idSkladiste = {IdSkladiste}");
                return conditions.Count > 0 ? string.Join(" AND ", conditions) : "";
            }
        }

        public List<IDomainObject> GetList(SqlDataReader reader)
        {
            List<IDomainObject> rezultati = new List<IDomainObject>();
            while (reader.Read())
            {
                rezultati.Add(new ProdSklad
                {
                    IdProdavac = (int)reader["idProdavac"],
                    IdSkladiste = (int)reader["idSkladiste"]
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
