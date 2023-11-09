namespace Mongo.RestApi.ErrorHandling
{
    public class ConnectionNotFoundException : Exception
    {
        public ConnectionNotFoundException(string message) : base(message)
        {
        }
    }
}
