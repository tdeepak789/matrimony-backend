
using Microsoft.EntityFrameworkCore;
using MyApp.Models;
using MyApp.Data;
using MyApp.Dtos;
using System.Threading.Tasks;

namespace MyApp.services;

public class UserService
{
    // Business logic related to user profiles can be added here
    private readonly AppDbContext _context;
    public UserService(AppDbContext context)
    {
        _context = context;
    }
    List<UserProfile> userProfiles = new List<UserProfile>
    {
        new UserProfile
        {
            Id = 1,
            Email = "user@gmail.com",
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = "Male",
            Bio = "Hello, I'm John!",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Height = 180,
            MaritalStatus = "Single",
            Religion = "Hindu",
            Caste = "Brahmin",
            Subcaste = "Iyer",
            Gothram = "Kashyap",
            Star = "Ashwini",
            Rasi = "Aries",
            Education = "B.Tech",
            Occupation = "Engineer",
            Income = 75000,
            WorkLocation = "New York",
            Country = "USA",
            State = "NY",
            City = "New York",
            MotherTongue = "English"
        },
        new UserProfile
        {
            Id = 2,
            Email = "jane@gmail.com",
            FirstName = "Jane",
            LastName = "Smith",
            DateOfBirth = new DateTime(1992, 2, 2),
            Gender = "Male",
            Bio = "Hi, I'm Jane!",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Height = 165,
            MaritalStatus = "Single",
            Religion = "Christian",
            Caste = "N/A",
            Subcaste = "N/A",
            Gothram = "N/A",
            Star = "Rohini",
            Rasi = "Taurus",
            Education = "MBA",
            Occupation = "Manager",
            Income = 85000,
            WorkLocation = "Los Angeles",
            Country = "USA",
            State = "CA",
            City = "Los Angeles",
            MotherTongue = "English"

        },
        new UserProfile
        {
            Id = 3,
            Email = "ex@gmail.com",
            FirstName = "Ex",
            LastName = "Ample",
            DateOfBirth = new DateTime(1988, 3, 3),
            Gender ="Female",
            Bio = "Hey, I'm Ex!",

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Height = 170,

            MaritalStatus = "Married",
            Religion = "Muslim",
            Caste = "N/A",
            Subcaste = "N/A",
            Gothram = "N/A",
            Star = "Magha",
            Rasi = "Leo",
            Education = "PhD",
            Occupation = "Scientist",
            Income = 95000,
            WorkLocation = "Chicago",
            Country = "USA",
            State = "IL",
            City = "Chicago",
            MotherTongue = "English"
        }
    };

    public List<UserProfile> GetAllUserProfiles()
    {
        return _context.Users.Where(user=>user.IsActive==1).ToList();
    }

    public UserProfile? GetUserProfileById(int id)
    {
        return _context.Users.Where(user => user.Id == id && user.IsActive==1).FirstOrDefault();
    }

    public UserProfile CreateUserProfile(UserProfile newUserProfile)
    {
        Console.WriteLine("++++++++++++++++++++");
        Console.WriteLine(newUserProfile.DateOfBirth);
        Console.WriteLine(newUserProfile.DateOfBirth.Kind);
        Console.WriteLine("++++++++++++++++++++");
        newUserProfile.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _context.Users.Add(newUserProfile);
        _context.SaveChanges();
        return newUserProfile;
    }

    public async Task<UserProfile> UpdateUserProfile(int id, CreateUserProfileDto updatedUserProfile)
    {
        var existingProfile = _context.Users.Where(user=> user.Id == id && user.IsActive==1).FirstOrDefault();
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
        existingProfile.PhoneNumber = updatedUserProfile.PhoneNumber;
        existingProfile.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
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
        _context.Users.Update(existingProfile);
        await _context.SaveChangesAsync();
        return true;
    }
}