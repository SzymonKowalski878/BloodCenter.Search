using Feree.ResultType;
using Feree.ResultType.Results;

namespace BloodCenter.Search.Domain.Interfaces
{
    public interface IIndexCreator
    {
        Task<IResult<Unit>> Create(CancellationToken token);
    }
}
