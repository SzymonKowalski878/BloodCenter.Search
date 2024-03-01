using BloodCenter.Search.Client.Models;
using Feree.ResultType.Results;

namespace BloodCenter.Search.Client
{
    public interface ISearchClient
    {
        Task<IResult<IReadOnlyList<UserDocumentDto>>> GetUsersByQuery(GetUsersByQueryRequestDto request);
    }
}
