using System.Security.Claims;
using Backend.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [HttpPost("CreateUser")]
    public IActionResult CreateUser(NewUser newUser)
    {
        string insertQuery = @"INSERT INTO users (email, username, hashed_password) VALUES (@email, @username, @hashedPassword)";
        string hashedPassword = hashPassword(newUser.Password);

        var conn = Database.Database.GetConn();
        
        var isEmailTaken = conn.ExecuteScalar<bool>("SELECT COUNT(1) FROM users WHERE email=@email",
            new { email = newUser.Email });

        if (isEmailTaken)
            return Conflict();
        
        conn.Execute(insertQuery, new { email = newUser.Email, username = newUser.Username, hashedPassword });

        return Ok();
    }

    [HttpGet("GetUser")]
    [Authorize]
    public ActionResult<User> GetUser(int userId)
    {
        string SelectQuery = @"SELECT * FROM users WHERE id = @id";

        var conn = Database.Database.GetConn();

        var user = conn.QueryFirstOrDefault<User>(SelectQuery, new { id = userId });
        if (user == null) return NotFound();
        
        return Ok(user);
    }
    
    [HttpGet("GetLoggedInUser")]
    [Authorize]
    public ActionResult<User> GetUser()
    {
        var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        return GetUser(userId);
    }

    [HttpDelete("DeleteLoggedInUser")]
    [Authorize]
    public IActionResult DeleteLoggedInUser()
    {
        string deletionquery = @"DELETE FROM users WHERE id=@id";
        var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var conn = Database.Database.GetConn();

        conn.Execute(deletionquery, new { id = userId });
        
        return Ok();
    }

    [HttpPut("UpdatePassword")]
    [Authorize]
    public IActionResult ChangePassword(UpdatePassword updatePassword)
    {
        var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        string userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (!Models.User.IsPasswordValid(userEmail, updatePassword.OldPassword)) return Forbid();

        string newHash = hashPassword(updatePassword.NewPassword);
        UpdateSingleColumnForUser("hashed_password", newHash, userId);
        
        return Ok();
    }
    
    private string hashPassword(string password)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password, 5);
    }

    private void UpdateSingleColumnForUser<T>(string column, T newValue, int userId)
    {
        var conn = Database.Database.GetConn();
        string updateQuery = @$"UPDATE users SET {column} = @newValue WHERE id = @id";

        conn.Execute(updateQuery, new { newValue = newValue, id = userId });
    }
    
    public class UpdatePassword
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
    
    public class NewUser
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}