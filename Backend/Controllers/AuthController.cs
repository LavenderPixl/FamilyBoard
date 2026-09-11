using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Backend.DataAccess;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("log-in")]
    public ActionResult<Tokens> Login(LoginUser loginUser)
    {
        if (!Models.User.IsPasswordValid(loginUser.Email, loginUser.Password)) return Unauthorized();

        var user = UserDataAccess.GetUserFromEmail(loginUser.Email);
        if (user == null) return Problem();
        var jwt = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken(user.Id);
        Tokens tokens = new Tokens { Jwt = jwt, RefreshToken = refreshToken };
        
        return Ok(tokens);
    }

    [HttpGet("refresh-jwt")]
    public ActionResult<string> RefreshJwt([FromBody]string token)
    {
        var refreshToken = RefreshTokenDataAccess.GetRefreshTokenFromToken(token);
        if (refreshToken == null ) return Unauthorized();
        if (refreshToken.Expiration < DateTime.UtcNow) return Unauthorized();
        
        var user = UserDataAccess.GetUser(refreshToken.UserId);
        if (user == null) return Unauthorized();
        
        var jwt = GenerateJwtToken(user);
        
        return Ok(jwt);
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
            expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(jwtSettings["ExpiryMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken(int userId)
    {
        var refreshTokenSettings = _configuration.GetSection("RefreshToken");
        
        string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        DateTime expiration = DateTime.UtcNow.AddHours(Convert.ToInt32(refreshTokenSettings["ExpiryDays"]));

        RefreshTokenDataAccess.CreateRefreshToken(token, expiration, userId);

        return token;
    }
    
    public class LoginUser
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    
    public class Tokens
    {
        public string Jwt { get; set; }
        public string RefreshToken { get; set; }
    }
}