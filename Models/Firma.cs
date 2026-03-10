using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Models
{
    [Serializable]
    public class Firma : IDomainObject
    {
        public int IdKupac { get; set; }
        public double Popust { get; set; }
        public string Naziv { get; set; }
        public string Pib { get; set; }
        public string Adresa { get; set; }
        public bool Partnerstvo { get; set; }

        [Browsable(false)]
        public string TableName => "Firma";

        [Browsable(false)]
        public string InsertValues => $"{IdKupac}, '{Naziv}', '{Pib}', '{Adresa}', {(Partnerstvo ? 1 : 0)}";

        [Browsable(false)]
        public string UpdateValues => $"naziv='{Naziv}', pib='{Pib}', adresa='{Adresa}', partnerstvo={(Partnerstvo ? 1 : 0)}";

        [Browsable(false)]
        public string WhereCondition
        {
            get
            {
                if (IdKupac > 0)
                    return $"f.idKupac = {IdKupac}";
                    
                List<string> conditions = new List<string>();
                if (!string.IsNullOrWhiteSpace(Naziv))
                    conditions.Add($"f.naziv LIKE '%{Naziv}%'");
                if (!string.IsNullOrWhiteSpace(Pib))
                    conditions.Add($"f.pib LIKE '%{Pib}%'");
                    
                return conditions.Count > 0 ? string.Join(" AND ", conditions) : "";
            }
        }

        public List<IDomainObject> GetList(SqlDataReader reader)
        {
            List<IDomainObject> rezultati = new List<IDomainObject>();
            while (reader.Read())
            {
                rezultati.Add(new Firma
                {
                    IdKupac = (int)reader["idKupac"],
                    Popust = reader["popust"] != DBNull.Value ? Convert.ToDouble(reader["popust"]) : 0,
                    Naziv = reader["naziv"].ToString(),
                    Pib = reader["pib"] != DBNull.Value ? reader["pib"].ToString() : "",
                    Adresa = reader["adresa"] != DBNull.Value ? reader["adresa"].ToString() : "",
                    Partnerstvo = reader["partnerstvo"] != DBNull.Value && Convert.ToBoolean(reader["partnerstvo"])
                });
            }
            return rezultati;
        }

        // TEMPLATE METHOD - prilagođeni SELECT query sa JOIN-om na Kupac
        public string GetCustomSelectQuery()
        {
            return @"SELECT f.*, k.popust 
                     FROM Firma f 
                     JOIN Kupac k ON f.idKupac = k.idKupac";
        }

        // TEMPLATE METHOD - koristi stored procedure za INSERT
        public bool ExecuteCustomInsert(SqlConnection connection, SqlTransaction? transaction)
        {
            SqlCommand cmd;
            if (transaction != null)
                cmd = new SqlCommand("DodajFirmu", connection, transaction);
            else
                cmd = new SqlCommand("DodajFirmu", connection);

            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@naziv", Naziv);
            cmd.Parameters.AddWithValue("@popust", Popust);
            cmd.Parameters.AddWithValue("@pib", Pib ?? "");
            cmd.Parameters.AddWithValue("@adresa", Adresa ?? "");
            cmd.Parameters.AddWithValue("@partnerstvo", Partnerstvo);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    IdKupac = (int)reader["IdKupac"];
                    return true;
                }
            }
            return false;
        }

        // TEMPLATE METHOD - koristi stored procedure za UPDATE
        public bool ExecuteCustomUpdate(SqlConnection connection, SqlTransaction? transaction)
        {
            Console.WriteLine($"[FIRMA DEBUG] Pozivam AzurirajFirmu za idKupac={IdKupac}");
            SqlCommand cmd;
            if (transaction != null)
                cmd = new SqlCommand("AzurirajFirmu", connection, transaction);
            else
                cmd = new SqlCommand("AzurirajFirmu", connection);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@idKupac", IdKupac);
            cmd.Parameters.AddWithValue("@naziv", Naziv);
            cmd.Parameters.AddWithValue("@popust", Popust);
            cmd.Parameters.AddWithValue("@pib", Pib ?? "");
            cmd.Parameters.AddWithValue("@adresa", Adresa ?? "");
            cmd.Parameters.AddWithValue("@partnerstvo", Partnerstvo);
            
            int affectedRows = cmd.ExecuteNonQuery();
            Console.WriteLine($"[FIRMA DEBUG] AzurirajFirmu affectedRows={affectedRows}");
            return true; // Ako nema exception-a, ažuriranje je uspelo
        }

        // Custom DELETE - briše prvo Firma, pa onda Kupac (parent)
        public bool ExecuteCustomDelete(SqlConnection connection, SqlTransaction? transaction)
        {
            Console.WriteLine($"[FIRMA DEBUG] ExecuteCustomDelete pozvan za idKupac={IdKupac}");

            // PRVO obrisi iz Firma (child) tabele
            SqlCommand cmdFirma;
            if (transaction != null)
                cmdFirma = new SqlCommand($"DELETE FROM Firma WHERE idKupac = {IdKupac}", connection, transaction);
            else
                cmdFirma = new SqlCommand($"DELETE FROM Firma WHERE idKupac = {IdKupac}", connection);

            int rowsFirma = cmdFirma.ExecuteNonQuery();
            Console.WriteLine($"[FIRMA DEBUG] Obrisano {rowsFirma} redova iz Firma");

            // Proveri da li Kupac još postoji (možda ga je trigger automatski obrisao)
            SqlCommand cmdCheck;
            if (transaction != null)
                cmdCheck = new SqlCommand($"SELECT COUNT(*) FROM Kupac WHERE idKupac = {IdKupac}", connection, transaction);
            else
                cmdCheck = new SqlCommand($"SELECT COUNT(*) FROM Kupac WHERE idKupac = {IdKupac}", connection);

            int countKupac = (int)cmdCheck.ExecuteScalar();
            Console.WriteLine($"[FIRMA DEBUG] Kupac preostalo redova nakon brisanja Firma: {countKupac}");

            if (countKupac > 0)
            {
                // Kupac još postoji, obriši ga ručno
                SqlCommand cmdKupac;
                if (transaction != null)
                    cmdKupac = new SqlCommand($"DELETE FROM Kupac WHERE idKupac = {IdKupac}", connection, transaction);
                else
                    cmdKupac = new SqlCommand($"DELETE FROM Kupac WHERE idKupac = {IdKupac}", connection);

                int rowsKupac = cmdKupac.ExecuteNonQuery();
                Console.WriteLine($"[FIRMA DEBUG] Ručno obrisano {rowsKupac} redova iz Kupac");

                bool success = rowsFirma > 0 && rowsKupac > 0;
                Console.WriteLine($"[FIRMA DEBUG] ExecuteCustomDelete rezultat: {success}");
                return success;
            }
            else
            {
                // Kupac je već obrisan (verovatno trigger), što je OK ako je Firma uspešno obrisana
                Console.WriteLine($"[FIRMA DEBUG] Kupac već obrisan (trigger?), Firma obrisana: {rowsFirma > 0}");
                bool success = rowsFirma > 0;
                Console.WriteLine($"[FIRMA DEBUG] ExecuteCustomDelete rezultat: {success}");
                return success;
            }
        }
    }
}
