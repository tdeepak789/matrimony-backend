
using Microsoft.EntityFrameworkCore;
using MyApp.Models;
using MyApp.Data;
using MyApp.Dtos;
using System.Threading.Tasks;
using MyApp.Controllers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace MyApp.services;

public class UserService
{
    // Business logic related to user profiles can be added here
    private readonly AppDbContext _context;
    private readonly IConfiguration _cfg;
    public UserService(AppDbContext context, IConfiguration cfg)
    {
        _cfg = cfg;

        _context = context;
    }
    public async Task<List<UserProfile>> GetAllUserProfiles(string? userRoleClaim, string? userGenderClaim)
    {
        if (userRoleClaim == "Admin")
        {
            return await _context.Users.Where(user => user.IsActive == 1).ToListAsync();
        }
        return await _context.Users.Where(user => user.IsActive == 1 && user.Gender!=userGenderClaim).ToListAsync();
    }

    public UserProfile? GetUserProfileById(int id)
    {
        return _context.Users.Where(user => user.Id == id && user.IsActive == 1).FirstOrDefault();
    }

    public string CreateUserProfile(UserProfile newUserProfile)
    {

        var existingProfile = _context.Users.Where(user => user.Id == newUserProfile.Id).FirstOrDefault();
        if (existingProfile != null)
        {
            throw new Exception("Bad request profile already exists");
        }

        newUserProfile.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        newUserProfile.Role = "User";
        newUserProfile.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUserProfile.PasswordHash);
        _context.Users.Add(newUserProfile);
        _context.SaveChanges();
        var token = GenerateJwt(newUserProfile);
        return token;
    }

    public async Task<UserProfile> UpdateUserProfile(int id, UserProfileDto updatedUserProfile)
    {
        var existingProfile = _context.Users.Where(user => user.Id == id && user.IsActive == 1).FirstOrDefault();

        if (existingProfile == null)
        {
            throw new Exception("User profile not found.");
        }
        // Update fields
        existingProfile.Email = updatedUserProfile.Email;
        existingProfile.FirstName = updatedUserProfile.FirstName;
        existingProfile.LastName = updatedUserProfile.LastName;
        existingProfile.DateOfBirth = updatedUserProfile.DateOfBirth;
        existingProfile.Bio = updatedUserProfile.Bio;
        existingProfile.Height = updatedUserProfile.Height;
        existingProfile.MaritalStatus = updatedUserProfile.MaritalStatus;
        existingProfile.Religion = updatedUserProfile.Religion;
        existingProfile.Caste = updatedUserProfile.Caste;
        existingProfile.Subcaste = updatedUserProfile.Subcaste;
        existingProfile.Gothram = updatedUserProfile.Gothram;
        existingProfile.Star = updatedUserProfile.Star;
        existingProfile.Rasi = updatedUserProfile.Rasi;
        existingProfile.Education = updatedUserProfile.Education;
        existingProfile.Occupation = updatedUserProfile.Occupation;
        existingProfile.Income = updatedUserProfile.Income;
        existingProfile.WorkLocation = updatedUserProfile.WorkLocation;
        existingProfile.Country = updatedUserProfile.Country;
        existingProfile.State = updatedUserProfile.State;
        existingProfile.City = updatedUserProfile.City;
        existingProfile.MotherTongue = updatedUserProfile.MotherTongue;
        existingProfile.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        existingProfile.PhoneNumber = updatedUserProfile.PhoneNumber;
        _context.Users.Update(existingProfile);
        await _context.SaveChangesAsync();
        return existingProfile;
    }

    public async Task<bool> DeleteUserProfile(int id)
    {
        UserProfile? existingProfile = _context.Users.Find(id);
        if (existingProfile == null)
        {
            return false;
        }
        existingProfile.IsActive = 0;
        existingProfile.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _context.Users.Update(existingProfile);
        await _context.SaveChangesAsync();
        return true;
    }
   

    public async Task<string> ValidateUserCredentials(LoginDto loginDto)
    {

        UserProfile userProfile = await _context.Users.Where(usr => (usr.PhoneNumber == loginDto.UserName) || (!string.IsNullOrEmpty(usr.Email) && usr.Email.ToLower() == loginDto.UserName.ToLower())).FirstOrDefaultAsync();
        if (userProfile == null)
        {
            throw new Exception();
        }
        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, userProfile.PasswordHash))
            throw new Exception("Invalid credentials");
        string token = GenerateJwt(userProfile);
        return token;
    }
    private string GenerateJwt(UserProfile user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("userProfileId",Convert.ToString(user.Id)),
            new Claim("FirstNamw",user.FirstName),
            new Claim("LastName",user.LastName),
            new Claim("Gender", user.Gender)
        };

        var token = new JwtSecurityToken(
            issuer: _cfg["Jwt:Issuer"],
            audience: _cfg["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}