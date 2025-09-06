namespace MyApp.controllers;
using Microsoft.AspNetCore.Mvc;
using MyApp.Models;
using System.Collections.Generic;
using MyApp.services;
using MyApp.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Storage;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("profiles")]
    [Authorize]
    public async Task<ActionResult<List<UserProfileDto>>> GetUserProfiles()
    {
        string? userRoleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        string? userGenderClaim = User.FindFirst("Gender")?.Value;
        List<UserProfile> profiles = await _userService.GetAllUserProfiles(userRoleClaim,userGenderClaim);
        if (profiles == null)
        {
            return new List<UserProfileDto>();
        }
        List<UserProfileDto> userProfileDtos = new List<UserProfileDto>();
        foreach (UserProfile user in profiles)
        {
            userProfileDtos.Add(ToUserProfileDto(user));
        }
        return Ok(userProfileDtos);

    }

    [HttpGet("profiles/{id}")]
    [Authorize]
    public async Task<ActionResult<UserProfileDto>> GetUserProfile(int id)
    {
        // var userIdClaim = User.FindFirst("userId")?.Value;
        // var userRoleClaim = User.FindFirst("Role")?.Value;
        // if (userRoleClaim != "Admin") return Forbid();
        var profile = _userService.GetUserProfileById(id);
        if (profile == null)
        {
            return NotFound();
        }
        UserProfileDto userProfileDto = ToUserProfileDto(profile);
        return Ok(userProfileDto);
    }

    private UserProfileDto ToUserProfileDto(UserProfile profile)
    {
        return new UserProfileDto()
        {
            Id = profile.Id,
            PhoneNumber = profile.PhoneNumber,
            Email = profile.Email,
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            DateOfBirth = profile.DateOfBirth,
            Gender = profile.Gender,
            Bio = profile.Bio,
            Height = profile.Height,
            MaritalStatus = profile.MaritalStatus,
            Religion = profile.Religion,
            Caste = profile.Caste,
            Subcaste = profile.Subcaste,
            Gothram = profile.Gothram,
            Star = profile.Star,
            Rasi = profile.Rasi,
            Education = profile.Education,
            Occupation = profile.Occupation,
            Income = profile.Income,
            WorkLocation = profile.WorkLocation,
            Country = profile.Country,
            State = profile.State,
            City = profile.City,
            MotherTongue = profile.MotherTongue,

        };
    }

    [HttpPost("profiles")]
    public async Task<ActionResult<UserProfile>> CreateUserProfile([FromBody] CreateUserProfileDto newUserProfileDto)
    {
        if (newUserProfileDto == null)
        {
            return BadRequest("User profile data is null.");
        }
        
        var newUserProfile = ToUserProfile(newUserProfileDto);
        if (newUserProfile == null)
        {
            return BadRequest("Invalid user profile data.");
        }


        var result = _userService.CreateUserProfile(newUserProfile);
        if (result.token == null)
        {
            return BadRequest("User Profile Already Created");
        }
        return Created("", new { token = result.token, userId = result.userId });
    }

    [HttpPut("profiles/{id}")]
    [Authorize]
    public async Task<ActionResult<UserProfileDto>> UpdateUserProfile(int id, [FromBody] UserProfileDto updatedUserProfileDto)
    {
        var userProfileIdClaim = Convert.ToInt32(User.FindFirst("userProfileId")?.Value);
        var userRoleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        if (userProfileIdClaim != id && userRoleClaim != "Admin") return Unauthorized();

        if (updatedUserProfileDto == null)
        {
            return BadRequest("User profile data is null.");
        }

        var updatedProfile = await _userService.UpdateUserProfile(id, updatedUserProfileDto);
        if (updatedProfile  == null)
        {
            return NotFound();
        }
        return Ok(ToUserProfileDto(updatedProfile));

    }

    [HttpDelete("profiles/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteUserProfile(int id)
    {
        var userProfileIdClaim = Convert.ToInt32(User.FindFirst("userProfileId")?.Value);
        var userRoleClaim = User.FindFirst("Role")?.Value;

        if (userProfileIdClaim != id && userRoleClaim != "Admin") return Unauthorized();
        // Implement delete logic if needed
        bool isSuccess = await _userService.DeleteUserProfile(id);
        if (!isSuccess)
        {
            return NotFound();
        }
        return NoContent();
    }

    private static UserProfile ToUserProfile(CreateUserProfileDto dto)
    {
        if (dto == null) throw new Exception();

        return new UserProfile
        {
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PasswordHash = dto.Password,
            Role = "User",
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            Bio = dto.Bio,
            Height = dto.Height,
            MaritalStatus = dto.MaritalStatus,
            Religion = dto.Religion,
            Caste = dto.Caste,
            Subcaste = dto.Subcaste,
            Gothram = dto.Gothram,
            Star = dto.Star,
            Rasi = dto.Rasi,
            Education = dto.Education,
            Occupation = dto.Occupation,
            Income = dto.Income,
            WorkLocation = dto.WorkLocation,
            Country = dto.Country,
            State = dto.State,
            City = dto.City,
            MotherTongue = dto.MotherTongue,

            // Initialize timestamps
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
            UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)

        };
    }
}