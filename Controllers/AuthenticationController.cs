// Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using MyApp.services;

namespace MyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly IConfiguration _cfg;

    public AuthController(UserService userService, IConfiguration cfg)
    {
        _userService = userService;
        _cfg = cfg;
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {

        var result = await _userService.ValidateUserCredentials(dto);
        if (result == null)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }
        return Ok( new { token = result } );
    }
}

public record LoginDto(string UserName, string Password);
