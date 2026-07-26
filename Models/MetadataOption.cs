using System.ComponentModel.DataAnnotations;

namespace MyApp.Models;

public class MetadataOption
{
    [Key]
    public int Id { get; set; }

    [MaxLength(64)]
    public string Category { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Code { get; set; }

    public int? ParentId { get; set; }

    public MetadataOption? Parent { get; set; }

    public ICollection<MetadataOption> Children { get; set; } = new List<MetadataOption>();

    public int DisplayOrder { get; set; }

    public short IsActive { get; set; } = 1;
}

public static class MetadataCategories
{
    public const string Religion = "Religion";
    public const string Caste = "Caste";
    public const string SubCaste = "SubCaste";
    public const string MaritalStatus = "MaritalStatus";
    public const string Language = "Language";
    public const string Country = "Country";
    public const string State = "State";
    public const string City = "City";
    public const string Town = "Town";
    public const string Gender = "Gender";
    public const string Star = "Star";
    public const string Rasi = "Rasi";

    public static readonly HashSet<string> All = new(StringComparer.OrdinalIgnoreCase)
    {
        Religion,
        Caste,
        SubCaste,
        MaritalStatus,
        Language,
        Country,
        State,
        City,
        Town,
        Gender,
        Star,
        Rasi
    };
}