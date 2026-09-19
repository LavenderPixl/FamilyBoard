using System.Security.Claims;
using Backend.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("task")]
public class TaskController : ControllerBase
{
    [HttpPost()]
    [Authorize]
    public ActionResult<Models.Task> CreateTask(TaskDto taskDto)
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = UserDataAccess.GetUser(userId);
        if (taskDto.UserId == 0) taskDto.UserId = null;

        if (user.FamilyId == 0) return Unauthorized("You are not in a family");
        if (!user.IsAdult) return Unauthorized("A non adult can not make a task");
        
        if (taskDto.UserId != null)
            if (!UserDataAccess.CheckIfUserExist((int)taskDto.UserId))
                return Conflict("The assigned user does not exist");

        var task = TaskDataAccess.CreateTask(taskDto.Name, taskDto.Reward, user.FamilyId, taskDto.UserId);
        if (task == null) return Problem();

        return Ok(task);
    }

    [HttpGet()]
    [Authorize]
    public ActionResult<Models.Task> GetTask(int id)
    {
        var task = TaskDataAccess.GetTask(id);
        if (task == null) return NotFound("Could not find the task");
        return task;
    }

    [HttpGet("get-tasks-for-your-family")]
    [Authorize]
    public ActionResult<List<Models.Task>> GetTasksForFamily()
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = UserDataAccess.GetUser(userId);

        if (user.FamilyId == null) return Conflict("You are not in a family");
        return Ok(TaskDataAccess.GetTasksForFamily(user.FamilyId));
    }
    
    [HttpGet("get-tasks-for-user")]
    [Authorize]
    public ActionResult<List<Models.Task>> GetTasksForFamily(int userId)
    {
        if (!UserDataAccess.CheckIfUserExist(userId)) return NotFound("The user was not found");

        return Ok(TaskDataAccess.GetTasksForUser(userId));
    }

    [HttpPatch]
    [Authorize]
    public ActionResult<Models.Task> UpdateTaskById(int taskId, TaskDto taskDto)
    {
        var task = TaskDataAccess.GetTask(taskId);
        if (task == null) return NotFound("Could not find a task with that id");
        
        if (taskDto.UserId == 0) taskDto.UserId = null;
        
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = UserDataAccess.GetUser(userId);
        if (!user.IsAdult) return Unauthorized("A non adult can not update a task");

        var updatedTask = TaskDataAccess.UpdateTask(taskId, taskDto.Name, taskDto.Reward, taskDto.UserId);
        if (updatedTask == null) return Problem();

        return Ok(updatedTask);
    }

    [HttpPatch("mark-task-as-completed")]
    [Authorize]
    public ActionResult<Models.Task> MarkTaskAsCompleted(int id)
    {
        var task = TaskDataAccess.GetTask(id);
        
        if (task == null) return NotFound("Could not find the task");
        if (task.Completed) return Conflict("Task is already completed");
        if (task.UserId == null) return Conflict("No user is assigned to the task ");
        
        var user = UserDataAccess.GetUser(task.UserId.Value);
        
        if (user == null)  return NotFound("Could not find the user");
        if (user.FamilyId == null) return Unauthorized("User is not in a family");

        var taskUpdated = TaskDataAccess.UpdateCompletedStatus(
            id, task.Reward, true, (int)task.UserId, task.GoalId);
        if (!taskUpdated) return Problem();
        var updatedTask = TaskDataAccess.GetTask(id);
        
        return Ok(updatedTask);
    }
    
    [HttpPatch("unmark-task-as-completed")]
    [Authorize]
    public ActionResult<Models.Task> UnmarkTaskAsCompleted(int id)
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = UserDataAccess.GetUser(userId);
        if (user == null) return Problem("Cannot find a user with this id");
        
        var task = TaskDataAccess.GetTask(id);
        if (task == null) return NotFound("Could not find the task");
        if (!task.Completed) return Conflict("Task is already not completed");
        if (task.UserId == null) return Conflict("No user is assigned to the task ");
        
        int pointsToBeRevert = task.Reward * -1;

        var taskUpdated = TaskDataAccess.UpdateCompletedStatus(
            id, pointsToBeRevert, false, (int)task.UserId, task.GoalId);
        if (!taskUpdated) return Problem();

        var updatedTask = TaskDataAccess.GetTask(id);
        return Ok(updatedTask);
    }

    [HttpDelete()]
    [Authorize]
    public IActionResult DeleteTask(int id)
    {
        if (!TaskDataAccess.DeleteTask(id))
            return Problem();

        return Ok();
    }
    
    public class TaskDto
    {
        public string Name { get; set; }
        public int Reward { get; set; }
        public int? UserId { get; set; }
    }
}