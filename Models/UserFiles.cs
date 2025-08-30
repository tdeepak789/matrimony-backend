using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Models;

public class UserFiles
{
    [Key]
    public Guid Id { get; set; }
    public int UserProfileId { get; set; }
    public string FileName { get; set; }
    public int SortOrder { get; set; }
    public short IsActive { get; set; } = 1;
}