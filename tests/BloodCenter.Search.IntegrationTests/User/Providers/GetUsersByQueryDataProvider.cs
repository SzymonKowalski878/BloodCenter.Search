using BloodCenter.Search.Domain.Models;
using BloodCenter.Search.IntegrationTests.Infrastructure;
using MassTransit;

namespace BloodCenter.Search.IntegrationTests.User.Providers
{
    public class GetUsersByQueryDataProvider : BaseDataProvider
    {
        public Guid UserId { get; set; }

        public IReadOnlyList<UserDocument> Documents { get; set; }

        public GetUsersByQueryDataProvider()
        {
            UserId = Guid.NewGuid();
            Documents = new List<UserDocument>()
            {
                new UserDocument
                {
                    Id = UserId.ToString(),
                    FirstName = "dasda",
                    LastName = "disagdujas",
                    Email = "dgasjd@gmail.com",
                    Role = "Admin"
                },
                new UserDocument
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = "da123sda",
                    LastName = "dis1235agdujas",
                    Email = "dgasjd23155@gmail.com",
                    Role = "Admin"
                },
                new UserDocument
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = "dasda21321",
                    LastName = "disagdujas32141",
                    Email = "dgasjd@gmai312l.com",
                    Role = "Admin"
                },
            };
        }
    }
}
