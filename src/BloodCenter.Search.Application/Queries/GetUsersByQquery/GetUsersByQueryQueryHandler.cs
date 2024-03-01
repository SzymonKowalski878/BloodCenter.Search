using BloodCenter.Search.Application.Mappers;
using BloodCenter.Search.Client.Models;
using BloodCenter.Search.Domain.Interfaces;
using BloodCenter.Search.Domain.Models;
using Feree.ResultType.Factories;
using Feree.ResultType.Results;
using Nest;

namespace BloodCenter.Search.Application.Queries.GetUsersByQquery
{
    public class GetUsersByQueryQueryHandler : MediatR.IRequestHandler<GetUsersByQueryQuery, IResult<IReadOnlyList<UserDocumentDto>>>
    {
        private readonly IUserQueryBuilder _userQueryBuilder;
        private readonly IElasticClient _elasticClient;

        public GetUsersByQueryQueryHandler(IElasticClient elasticClient, IUserQueryBuilder userQueryBuilder)
        {
            _elasticClient = elasticClient;
            _userQueryBuilder = userQueryBuilder;
        }

        public async Task<IResult<IReadOnlyList<UserDocumentDto>>> Handle(GetUsersByQueryQuery request, CancellationToken cancellationToken)
        {
            var query = _userQueryBuilder.GetByQuery(request.request.QueryString);

            var response = await _elasticClient.SearchAsync<UserDocument>(query);

            if (!response.IsValid)
                return ResultFactory.CreateFailure<IReadOnlyList<UserDocumentDto>>(response.OriginalException.Message);

            var items = response.Documents.ToArray();

            return ResultFactory.CreateSuccess<IReadOnlyList<UserDocumentDto>>(items.Select(x => UserMapper.Map(x)).ToArray());
        }

    }
}
