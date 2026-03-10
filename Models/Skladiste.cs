using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Collections.Generic;

namespace Models
{
    [Serializable]
    public class Skladiste : IDomainObject
{
    public int IdSkladiste { get; set; }
    public string Naziv { get; set; }
    public string Adresa { get; set; }

    [Browsable(false)]
    public string TableName => "Skladiste";
    [Browsable(false)]
    public string InsertValues => $"'{Naziv}', '{Adresa}'";
    [Browsable(false)]
    public string UpdateValues => $"naziv='{Naziv}', adresa='{Adresa}'";
    [Browsable(false)]
    public string WhereCondition
    {
        get
        {
            if (IdSkladiste > 0)
                return $"idSkladiste = {IdSkladiste}";
                
            List<string> conditions = new List<string>();
            if (!string.IsNullOrWhiteSpace(Naziv))
                conditions.Add($"naziv LIKE '%{Naziv}%'");
            if (!string.IsNullOrWhiteSpace(Adresa))
                conditions.Add($"adresa LIKE '%{Adresa}%'");
                
            return conditions.Count > 0 ? string.Join(" AND ", conditions) : "";
        }
    }

    public List<IDomainObject> GetList(SqlDataReader reader)
    {
        List<IDomainObject> rezultati = new List<IDomainObject>();
        while (reader.Read())
        {
            rezultati.Add(new Skladiste
            {
                IdSkladiste = (int)reader["idSkladiste"],
                Naziv = reader["naziv"].ToString(),
                Adresa = reader["adresa"].ToString(),
            });
        }
        return rezultati;
    }

    // Standardni INSERT, UPDATE, DELETE - nema custom logike
    public string GetCustomSelectQuery() => null;
    public bool ExecuteCustomInsert(SqlConnection connection, SqlTransaction? transaction) => false;
    public bool ExecuteCustomUpdate(SqlConnection connection, SqlTransaction? transaction) => false;
    public bool ExecuteCustomDelete(SqlConnection connection, SqlTransaction? transaction) => false;
    }
}                   