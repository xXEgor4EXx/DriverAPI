using System.Data;
using Microsoft.Data.Sqlite;
using MyAPP.Common;
using MyAPP.Driver;

namespace MyAPP.Driver.DB;

class DBProfessions : IDBProfessions
{
    private Professions FromReader(SqliteDataReader reader)
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
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@id", id)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }
    public Professions? Get(int id)
    {
        string query = "SELECT ProfessionID, Title FROM Professions WHERE ProfessionID = @id";
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@id", id)
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
        
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@ProfessionID", item.ProfessionID),
            new SqliteParameter("@Title", item.Title)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }
    public void Post(Professions item)
    {
        string query = "INSERT INTO Professions (Title) VALUES (@Title)";
        
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@Title", item.Title)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }
}