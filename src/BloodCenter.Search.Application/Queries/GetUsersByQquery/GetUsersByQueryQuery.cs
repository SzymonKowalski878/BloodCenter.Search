using BloodCenter.Search.Client.Models;
using BloodCenter.Search.Domain.Models;
using Feree.ResultType.Results;

namespace BloodCenter.Search.Application.Queries.GetUsersByQquery
{
    public record GetUsersByQueryQuery(GetUsersByQueryRequestDto request) : MediatR.IRequest<IResult<IReadOnlyList<UserDocumentDto>>>;
}
