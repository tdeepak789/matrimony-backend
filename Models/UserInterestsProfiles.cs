using System.ComponentModel.DataAnnotations;

namespace MyApp.Models;

public class UserInterestedProfiles
{
    [Key]
    public Guid Id { get; set; }
    public int userId { get; set; }
    public int interestedProfileUserId { get; set; }
    public DateTime createdOn { get; set; }
    public DateTime updatedOn { get; set; }
    public short isActive { get; set; }
}