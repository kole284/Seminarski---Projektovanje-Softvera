using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Models
{
    [Serializable]
    public class StavkaRacuna : IDomainObject
    {
        public int IdRacun { get; set; } // FK ka Racunu
        public int Kolicina { get; set; }
        public double Cena { get; set; } // Cena u trenutku prodaje
        public Oprema Oprema { get; set; } = new Oprema(); // Veza 1..1 ka Opremi

        // Computed properties (not in database)
        [Browsable(false)]
        public int RbStavke { get; set; } // For display only, not stored in DB
        
        [Browsable(false)]
        public DateTime DatumKupovine { get; set; } // For display only, not stored in DB
        
        [Browsable(false)]
        public double Iznos => Kolicina * Cena; // Computed: Kolicina * Cena

        [Browsable(false)]
        public string TableName => "StavkaRacuna";

        [Browsable(false)]
        public string InsertValues => $"{IdRacun}, {Oprema?.IdOprema ?? 0}, {Kolicina}, {Cena.ToString().Replace(',', '.')}";

        [Browsable(false)]
        public string UpdateValues => $"kolicina={Kolicina}, cena={Cena.ToString().Replace(',', '.')}";

        [Browsable(false)]
        public string WhereCondition
        {
            get
            {
                List<string> conditions = new List<string>();

                if (IdRacun > 0)
                    conditions.Add($"idRacun = {IdRacun}");

                if (Oprema != null && Oprema.IdOprema > 0)
                    conditions.Add($"idOprema = {Oprema.IdOprema}");

                return conditions.Count > 0 ? string.Join(" AND ", conditions) : "";
            }
        }

        public List<IDomainObject> GetList(SqlDataReader reader)
        {
            List<IDomainObject> rezultati = new List<IDomainObject>();
            Console.WriteLine($"DEBUG GetList: FieldCount = {reader.FieldCount}");
            for (int i = 0; i < reader.FieldCount; i++)
            {
                Console.WriteLine($"DEBUG GetList: Column[{i}] = {reader.GetName(i)}");
            }
            
            while (reader.Read())
            {
                int idOprema = (int)reader["idOprema"];
                Oprema oprema = new Oprema { IdOprema = idOprema };
                
                // Ako su učitani detalji opreme iz JOIN-a
                try
                {
                    // Proveri da li postoji kolona 'ime' (iz JOIN-a sa Oprema tabelom)
                    bool hasIme = false;
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (reader.GetName(i).Equals("ime", StringComparison.OrdinalIgnoreCase))
                        {
                            hasIme = true;
                            break;
                        }
                    }

                    if (hasIme)
                    {
                        oprema.Ime = reader["ime"].ToString() ?? "";
                        oprema.Kategorija = reader["kategorija"].ToString() ?? "";
                        oprema.Cena = Convert.ToDouble(reader["cenaOprema"]);
                        Console.WriteLine($"DEBUG GetList: Popunjena oprema - Ime: {oprema.Ime}, Kategorija: {oprema.Kategorija}, Cena: {oprema.Cena}");
                    }
                    else
                    {
                        Console.WriteLine($"DEBUG GetList: Kolona 'ime' nije pronađena!");
                    }
                }
                catch
                {
                    // Detalji opreme nisu učitani, samo ID
                }

                rezultati.Add(new StavkaRacuna
                {
                    IdRacun = (int)reader["idRacun"],
                    Kolicina = (int)reader["kolicina"],
                    Cena = Convert.ToDouble(reader["cena"]),
                    Oprema = oprema
                });
            }
            return rezultati;
        }

        // TEMPLATE METHOD - prilagođeni SELECT query sa JOIN-om na Oprema
        public string GetCustomSelectQuery()
        {
            if (IdRacun > 0)
            {
                return $@"SELECT s.*, o.ime, o.kategorija, o.cena as cenaOprema 
                          FROM StavkaRacuna s 
                          JOIN Oprema o ON s.idOprema = o.idOprema 
                          WHERE s.idRacun = {IdRacun}";
            }
            return null;
        }

        // Standardni INSERT, UPDATE, DELETE - nema custom logike
        public bool ExecuteCustomInsert(SqlConnection connection, SqlTransaction? transaction) => false;
        public bool ExecuteCustomUpdate(SqlConnection connection, SqlTransaction? transaction) => false;
        public bool ExecuteCustomDelete(SqlConnection connection, SqlTransaction? transaction) => false;
    }
}