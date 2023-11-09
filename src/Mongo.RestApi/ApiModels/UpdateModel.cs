namespace Mongo.RestApi.ApiModels;

public class UpdateModel
{
    public UpdateStatementModel[] Updates { get; set; } = Array.Empty<UpdateStatementModel>();
}
