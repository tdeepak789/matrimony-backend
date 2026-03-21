
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

    public async Task<(List<UserProfile> Users, int TotalCount)> GetPagedUserProfiles(
        string? userRoleClaim,
        string? userGenderClaim,
        int page,
        int pageSize,
        string? search,
        string? gender,
        string? religion,
        string? caste,
        string? maritalStatus)
    {
        var sanitizedPage = page < 1 ? 1 : page;
        var sanitizedPageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

        IQueryable<UserProfile> query = _context.Users.AsNoTracking().Where(user => user.IsActive == 1);

        if (userRoleClaim != "Admin")
        {
            query = query.Where(user => user.Gender != userGenderClaim);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLower();
            query = query.Where(user =>
                (user.FirstName + " " + user.LastName).ToLower().Contains(normalizedSearch) ||
                (!string.IsNullOrEmpty(user.FirstName) && user.FirstName.ToLower().Contains(normalizedSearch)) ||
                (!string.IsNullOrEmpty(user.LastName) && user.LastName.ToLower().Contains(normalizedSearch)));
        }

        if (!string.IsNullOrWhiteSpace(gender))
        {
            query = query.Where(user => user.Gender == gender);
        }

        if (!string.IsNullOrWhiteSpace(religion))
        {
            query = query.Where(user => user.Religion == religion);
        }

        if (!string.IsNullOrWhiteSpace(caste))
        {
            var normalizedCaste = caste.Trim().ToLower();
            query = query.Where(user => user.Caste != null && user.Caste.ToLower().Contains(normalizedCaste));
        }

        if (!string.IsNullOrWhiteSpace(maritalStatus))
        {
            query = query.Where(user => user.MaritalStatus == maritalStatus);
        }

        var totalCount = await query.CountAsync();

        var users = await query
            .OrderByDescending(user => user.UpdatedAt)
            .ThenByDescending(user => user.Id)
            .Skip((sanitizedPage - 1) * sanitizedPageSize)
            .Take(sanitizedPageSize)
            .ToListAsync();

        return (users, totalCount);
    }

    public UserProfile? GetUserProfileById(int id)
    {
        return _context.Users.Where(user => user.Id == id && user.IsActive == 1).FirstOrDefault();
    }

    public (string token,int userId) CreateUserProfile(UserProfile newUserProfile)
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
        return (token,newUserProfile.Id);
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
   

    public async Task<(string token,int userId)> ValidateUserCredentials(LoginDto loginDto)
    {

        UserProfile userProfile = await _context.Users.Where(usr => (usr.PhoneNumber == loginDto.UserName) || (!string.IsNullOrEmpty(usr.Email) && usr.Email.ToLower() == loginDto.UserName.ToLower())).FirstOrDefaultAsync();
        if (userProfile == null)
        {
            throw new Exception();
        }
        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, userProfile.PasswordHash))
            throw new Exception("Invalid credentials");
        string token = GenerateJwt(userProfile);
        return (token,userProfile.Id);
    }
    private string GenerateJwt(UserProfile user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("userProfileId",Convert.ToString(user.Id)),
            new Claim("FirstName",user.FirstName),
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