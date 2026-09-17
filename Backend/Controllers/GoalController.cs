using System.Security.Claims;
using Backend.DataAccess;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("goal")]

public class GoalController : ControllerBase
{
    [HttpPost()]
    [Authorize]
    public ActionResult CreateGoal(GoalDto goalDto)
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        User? user = UserDataAccess.GetUser(userId);
        if (user == null) return BadRequest("Cannot find user with this id");
        if (!user.IsAdult) return Unauthorized("Only an adult can create a goal");
        
        bool exists = UserDataAccess.CheckIfUserExist(goalDto.UserId);
        if (!exists) return BadRequest("Cannot find a user with this id");
        
        Goal createdGoal = GoalDataAccess.CreateGoal(goalDto.Name, goalDto.Cost, goalDto.UserId);
        return Ok(createdGoal); 
    }

    [HttpDelete()]
    [Authorize]
    public ActionResult DeleteGoal(int goalId)
    {
        var loggedInUser = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        User? user = UserDataAccess.GetUser(loggedInUser);
        if (user == null) return BadRequest("Cannot find user with this id");
        if (!user.IsAdult) return Unauthorized("Only an adult can delete a goal");
        
        bool deleted = GoalDataAccess.DeleteGoal(goalId);
        if (!deleted) return Problem("Could not delete goal");
        return Ok();        
    }
    

    // [HttpPatch]
    // [Authorize]
    // public ActionResult UpdateGoal(GoalDto goalDto)
    // {
    //     
    //     
    // }
    // [HttpDelete]
    // [HttpGet]
    
    // Create
    // Delete
    // Make active
    // Edit
    // Complete
    
    public class GoalDto
    {
        public string Name { get; set; }
        public int Cost { get; set; }
        public int UserId { get; set; }
    }
}