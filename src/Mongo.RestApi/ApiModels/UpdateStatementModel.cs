namespace Mongo.RestApi.ApiModels
{
    public class UpdateStatementModel
    {
        public dynamic Q { get; set; } = null!;
        public dynamic U { get; set; } = null!;
        public bool Upsert { get; set; }
        public bool Multi { get; set; }
        public dynamic[]? ArrayFilters { get; set; }
    }
}
