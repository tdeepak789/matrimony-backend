using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Dtos;
using MyApp.Models;

namespace MyApp.services;

public class MetaDataService
{
    private readonly AppDbContext _context;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<MetaDataService> _logger;

    public MetaDataService(AppDbContext context, IHostEnvironment environment, ILogger<MetaDataService> logger)
    {
        _context = context;
        _environment = environment;
        _logger = logger;
    }

    public async Task<MetaDataResponseDto> GetMetaDataAsync(CancellationToken cancellationToken = default)
    {
        var activeOptions = await _context.MetadataOptions
            .AsNoTracking()
            .Where(option => option.IsActive == 1)
            .OrderBy(option => option.DisplayOrder)
            .ThenBy(option => option.Name)
            .ToListAsync(cancellationToken);

        return new MetaDataResponseDto
        {
            Religions = GetNamesByCategory(activeOptions, MetadataCategories.Religion),
            MaritalStatuses = GetNamesByCategory(activeOptions, MetadataCategories.MaritalStatus),
            Languages = GetNamesByCategory(activeOptions, MetadataCategories.Language),
            Countries = GetNamesByCategory(activeOptions, MetadataCategories.Country),
            States = GetNamesByCategory(activeOptions, MetadataCategories.State),
            Cities = GetNamesByCategory(activeOptions, MetadataCategories.City),
            Towns = GetNamesByCategory(activeOptions, MetadataCategories.Town),
            Genders = GetNamesByCategory(activeOptions, MetadataCategories.Gender),
            Star = GetNamesByCategory(activeOptions, MetadataCategories.Star),
            Rasi = GetNamesByCategory(activeOptions, MetadataCategories.Rasi),
            Castes = GetNamesByCategory(activeOptions, MetadataCategories.Caste),
            SubCastes = GetNamesByCategory(activeOptions, MetadataCategories.SubCaste)
        };
    }

    public async Task<List<MetadataOptionDto>> GetOptionsByCategoryAsync(
        string category,
        int? parentId = null,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        if (!MetadataCategories.All.Contains(category))
            throw new ArgumentException("Invalid metadata category.", nameof(category));

        var normalizedCategory = MetadataCategories.All.First(item => item.Equals(category, StringComparison.OrdinalIgnoreCase));

        var query = _context.MetadataOptions
            .AsNoTracking()
            .Where(option => option.IsActive == 1 && option.Category == normalizedCategory);

        if (parentId.HasValue)
        {
            query = query.Where(option => option.ParentId == parentId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLower();
            query = query.Where(option => option.Name.ToLower().Contains(normalizedSearch));
        }

        return await query
            .OrderBy(option => option.DisplayOrder)
            .ThenBy(option => option.Name)
            .Select(option => new MetadataOptionDto
            {
                Id = option.Id,
                Name = option.Name,
                Code = option.Code,
                ParentId = option.ParentId
            })
            .ToListAsync(cancellationToken);
    }

    public async Task EnsureSeededAsync(CancellationToken cancellationToken = default)
    {
        if (await _context.MetadataOptions.AnyAsync(option => option.IsActive == 1, cancellationToken))
        {
            return;
        }

        var seedFilePath = Path.Combine(_environment.ContentRootPath, "SeedData", "metadata.seed.json");
        if (!File.Exists(seedFilePath))
        {
            _logger.LogWarning("Metadata seed file not found at {SeedFilePath}.", seedFilePath);
            return;
        }

        await using var seedFileStream = File.OpenRead(seedFilePath);
        var seedRoot = await JsonSerializer.DeserializeAsync<MetadataSeedRoot>(
            seedFileStream,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            },
            cancellationToken: cancellationToken);
        if (seedRoot == null)
        {
            _logger.LogWarning("Metadata seed file is empty or invalid JSON.");
            return;
        }

        AddFlatOptions(MetadataCategories.Religion, seedRoot.Religions);
        AddFlatOptions(MetadataCategories.MaritalStatus, seedRoot.MaritalStatuses);
        AddFlatOptions(MetadataCategories.Language, seedRoot.Languages);
        AddFlatOptions(MetadataCategories.Gender, seedRoot.Genders);
        AddFlatOptions(MetadataCategories.Star, seedRoot.Stars);
        AddFlatOptions(MetadataCategories.Rasi, seedRoot.Rasi);

        await _context.SaveChangesAsync(cancellationToken);

        await AddLocationHierarchyAsync(seedRoot, cancellationToken);
        await AddReligionHierarchyAsync(seedRoot, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task AddLocationHierarchyAsync(MetadataSeedRoot seedRoot, CancellationToken cancellationToken)
    {
        foreach (var countrySeed in seedRoot.Locations)
        {
            var country = CreateOption(MetadataCategories.Country, countrySeed.Name, countrySeed.Code, null, countrySeed.DisplayOrder);
            _context.MetadataOptions.Add(country);
            await _context.SaveChangesAsync(cancellationToken);

            foreach (var stateSeed in countrySeed.States)
            {
                var state = CreateOption(MetadataCategories.State, stateSeed.Name, stateSeed.Code, country.Id, stateSeed.DisplayOrder);
                _context.MetadataOptions.Add(state);
                await _context.SaveChangesAsync(cancellationToken);

                var cityDisplayOrder = 0;
                foreach (var cityName in stateSeed.Cities)
                {
                    var city = CreateOption(MetadataCategories.City, cityName, null, state.Id, cityDisplayOrder++);
                    _context.MetadataOptions.Add(city);
                }

                var townDisplayOrder = 0;
                foreach (var townName in stateSeed.Towns)
                {
                    var town = CreateOption(MetadataCategories.Town, townName, null, state.Id, townDisplayOrder++);
                    _context.MetadataOptions.Add(town);
                }
            }
        }
    }

    private async Task AddReligionHierarchyAsync(MetadataSeedRoot seedRoot, CancellationToken cancellationToken)
    {
        foreach (var religionHierarchy in seedRoot.CastesByReligion)
        {
            var religion = await _context.MetadataOptions
                .FirstOrDefaultAsync(option => option.Category == MetadataCategories.Religion && option.Name == religionHierarchy.Religion && option.IsActive == 1, cancellationToken);

            if (religion == null)
                continue;

            foreach (var casteSeed in religionHierarchy.Castes)
            {
                var caste = CreateOption(MetadataCategories.Caste, casteSeed.Name, casteSeed.Code, religion.Id, casteSeed.DisplayOrder);
                _context.MetadataOptions.Add(caste);
                await _context.SaveChangesAsync(cancellationToken);

                var subCasteDisplayOrder = 0;
                foreach (var subCasteName in casteSeed.SubCastes)
                {
                    var subCaste = CreateOption(MetadataCategories.SubCaste, subCasteName, null, caste.Id, subCasteDisplayOrder++);
                    _context.MetadataOptions.Add(subCaste);
                }
            }
        }
    }

    private void AddFlatOptions(string category, List<string> names)
    {
        for (var index = 0; index < names.Count; index++)
        {
            _context.MetadataOptions.Add(CreateOption(category, names[index], null, null, index));
        }
    }

    private static MetadataOption CreateOption(string category, string name, string? code, int? parentId, int displayOrder)
    {
        return new MetadataOption
        {
            Category = category,
            Name = name,
            Code = code,
            ParentId = parentId,
            DisplayOrder = displayOrder,
            IsActive = 1
        };
    }

    private static List<string> GetNamesByCategory(IEnumerable<MetadataOption> options, string category)
    {
        return options
            .Where(option => option.Category == category)
            .Select(option => option.Name)
            .ToList();
    }

    private sealed class MetadataSeedRoot
    {
        public List<string> Religions { get; set; } = new();
        public List<string> MaritalStatuses { get; set; } = new();
        public List<string> Languages { get; set; } = new();
        public List<string> Genders { get; set; } = new();
        public List<string> Stars { get; set; } = new();
        public List<string> Rasi { get; set; } = new();
        public List<CountrySeed> Locations { get; set; } = new();
        public List<ReligionCasteSeed> CastesByReligion { get; set; } = new();
    }

    private sealed class CountrySeed
    {
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public int DisplayOrder { get; set; }
        public List<StateSeed> States { get; set; } = new();
    }

    private sealed class StateSeed
    {
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public int DisplayOrder { get; set; }
        public List<string> Cities { get; set; } = new();
        public List<string> Towns { get; set; } = new();
    }

    private sealed class ReligionCasteSeed
    {
        public string Religion { get; set; } = string.Empty;
        public List<CasteSeed> Castes { get; set; } = new();
    }

    private sealed class CasteSeed
    {
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public int DisplayOrder { get; set; }
        public List<string> SubCastes { get; set; } = new();
    }
}