using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Dtos;
using MyApp.Models;
using System.Security.Claims;

namespace MyApp.controllers;

[Route("api/[controller]")]
[ApiController]
public class FileController : ControllerBase
{
    private const long MaxPhotoSizeBytes = 10 * 1024 * 1024;
    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".gif"
    };

    private readonly IWebHostEnvironment _environment;
    private readonly AppDbContext _context;

    public FileController(IWebHostEnvironment environment, AppDbContext context)
    {
        _environment = environment;
        _context = context;
    }

    [HttpPost("upload/{id}")]
    [Authorize]
    public async Task<IActionResult> UploadFile([FromForm] List<IFormFile> files, [FromForm] List<int> sortOrders, int id)
    {
        if (files == null || files.Count == 0)
            return BadRequest("No files uploaded.");

        if (sortOrders == null || sortOrders.Count != files.Count)
            return BadRequest("sortOrders count must match files count.");

        if (sortOrders.Any(order => order < 0))
            return BadRequest("sortOrders must be non-negative values.");

        var requesterUserId = Convert.ToInt32(User.FindFirst("userProfileId")?.Value ?? "0");
        var requesterRole = User.FindFirst(ClaimTypes.Role)?.Value;
        if (requesterUserId != id && requesterRole != "Admin")
            return Unauthorized();

        var userExists = await _context.Users.AnyAsync(user => user.Id == id && user.IsActive == 1);
        if (!userExists)
            return NotFound("User not found.");

        var uploadsFolder = Path.Combine(_environment.ContentRootPath, "Uploads", id.ToString());
        Directory.CreateDirectory(uploadsFolder);

        var filesWritten = new List<string>();

        try
        {
            var existingFilesForUser = await _context.UserFiles
                .Where(file => file.UserProfileId == id && file.IsActive == 1)
                .ToListAsync();

            var existingBySortOrder = existingFilesForUser
                .GroupBy(file => file.SortOrder)
                .ToDictionary(group => group.Key, group => group.ToList());

            for (var index = 0; index < files.Count; index++)
            {
                var file = files[index];

                if (file.Length <= 0)
                    return BadRequest($"{file.FileName} is empty.");

                if (file.Length > MaxPhotoSizeBytes)
                    return BadRequest($"{file.FileName} exceeds the 10 MB limit.");

                var extension = Path.GetExtension(file.FileName);
                if (string.IsNullOrWhiteSpace(extension) || !AllowedImageExtensions.Contains(extension))
                    return BadRequest($"{file.FileName} is not a supported image type.");

                var requestedSortOrder = sortOrders[index];
                if (existingBySortOrder.TryGetValue(requestedSortOrder, out var existingSortEntries))
                {
                    foreach (var existingEntry in existingSortEntries)
                    {
                        existingEntry.IsActive = 0;
                        var existingPhysicalPath = GetPhysicalPath(existingEntry.FilePath);
                        if (System.IO.File.Exists(existingPhysicalPath))
                        {
                            System.IO.File.Delete(existingPhysicalPath);
                        }
                    }
                }

                var generatedFileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
                var filePath = Path.Combine(uploadsFolder, generatedFileName);

                await using (var stream = new FileStream(filePath, FileMode.CreateNew))
                {
                    await file.CopyToAsync(stream);
                }

                filesWritten.Add(filePath);

                var relativePath = $"/uploads/{id}/{generatedFileName}";
                _context.UserFiles.Add(new UserFiles
                {
                    Id = Guid.NewGuid(),
                    UserProfileId = id,
                    FileName = Path.GetFileName(file.FileName),
                    FilePath = relativePath,
                    SortOrder = requestedSortOrder,
                    IsActive = 1
                });
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserImages), new { id }, new
            {
                Message = $"{files.Count} images uploaded successfully"
            });
        }
        catch (Exception)
        {
            foreach (var writtenFile in filesWritten)
            {
                if (System.IO.File.Exists(writtenFile))
                {
                    System.IO.File.Delete(writtenFile);
                }
            }

            throw;
        }
    }

    [HttpGet("users/{id}/images")]
    public async Task<ActionResult<List<UserPhotoDto>>> GetUserImages(int id)
    {
        var files = await _context.UserFiles
            .Where(file => file.UserProfileId == id && file.IsActive == 1)
            .OrderBy(file => file.SortOrder)
            .ThenBy(file => file.Id)
            .ToListAsync();

        if (files.Count == 0)
        {
            return Ok(new List<UserPhotoDto>());
        }

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var response = files.Select(file => new UserPhotoDto
        {
            Id = file.Id,
            FileName = file.FileName,
            SortOrder = file.SortOrder,
            Url = BuildPublicUrl(baseUrl, file.FilePath)
        }).ToList();

        return Ok(response);
    }

    [HttpGet("download/{id}")]
    public async Task<IActionResult> DownloadFile(int id)
    {
        var firstImage = await _context.UserFiles
            .Where(file => file.UserProfileId == id && file.IsActive == 1)
            .OrderBy(file => file.SortOrder)
            .ThenBy(file => file.Id)
            .FirstOrDefaultAsync();

        if (firstImage == null)
            return NotFound("No files found for this user.");

        var filePath = GetPhysicalPath(firstImage.FilePath);
        if (!System.IO.File.Exists(filePath))
            return NotFound("File not found on disk.");

        var contentType = GetContentType(filePath);
        return PhysicalFile(filePath, contentType);
    }

    [HttpDelete("users/{userId}/images/{imageId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteUserImage(int userId, Guid imageId)
    {
        var requesterUserId = Convert.ToInt32(User.FindFirst("userProfileId")?.Value ?? "0");
        var requesterRole = User.FindFirst(ClaimTypes.Role)?.Value;
        if (requesterUserId != userId && requesterRole != "Admin")
            return Unauthorized();

        var image = await _context.UserFiles
            .FirstOrDefaultAsync(file => file.Id == imageId && file.UserProfileId == userId && file.IsActive == 1);

        if (image == null)
            return NotFound("Image not found.");

        image.IsActive = 0;

        var physicalPath = GetPhysicalPath(image.FilePath);
        if (System.IO.File.Exists(physicalPath))
        {
            System.IO.File.Delete(physicalPath);
        }

        await NormalizeSortOrder(userId);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("users/{userId}/images/order")]
    [Authorize]
    public async Task<IActionResult> UpdateUserImageOrder(int userId, [FromBody] List<Guid> orderedImageIds)
    {
        if (orderedImageIds == null || orderedImageIds.Count == 0)
            return BadRequest("orderedImageIds is required.");

        if (orderedImageIds.Count != orderedImageIds.Distinct().Count())
            return BadRequest("orderedImageIds contains duplicate values.");

        var requesterUserId = Convert.ToInt32(User.FindFirst("userProfileId")?.Value ?? "0");
        var requesterRole = User.FindFirst(ClaimTypes.Role)?.Value;
        if (requesterUserId != userId && requesterRole != "Admin")
            return Unauthorized();

        var activeImages = await _context.UserFiles
            .Where(file => file.UserProfileId == userId && file.IsActive == 1)
            .OrderBy(file => file.SortOrder)
            .ThenBy(file => file.Id)
            .ToListAsync();

        if (activeImages.Count == 0)
            return NotFound("No active images found for this user.");

        if (activeImages.Count != orderedImageIds.Count)
            return BadRequest("orderedImageIds count must match the number of active images.");

        var activeImageIds = activeImages.Select(file => file.Id).OrderBy(id => id).ToList();
        var requestImageIds = orderedImageIds.OrderBy(id => id).ToList();
        if (!activeImageIds.SequenceEqual(requestImageIds))
            return BadRequest("orderedImageIds must contain exactly the active image ids for this user.");

        var imageById = activeImages.ToDictionary(file => file.Id, file => file);
        for (var index = 0; index < orderedImageIds.Count; index++)
        {
            imageById[orderedImageIds[index]].SortOrder = index;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task NormalizeSortOrder(int userId)
    {
        var activeFiles = await _context.UserFiles
            .Where(file => file.UserProfileId == userId && file.IsActive == 1)
            .OrderBy(file => file.SortOrder)
            .ThenBy(file => file.Id)
            .ToListAsync();

        for (var index = 0; index < activeFiles.Count; index++)
        {
            activeFiles[index].SortOrder = index;
        }
    }

    private string BuildPublicUrl(string baseUrl, string filePath)
    {
        if (filePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            filePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return filePath;
        }

        var normalizedPath = filePath.StartsWith('/') ? filePath : $"/{filePath}";
        return $"{baseUrl}{normalizedPath.Replace("\\", "/")}";
    }

    private string GetPhysicalPath(string filePath)
    {
        var normalizedPath = filePath.TrimStart('/').Replace('\\', '/');
        if (normalizedPath.StartsWith("uploads/", StringComparison.OrdinalIgnoreCase))
        {
            normalizedPath = normalizedPath["uploads/".Length..];
        }

        var relativePath = normalizedPath.Replace('/', Path.DirectorySeparatorChar);
        return Path.Combine(_environment.ContentRootPath, "Uploads", relativePath);
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