using BloodCenter.Search.Domain.Models;
using BloodCenter.Search.Infrastructure.IndexBase;
using Nest;

namespace BloodCenter.Search.Infrastructure.UserIndex
{
    public class UserIndexCreator : IndexCreator<UserDocument>
    {
        public UserIndexCreator(IElasticClient client) 
            : base(client)
        {
        }
    }
}
