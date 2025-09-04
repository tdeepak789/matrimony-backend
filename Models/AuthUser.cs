// Models/AuthUser.cs
using System.ComponentModel.DataAnnotations;

namespace MyApp.Models;

public class AuthUser
{
    [Key] public string Id { get; set; } = Guid.NewGuid().ToString();

    // Choose your unique login field. Example: Phone as username.
    [Required] public string UserName { get; set; }  // could be phone number string
    public string? Email { get; set; }               // optional
    public string PasswordHash { get; set; }         // BCrypt
    public string Role { get; set; } = "User";       // "User" | "Admin"

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
