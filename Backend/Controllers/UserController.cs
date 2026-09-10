using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [HttpPost("create-user")]
    public IActionResult CreateUser(NewUser newUser)
    {
        string insertQuery = @"INSERT INTO users (email, username, hashed_password) VALUES (@email, @username, @hashedPassword)";
        string hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(newUser.Password, 5);

        var conn = Database.Database.GetConn();
        
        var isEmailTaken = conn.ExecuteScalar<bool>("SELECT COUNT(1) FROM users WHERE email=@email",
            new { email = newUser.Email });

        if (isEmailTaken)
            return Conflict();
        
        conn.Query(insertQuery, new { email = newUser.Email, username = newUser.Username, hashedPassword });

        return Ok();
    }
    
    public class NewUser
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}