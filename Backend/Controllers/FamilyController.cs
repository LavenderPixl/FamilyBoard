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
    public ActionResult CreateFamily(string familyName)
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
        
        if (!FamilyDataAccess.DeleteFamily(familyId)) return Problem();
        return Ok();
    }

    [HttpPut("join-family")]
    [Authorize]
    public IActionResult JoinFamily(string inviteCode)
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = UserDataAccess.GetUser(userId);
        if (user == null) return Problem();
        
        int? familyId = FamilyDataAccess.GetFamilyByCode(inviteCode);
        if (familyId == null) return NotFound();
        
        FamilyDataAccess.JoinFamily(userId, familyId);
        
        return Ok();
    }

    [HttpPut("leave-family")]
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
    public IActionResult GetFamilyMembers()
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
    public IActionResult GetFamilyName()
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
    public IActionResult GenerateInvite()
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = UserDataAccess.GetUser(userId);
        
        if (user?.FamilyId == 0) return NotFound(); // No family
        if (!user.IsAdult) return Forbid(); // Not an adult
        
        FamilyCode familyCode = new FamilyCode
        {
            CreatedBy = userId,
            FamilyId = user.FamilyId,
            Expiration = new DateTime().AddDays(7),
        };

        // Retries creating the invite code 3 times if it already exists 
        if (!FamilyDataAccess.CreateFamilyInvite(familyCode)) 
        {
            return Problem();
        }
        return Ok(familyCode.Code);
    }

    // leaveFamily
}