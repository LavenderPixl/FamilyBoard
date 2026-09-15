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
        return Ok(family);
    }
    
    // [HttpPut("join-family")]
    // [Authorize]
    // public IActionResult JoinFamily()
    // {
    //     var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
    //     
    //     var user = UserDataAccess.GetUser(userId);
    //     if (user == null) return Problem();
    //     
    //     var family = FamilyDataAccess.
    // }

    // Invite codes = family codes  
    [HttpPost("generate-invite")]
    [Authorize]
    public IActionResult GenerateInvite(int familyId)
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var id = familyId;
        
        FamilyCode familyCode = new FamilyCode
        {
            CreatedBy = userId,
            FamilyId = familyId,
            Expiration = new DateTime().AddDays(7),
            Code = RandomNumberGenerator.GetString("0123456789", 8)
        };

        if (!FamilyDataAccess.CreateFamilyInvite(familyCode))
        {
            return Problem();
        }
        return Ok();
    }

    // leaveFamily
    // createFamilyInvite
    // getFamilyMembers
    // getFamily
}