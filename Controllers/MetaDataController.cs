namespace MyApp.controllers;

using MyApp.Models;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class MetaDataController : ControllerBase
{
    [HttpGet]
    public ActionResult<MetaData> GetMetaData()
    {
        var metaData = new MetaData();
        return Ok(metaData);
    }
}