using System.Data;
using Microsoft.Data.Sqlite;
using MyAPP.Common;
using MyAPP.Driver;

namespace MyAPP.Driver.DB;

class DBUsers : IDBUsers
{
    private User FromReader(SqliteDataReader reader)
    {
        return new User
        {
            UserID = (int)reader.GetInt64("UserID"),
            Email = reader.GetString("Email"),
            PasswordHash = reader.GetString("PasswordHash"),
            Role = reader.GetString("Role"),
            IsActive = reader.GetInt32("IsActive"),
            CreatedAt = reader.GetDateTime("CreatedAt"),
            UpdatedAt = reader.IsDBNull("UpdatedAt")
                ? null
                : reader.GetDateTime("UpdatedAt")
        };
    }

    public User? Get(int id)
    {
        string query = """
            SELECT UserID, Email, PasswordHash, Role, IsActive, CreatedAt, UpdatedAt
            FROM Users
            WHERE UserID = @id
        """;

        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@id", id)
        };

        return DAO.Instance.ReadSingle(query, FromReader, parameters);
    }

    public User? GetByEmail(string email)
    {
        string query = """
            SELECT UserID, Email, PasswordHash, Role, IsActive, CreatedAt, UpdatedAt
            FROM Users
            WHERE Email = @email AND IsActive = 1
        """;

        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@email", email)
        };

        return DAO.Instance.ReadSingle(query, FromReader, parameters);
    }

    public List<User> Get()
    {
        string query = """
            SELECT UserID, Email, PasswordHash, Role, IsActive, CreatedAt, UpdatedAt
            FROM Users
        """;

        return DAO.Instance.ExecuteReader(query, FromReader);
    }

    public void Post(User item)
    {
        string query = """
            INSERT INTO Users (Email, PasswordHash, Role, IsActive)
            VALUES (@Email, @PasswordHash, @Role, @IsActive)
        """;

        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@Email", item.Email),
            new SqliteParameter("@PasswordHash", item.PasswordHash),
            new SqliteParameter("@Role", item.Role),
            new SqliteParameter("@IsActive", item.IsActive)
        };

        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public void Put(int id, User item)
    {
        string query = """
            UPDATE Users
            SET Email = @Email,
                PasswordHash = @PasswordHash,
                Role = @Role,
                IsActive = @IsActive,
                UpdatedAt = CURRENT_TIMESTAMP
            WHERE UserID = @UserID
        """;

        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@UserID", id),
            new SqliteParameter("@Email", item.Email),
            new SqliteParameter("@PasswordHash", item.PasswordHash),
            new SqliteParameter("@Role", item.Role),
            new SqliteParameter("@IsActive", item.IsActive)
        };

        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public void Delete(int id)
    {
        string query = "UPDATE Users SET IsActive = 0 WHERE UserID = @id";

        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@id", id)
        };

        DAO.Instance.ExecuteNonQuery(query, parameters);
    }
}
