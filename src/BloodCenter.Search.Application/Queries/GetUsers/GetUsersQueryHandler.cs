using BloodCenter.Search.Application.Mappers;
using BloodCenter.Search.Client.Models;
using BloodCenter.Search.Domain.Interfaces;
using BloodCenter.Search.Domain.Models;
using Feree.ResultType.Factories;
using Feree.ResultType.Results;
using Nest;

namespace BloodCenter.Search.Application.Queries.GetUsers
{
    public class GetUsersQueryHandler : MediatR.IRequestHandler<GetUsersQuery, IResult<IReadOnlyList<UserDocumentDto>>>
    {
        private readonly IUserQueryBuilder _userQueryBuilder;
        private readonly IElasticClient _elasticClient;

        public GetUsersQueryHandler(IElasticClient elasticClient, IUserQueryBuilder userQueryBuilder)
        {
            _elasticClient = elasticClient;
            _userQueryBuilder = userQueryBuilder;
        }

        public async Task<IResult<IReadOnlyList<UserDocumentDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var query = _userQueryBuilder.GetByQuery(request.Request.QueryString, request.Request.Roles);

            var response = await _elasticClient.SearchAsync<UserDocument>(query);

            if (!response.IsValid)
                return ResultFactory.CreateFailure<IReadOnlyList<UserDocumentDto>>(response.OriginalException.Message);

            var items = response.Documents.ToArray();

            return ResultFactory.CreateSuccess<IReadOnlyList<UserDocumentDto>>(items.Select(x => UserMapper.Map(x)).ToArray());
        }

    }
}
