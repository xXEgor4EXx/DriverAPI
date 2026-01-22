using System.Data;
using Microsoft.Data.Sqlite;
using MyAPP.Common;
using MyAPP.Driver;

namespace MyAPP.Driver.DB;

class DBProfessionEmployers : IDBProfessionEmployers
{
    private ProfessionEmployers FromReader(SqliteDataReader reader)
    {
        return new ProfessionEmployers
        {
            ProfessionEmployeeID = (int)reader.GetInt64("ProfessionEmployeeID"),
            EmployeeID = (int)reader.GetInt64("EmployeeID"),
            ProfessionID = (int)reader.GetInt64("ProfessionID"),
            Name = reader.GetString("Name"),
            DateOfStart = reader.GetDateTime("DateOfStart"),
            DateOfEnd = reader.IsDBNull("DateOfEnd") ? null : reader.GetDateTime("DateOfEnd")
        };
    }

    public void Delete(int id)
    {
        string query = "DELETE FROM ProfessionEmployers WHERE ProfessionEmployeeID = @id";
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@id", id)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public ProfessionEmployers? Get(int id)
    {
        string query = "SELECT ProfessionEmployeeID, EmployeeID, ProfessionID, Name, DateOfStart, DateOfEnd FROM ProfessionEmployers WHERE ProfessionEmployeeID = @id";
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@id", id)
        };
        
        return DAO.Instance.ReadSingle(query, FromReader, parameters);
    }

    public List<ProfessionEmployers> Get()
    {
        string query = "SELECT ProfessionEmployeeID, EmployeeID, ProfessionID, Name, DateOfStart, DateOfEnd FROM ProfessionEmployers";
        return DAO.Instance.ExecuteReader(query, FromReader);
    }

    public void Put(int id, ProfessionEmployers item)
    {
        string query = @"UPDATE ProfessionEmployeeID 
                        SET EmployeeID = @EmployeeID,
                            ProfessionID = @ProfessionID,
                            Name = @Name,
                            DateOfStart = @DateOfStart,
                            DateOfEnd = @DateOfEnd
                        WHERE ProfessionEmployeeID = @ProfessionEmployeeID";
        
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@ProfessionEmployeeID", item.ProfessionEmployeeID),
            new SqliteParameter("@EmployeeID", item.EmployeeID),
            new SqliteParameter("@ProfessionID", item.ProfessionID),
            new SqliteParameter("@Name", item.Name),
            new SqliteParameter("@DateOfStart", item.DateOfStart),
            new SqliteParameter("@DateOfEnd",item.DateOfEnd)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public void Post(ProfessionEmployers item)
    {
        string query = "INSERT INTO ProfessionEmployers (ProfessionEmployeeID, EmployeeID, ProfessionID, Name, DateOfStart, DateOfEnd) VALUES (@ProfessionEmployeeID, @EmployeeID, @ProfessionID, @Name, @DateOfStart, @DateOfEnd)";
        
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@ProfessionEmployeeID", item.ProfessionEmployeeID),
            new SqliteParameter("@EmployeeID", item.EmployeeID),
            new SqliteParameter("@ProfessionID", item.ProfessionID),
            new SqliteParameter("@Name", item.Name),
            new SqliteParameter("@DateOfStart", item.DateOfStart),
            new SqliteParameter("@DateOfEnd", item.DateOfEnd)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }
}