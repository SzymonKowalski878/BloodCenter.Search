using Feree.ResultType.Results;
using System.Net;

namespace BloodCenter.Search.Client.Errors
{
    public record HttpResponseError(string Message, HttpStatusCode StatusCode, IReadOnlyList<CustomError> Errors) : IError;
}
