using BloodCenter.Search.Domain.Interfaces;
using Feree.ResultType;
using Feree.ResultType.Factories;
using Feree.ResultType.Results;
using Nest;

namespace BloodCenter.Search.Infrastructure.IndexBase
{
    public class IndexUpdater<T> : IIndexUpdater<T> where T : class, IDocumentBase
    {
        private readonly IElasticClient _client;
        private string _indexName;

        public IndexUpdater(IElasticClient client, string indexName)
        {
            _client = client;
            _indexName = indexName;
        }

        public async Task<IResult<Unit>> AddOrUpdate(T document, CancellationToken token)
        {
            var response = await _client.UpdateAsync(DocumentPath<T>.Id(document.Id),
                x => x.Index(_indexName).Doc(document).Upsert(document));

            return response.IsValid
                ? ResultFactory.CreateSuccess()
                : ResultFactory.CreateFailure(response.OriginalException.Message);
        }
    }
}
