using BloodCenter.Search.Client.Models;
using BloodCenter.Search.Domain.Models;
using Feree.ResultType.Results;

namespace BloodCenter.Search.Application.Queries.GetUsers
{
    public record GetUsersQuery(GetUsersRequestDto Request) : MediatR.IRequest<IResult<IReadOnlyList<UserDocumentDto>>>;
}
