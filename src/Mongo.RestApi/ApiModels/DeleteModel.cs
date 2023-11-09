namespace Mongo.RestApi.ApiModels;

public class DeleteModel
{
    public DeleteStatement[] Deletes { get; set; } = Array.Empty<DeleteStatement>();
}
