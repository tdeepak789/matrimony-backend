using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Models;

public class UserProfile
{
    [Key]
    public int? Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; }
    public string Bio { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; } 
    [Column(TypeName = "timestamp without time zone")]
    public DateTime UpdatedAt { get; set; } =  DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

    public int Height { get; set; }
    public string MaritalStatus { get; set; }

    public string Religion { get; set; }
    public string Caste { get; set; }
    public string Subcaste { get; set; }
    public string Gothram { get; set; }
    public string Star { get; set; }
    public string Rasi { get; set; }
    public string Education { get; set; }
    public string Occupation { get; set; }
    public int Income { get; set; }
    public string WorkLocation { get; set; }
    public string Country { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public string MotherTongue { get; set; }
    public long PhoneNumber { get; set; }
    public short IsActive { get; set; } = 1;

}
