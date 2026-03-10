using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    [Serializable]
    public class Racun : IDomainObject
    {
        public Racun()
        {
            // Ovo sprečava NullReferenceException pri pozivu InsertValues
            Prodavac = new Prodavac();
            Kupac = new Kupac();
            Stavke = new List<StavkaRacuna>();
        }
        public int IdRacun { get; set; }
        public DateTime DatumIzdavanja { get; set; }
        public double KonacanIznos { get; set; }
        public double Pdv { get; set; }
        public double CenaSaPopustom { get; set; }
        public double CenaStavke { get; set; }

        // Kompleksni objekti - sakriveni iz DataGridView-a
        [Browsable(false)]
        public Prodavac Prodavac { get; set; } // Veza ka Prodavcu koji izdaje
        
        [Browsable(false)]
        public Kupac Kupac { get; set; }       // Veza ka Kupcu koji dobija

        [Browsable(false)]
        public string NazivKupca { get; set; } = ""; // Naziv firme ili ime prezime fizičkog lica

        // Svojstva za prikaz u DataGridView-u
        [DisplayName("Prodavac")]
        public string ImeProdavca => Prodavac?.ImePrezime ?? "";
        
        [DisplayName("Kupac")]
        public string ImeKupca => NazivKupca ?? "";

        [Browsable(false)]
        public List<StavkaRacuna> Stavke { get; set; } = new List<StavkaRacuna>();

        // Filter za fleksibilnu pretragu po datumu (YYYY, YYYY-MM, ili YYYY-MM-DD)
        [Browsable(false)]
        public string? DatumFilter { get; set; }

        [Browsable(false)]
        public string TableName => "Racun";

        [Browsable(false)]
        public string InsertValues => $"'{DatumIzdavanja:yyyy-MM-dd}', {KonacanIznos.ToString().Replace(',', '.')}, {Pdv.ToString().Replace(',', '.')}, {CenaSaPopustom.ToString().Replace(',', '.')}, {CenaStavke.ToString().Replace(',', '.')}, {Prodavac.IdProdavac}, {Kupac.IdKupac}";

        [Browsable(false)]
        public string UpdateValues => $"datumIzdavanja='{DatumIzdavanja:yyyy-MM-dd}', konacanIznos={KonacanIznos.ToString().Replace(',', '.')}, pdv={Pdv.ToString().Replace(',', '.')}, cenaSaPopustom={CenaSaPopustom.ToString().Replace(',', '.')}, cenaStavke={CenaStavke.ToString().Replace(',', '.')}, idProdavac={Prodavac.IdProdavac}, idKupac={Kupac.IdKupac}";

        [Browsable(false)]
        public string WhereCondition
        {
            get
            {
                if (IdRacun > 0)
                    return $"r.idRacun = {IdRacun}";
                    
                List<string> conditions = new List<string>();
                if (Prodavac != null && Prodavac.IdProdavac > 0)
                    conditions.Add($"r.idProdavac = {Prodavac.IdProdavac}");
                if (Kupac != null && Kupac.IdKupac > 0)
                    conditions.Add($"r.idKupac = {Kupac.IdKupac}");
                    
                // Fleksibilna pretraga po datumu
                if (!string.IsNullOrWhiteSpace(DatumFilter))
                {
                    if (DatumFilter.Length == 4) // YYYY - po godini
                        conditions.Add($"YEAR(r.datumIzdavanja) = {DatumFilter}");
                    else if (DatumFilter.Length == 7) // YYYY-MM - po mesecu
                        conditions.Add($"FORMAT(r.datumIzdavanja, 'yyyy-MM') = '{DatumFilter}'");
                    else // YYYY-MM-DD - tačan datum
                        conditions.Add($"CAST(r.datumIzdavanja AS DATE) = '{DatumFilter}'");
                }
                    
                return conditions.Count > 0 ? string.Join(" AND ", conditions) : "";
            }
        }

        public List<IDomainObject> GetList(SqlDataReader reader)
        {
            List<IDomainObject> rezultati = new List<IDomainObject>();
            while (reader.Read())
            {
                rezultati.Add(new Racun
                {
                    IdRacun = (int)reader["idRacun"],
                    DatumIzdavanja = (DateTime)reader["datumIzdavanja"],
                    KonacanIznos = Convert.ToDouble(reader["konacanIznos"]),
                    Pdv = Convert.ToDouble(reader["pdv"]),
                    CenaSaPopustom = Convert.ToDouble(reader["cenaSaPopustom"]),
                    CenaStavke = reader["cenaStavke"] != DBNull.Value ? Convert.ToDouble(reader["cenaStavke"]) : 0,
                    Prodavac = new Prodavac 
                    { 
                        IdProdavac = (int)reader["idProdavac"],
                        ImePrezime = reader["imeProdavca"].ToString()
                    },
                    Kupac = new Kupac 
                    { 
                        IdKupac = (int)reader["idKupac"]
                    },
                    NazivKupca = reader["naziv"]?.ToString() ?? ""
                });
            }
            return rezultati;
        }

        // TEMPLATE METHOD - prilagođeni SELECT query
        public string GetCustomSelectQuery()
        {
            return @"SELECT r.*, CONCAT(p.ime, ' ', p.prezime) as imeProdavca, 
                     COALESCE(f.naziv, fl.imePrezime) as naziv
              FROM Racun r 
              JOIN Prodavac p ON r.idProdavac = p.idProdavac 
              JOIN Kupac k ON r.idKupac = k.idKupac
              LEFT JOIN Firma f ON k.idKupac = f.idKupac
              LEFT JOIN FizickoLice fl ON k.idKupac = fl.idKupac";
        }

        // TEMPLATE METHOD - prilagođeni INSERT (sa transakcijom za stavke)
        public bool ExecuteCustomInsert(SqlConnection connection, SqlTransaction? transaction)
        {
            Console.WriteLine("[RACUN DEBUG] Počinje dodavanje Računa sa stavkama");
            Console.WriteLine($"[RACUN DEBUG] Racun.InsertValues: {InsertValues}");
            Console.WriteLine($"[RACUN DEBUG] Broj stavki: {Stavke?.Count ?? 0}");
            
            bool ownTransaction = transaction == null;
            SqlTransaction activeTransaction = transaction ?? connection.BeginTransaction();
            try
            {
                // Ako Kupac nije izabran (IdKupac == 0), prvo ga ubaci u tabelu Kupac
                if (Kupac == null)
                    Kupac = new Kupac();
                if (Kupac.IdKupac == 0)
                {
                    string insertKupac = $"INSERT INTO Kupac (popust) VALUES ({Kupac.Popust.ToString().Replace(',', '.')}); SELECT SCOPE_IDENTITY()";
                    Console.WriteLine($"[RACUN DEBUG] SQL Query Kupac: {insertKupac}");
                    SqlCommand cmdKupac = new SqlCommand(insertKupac, connection, activeTransaction);
                    int newIdKupac = Convert.ToInt32(cmdKupac.ExecuteScalar());
                    Kupac.IdKupac = newIdKupac;
                    Console.WriteLine($"[RACUN DEBUG] Napravljen Kupac sa ID: {newIdKupac}");
                }

                // Dodaj račun
                string queryRacun = $"INSERT INTO Racun VALUES ({InsertValues}); SELECT SCOPE_IDENTITY()";
                Console.WriteLine($"[RACUN DEBUG] SQL Query Racun: {queryRacun}");
                
                SqlCommand cmdRacun = new SqlCommand(queryRacun, connection, activeTransaction);
                int idRacun = Convert.ToInt32(cmdRacun.ExecuteScalar());
                Console.WriteLine($"[RACUN DEBUG] Račun dodat sa ID: {idRacun}");
                
                // Dodaj stavke računa
                int stavkaIndex = 0;
                foreach (var stavka in Stavke)
                {
                    stavkaIndex++;
                    stavka.IdRacun = idRacun;
                    string queryStavka = $"INSERT INTO StavkaRacuna VALUES ({stavka.InsertValues})";
                    Console.WriteLine($"[RACUN DEBUG] SQL Query Stavka {stavkaIndex}: {queryStavka}");
                    
                    SqlCommand cmdStavka = new SqlCommand(queryStavka, connection, activeTransaction);
                    cmdStavka.ExecuteNonQuery();
                    Console.WriteLine($"[RACUN DEBUG] Stavka {stavkaIndex} dodata uspešno");
                }
                if (ownTransaction)
                    activeTransaction.Commit();
                Console.WriteLine("[RACUN DEBUG] Transakcija commit-ovana uspešno");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RACUN ERROR] Greška u transakciji: {ex.Message}");
                Console.WriteLine($"[RACUN ERROR] Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[RACUN ERROR] Inner exception: {ex.InnerException.Message}");
                }
                if (ownTransaction)
                    activeTransaction.Rollback();
                throw;
            }
        }

        // TEMPLATE METHOD - prilagođeni UPDATE (sa transakcijom za stavke)
        public bool ExecuteCustomUpdate(SqlConnection connection, SqlTransaction? transaction)
        {
            Console.WriteLine("[RACUN DEBUG] Počinje ažuriranje Računa sa stavkama");
            
            bool ownTransaction = transaction == null;
            SqlTransaction activeTransaction = transaction ?? connection.BeginTransaction();
            try
            {
                // Ako Kupac nije izabran (IdKupac == 0), prvo ga ubaci u tabelu Kupac
                if (Kupac == null)
                    Kupac = new Kupac();
                if (Kupac.IdKupac == 0)
                {
                    string insertKupac = $"INSERT INTO Kupac (popust) VALUES ({Kupac.Popust.ToString().Replace(',', '.')}); SELECT SCOPE_IDENTITY()";
                    Console.WriteLine($"[RACUN DEBUG] SQL Query Kupac: {insertKupac}");
                    SqlCommand cmdKupac = new SqlCommand(insertKupac, connection, activeTransaction);
                    int newIdKupac = Convert.ToInt32(cmdKupac.ExecuteScalar());
                    Kupac.IdKupac = newIdKupac;
                    Console.WriteLine($"[RACUN DEBUG] Napravljen Kupac sa ID: {newIdKupac}");
                }

                // Prvo obriši stare stavke
                string deleteStavke = $"DELETE FROM StavkaRacuna WHERE idRacun = {IdRacun}";
                SqlCommand cmdDelete = new SqlCommand(deleteStavke, connection, activeTransaction);
                cmdDelete.ExecuteNonQuery();
                Console.WriteLine($"[RACUN DEBUG] Stare stavke obrisane");
                
                // Ažuriraj račun
                string queryRacun = $"UPDATE Racun SET {UpdateValues} WHERE idRacun = {IdRacun}";
                Console.WriteLine($"[RACUN DEBUG] SQL Query Racun: {queryRacun}");
                
                SqlCommand cmdRacun = new SqlCommand(queryRacun, connection, activeTransaction);
                cmdRacun.ExecuteNonQuery();
                Console.WriteLine($"[RACUN DEBUG] Račun ažuriran");
                
                // Dodaj nove stavke
                int stavkaIndex = 0;
                foreach (var stavka in Stavke)
                {
                    stavkaIndex++;
                    stavka.IdRacun = IdRacun;
                    string queryStavka = $"INSERT INTO StavkaRacuna VALUES ({IdRacun}, {stavka.Oprema.IdOprema}, {stavka.Kolicina}, {stavka.Cena.ToString().Replace(',', '.')})";
                    Console.WriteLine($"[RACUN DEBUG] SQL Query Stavka {stavkaIndex}: {queryStavka}");
                    
                    SqlCommand cmdStavka = new SqlCommand(queryStavka, connection, activeTransaction);
                    cmdStavka.ExecuteNonQuery();
                    Console.WriteLine($"[RACUN DEBUG] Stavka {stavkaIndex} dodata uspešno");
                }
                if (ownTransaction)
                    activeTransaction.Commit();
                Console.WriteLine("[RACUN DEBUG] Transakcija commit-ovana uspešno");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RACUN ERROR] Greška u transakciji: {ex.Message}");
                Console.WriteLine($"[RACUN ERROR] Stack trace: {ex.StackTrace}");
                if (ownTransaction)
                    activeTransaction.Rollback();
                throw;
            }
        }

        // Standardno brisanje - nema custom logike
        public bool ExecuteCustomDelete(SqlConnection connection, SqlTransaction? transaction) => false;
    }
}