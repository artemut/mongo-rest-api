using Microsoft.AspNetCore.Mvc;
using Mongo.RestApi.ApiModels;
using Mongo.RestApi.Database;

namespace Mongo.RestApi.Controllers;

[ApiController]
public class InsertController : ControllerBase
{
    private readonly IInserter _inserter;

    public InsertController(IInserter inserter)
    {
        _inserter = inserter;
    }

    [HttpPost]
    [Route("{connectionName}/{databaseName}/{collectionName}/insert")]
    public async Task<IActionResult> InsertAsync(
        string connectionName,
        string databaseName,
        string collectionName,
        InsertModel model)
    {
        await _inserter.RunAsync(
            connectionName,
            databaseName,
            collectionName,
            model,
            HttpContext.RequestAborted);
        return Ok();
    }
}
