using BloodCenter.Identity.Client.Models;
using BloodCenter.Search.IntegrationTests.Infrastructure;

namespace BloodCenter.Search.IntegrationTests.Events.Providers
{
    public class UserAddedEventDataProvider : BaseDataProvider
    {
        public UserDto GetUserByIdResponse = new(Guid.NewGuid(), "zag@gmail.com", "Admin", "First", "Last");
    }
}
