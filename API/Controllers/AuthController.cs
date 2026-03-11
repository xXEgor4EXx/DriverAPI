using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAPP.Common;
using MyAPP.API.Services;

namespace MyAPP.API.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;
    private readonly JwtService jwtService;
    private readonly IRefreshToken refreshDao;
    public AuthController(
    IAuthService authService,
    JwtService jwtService,
    IRefreshToken refreshDao)
    {
    this.authService = authService;
    this.jwtService = jwtService;
    this.refreshDao = refreshDao;
    }
    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
    var user = authService
        .AuthenticateAsync(request.Email, request.Password)
        .Result;

    if (user == null)
        return Unauthorized();

    var access = jwtService.Generate(user);
    var refresh = jwtService.GenerateRefreshToken();

    refreshDao.Add(new RefreshToken
    {
        UserId = user.UserID,
        Token = refresh,
        CreatedAt = DateTime.UtcNow,
        ExpiresAt = DateTime.UtcNow.AddDays(14),
        IsRevoked = false
    });
    return Ok(new
    {
        accessToken = access,
        refreshToken = refresh
    });
    }
    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] string refreshToken)
    {
    var stored = refreshDao.Get(refreshToken);

    if (stored == null ||
        stored.IsRevoked ||
        stored.ExpiresAt < DateTime.UtcNow)
        return Unauthorized();

    refreshDao.Revoke(stored.Id);

    var user = DAO.Instance.Users.Get(stored.UserId);
    if (user == null)
        return Unauthorized();


    var newAccess = jwtService.Generate(user);
    var newRefresh = jwtService.GenerateRefreshToken();

    refreshDao.Add(new RefreshToken
    {
        UserId = user.UserID,
        Token = newRefresh,
        CreatedAt = DateTime.UtcNow,
        ExpiresAt = DateTime.UtcNow.AddDays(14),
        IsRevoked = false
    });
    return Ok(new
    {
        accessToken = newAccess,
        refreshToken = newRefresh
    });
    }
}
