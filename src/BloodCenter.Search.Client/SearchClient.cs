using BloodCenter.Search.Client.Models;
using Feree.ResultType.Factories;
using Feree.ResultType.Results;

namespace BloodCenter.Search.Client
{
    public class SearchClient : CustomHttpClient, ISearchClient
    {
        public SearchClient(HttpClient httpClient, Uri baseUri)
            : base(httpClient, baseUri)
        {
        }

        public Task<IResult<IReadOnlyList<UserDocumentDto>>> GetUsersByQuery(GetUsersRequestDto request) =>
            SendPostAsync<GetUsersRequestDto, CustomActionResult<IReadOnlyList<UserDocumentDto>>>("api/user/search", request)
                .BindAsync(response => ExtractResult(response));


        private IResult<T> ExtractResult<T>(CustomActionResult<T> resultTask) =>
            resultTask.StatusCode.IsSuccessfulStatusCode() && resultTask.Value is not null
                ? ResultFactory.CreateSuccess(resultTask.Value)
                : ResultFactory.CreateFailure<T>(resultTask.Message);
    }
}
