namespace MyAPP.Common;

public interface ITokenService
{
    string GenerateToken(User user);
}
