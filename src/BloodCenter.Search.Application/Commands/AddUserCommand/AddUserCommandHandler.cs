using BloodCenter.Search.Domain.Interfaces;
using BloodCenter.Search.Domain.Models;
using Feree.ResultType;
using Feree.ResultType.Results;

namespace BloodCenter.Search.Application.Commands.AddUserCommand
{
    public class AddUserCommandHandler : MediatR.IRequestHandler<AddUserCommand, IResult<Unit>>
    {
        private readonly IIndexUpdater<UserDocument> _indexUpdater;

        public AddUserCommandHandler(IIndexUpdater<UserDocument> indexUpdater)
        {
            _indexUpdater = indexUpdater;
        }

        public Task<IResult<Unit>> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            return _indexUpdater.AddOrUpdate(new()
            {
                Id = request.request.Id.ToString() ?? Guid.NewGuid().ToString(),
                Email = request.request.Email,
                Role = request.request.Role
            }, cancellationToken);
        }
    }
}
