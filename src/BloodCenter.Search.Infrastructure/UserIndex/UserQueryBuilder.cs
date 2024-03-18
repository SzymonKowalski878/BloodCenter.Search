using BloodCenter.Search.Domain.Interfaces;
using BloodCenter.Search.Domain.Models;
using BloodCenter.Search.Infrastructure.Configuration;
using Nest;

namespace BloodCenter.Search.Infrastructure.UserIndex
{
    public class UserQueryBuilder : IUserQueryBuilder
    {
        public SearchDescriptor<UserDocument> GetByQuery(string? query, IReadOnlyList<string>? roles)
        {
            SearchDescriptor<UserDocument> searchDescriptor = new SearchDescriptor<UserDocument>(ElasticConfiguration.UserDocument.IndexName);

            searchDescriptor.Query(q => q
                .QueryString(qs => qs
                    .Query(query)
                    .DefaultOperator(Operator.And)
                    .Fuzziness(Fuzziness.Ratio(1))
                    .Fields(fs => fs
                        .Field(x => x.FirstName)
                        .Field(x => x.LastName)
                        .Field(x => x.Email))
                    ));

            if (roles is not null && roles.Any())
                searchDescriptor = searchDescriptor
                    .Query(q => q
                        .Terms(ts => ts
                            .Field(f => f.Role.Suffix("keyword"))
                            .Terms(roles)));

            return searchDescriptor;
        }
    }
}
