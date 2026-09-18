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
    public ActionResult<Goal> CreateGoal(GoalDto goalDto)
    {
        if (goalDto.Cost <= 0) return BadRequest("Goal must cost more than 0");
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
    public IActionResult DeleteGoal(int goalId)
    {
        var loggedInUser = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        User? user = UserDataAccess.GetUser(loggedInUser);
        if (user == null) return BadRequest("Cannot find user with this id");
        if (!user.IsAdult) return Unauthorized("Only an adult can delete a goal");
        
        bool deleted = GoalDataAccess.DeleteGoal(goalId);
        if (!deleted) return Problem("Could not delete goal");
        return Ok();        
    }
    
    [HttpPatch]
    [Authorize]
    public ActionResult<Goal> UpdateGoal(int goalId, GoalDto goalDto)
    {
        var loggedInUser = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        User? user = UserDataAccess.GetUser(loggedInUser);
        if (user == null) return BadRequest("Cannot find user with this id");
        if (!user.IsAdult) return Unauthorized("Only an adult can update goals");
        
        Goal? updatedGoal = GoalDataAccess.UpdateGoal(goalId, goalDto.Name, goalDto.Cost, goalDto.UserId );
        if (updatedGoal == null) return Problem("Could not update goal.");
        return Ok(updatedGoal);
    }

    [HttpGet]
    [Authorize]
    public ActionResult<Goal> GetGoal(int goalId)
    {
        Goal? goal = GoalDataAccess.GetGoal(goalId);
        if (goal == null) return BadRequest("Could not find goal with this id");
        return Ok(goal);
    }

    public ActionResult<List<Goal>> GetGoals(int userId)
    {
        User? user = UserDataAccess.GetUser(userId);
        if (user == null) return BadRequest("Could not find a user, with this id.");
        List<Goal>? goals = GoalDataAccess.GetUserGoals(userId);
        if (goals == null) return Ok("User has no goals associated yet.");
        return Ok(goals);
    }
    // Get all goals for user

    [HttpPatch("assign-goal")]
    [Authorize]
    public IActionResult AssignGoal(int assigneeId, int goalId)
    {
        var loggedInUser = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        User? user = UserDataAccess.GetUser(loggedInUser);
        if (user == null) return BadRequest("Cannot find user with this id");
        if (!user.IsAdult) return Unauthorized("Only an adult can assign goals");
        
        User? assignee = UserDataAccess.GetUser(assigneeId);
        if (assignee == null) return BadRequest("Cannot find user with this id");
                
        bool assigned = GoalDataAccess.AssignGoal(assigneeId, goalId);
        if (!assigned) return Problem("Could not assign goal.");
        return Ok();
    }

    [HttpPatch("make-active")]
    [Authorize]
    public ActionResult<Goal> MakeActive(int assigneeId, int goalId)
    {
        User? user = UserDataAccess.GetUser(assigneeId);
        if (user == null) return BadRequest("Could not find user with this id.");
        
        Goal? activeGoal = GoalDataAccess.MakeActive(assigneeId, goalId);
        return Ok(activeGoal);
    }


    
    [HttpGet("check-if-completed")]
    [Authorize]
    public ActionResult<bool?> CheckIfCompleted(int goalId)
    {
        bool? completed = GoalDataAccess.CheckIfCompleted(goalId);
        if (completed == null) return BadRequest("Could not find goal with that id.");
        return Ok(completed);
    }
    
    public class GoalDto
    {
        public string Name { get; set; }
        public int Cost { get; set; }
        public int UserId { get; set; }
    }
}