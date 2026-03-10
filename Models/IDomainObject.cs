using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    // Template Method Pattern - interfejs definiše základne metode
    // Konkretne klase implementiraju specifičnu logiku
    public interface IDomainObject
    {
        string TableName { get; }
        string WhereCondition { get; }
        string InsertValues { get; }
        string UpdateValues { get; }
        List<IDomainObject> GetList(SqlDataReader reader);
        
        // Template metode za prilagođene operacije
        // Vraća null ako nema specijalne logike, broker koristi standardnu logiku
        string GetCustomSelectQuery() => null;
        bool ExecuteCustomInsert(SqlConnection connection, SqlTransaction? transaction) => false;
        bool ExecuteCustomUpdate(SqlConnection connection, SqlTransaction? transaction) => false;
        bool ExecuteCustomDelete(SqlConnection connection, SqlTransaction? transaction) => false;
    }
}
