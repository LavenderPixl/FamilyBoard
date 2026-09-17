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
    public ActionResult<Models.Task> CreateTask(NewTask newTask)
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = UserDataAccess.GetUser(userId);
        if (newTask.UserId == 0) newTask.UserId = null;

        if (!user.IsAdult) return Unauthorized("A non adult can not make a task");
        
        if (newTask.UserId != null)
            if (!UserDataAccess.CheckIfUserExist((int)newTask.UserId))
                return Conflict("The assigned user does not exist");

        var task = TaskDataAccess.CreateTask(newTask.Name, newTask.Reward, user.FamilyId, newTask.UserId);
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

    [HttpPut]
    [Authorize]
    public ActionResult<Models.Task> UpdateTaskById(UpdateTask updateTask)
    {
        if (updateTask.UserId == 0) updateTask.UserId = null;
        
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = UserDataAccess.GetUser(userId);
        if (!user.IsAdult) return Unauthorized("A non adult can not update a task");

        var updatedTask = TaskDataAccess.UpdateTask(updateTask.Id, updateTask.name, updateTask.Reward, updateTask.UserId);
        if (updatedTask == null) return Problem();

        return Ok(updatedTask);
    }

    [HttpPut("mark-task-as-completed")]
    [Authorize]
    public ActionResult<Models.Task> MarkTaskAsCompleted(int id)
    {
        var task = TaskDataAccess.GetTask(id);
        if (task == null) return NotFound("Could not find the task");
        if (task.Completed) return Conflict("Task is already completed");
        if (task.UserId == null) return Conflict("No user is assigned to the task ");

        var taskUpdated = TaskDataAccess.UpdateCompletedStatus(id, task.Reward, true, (int)task.UserId);
        if (!taskUpdated) return Problem();

        var updatedTask = TaskDataAccess.GetTask(id);
        return Ok(updatedTask);
    }
    
    [HttpPut("unmark-task-as-completed")]
    [Authorize]
    public ActionResult<Models.Task> UnmarkTaskAsCompleted(int id)
    {
        var task = TaskDataAccess.GetTask(id);
        if (task == null) return NotFound("Could not find the task");
        if (!task.Completed) return Conflict("Task is already not completed");
        if (task.UserId == null) return Conflict("No user is assigned to the task ");

        int pointsToBeRevert = task.Reward * -1;

        var taskUpdated = TaskDataAccess.UpdateCompletedStatus(id, pointsToBeRevert, false, (int)task.UserId);
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
    
    public class NewTask
    {
        public string Name { get; set; }
        public int Reward { get; set; }
        public int? UserId { get; set; }
    }
    
    public class UpdateTask
    {
        public int Id { get; set; }
        public string name { get; set; }
        public int Reward { get; set; }
        public int? UserId { get; set; }
    }
}