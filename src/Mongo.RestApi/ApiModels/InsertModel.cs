namespace Mongo.RestApi.ApiModels;

public class InsertModel
{
    public dynamic[] Documents { get; set; } = Array.Empty<dynamic>();
}
