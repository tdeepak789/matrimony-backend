namespace MyApp.Dtos;

public class MetaDataResponseDto
{
    public List<string> Religions { get; set; } = new();
    public List<string> MaritalStatuses { get; set; } = new();
    public List<string> Languages { get; set; } = new();
    public List<string> Countries { get; set; } = new();
    public List<string> States { get; set; } = new();
    public List<string> Cities { get; set; } = new();
    public List<string> Towns { get; set; } = new();
    public List<string> Genders { get; set; } = new();
    public List<string> Star { get; set; } = new();
    public List<string> Rasi { get; set; } = new();
    public List<string> Castes { get; set; } = new();
    public List<string> SubCastes { get; set; } = new();
}

public class MetadataOptionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public int? ParentId { get; set; }
}