using BloodCenter.Identity.Client;
using BloodCenter.Identity.Client.Models;
using BloodCenter.Identity.Events;
using BloodCenter.Search.Application.Mappers;
using BloodCenter.Search.Domain.Interfaces;
using BloodCenter.Search.Domain.Models;
using Feree.ResultType;
using Feree.ResultType.Operations;
using Feree.ResultType.Results;
using MassTransit;

namespace BloodCenter.Search.Application.Events
{
    public class UserAddedEventHandler : IConsumer<UserAddedEvent>
    {
        private readonly IIndexUpdater<UserDocument> _indexUpdater;
        private readonly IIdentityClient _identityClient;

        public UserAddedEventHandler(IIndexUpdater<UserDocument> indexUpdater,
            IIdentityClient identityClient)
        {
            _indexUpdater = indexUpdater;
            _identityClient = identityClient;
        }

        public async Task Consume(ConsumeContext<UserAddedEvent> context)
        {
            var result = await _identityClient.GetUserById(context.Message.UserId);

            if(result is Success<UserDto> success)
            {
                await AddOrUpdate(success.Payload, context.CancellationToken);

                context.Respond(success.Payload);
            }
            else if(result is Failure<UserDto> failure)
            {
                context.Respond(failure.Error);
            }

        }

        private  Task<IResult<Unit>> AddOrUpdate(UserDto user, CancellationToken cancellationToken)
        {
            return _indexUpdater.AddOrUpdate(UserMapper.Map(user), cancellationToken);
        }
    }
}
