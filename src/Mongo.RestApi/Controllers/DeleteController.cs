using Microsoft.AspNetCore.Mvc;
using Mongo.RestApi.ApiModels;
using Mongo.RestApi.Database;

namespace Mongo.RestApi.Controllers;

[ApiController]
public class DeleteController : ControllerBase
{
    private readonly IDeleter _deleter;

    public DeleteController(IDeleter deleter)
    {
        _deleter = deleter;
    }

    [HttpPost]
    [Route("{databaseName}/{collectionName}/delete")]
    public async Task<IActionResult> DeleteAsync(string databaseName, string collectionName, DeleteModel model)
    {
        await _deleter.RunAsync(
            databaseName,
            collectionName,
            model,
            HttpContext.RequestAborted);
        return Ok();
    }
}
