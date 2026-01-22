namespace MyAPP.Common;

public class RefreshToken
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; }
}
