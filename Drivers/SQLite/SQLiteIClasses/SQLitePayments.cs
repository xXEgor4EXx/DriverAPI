using System.Data;
using Microsoft.Data.Sqlite;
using MyAPP.Common;
using MyAPP.Driver;

namespace MyAPP.Driver.DB;

class DBPayments : IDBPayments
{
     private Payments FromReader(SqliteDataReader reader)
    {
        return new Payments
        {
            PaymentID = (int)reader.GetInt64("PaymentID"),
            EmployeeID = (int)reader.GetInt64("EmployeeID"),
            AmountToPay = (float)reader.GetDouble("AmountToPay"),
            PaymentDate = reader.GetDateTime("PaymentDate"),
        };
    }
    public void Delete(int PaymentID)
    {
        string query = "DELETE FROM Payments WHERE PaymentID = @PaymentID";
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@PaymentID", PaymentID)
        };
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public Payments? Get(int PaymentsID)
    {
          string query = "SELECT PaymentID, EmployeeID, AmountToPay, PaymentDate FROM Payments WHERE PaymentID = @PaymentID";
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@PaymentsID", PaymentsID)
        };
        return DAO.Instance.ReadSingle(query, FromReader, parameters);
    }

    public List<Payments> Get()
    {
        string query = "SELECT PaymentID, EmployeeID, AmountToPay, PaymentDate FROM Payments";
        return DAO.Instance.ExecuteReader(query, FromReader);
    }

    public void Put(int id, Payments item)
    {
          string query = @"UPDATE Payments 
                        SET EmployeeID = @EmployeeID,
                            AmountToPay = @AmountToPay,
                            PaymentDate = @PaymentDate
                        WHERE PaymentID = @PaymentID";
        
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@PaymentID", item.PaymentID),
            new SqliteParameter("@EmployeeID", item.EmployeeID),
            new SqliteParameter("@AmountToPay", item.AmountToPay),
            new SqliteParameter("@PaymentDate", item.PaymentDate),

        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public void Post(Payments item)
    {
        string query = "INSERT INTO Payments (EmployeeID, AmountToPay, PaymentDate) VALUES (@EmployeeID, @AmountToPay, @PaymentDate);";
        List<SqliteParameter> parameters = new List<SqliteParameter>();
        parameters.Add(
            new SqliteParameter("@EmployeeID", item.EmployeeID)
        );
        parameters.Add(
            new SqliteParameter("@AmountToPay", item.AmountToPay)
        );
        parameters.Add(
            new SqliteParameter("@PaymentDate", item.PaymentDate)
        );
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }
}