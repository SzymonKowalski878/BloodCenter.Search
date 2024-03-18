using BloodCenter.Search.Domain.Models;
using Nest;

namespace BloodCenter.Search.Domain.Interfaces
{
    public interface IUserQueryBuilder
    {
        SearchDescriptor<UserDocument> GetByQuery(string? query, IReadOnlyList<string>? roles = null);
    }
}
