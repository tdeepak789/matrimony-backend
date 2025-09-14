namespace MyApp.Dtos;

public class UserInterestsDto
{
    public int userId { get; set; }
    public List<UserProfileDto> profilesInterestedInUser { get; set; }
    public List<UserProfileDto> profilesInterestedByUser { get; set; }

}