using MyAPP.Common;
using MyAPP.Singletons;

namespace MyAPP.API.Services;

public class AuthService : IAuthService
{
    private readonly IDatabase db;

    public AuthService()
    {
        db = DAO.Instance;
    }

    public Task<User?> AuthenticateAsync(string email, string password)
    {
        // 1. ищем пользователя
        var user = db.Users
            .Get()
            .FirstOrDefault(u => u.Email == email);

        if (user == null)
            return Task.FromResult<User?>(null);

        // 2. проверяем активность
        if (user.IsActive != 1)
            return Task.FromResult<User?>(null);

        // 3. проверяем пароль
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return Task.FromResult<User?>(null);

        // 4. успех
        return Task.FromResult<User?>(user);
    }
}
