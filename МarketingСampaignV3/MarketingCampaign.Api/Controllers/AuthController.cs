using Microsoft.AspNetCore.Mvc;
using MarketingCampaign.Api.Services;

namespace MarketingCampaign.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;

    public AuthController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // В данном тестовом задании проверка идёт по пользователям, которые жёстко заданы для демонстрации.
        // В production — проверка обязательно через БД и хеш пароли.
        var user = request.Username switch
        {
            "admin" when request.Password == "admin123" => (Role: "AdministratorMarketingCampaign", Valid: true),
            "viewer" when request.Password == "viewer123" => (Role: "ViewerMarketingCampaign", Valid: true),
            _ => (Role: "", Valid: false)
        };

        if (!user.Valid)
            return Unauthorized(new { message = "Неверный логин или пароль." });

        var token = _tokenService.GenerateToken(request.Username, user.Role);
        return Ok(new { token, role = user.Role });
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}