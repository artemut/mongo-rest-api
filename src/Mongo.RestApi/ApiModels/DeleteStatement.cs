namespace Mongo.RestApi.ApiModels
{
    public class DeleteStatement
    {
        public dynamic Q { get; set; } = null!;
        public int Limit { get; set; }
    }
}
