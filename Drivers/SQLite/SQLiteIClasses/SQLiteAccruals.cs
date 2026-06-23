using System.Data;
using Microsoft.Data.Sqlite;
using MyAPP.Common;
using MyAPP.Driver;
namespace MyAPP.Driver.DB;
class DBAccruals : IDBAccruals
{
    private Accruals FromReader(SqliteDataReader reader)
    {
        return new Accruals
        {
            AccrualID = (int)reader.GetInt64("AccrualID"),
            AccrualsTypeID = (int)reader.GetInt64("AccrualsTypeID"),
            WorkID = (int)reader.GetInt64("WorkID"),
            Bonus = (float)reader.GetDouble("Bonus"),
            AccrualTotal = (float)reader.GetDouble("AccrualTotal")
        };
    }
    public void Delete(int id)
    {
        string query = "DELETE FROM Accruals WHERE AccrualID = @id";
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@id", id)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }
    public Accruals? Get(int id)
    {
        string query = "SELECT AccrualID, AccrualsTypeID, WorkID, Bonus, AccrualTotal FROM Accruals WHERE AccrualID = @id";
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@id", id)
        };
        
        return DAO.Instance.ReadSingle(query, FromReader, parameters);
    }
    public List<Accruals> Get()
    {
        string query = "SELECT AccrualID, AccrualsTypeID, WorkID, Bonus, AccrualTotal FROM Accruals";
        return DAO.Instance.ExecuteReader(query, FromReader);
    }
    public void Put(int id, Accruals item)
    {
        string query = @"UPDATE Accruals 
                        SET AccrualsTypeID = @AccrualsTypeID, 
                            WorkID = @WorkID, 
                            Bonus = @Bonus, 
                            AccrualTotal = @AccrualTotal 
                        WHERE AccrualID = @AccrualID";
        
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@AccrualID", item.AccrualID),
            new SqliteParameter("@AccrualsTypeID", item.AccrualsTypeID),
            new SqliteParameter("@WorkID", item.WorkID),
            new SqliteParameter("@Bonus", item.Bonus),
            new SqliteParameter("@AccrualTotal", item.AccrualTotal)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }
    public void Post(Accruals item)
    {
        string query = "INSERT INTO Accruals (AccrualsTypeID, WorkID, Bonus, AccrualTotal) VALUES (@AccrualsTypeID, @WorkID, @Bonus, @AccrualTotal)";
        
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@AccrualsTypeID", item.AccrualsTypeID),
            new SqliteParameter("@WorkID", item.WorkID),
            new SqliteParameter("@Bonus", item.Bonus),
            new SqliteParameter("@AccrualTotal", item.AccrualTotal)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }
}