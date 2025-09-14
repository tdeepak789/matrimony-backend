using MyApp.Data;
using MyApp.Dtos;
using MyApp.Models;


namespace MyApp.services;

public class InterestService
{
    private readonly AppDbContext _context;
    public InterestService(AppDbContext context)

    {
        _context = context;
    }


    public bool AddUserInterest(int userId, int interestedUserProfileId)
    {
        var user = _context.Users.Where(x => x.Id == userId && x.IsActive == 1);
        if (user == null) return false;

        var interestedProfile = _context.Users.Where(x => x.Id == interestedUserProfileId);

        if (interestedProfile == null) return false;

        var existingUserInterest = _context.UserInterests.Where(x => x.userId == userId && x.interestedProfileUserId == interestedUserProfileId).FirstOrDefault();
        if (existingUserInterest != null && existingUserInterest.isActive == 1)
        {
            return false;
        }
        else if (existingUserInterest != null && existingUserInterest.isActive == 0)
        {
            existingUserInterest.isActive = 1;
            _context.UserInterests.Update(existingUserInterest);
        }
        UserInterestedProfiles userInterest = new UserInterestedProfiles
        {
            userId = userId,
            interestedProfileUserId = interestedUserProfileId,
            createdOn = DateTime.UtcNow,
            updatedOn = DateTime.UtcNow,
            isActive = 1
        };

        _context.UserInterests.Add(userInterest);
        _context.SaveChanges();
        return true;
    }


    public bool UpdateUserInterest(int userId, int interestedUserProfileId, Boolean deleteInterest = false)
    {
        var user = _context.Users.Where(x => x.Id == userId && x.IsActive == 1);
        if (user == null) return false;

        var interestedProfile = _context.Users.Where(x => x.Id == interestedUserProfileId);

        if (interestedProfile == null) return false;

        var existingUserInterest = _context.UserInterests.Where(x => x.userId == userId && x.interestedProfileUserId == interestedUserProfileId).FirstOrDefault();
        if (existingUserInterest != null && existingUserInterest.isActive == 1)
        {
            if (deleteInterest)
            {
                existingUserInterest.isActive = 0;
                _context.UserInterests.Update(existingUserInterest);
            }
            
        }
        else if (existingUserInterest != null && existingUserInterest.isActive == 0)
        {
            if (!deleteInterest)
            {
                existingUserInterest.isActive = 1;
                _context.UserInterests.Update(existingUserInterest);
            }
            
            
        }
        _context.SaveChanges();
        return true;
    }


    public UserInterestsDto getUserInterestedProfilesByUserId(int userId)
    {
        UserInterestsDto userInterests = new UserInterestsDto();
        userInterests.userId = userId;

        List<int> profileIdsInterestedInUser = _context.UserInterests.Where(x => x.interestedProfileUserId == userId && x.isActive == 1).Select(x => x.userId).ToList();
        List<UserProfileDto> profilesInterestedInUser = new List<UserProfileDto>();
        if (profileIdsInterestedInUser != null && profileIdsInterestedInUser.Any())
        {
            var users = _context.Users.Where(x => profileIdsInterestedInUser.Contains(x.Id)).ToList();
            if (users != null)
            {
                profilesInterestedInUser.AddRange(users.Select(ToUserProfileDto));
            }

        }

        List<UserProfileDto> profilesInterestedByUser = new List<UserProfileDto>();

        List<int> profileIdsInterestedByUser = _context.UserInterests.Where(x => x.userId == userId && x.isActive == 1).Select(x => x.interestedProfileUserId).ToList();
        if (profileIdsInterestedByUser != null && profileIdsInterestedByUser.Any())
        {
            var users = _context.Users.Where(x => profileIdsInterestedByUser.Contains(x.Id)).ToList();

            if (users != null)
            {
                profilesInterestedByUser.AddRange(users.Select(ToUserProfileDto));
            }
        }

        if (profilesInterestedByUser != null && profilesInterestedByUser.Any())
        {
            userInterests.profilesInterestedByUser = profilesInterestedByUser;
        }
        if (profilesInterestedInUser != null && profilesInterestedInUser.Any())
        {
            userInterests.profilesInterestedInUser = profilesInterestedInUser;
        }
        return userInterests;

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

}