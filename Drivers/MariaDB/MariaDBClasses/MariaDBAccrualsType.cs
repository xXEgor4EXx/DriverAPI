using System.Data;
using MyAPP.Common;
using MySqlConnector;
using MyAPP.Driver.MariaDB;

namespace MyAPP.Driver.MariaDB;

class MariaDBAccrualsType : IDBAccrualsType
{
    private AccrualsType FromReader(MySqlDataReader reader)
    {
        return new AccrualsType
        {
            AccrualsTypeID = (int)reader.GetInt64("AccrualsTypeID"),
            Name = reader.GetString("Name"),
            position = (int)reader.GetInt64("position")
        };
    }

    public void Delete(int AccrualsTypeID)
    {
        string query = "DELETE FROM AccrualsType WHERE AccrualsTypeID = @AccrualsTypeID";
        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@AccrualsTypeID", AccrualsTypeID)
        };
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public AccrualsType? Get(int AccrualsTypeID)
    {
        string query = "SELECT AccrualsTypeID, Name, position FROM AccrualsType WHERE AccrualsTypeID = @AccrualsTypeID";
        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@AccrualsTypeID", AccrualsTypeID)
        };
        return DAO.Instance.ReadSingle(query, FromReader, parameters);
    }

    public List<AccrualsType> Get()
    {
        string query = "SELECT AccrualsTypeID, Name, position FROM AccrualsType";
        return DAO.Instance.ExecuteReader(query, FromReader);
    }

    public void Put(int id, AccrualsType item)
    {
        string query = @"UPDATE AccrualsType
                        SET Name = @Name, 
                            position = @position
                        WHERE AccrualsTypeID = @AccrualsTypeID";
        
        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@AccrualsTypeID", item.AccrualsTypeID),
            new MySqlParameter("@Name", item.Name),
            new MySqlParameter("@position", item.position)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public void Post(AccrualsType item)
    {
        string query = "INSERT INTO AccrualsType (AccrualsTypeID,Name,position,) VALUES(@AccrualsTypeID,@Name,@position);";
        List<MySqlParameter> parameters = new List<MySqlParameter>();
        parameters.Add(
            new MySqlParameter("@AccrualsTypeID",item.AccrualsTypeID)
        );
        parameters.Add(
            new MySqlParameter("@Name",item.Name)
        );
        parameters.Add(
            new MySqlParameter("@position",item.position)
        );
       
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }
}