namespace Mongo.RestApi.ErrorHandling
{
    public class CommandException : Exception
    {
        public CommandException(dynamic value) => Value = value;

        public dynamic Value { get; }
    }
}
