namespace MyApp.Dtos;

public class UserPhotoDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}