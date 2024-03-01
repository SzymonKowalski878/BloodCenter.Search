using Feree.ResultType.Results;
using Feree.ResultType;

namespace BloodCenter.Search.Domain.Interfaces
{
    public interface IIndexUpdater<T> where T : class, IDocumentBase
    {
        Task<IResult<Unit>> AddOrUpdate(T document, CancellationToken token);
    }
}
