using System.Data;
using Microsoft.Data.Sqlite;
using MyAPP.Common;
using MyAPP.Driver;

namespace MyAPP.Driver.DB;

class DBEmployers : IDBEmployers
{
     private Employers FromReader(SqliteDataReader reader)
    {
        return new Employers
        {
            EmployeeID = (int)reader.GetInt64("EmployeeID"),
            FullName = reader.GetString("FullName"),
            Phone = reader.GetString("Phone")
        };
    }
    public void Delete(int EmployeeID)
    {
        string query = "DELETE FROM Employers WHERE EmployeeID = @EmployeeID";
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@EmployeeID", EmployeeID)
        };
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public Employers? Get(int EmployeeID)
    {
        string query = "SELECT EmployeeID, FullName, Phone FROM Employers WHERE EmployeeID = @EmployeeID";
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@EmployeeID", EmployeeID)
        };
        return DAO.Instance.ReadSingle(query, FromReader, parameters);throw new NotImplementedException();
    }

    public List<Employers> Get()
    {
        string query = "SELECT EmployeeID, FullName, Phone FROM Employers";
        return DAO.Instance.ExecuteReader(query, FromReader);
    }

    public void Put(int id, Employers item)
    {
         string query = @"UPDATE Employers
                        SET FullName = @FullName, 
                            Phone = @Phone
                        WHERE EmployeeID = @EmployeeID";
        
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@EmployeeID", item.EmployeeID),
            new SqliteParameter("@FullName", item.FullName),
            new SqliteParameter("@Phone", item.Phone)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public void Post(Employers item)
    {
        string query = "INSERT INTO Employers (FullName, Phone) VALUES (@FullName, @Phone);";
        List<SqliteParameter> parameters = new List<SqliteParameter>();
        parameters.Add(
            new SqliteParameter("@FullName", item.FullName)
        );
        parameters.Add(
            new SqliteParameter("@Phone", item.Phone)
        );
       
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }
}