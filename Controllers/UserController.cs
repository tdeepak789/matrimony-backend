namespace MyApp.controllers;
using Microsoft.AspNetCore.Mvc;
using MyApp.Models;
using System.Collections.Generic;
using MyApp.services;
using MyApp.Dtos;

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
    public async Task<ActionResult<List<UserProfile>>> GetUserProfiles()
    {

        List<UserProfile> profiles = _userService.GetAllUserProfiles();
        return Ok(profiles);

    }

    [HttpGet("profiles/{id}")]
    public async Task<ActionResult<UserProfile>> GetUserProfile(int id)
    {
        var profile = _userService.GetUserProfileById(id);
        if (profile == null)
        {
            return NotFound();
        }
        return Ok(profile);
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
    

        var createdProfile = _userService.CreateUserProfile(newUserProfile);
        
        return CreatedAtAction(nameof(GetUserProfile), new { id = createdProfile.Id }, createdProfile);
    }

    [HttpPut("profiles/{id}")]
    public async Task<ActionResult<UserProfile>> UpdateUserProfile(int id, [FromBody] CreateUserProfileDto updatedUserProfileDto)
    {
        if (updatedUserProfileDto == null)
        {
            return BadRequest("User profile data is null.");
        }

        var existingProfile = _userService.UpdateUserProfile(id,updatedUserProfileDto);
        if (existingProfile == null)
        {
            return NotFound();
        }
        return Ok(existingProfile);
        
    }

    [HttpDelete("profiles/{id}")]
    public async Task<IActionResult> DeleteUserProfile(int id)
    {
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
        if (dto == null) return null;

        return new UserProfile
        {
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
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
            PhoneNumber = dto.PhoneNumber,

            // Initialize timestamps
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
            UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)

        };
    }
}