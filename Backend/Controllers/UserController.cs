using System.Security.Claims;
using Backend.DataAccess;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    [HttpPost("create-user")]
    public IActionResult CreateUser(NewUser newUser)
    {
        if (!UserDataAccess.CreateUser(newUser.Email, newUser.Username, newUser.Password))
            return Conflict();
            
        return Ok();
    }

    [HttpGet("get-user")]
    [Authorize]
    public ActionResult<User> GetUser(int userId)
    {
        var user = UserDataAccess.GetUser(userId);
        if (user == null) return NotFound();
        
        return Ok(user);
    }
    
    [HttpGet("get-logged-in-user")]
    [Authorize]
    public ActionResult<User> GetUser()
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return GetUser(userId);
    }

    [HttpDelete("delete-logged-in-user")]
    [Authorize]
    public IActionResult DeleteLoggedInUser()
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

        if (!UserDataAccess.DeleteUser(userId))
            return StatusCode(500);
        
        // Internal Server Error
        return Ok();
    }

    [HttpPut("update-password")]
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
    
    [HttpPut("give-points")]
    [Authorize]
    public ActionResult<User> GivePoints(PointsToGive pointsToGive)
    {
        if (!UserDataAccess.AddPoints(pointsToGive.Id, pointsToGive.Amount)) return NotFound("dsa");

        var updatedUser = UserDataAccess.GetUser(pointsToGive.Id);
        return Ok(updatedUser);
    }

    [HttpPut("update-adult-status")]
    [Authorize]
    public ActionResult<User> ChangeAdultStatus(UpdateAdultStatus adultStatus)
    {
        if (!UserDataAccess.UpdateAdultStatus(adultStatus.UserId, adultStatus.IsAdult)) return NotFound();

        var updateUser = UserDataAccess.GetUser(adultStatus.UserId);
        return Ok(updateUser);
    }

    [HttpPut("update-username")]
    [Authorize]
    public ActionResult<User> ChangeUsername([FromBody]string username)
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (!UserDataAccess.UpdateUsername(userId, username)) return NotFound();
        
        var updateUser = UserDataAccess.GetUser(userId);
        return Ok(updateUser);
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

    public class PointsToGive
    {
        public int Id { get; set; }
        public int Amount { get; set; }
    }
    
    public class UpdateAdultStatus
    {
        public int UserId { get; set; }
        public bool IsAdult { get; set; }
    }
    
}