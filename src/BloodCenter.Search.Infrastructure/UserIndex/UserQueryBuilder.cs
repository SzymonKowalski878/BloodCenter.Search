using BloodCenter.Search.Domain.Interfaces;
using BloodCenter.Search.Domain.Models;
using BloodCenter.Search.Infrastructure.Configuration;
using Nest;

namespace BloodCenter.Search.Infrastructure.UserIndex
{
    public class UserQueryBuilder : IUserQueryBuilder
    {
        public SearchDescriptor<UserDocument> GetByQuery(string? query)
        {
            SearchDescriptor<UserDocument> searchDescriptor = new SearchDescriptor<UserDocument>(ElasticConfiguration.UserDocument.IndexName);

            searchDescriptor.Query(q => q
                .QueryString(qs => qs
                    .Query(query)
                    .DefaultOperator(Operator.And)
                    .Fuzziness(Fuzziness.Ratio(1))
                    .Fields(fs => fs
                        .Field(x => x.Id))
                    ));

            return searchDescriptor;
        }
    }
}
