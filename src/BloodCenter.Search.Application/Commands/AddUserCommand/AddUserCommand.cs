using BloodCenter.Search.Client.Models;
using Feree.ResultType;
using Feree.ResultType.Results;

namespace BloodCenter.Search.Application.Commands.AddUserCommand
{
    public record AddUserCommand(AddUserRequestDto request) : MediatR.IRequest<IResult<Unit>>;
}
