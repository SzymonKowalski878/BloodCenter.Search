using BloodCenter.Search.Domain.Interfaces;
using Elasticsearch.Net;
using Nest;
using System.Collections.Concurrent;

namespace BloodCenter.Search.IntegrationTests.Infrastructure
{
    public class Seeder<T> where T : class, IDocumentBase
    {
        private IElasticClient _elasticClient;
        private readonly string _index;
        //store ids of documents that have been seeded to remove them after tests
        private readonly ConcurrentBag<string> _ids = new();

        public Seeder(IElasticClient elasticClient, string index)
        {
            _elasticClient = elasticClient;
            _index = index;
        }

        public async Task Seed(IEnumerable<T> documents)
        {
            var operations = new BulkOperationsCollection<IBulkOperation>();
            foreach (var doc in documents)
            {
                _ids.Add(doc.Id);
                operations.Add(new BulkCreateOperation<T>(doc)
                {
                    Id = doc.Id,
                    Index = _index
                });
            }

            await _elasticClient.BulkAsync(new BulkRequest { Operations = operations, Refresh = Refresh.WaitFor });
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        public void AddDocumentIdToBeRemoved(Guid id)
        {
            _ids.Add(id.ToString());
        }

        public async Task<T?> FindDocumentById(string id)
        {
            var response = await _elasticClient.SearchAsync<T>(x =>
                x.Query(q => q
                        .Match(t => t
                            .Field(f => f.Id)
                            .Query(id)))
                    .Index(_index));

            return response.Documents.FirstOrDefault();
        }

        public async Task DeleteDocumentByIds(IEnumerable<string> ids)
        {
            if (ids.Any())
            {
                var response = await _elasticClient.DeleteByQueryAsync<T>(x => x
                    .Query(q => q
                        .Terms(t => t
                            .Field(f => f.Id.Suffix("keyword"))
                            .Terms(_ids.ToArray())))
                    .Index(_index));
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }

        public async Task DeleteAll()
        {
            if (_ids.Any())
            {
                var response = await _elasticClient.DeleteByQueryAsync<T>(x => x
                    .Query(q => q
                        .Terms(t => t
                            .Field(f => f.Id.Suffix("keyword"))
                            .Terms(_ids.ToArray())))
                    .Index(_index));
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }
    }
}
