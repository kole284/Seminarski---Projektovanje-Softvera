using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Models
{
    [Serializable]
    public class Prodavac : IDomainObject
    {
        public int IdProdavac { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        
        [Browsable(false)]
        public string Password { get; set; }

        // Kombinovano polje za prikaz
        [Browsable(false)]
        public string ImePrezime 
        { 
            get => $"{Ime} {Prezime}".Trim();
            set 
            { 
                if (string.IsNullOrWhiteSpace(value)) return;
                var parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                Ime = parts.Length > 0 ? parts[0] : "";
                Prezime = parts.Length > 1 ? parts[1] : "";
            }
        }

        // Skladišta na kojima radi prodavac (za prikaz)
        public string Skladista { get; set; }

        // Atributi za univerzalni SQL
        [Browsable(false)]
        public string TableName => "Prodavac";

        // Za LOGIN i pretragu po emailu/lozinki
        // COLLATE Latin1_General_CS_AS čini poređenje case-sensitive (CS = Case Sensitive)
        [Browsable(false)]
        public string WhereCondition
        {
            get
            {
                if (IdProdavac > 0)
                    return $"p.idProdavac = {IdProdavac}";
                    
                if (!string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password))
                    return $"p.email COLLATE Latin1_General_CS_AS = '{Email}' AND p.password COLLATE Latin1_General_CS_AS = '{Password}'";
                    
                List<string> conditions = new List<string>();
                // Za pretragu po imenu ili prezimenu
                if (!string.IsNullOrWhiteSpace(Ime) || !string.IsNullOrWhiteSpace(Prezime))
                {
                    string imePreziFilter = ImePrezime.Trim(); // Kombinirano ime i prezime
                    if (!string.IsNullOrWhiteSpace(imePreziFilter))
                        conditions.Add($"CONCAT(p.ime, ' ', p.prezime) LIKE '%{imePreziFilter}%'");
                }
                if (!string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(Password))
                    conditions.Add($"p.email LIKE '%{Email}%'");
                if (!string.IsNullOrWhiteSpace(Telefon))
                    conditions.Add($"p.telefon LIKE '%{Telefon}%'");
                    
                return conditions.Count > 0 ? string.Join(" AND ", conditions) : "";
            }
        }

        // Za KREIRANJE (INSERT) - redosled mora biti isti kao u bazi (bez ID-a jer je identity)
        [Browsable(false)]
        public string InsertValues => $"'{Ime}', '{Prezime}', '{Email}', '{Telefon}', '{Password}'";

        // Za IZMENU (UPDATE)
        [Browsable(false)]
        public string UpdateValues => $"ime = '{Ime}', prezime = '{Prezime}', email = '{Email}', telefon = '{Telefon}', password = '{Password}'";

        public List<IDomainObject> GetList(SqlDataReader reader)
        {
            List<IDomainObject> rezultati = new List<IDomainObject>();
            while (reader.Read())
            {
                rezultati.Add(new Prodavac
                {
                    IdProdavac = (int)reader["idProdavac"],
                    Ime = reader["ime"]?.ToString() ?? "",
                    Prezime = reader["prezime"]?.ToString() ?? "",
                    Telefon = reader["telefon"]?.ToString() ?? "",
                    Email = reader["email"]?.ToString() ?? "",
                    Skladista = reader["Skladista"]?.ToString() ?? ""
                });
            }
            return rezultati;
        }

        // TEMPLATE METHOD - prilagođeni SELECT query sa skladištima
        public string GetCustomSelectQuery()
        {
            return @"SELECT p.*, 
                     ISNULL(STUFF((SELECT ', ' + s.naziv 
                            FROM ProdSklad ps 
                            JOIN Skladiste s ON ps.idSkladiste = s.idSkladiste 
                            WHERE ps.idProdavac = p.idProdavac 
                            FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, ''), '') AS Skladista
                     FROM Prodavac p";
        }

        // Standardni INSERT, UPDATE, DELETE - nema custom logike
        public bool ExecuteCustomInsert(SqlConnection connection, SqlTransaction? transaction) => false;
        public bool ExecuteCustomUpdate(SqlConnection connection, SqlTransaction? transaction) => false;
        public bool ExecuteCustomDelete(SqlConnection connection, SqlTransaction? transaction) => false;
    }
}
