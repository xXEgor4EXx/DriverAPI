using System.Data;
using MyAPP.Common;
using MySqlConnector;
using MyAPP.Driver.MariaDB;

namespace MyAPP.Driver.MariaDB;

class MariaDBEmployers : IDBEmployers
{
     private Employers FromReader(MySqlDataReader reader)
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
        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@EmployeeID", EmployeeID)
        };
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public Employers? Get(int EmployeeID)
    {
        string query = "SELECT EmployeeID, FullName, Phone FROM Employers WHERE EmployeeID = @EmployeeID";
        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@EmployeeID", EmployeeID)
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
        
        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@EmployeeID", item.EmployeeID),
            new MySqlParameter("@FullName", item.FullName),
            new MySqlParameter("@Phone", item.Phone)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public void Post(Employers item)
    {
        string query = "INSERT INTO Employers (EmployeeID,FullName,Phone) VALUES(@EmployeeID,@FullName,@Phone);";
        List<MySqlParameter> parameters = new List<MySqlParameter>();
        parameters.Add(
            new MySqlParameter("EmployeeID",item.EmployeeID)
        );
        parameters.Add(
            new MySqlParameter("FullName",item.FullName)
        );
        parameters.Add(
            new MySqlParameter("Phone",item.Phone)
        );
       
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }
}