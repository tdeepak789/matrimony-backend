
namespace MyApp.Dtos;

public class CreateUserProfileDto
{
    public string? Email { get; set; }
    required public string PhoneNumber { get; set; }
    required public string Password { get; set; }
    required public string FirstName { get; set; }
    required public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    required public string Gender { get; set; }
    public string? Bio { get; set; }
    public int Height { get; set; }
    required public string MaritalStatus { get; set; }
    required public string Religion { get; set; }
    public string? Caste{ get; set; }
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

}

public class UserProfileDto
{
    public int Id { get; set; }
    public string? Email { get; set; }
    required public string PhoneNumber { get; set; }
    required public string FirstName { get; set; }
    required public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    required public string Gender { get; set; }
    public string? Bio { get; set; }
    public int Height { get; set; }
    required public string MaritalStatus { get; set; }
    required public string Religion { get; set; }
    public string? Caste{ get; set; }
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
    
}


