using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Dtos;
using MyApp.services;

namespace MyApp.Controllers;


[Route("api/[Controller]")]
[ApiController]
public class InterestsController : ControllerBase
{
    private InterestService interestService;
    public InterestsController(InterestService _interestService)
    {
        interestService = _interestService;
    }


    [HttpPost("{userId}/{interestedProfileId}")]
    [Authorize]
    public ActionResult AddProfileAsInterested(int userId, int interestedProfileId)
    {
        var userProfileIdClaim = Convert.ToInt32(User.FindFirst("userProfileId")?.Value);
        var userRoleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        if (userProfileIdClaim != userId && userRoleClaim != "Admin") return Unauthorized();

        bool result = interestService.AddUserInterest(userId, interestedProfileId);
        if (result)
        {
            return Ok();
        }
        return BadRequest("Error while adding user interest");
    }

    [HttpDelete("{userId}/{interestedProfileId}")]
    [Authorize]
    public ActionResult UpdateUserInterest(int userId, int interestedProfileId)
    {
        var userProfileIdClaim = Convert.ToInt32(User.FindFirst("userProfileId")?.Value);
        var userRoleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        if (userProfileIdClaim != userId && userRoleClaim != "Admin") return Unauthorized();

        bool result = interestService.UpdateUserInterest(userId, interestedProfileId, deleteInterest: true);

        if (result) return NoContent();
        return BadRequest("Error while deleting existing user interest");
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<UserInterestsDto>> GetUserInterests(int userId)
    {
         var userProfileIdClaim = Convert.ToInt32(User.FindFirst("userProfileId")?.Value);
        var userRoleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        if (userProfileIdClaim != userId && userRoleClaim != "Admin") return Unauthorized();

        var userInterests = interestService.getUserInterestedProfilesByUserId(userId);

        if (userInterests == null)
        {
            return NotFound();
        }
        return Ok(userInterests);
    }
}