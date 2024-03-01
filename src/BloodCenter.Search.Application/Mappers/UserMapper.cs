using BloodCenter.Identity.Client.Models;
using BloodCenter.Search.Client.Models;
using BloodCenter.Search.Domain.Models;

namespace BloodCenter.Search.Application.Mappers
{
    public static class UserMapper
    {
        public static UserDocument Map(UserDto model) =>
            new UserDocument()
            {
                Id = model.Id.ToString(),
                Email = model.Email,
                Role = model.Role,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };

        public static UserDocumentDto Map(UserDocument model) =>
            new(Guid.Parse(model.Id), model.Email, model.Role, model.FirstName, model.LastName);
    }
}
