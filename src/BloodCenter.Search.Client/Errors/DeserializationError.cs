using Feree.ResultType.Results;

namespace BloodCenter.Search.Client.Errors
{
    public class DeserializationError : IError
    {
        public string Message { get; set; }
        public List<string> Errors { get; set; }

        public DeserializationError(string message, List<string> errors)
        {
            Message = message;
            Errors = errors;
        }
    }
}
