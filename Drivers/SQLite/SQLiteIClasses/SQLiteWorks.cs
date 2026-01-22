using System.Data;
using Microsoft.Data.Sqlite;
using MyAPP.Common;
using MyAPP.Driver;

namespace MyAPP.Driver.DB;

class DBWorks : IDBWorks
{
    private Works FromReader(SqliteDataReader reader)
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
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@id", id)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public Works? Get(int id)
    {
        string query = "SELECT WorkID, EmployeeID, OperationID, WorkDate, Quantity, RejectedQuantity FROM Works WHERE WorkID = @id";
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@id", id)
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
        
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@WorkID", item.WorkID),
            new SqliteParameter("@EmployeeID", item.EmployeeID),
            new SqliteParameter("@OperationID", item.OperationID),
            new SqliteParameter("@WorkDate", item.WorkDate),
            new SqliteParameter("@Quantity", item.Quantity),
            new SqliteParameter("@RejectedQuantity", item.RejectedQuantity)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public void Post(Works item)
    {
        string query = "INSERT INTO Works (WorkID, EmployeeID, OperationID, WorkDate, Quantity, RejectedQuantity) VALUES (@WorkID, @EmployeeID, @OperationID, @WorkDate, @Quantity, @RejectedQuantity)";
        
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@WorkID", item.WorkID),
            new SqliteParameter("@EmployeeID", item.EmployeeID),
            new SqliteParameter("@OperationID", item.OperationID),
            new SqliteParameter("@WorkDate", item.WorkDate),
            new SqliteParameter("@Quantity", item.Quantity),
            new SqliteParameter("@RejectedQuantity", item.RejectedQuantity)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }
}