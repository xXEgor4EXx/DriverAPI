using System.Data;
using MyAPP.Common;
using MySqlConnector;

namespace MyAPP.Driver.MariaDB;

class MariaDBProfessionEmployers : IDBProfessionEmployers
{
    private ProfessionEmployers FromReader(MySqlDataReader reader)
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
        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@id", id)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public ProfessionEmployers? Get(int id)
    {
        string query = "SELECT ProfessionEmployeeID, EmployeeID, ProfessionID, Name, DateOfStart, DateOfEnd FROM ProfessionEmployers WHERE ProfessionEmployeeID = @id";
        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@id", id)
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
        string query = @"UPDATE ProfessionEmployers 
                        SET EmployeeID = @EmployeeID,
                            ProfessionID = @ProfessionID,
                            Name = @Name,
                            DateOfStart = @DateOfStart,
                            DateOfEnd = @DateOfEnd
                        WHERE ProfessionEmployeeID = @ProfessionEmployeeID";
        
        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@ProfessionEmployeeID", item.ProfessionEmployeeID),
            new MySqlParameter("@EmployeeID", item.EmployeeID),
            new MySqlParameter("@ProfessionID", item.ProfessionID),
            new MySqlParameter("@Name", item.Name),
            new MySqlParameter("@DateOfStart", item.DateOfStart),
            new MySqlParameter("@DateOfEnd",item.DateOfEnd)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public void Post(ProfessionEmployers item)
    {
        string query = "INSERT INTO ProfessionEmployers (ProfessionEmployeeID, EmployeeID, ProfessionID, Name, DateOfStart, DateOfEnd) VALUES (@ProfessionEmployeeID, @EmployeeID, @ProfessionID, @Name, @DateOfStart, @DateOfEnd)";
        
        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@ProfessionEmployeeID", item.ProfessionEmployeeID),
            new MySqlParameter("@EmployeeID", item.EmployeeID),
            new MySqlParameter("@ProfessionID", item.ProfessionID),
            new MySqlParameter("@Name", item.Name),
            new MySqlParameter("@DateOfStart", item.DateOfStart),
            new MySqlParameter("@DateOfEnd", item.DateOfEnd)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }
}