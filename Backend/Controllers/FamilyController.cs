using System.Security.Claims;
using System.Security.Cryptography;
using Backend.DataAccess;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Route("family")]
[ApiController]
public class FamilyController : ControllerBase
{
    [HttpPost("create-family")]
    [Authorize]
    public ActionResult<Family> CreateFamily(string familyName)
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var family = FamilyDataAccess.CreateFamily(familyName);
        if (!FamilyDataAccess.JoinFamily(userId, family.Id))
        {
            return Conflict();
        }

        UserDataAccess.UpdateAdultStatus(userId, true);
        return Ok(family);
    }

    [HttpDelete("delete-family")]
    [Authorize]
    public ActionResult DeleteFamily(int familyId) {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = UserDataAccess.GetUser(userId);
        if (user == null) return NotFound("User with this ID, not found");

        var family = FamilyDataAccess.GetFamily(familyId);
        if (family == null) return NotFound("Family with this ID, not found");
        
        if (!user.IsAdult) return Unauthorized("A non adult can not make a task");
        if (!FamilyDataAccess.DeleteFamily(familyId)) return Problem("Could not find family with that ID");

        return Ok();
    }

    [HttpPatch("join-family")]
    [Authorize]
    public IActionResult JoinFamily(string inviteCode)
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = UserDataAccess.GetUser(userId);
        if (user == null) return Problem();
        
        int? familyId = FamilyDataAccess.GetFamilyByCode(inviteCode);
        
        if (familyId == null) return NotFound("No family with that ID");
        if (user.FamilyId == familyId) return Conflict("User is already in family");
        if (user.FamilyId != 0) return Conflict("User is in another family"); 
        
        FamilyDataAccess.JoinFamily(userId, familyId);
        UserDataAccess.UpdateAdultStatus(userId, false);
        
        
        return Ok();
    }

    [HttpPatch("leave-family")]
    [Authorize]
    public IActionResult LeaveFamily(int familyId)
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = UserDataAccess.GetUser(userId);
        if (user == null) return Problem();
        
        var attemptLeave = FamilyDataAccess.LeaveFamily(userId, familyId);
        if (!attemptLeave) return Problem();
        
        return Ok();
    }
    
    
    [HttpGet("get-family-members")]
    [Authorize]
    public ActionResult<List<User>> GetFamilyMembers()
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = UserDataAccess.GetUser(userId);
        if (user == null) return Problem();

        List<User> members = FamilyDataAccess.GetFamilyMembers(user.FamilyId);
        if (members.Count < 0 ) return Problem();
        return Ok(members);
    }

    [HttpGet("get-family-name")]
    [Authorize]
    public ActionResult<string> GetFamilyName()
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = UserDataAccess.GetUser(userId);
        if (user == null) return Problem();
        
        Family family = FamilyDataAccess.GetFamily(user.FamilyId);
        return Ok(family.Name);
    }
    
    
    // Invite codes = family codes  
    [HttpPost("generate-invite")]
    [Authorize]
    public ActionResult<string> GenerateInvite()
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = UserDataAccess.GetUser(userId);
        
        if (user?.FamilyId == 0) return NotFound("No family");
        if (!user.IsAdult) return Forbid("Not an adult");
        DateTime expiration = DateTime.UtcNow.AddDays(7);
        
        
        FamilyCode familyCode = new FamilyCode
        {
            CreatedBy = userId,
            FamilyId = user.FamilyId,
            Expiration = expiration
        };

        // Retries creating the invite code 3 times if it already exists 
        if (!FamilyDataAccess.CreateFamilyInvite(familyCode)) 
        {
            return Problem("Could not create invite.");
        }
        return Ok(familyCode.Code);
    }
}