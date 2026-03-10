using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Models
{
    [Serializable]
    public class FizickoLice : IDomainObject
    {
        public int IdKupac { get; set; }
        public double Popust { get; set; }
        public string ImePrezime { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public bool LoyaltyClan { get; set; }

        [Browsable(false)]
        public string TableName => "FizickoLice";

        [Browsable(false)]
        public string InsertValues => $"{IdKupac}, '{ImePrezime}', '{Email}', '{Telefon}', {(LoyaltyClan ? 1 : 0)}";

        [Browsable(false)]
        public string UpdateValues => $"imePrezime='{ImePrezime}', email='{Email}', telefon='{Telefon}', loyaltyClan={(LoyaltyClan ? 1 : 0)}";

        [Browsable(false)]
        public string WhereCondition
        {
            get
            {
                if (IdKupac > 0)
                    return $"fl.idKupac = {IdKupac}";
                    
                List<string> conditions = new List<string>();
                if (!string.IsNullOrWhiteSpace(ImePrezime))
                    conditions.Add($"fl.imePrezime LIKE '%{ImePrezime}%'");
                if (!string.IsNullOrWhiteSpace(Email))
                    conditions.Add($"fl.email LIKE '%{Email}%'");
                if (!string.IsNullOrWhiteSpace(Telefon))
                    conditions.Add($"fl.telefon LIKE '%{Telefon}%'");
                    
                return conditions.Count > 0 ? string.Join(" AND ", conditions) : "";
            }
        }

        public List<IDomainObject> GetList(SqlDataReader reader)
        {
            List<IDomainObject> rezultati = new List<IDomainObject>();
            while (reader.Read())
            {
                rezultati.Add(new FizickoLice
                {
                    IdKupac = (int)reader["idKupac"],
                    Popust = reader["popust"] != DBNull.Value ? Convert.ToDouble(reader["popust"]) : 0,
                    ImePrezime = reader["imePrezime"] != DBNull.Value ? reader["imePrezime"].ToString() : "",
                    Email = reader["email"] != DBNull.Value ? reader["email"].ToString() : "",
                    Telefon = reader["telefon"] != DBNull.Value ? reader["telefon"].ToString() : "",
                    LoyaltyClan = reader["loyaltyClan"] != DBNull.Value && Convert.ToBoolean(reader["loyaltyClan"])
                });
            }
            return rezultati;
        }

        // TEMPLATE METHOD - prilagođeni SELECT query sa JOIN-om na Kupac
        public string GetCustomSelectQuery()
        {
            return @"SELECT fl.*, k.popust 
                     FROM FizickoLice fl 
                     JOIN Kupac k ON fl.idKupac = k.idKupac";
        }

        // TEMPLATE METHOD - koristi stored procedure za INSERT
        public bool ExecuteCustomInsert(SqlConnection connection, SqlTransaction? transaction)
        {
            SqlCommand cmd;
            if (transaction != null)
                cmd = new SqlCommand("DodajFizickoLice", connection, transaction);
            else
                cmd = new SqlCommand("DodajFizickoLice", connection);

            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@popust", Popust);
            cmd.Parameters.AddWithValue("@imePrezime", ImePrezime ?? "");
            cmd.Parameters.AddWithValue("@email", Email ?? "");
            cmd.Parameters.AddWithValue("@telefon", Telefon ?? "");
            cmd.Parameters.AddWithValue("@loyaltyClan", LoyaltyClan);

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
            Console.WriteLine($"[FIZICKO_LICE DEBUG] Pozivam AzurirajFizickoLice za idKupac={IdKupac}");
            SqlCommand cmd;
            if (transaction != null)
                cmd = new SqlCommand("AzurirajFizickoLice", connection, transaction);
            else
                cmd = new SqlCommand("AzurirajFizickoLice", connection);

            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@idFizickoLice", IdKupac); // Koristi IdKupac kao idFizickoLice (Table per Type pattern)
            cmd.Parameters.AddWithValue("@idKupac", IdKupac);
            cmd.Parameters.AddWithValue("@popust", Popust);
            cmd.Parameters.AddWithValue("@imePrezime", ImePrezime ?? "");
            cmd.Parameters.AddWithValue("@email", Email ?? "");
            cmd.Parameters.AddWithValue("@telefon", Telefon ?? "");
            cmd.Parameters.AddWithValue("@loyaltyClan", LoyaltyClan);

            int affectedRows = cmd.ExecuteNonQuery();
            Console.WriteLine($"[FIZICKO_LICE DEBUG] AzurirajFizickoLice affectedRows={affectedRows}");
            return true; // Ako nema exception-a, ažuriranje je uspelo
        }

        // Custom DELETE - briše prvo FizickoLice, pa onda Kupac (parent)
        public bool ExecuteCustomDelete(SqlConnection connection, SqlTransaction? transaction)
        {
            Console.WriteLine($"[FIZICKO_LICE DEBUG] ExecuteCustomDelete pozvan za idKupac={IdKupac}");

            // PRVO obrisi iz FizickoLice (child) tabele
            SqlCommand cmdFizicko;
            if (transaction != null)
                cmdFizicko = new SqlCommand($"DELETE FROM FizickoLice WHERE idKupac = {IdKupac}", connection, transaction);
            else
                cmdFizicko = new SqlCommand($"DELETE FROM FizickoLice WHERE idKupac = {IdKupac}", connection);

            int rowsFizicko = cmdFizicko.ExecuteNonQuery();
            Console.WriteLine($"[FIZICKO_LICE DEBUG] Obrisano {rowsFizicko} redova iz FizickoLice");

            // Proveri da li Kupac još postoji (možda ga je trigger automatski obrisao)
            SqlCommand cmdCheck;
            if (transaction != null)
                cmdCheck = new SqlCommand($"SELECT COUNT(*) FROM Kupac WHERE idKupac = {IdKupac}", connection, transaction);
            else
                cmdCheck = new SqlCommand($"SELECT COUNT(*) FROM Kupac WHERE idKupac = {IdKupac}", connection);

            int countKupac = (int)cmdCheck.ExecuteScalar();
            Console.WriteLine($"[FIZICKO_LICE DEBUG] Kupac preostalo redova nakon brisanja FizickoLice: {countKupac}");

            if (countKupac > 0)
            {
                // Kupac još postoji, obriši ga ručno
                SqlCommand cmdKupac;
                if (transaction != null)
                    cmdKupac = new SqlCommand($"DELETE FROM Kupac WHERE idKupac = {IdKupac}", connection, transaction);
                else
                    cmdKupac = new SqlCommand($"DELETE FROM Kupac WHERE idKupac = {IdKupac}", connection);

                int rowsKupac = cmdKupac.ExecuteNonQuery();
                Console.WriteLine($"[FIZICKO_LICE DEBUG] Ručno obrisano {rowsKupac} redova iz Kupac");

                bool success = rowsFizicko > 0 && rowsKupac > 0;
                Console.WriteLine($"[FIZICKO_LICE DEBUG] ExecuteCustomDelete rezultat: {success}");
                return success;
            }
            else
            {
                // Kupac je već obrisan (verovatno trigger), što je OK ako je FizickoLice uspešno obrisan
                Console.WriteLine($"[FIZICKO_LICE DEBUG] Kupac već obrisan (trigger?), FizickoLice obrisano: {rowsFizicko > 0}");
                bool success = rowsFizicko > 0;
                Console.WriteLine($"[FIZICKO_LICE DEBUG] ExecuteCustomDelete rezultat: {success}");
                return success;
            }
        }
    }
}
