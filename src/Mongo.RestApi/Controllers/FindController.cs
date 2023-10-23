using Microsoft.AspNetCore.Mvc;
using Mongo.RestApi.ApiModels;
using Mongo.RestApi.Database;

namespace Mongo.RestApi.Controllers;

[ApiController]
public class FindController : ControllerBase
{
    private readonly IFinder _finder;

    public FindController(IFinder finder)
    {
        _finder = finder;
    }

    [HttpPost]
    [Route("{databaseName}/{collectionName}/find")]
    public async Task<IActionResult> FindAsync(string databaseName, string collectionName, FindModel model)
    {
        var results = await _finder.RunAsync(
            databaseName,
            collectionName,
            model,
            HttpContext.RequestAborted);
        return Ok(results);
    }
}
