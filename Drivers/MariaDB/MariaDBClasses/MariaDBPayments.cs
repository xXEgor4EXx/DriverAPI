using System.Data;
using MyAPP.Common;
using MySqlConnector;
using MyAPP.Driver.MariaDB;

namespace MyAPP.Driver.MariaDB;

class MariaDBPayments : IDBPayments
{
     private Payments FromReader(MySqlDataReader reader)
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
        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@PaymentID", PaymentID)
        };
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public Payments? Get(int PaymentsID)
    {
          string query = "SELECT PaymentID, EmployeeID, AmountToPay, PaymentDate FROM Payments WHERE PaymentID = @PaymentID";
        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@PaymentsID", PaymentsID)
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
        
        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@PaymentID", item.PaymentID),
            new MySqlParameter("@EmployeeID", item.EmployeeID),
            new MySqlParameter("@AmountToPay", item.AmountToPay),
            new MySqlParameter("@PaymentDate", item.PaymentDate),

        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public void Post(Payments item)
    {
        string query = "INSERT INTO Payments (PaymentID,EmployeeID,AmountToPay,PaymentDate) VALUES(:PaymentID,:FullName,:AmountToPay,:PaymentDate);";
        List<MySqlParameter> parameters = new List<MySqlParameter>();
        parameters.Add(
            new MySqlParameter("PaymentID",item.PaymentID)
        );
        parameters.Add(
            new MySqlParameter("EmployeeID",item.EmployeeID)
        );
        parameters.Add(
            new MySqlParameter("AmountToPay",item.AmountToPay)
        );
        parameters.Add(
            new MySqlParameter("PaymentDate",item.PaymentDate)
        );
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }
}