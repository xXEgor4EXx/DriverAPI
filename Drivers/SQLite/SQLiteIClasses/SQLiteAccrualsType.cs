using Microsoft.Data.Sqlite;
using System.Data;
using MyAPP.Common;
using MyAPP.Driver;

namespace MyAPP.Driver.DB;

class DBAccrualsType : IDBAccrualsType
{
    private AccrualsType FromReader(SqliteDataReader reader)
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
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@AccrualsTypeID", AccrualsTypeID)
        };
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public AccrualsType? Get(int AccrualsTypeID)
    {
        string query = "SELECT AccrualsTypeID, Name, position FROM AccrualsType WHERE AccrualsTypeID = @AccrualsTypeID";
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@AccrualsTypeID", AccrualsTypeID)
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
        
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@AccrualsTypeID", item.AccrualsTypeID),
            new SqliteParameter("@Name", item.Name),
            new SqliteParameter("@position", item.position)
        };
        
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public void Post(AccrualsType item)
    {
        string query = "INSERT INTO AccrualsType (AccrualsTypeID,Name,position,) VALUES(@AccrualsTypeID,@Name,@position);";
        List<SqliteParameter> parameters = new List<SqliteParameter>();
        parameters.Add(
            new SqliteParameter("@AccrualsTypeID",item.AccrualsTypeID)
        );
        parameters.Add(
            new SqliteParameter("@Name",item.Name)
        );
        parameters.Add(
            new SqliteParameter("@position",item.position)
        );
       
        DAO.Instance.ExecuteNonQuery(query, parameters);
    }
}