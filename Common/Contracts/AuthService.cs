namespace MyAPP.Common;

public interface IAuthService
{
    Task<User?> AuthenticateAsync(string email, string password);
}
