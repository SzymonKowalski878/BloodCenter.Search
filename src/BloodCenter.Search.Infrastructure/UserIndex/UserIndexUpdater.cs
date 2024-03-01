using BloodCenter.Search.Domain.Models;
using BloodCenter.Search.Infrastructure.IndexBase;
using Nest;

namespace BloodCenter.Search.Infrastructure.UserIndex
{
    public class UserIndexUpdater : IndexUpdater<UserDocument>
    {
        public UserIndexUpdater(IElasticClient client) 
            :base(client, "user-index")
        {

        }

    }
}
