using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Models
{
    [Serializable]
    public class Oprema : IDomainObject
    {
        public int IdOprema { get; set; }
        public string Ime { get; set; }
        
        private KategorijaOpreme _kategorija = KategorijaOpreme.Ostalo;
        private bool _kategorijaIsSet = false; // Flag koji prati da li je kategorija eksplicitno postavljena
        
        [Browsable(false)]
        public KategorijaOpreme KategorijaEnum 
        { 
            get => _kategorija;
            set 
            {
                _kategorija = value;
                // Postavi flag samo ako vrednost NIJE default (Ostalo)
                if (value != KategorijaOpreme.Ostalo)
                    _kategorijaIsSet = true;
            }
        }
        
        // String property za bazu i serializaciju
        public string Kategorija 
        { 
            get => _kategorijaIsSet ? _kategorija.GetDescription() : string.Empty;
            set 
            {
                // Ne konvertuj ako je prazan string (za filtriranje)
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _kategorija = KategorijaOpremeExtensions.FromString(value);
                    _kategorijaIsSet = true;
                }
            }
        }
        
        public double Cena { get; set; }

        [Browsable(false)]
        public string TableName => "Oprema";

        [Browsable(false)]
        public string InsertValues => $"'{Ime}', {(int)_kategorija}, {Cena.ToString().Replace(',', '.')}";

        [Browsable(false)]
        public string UpdateValues => $"ime='{Ime}', kategorija={(int)_kategorija}, cena={Cena.ToString().Replace(',', '.')}";

        [Browsable(false)]
        public string WhereCondition
        {
            get
            {
                if (IdOprema > 0)
                    return $"idOprema = {IdOprema}";
                    
                List<string> conditions = new List<string>();
                if (!string.IsNullOrWhiteSpace(Ime))
                    conditions.Add($"ime LIKE '%{Ime}%'");
                if (!string.IsNullOrWhiteSpace(Kategorija))
                    conditions.Add($"kategorija LIKE '%{Kategorija}%'");
                    
                return conditions.Count > 0 ? string.Join(" AND ", conditions) : "";
            }
        }

        public List<IDomainObject> GetList(SqlDataReader reader)
        {
            List<IDomainObject> rezultati = new List<IDomainObject>();
            while (reader.Read())
            {
                // Kategorija u bazi je čuvana kao INT -> parsiraj direktno u enum
                int katInt = Convert.ToInt32(reader["kategorija"]);
                rezultati.Add(new Oprema
                {
                    IdOprema = (int)reader["idOprema"],
                    Ime = reader["ime"].ToString(),
                    KategorijaEnum = (KategorijaOpreme)katInt,
                    Cena = Convert.ToDouble(reader["cena"])
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