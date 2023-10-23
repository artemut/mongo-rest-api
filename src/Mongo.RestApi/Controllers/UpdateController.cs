using Microsoft.AspNetCore.Mvc;
using Mongo.RestApi.ApiModels;
using Mongo.RestApi.Database;

namespace Mongo.RestApi.Controllers;

[ApiController]
public class UpdateController : ControllerBase
{
    private readonly IUpdater _updater;

    public UpdateController(IUpdater updater)
    {
        _updater = updater;
    }

    [HttpPost]
    [Route("{databaseName}/{collectionName}/update")]
    public async Task<IActionResult> UpdateAsync(string databaseName, string collectionName, UpdateModel model)
    {
        await _updater.RunAsync(
            databaseName,
            collectionName,
            model,
            HttpContext.RequestAborted);
        return Ok();
    }
}
