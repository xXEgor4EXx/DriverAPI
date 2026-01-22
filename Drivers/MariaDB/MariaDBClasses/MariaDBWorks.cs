using System.Data;
using MyAPP.Common;
using MySqlConnector;
using MyAPP.Driver.MariaDB;

namespace MyAPP.Driver.MariaDB;

class MariaDBWorks : IDBWorks
{
    private Works FromReader(MySqlDataReader reader)
    {
        return new Works
        {
            WorkID = (int)reader.GetInt64("WorkID"),
            EmployeeID = (int)reader.GetInt64("EmployeeID"),
            OperationID = (int)reader.GetInt64("OperationID"),
            WorkDate = reader.GetDateTime("WorkDate"),
            Quantity = (int)reader.GetInt64("Quantity"),
            RejectedQuantity = (int)reader.GetInt64("RejectedQuantity")
        };
    }

    public void Delete(int id)
    {
        string query = "DELETE FROM Works WHERE WorkID = @id";
        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@id", id)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public Works? Get(int id)
    {
        string query = "SELECT WorkID, EmployeeID, OperationID, WorkDate, Quantity, RejectedQuantity FROM Works WHERE WorkID = @id";
        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@id", id)
        };
        
        return DAO.Instance.ReadSingle(query, FromReader, parameters);
    }

    public List<Works> Get()
    {
        string query = "SELECT WorkID, EmployeeID, OperationID, WorkDate, Quantity, RejectedQuantity FROM Works";
        return DAO.Instance.ExecuteReader(query, FromReader);
    }

    public void Put(int id, Works item)
    {
        string query = @"UPDATE Works 
                        SET EmployeeID = @EmployeeID,
                            OperationID = @OperationID,
                            WorkDate = @WorkDate,
                            Quantity = @Quantity,
                            RejectedQuantity = @RejectedQuantity
                        WHERE WorkID = @WorkID";
        
        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@WorkID", item.WorkID),
            new MySqlParameter("@EmployeeID", item.EmployeeID),
            new MySqlParameter("@OperationID", item.OperationID),
            new MySqlParameter("@WorkDate", item.WorkDate),
            new MySqlParameter("@Quantity", item.Quantity),
            new MySqlParameter("@RejectedQuantity", item.RejectedQuantity)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public void Post(Works item)
    {
        string query = "INSERT INTO Works (WorkID, EmployeeID, OperationID, WorkDate, Quantity, RejectedQuantity) VALUES (@WorkID, @EmployeeID, @OperationID, @WorkDate, @Quantity, @RejectedQuantity)";
        
        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@WorkID", item.WorkID),
            new MySqlParameter("@EmployeeID", item.EmployeeID),
            new MySqlParameter("@OperationID", item.OperationID),
            new MySqlParameter("@WorkDate", item.WorkDate),
            new MySqlParameter("@Quantity", item.Quantity),
            new MySqlParameter("@RejectedQuantity", item.RejectedQuantity)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }
}