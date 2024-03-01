using Feree.ResultType.Results;

namespace BloodCenter.Search.Client.Errors
{
    public record ValidationError(string Message, IReadOnlyList<ValidationError.Error> Errors) : IError
    {
        public record Error(string Message, string PropertyName);
    }
}
