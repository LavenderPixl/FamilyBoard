using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]/")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // [Route("Auth/login")]
    [HttpPost("login")]
    public IActionResult Login(LoginUser loginUser)
    {
        var conn = Database.Database.GetConn();
        string hashedPasswordQuery = @"SElECT hashed_password FROM users WHERE email = @email";
        string selectUserQuery = @"SELECT * FROM users WHERE email = @email";

        string? hashedPassword = conn.QueryFirstOrDefault<string>(hashedPasswordQuery, new { email = loginUser.Email });
        if (hashedPassword == null) return Unauthorized();
        // Checks if the password matches our hashed
        if (!BCrypt.Net.BCrypt.EnhancedVerify(loginUser.Password, hashedPassword)) return Unauthorized();

        var user = conn.QueryFirstOrDefault<User>(selectUserQuery, new { email = loginUser.Email });
        if (user == null) return Problem();
        var token = GenerateJwtToken(user);
        
        return Ok(token);
    }
    
    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(Convert.ToDouble(jwtSettings["ExpiryHours"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public class LoginUser
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}