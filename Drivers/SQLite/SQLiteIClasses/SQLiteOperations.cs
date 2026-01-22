using System.Data;
using Microsoft.Data.Sqlite;
using MyAPP.Common;
using MyAPP.Driver;

namespace MyAPP.Driver.DB;

class DBOperations : IDBOperations
{
    private Operations FromReader(SqliteDataReader reader)
    {
        return new Operations
        {
            OperationID = (int)reader.GetInt64("OperationID"),
            Description = reader.GetString("Description"),
            RatePerUnit = (float)reader.GetDouble("RatePerUnit")
        };
    }
    public void Delete(int OperationID)
    {
        string query = "DELETE FROM Operations WHERE OperationID = @OperationID";
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@OperationID", OperationID)
        };
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public Operations? Get(int OperationID)
    {
        string query = "SELECT OperationID, Description, RatePerUnit FROM Operations WHERE OperationID = @OperationID";
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@OperationID", OperationID)
        };
        return DAO.Instance.ReadSingle(query, FromReader, parameters);
    }

    public List<Operations> Get()
    {
        string query = "SELECT OperationID, Description, RatePerUnit FROM OperationID";
        return DAO.Instance.ExecuteReader(query, FromReader);
    }

    public void Put(int OperationID, Operations item)
    {
        string query = @"UPDATE Operations
                        SET Description = @Description, 
                            RatePerUnit = @RatePerUnit
                        WHERE OperationID = @OperationID";
        
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@OperationID", item.OperationID),
            new SqliteParameter("@Description", item.Description),
            new SqliteParameter("@RatePerUnit", item.RatePerUnit)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public void Post(Operations item)
    {
        string query = "INSERT INTO Operations (OperationID,Description,RatePerUnit) VALUES(:OperationID,:Description,:RatePerUnit);";
        List<SqliteParameter> parameters = new List<SqliteParameter>();
        parameters.Add(
            new SqliteParameter("OperationID",item.OperationID)
        );
        parameters.Add(
            new SqliteParameter("Description",item.Description)
        );
        parameters.Add(
            new SqliteParameter("RatePerUnit",item.RatePerUnit)
        );
       
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }
}