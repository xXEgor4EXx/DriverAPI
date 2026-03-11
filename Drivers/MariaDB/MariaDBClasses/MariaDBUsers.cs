using System.Data;
using MyAPP.Common;
using MySqlConnector;
using MyAPP.Driver;

namespace MyAPP.Driver.MariaDB;

class MariaDBUsers : IDBUsers
{
    private User FromReader(MySqlDataReader reader)
    {
        return new User
        {
            UserID = reader.GetInt32("UserID"),
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

        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@id", id)
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

        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@email", email)
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

        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@Email", item.Email),
            new MySqlParameter("@PasswordHash", item.PasswordHash),
            new MySqlParameter("@Role", item.Role),
            new MySqlParameter("@IsActive", item.IsActive)
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

        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@UserID", id),
            new MySqlParameter("@Email", item.Email),
            new MySqlParameter("@PasswordHash", item.PasswordHash),
            new MySqlParameter("@Role", item.Role),
            new MySqlParameter("@IsActive", item.IsActive)
        };

        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public void Delete(int id)
    {
        string query = "UPDATE Users SET IsActive = 0 WHERE UserID = @id";

        var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@id", id)
        };

        DAO.Instance.ExecuteNonQuery(query, parameters);
    }
}
