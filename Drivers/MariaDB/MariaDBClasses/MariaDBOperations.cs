using System.Data;
using MyAPP.Common;
using MySqlConnector;
using MyAPP.Driver.MariaDB;

namespace MyAPP.Driver.MariaDB;

class MariaDBOperations : IDBOperations
{
    private Operations FromReader(MySqlDataReader reader)
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
        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@OperationID", OperationID)
        };
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public Operations? Get(int OperationID)
    {
        string query = "SELECT OperationID, Description, RatePerUnit FROM Operations WHERE OperationID = @OperationID";
        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@OperationID", OperationID)
        };
        return DAO.Instance.ReadSingle(query, FromReader, parameters);
    }

    public List<Operations> Get()
    {
        string query = "SELECT OperationID, Description, RatePerUnit FROM Operations";
        return DAO.Instance.ExecuteReader(query, FromReader);
    }

    public void Put(int OperationID, Operations item)
    {
        string query = @"UPDATE Operations
                        SET Description = @Description, 
                            RatePerUnit = @RatePerUnit
                        WHERE OperationID = @OperationID";
        
        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@OperationID", item.OperationID),
            new MySqlParameter("@Description", item.Description),
            new MySqlParameter("@RatePerUnit", item.RatePerUnit)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public void Post(Operations item)
    {
        string query = "INSERT INTO Operations (OperationID,Description,RatePerUnit) VALUES(:OperationID,:Description,:RatePerUnit);";
        List<MySqlParameter> parameters = new List<MySqlParameter>();
        parameters.Add(
            new MySqlParameter("OperationID",item.OperationID)
        );
        parameters.Add(
            new MySqlParameter("Description",item.Description)
        );
        parameters.Add(
            new MySqlParameter("RatePerUnit",item.RatePerUnit)
        );
       
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }
}