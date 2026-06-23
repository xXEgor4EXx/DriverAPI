using System.Data;
using Microsoft.Data.Sqlite;
using MyAPP.Common;
using MyAPP.Driver;

namespace MyAPP.Driver.DB;

public class DBRefreshTokens : IRefreshToken
{
    private RefreshToken FromReader(SqliteDataReader reader)
    {
        return new RefreshToken
        {
            Id = (int)reader.GetInt64("Id"),
            UserId = (int)reader.GetInt64("UserId"),
            Token = reader.GetString("Token"),
            CreatedAt = reader.GetDateTime("CreatedAt"),
            ExpiresAt = reader.GetDateTime("ExpiresAt"),
            IsRevoked = reader.GetBoolean("IsRevoked")
        };
    }

    public void Add(RefreshToken token)
    {
        string query = @"
            INSERT INTO RefreshTokens 
            (UserId, Token, CreatedAt, ExpiresAt, IsRevoked)
            VALUES (@UserId, @Token, @CreatedAt, @ExpiresAt, @IsRevoked)";

        var parameters = new List<SqliteParameter>
        {
            new("@UserId", token.UserId),
            new("@Token", token.Token),
            new("@CreatedAt", token.CreatedAt),
            new("@ExpiresAt", token.ExpiresAt),
            new("@IsRevoked", token.IsRevoked)
        };

        DAO.Instance.ExecuteNonQuery(query, parameters);
    }

    public RefreshToken? Get(string token)
    {
        string query = @"
            SELECT Id, UserId, Token, CreatedAt, ExpiresAt, IsRevoked
            FROM RefreshTokens
            WHERE Token = @Token";

        var parameters = new List<SqliteParameter>
        {
            new("@Token", token)
        };

        return DAO.Instance.ReadSingle(query, FromReader, parameters);
    }

    public void Revoke(int id)
    {
        string query = @"
            UPDATE RefreshTokens
            SET IsRevoked = 1
            WHERE Id = @Id";

        var parameters = new List<SqliteParameter>
        {
            new("@Id", id)
        };

        DAO.Instance.ExecuteNonQuery(query, parameters);
    }
}
