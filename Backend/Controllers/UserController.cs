using System.Security.Claims;
using Backend.DataAccess;
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
        if (!UserDataAccess.CreateUser(newUser.Email, newUser.Username, newUser.Password))
            return Conflict();
            
        return Ok();
    }

    [HttpGet("GetUser")]
    [Authorize]
    public ActionResult<User> GetUser(int userId)
    {
        var user = UserDataAccess.GetUser(userId);
        if (user == null) return NotFound();
        
        return Ok(user);
    }
    
    [HttpGet("GetLoggedInUser")]
    [Authorize]
    public ActionResult<User> GetUser()
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return GetUser(userId);
    }

    [HttpDelete("DeleteLoggedInUser")]
    [Authorize]
    public IActionResult DeleteLoggedInUser()
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

        if (!UserDataAccess.DeleteUser(userId))
            return StatusCode(500);
        
        // Internal Server Error
        return Ok();
    }

    [HttpPut("UpdatePassword")]
    [Authorize]
    public IActionResult ChangePassword(UpdatePassword updatePassword)
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        string userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (userEmail == null) return Forbid();

        if (!UserDataAccess.ChangePassword(userId, userEmail, updatePassword.OldPassword, updatePassword.NewPassword))
            return Forbid();
        
        return Ok();
    }

    public class NewUser
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class UpdatePassword
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
    
}