using System.Data;
using MyAPP.Common;
using MySqlConnector;
using MyAPP.Driver.MariaDB;

namespace MyAPP.Driver.MariaDB;

class MariaDBProfessions : IDBProfessions
{
    private Professions FromReader(MySqlDataReader reader)
    {
        return new Professions
        {
            ProfessionID = (int)reader.GetInt64("ProfessionID"),
            Title = reader.GetString("Title")
        };
    }

    public void Delete(int id)
    {
        string query = "DELETE FROM Professions WHERE ProfessionID = @id";
        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@id", id)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public Professions? Get(int id)
    {
        string query = "SELECT ProfessionID, Title FROM Professions WHERE ProfessionID = @id";
        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@id", id)
        };
        
        return DAO.Instance.ReadSingle(query, FromReader, parameters);
    }

    public List<Professions> Get()
    {
        string query = "SELECT ProfessionID, Title FROM Professions";
        return DAO.Instance.ExecuteReader(query, FromReader);
    }

    public void Put(int id, Professions item)
    {
        string query = @"UPDATE Professions 
                        SET Title = @Title
                        WHERE ProfessionID = @ProfessionID";
        
        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@ProfessionID", item.ProfessionID),
            new MySqlParameter("@Title", item.Title)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public void Post(Professions item)
    {
        string query = "INSERT INTO Professions (ProfessionID, Title) VALUES (@ProfessionID, @Title)";
        
        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@ProfessionID", item.ProfessionID),
            new MySqlParameter("@Title", item.Title)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }
}