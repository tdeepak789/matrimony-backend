using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Models;

public class UserProfile
{
    [Key]
    public int Id { get; set; }
    public string? Email { get; set; }
    required public string PhoneNumber { get; set; }

    required public string FirstName { get; set; }
    required public string LastName { get; set; }

    required public string PasswordHash { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime DateOfBirth { get; set; }
    required public string Gender { get; set; }
    public string? Bio { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }
    [Column(TypeName = "timestamp without time zone")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int Height { get; set; }
    required public string MaritalStatus { get; set; }

    required public string Religion { get; set; }
    public string? Caste { get; set; }
    public string? Subcaste { get; set; }
    required public string Gothram { get; set; }
    required public string Star { get; set; }
    required public string Rasi { get; set; }
    public string? Education { get; set; }
    public string? Occupation { get; set; }
    public int Income { get; set; }
    public string? WorkLocation { get; set; }
    public string? Country { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? MotherTongue { get; set; }
    public short IsActive { get; set; } = 1;
    required public string Role { get; set; }

}
