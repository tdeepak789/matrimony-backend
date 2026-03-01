using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    public FileController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpPost("upload/{id}")]
    public async Task<IActionResult> UploadFile([FromForm] List<IFormFile> files, int id)
    {
        if (files == null || files.Count == 0)
            return BadRequest("No file uploaded.");

        // Using ContentRootPath is good, but ensure the "Uploads" folder exists at the root
        var uploadsFolder = Path.Combine(_environment.ContentRootPath, "Uploads", id.ToString());
        
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }
        
        foreach (var file in files)
        {
            // Security Tip: Use a safe filename or GUID to prevent directory traversal attacks
            var safeFileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadsFolder, safeFileName);
            
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
        }

        return Ok(new { Message = $"{files.Count} files uploaded successfully" });
    }

    [HttpGet("download/{id}")]
    public IActionResult DownloadFile(int id)
    {
        var uploadsFolder = Path.Combine(_environment.ContentRootPath, "Uploads", $"{id}");
        var folderPath = Path.Combine(uploadsFolder);
        if (!Directory.Exists(folderPath))
            return NotFound("User folder not found.");
        var files = Directory.GetFiles(folderPath);
        if (files.Length == 0)
            return NotFound("No files found for this user.");
        var filePath = Path.Combine(uploadsFolder, files[0]); // Get the first file
        if (!System.IO.File.Exists(filePath))
            return NotFound("File not found.");
        var contentType = GetContentType(filePath);
        return PhysicalFile(filePath, contentType);
    }

        private string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
}