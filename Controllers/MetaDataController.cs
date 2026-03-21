namespace MyApp.controllers;

using Microsoft.AspNetCore.Mvc;
using MyApp.Dtos;
using MyApp.services;

[Route("api/[controller]")]
[ApiController]
public class MetaDataController : ControllerBase
{
    private readonly MetaDataService _metaDataService;

    public MetaDataController(MetaDataService metaDataService)
    {
        _metaDataService = metaDataService;
    }

    [HttpGet]
    public async Task<ActionResult<MetaDataResponseDto>> GetMetaData(CancellationToken cancellationToken)
    {
        var metaData = await _metaDataService.GetMetaDataAsync(cancellationToken);
        return Ok(metaData);
    }

    [HttpGet("options/{category}")]
    public async Task<ActionResult<List<MetadataOptionDto>>> GetMetadataByCategory(
        string category,
        [FromQuery] int? parentId,
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        try
        {
            var options = await _metaDataService.GetOptionsByCategoryAsync(category, parentId, search, cancellationToken);
            return Ok(options);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}