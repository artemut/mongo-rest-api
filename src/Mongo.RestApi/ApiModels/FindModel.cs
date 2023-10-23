namespace Mongo.RestApi.ApiModels;

public class FindModel
{
    public dynamic Filter { get; set; } = null!;
    public dynamic? Sort { get; set; }
    public dynamic? Projection { get; set; }
    public int? Skip { get; set; }
    public int? Limit { get; set; }
}
