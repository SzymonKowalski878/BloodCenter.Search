using BloodCenter.Search.Domain.Interfaces;
using BloodCenter.Search.Infrastructure.Configuration;
using Feree.ResultType;
using Feree.ResultType.Factories;
using Feree.ResultType.Results;
using Nest;

namespace BloodCenter.Search.Infrastructure.IndexBase
{
    public class IndexCreator<T> : IIndexCreator where T : class, IDocumentBase
    {
        private readonly IElasticClient _client;

        public IndexCreator(IElasticClient client)
        {
            _client = client;
        }

        public async Task<IResult<Unit>> Create(CancellationToken token)
        {
            var indexExists = await _client.Indices.ExistsAsync(ElasticConfiguration.UserDocument.IndexName);

            if (!indexExists.Exists)
            {

                var indexDescriptor = new CreateIndexDescriptor(ElasticConfiguration.UserDocument.IndexName)
                    .Map(m => m.AutoMap<T>());

                try
                {
                    await _client.Indices.CreateAsync(indexDescriptor);
                }
                catch (Exception ex)
                {
                    return ResultFactory.CreateFailure("Failure during index creation: " + ex.Message);
                }
            }
            else
            {
                try
                {
                    await _client.MapAsync<T>(m => m.AutoMap().Index(ElasticConfiguration.UserDocument.IndexName));
                }
                catch (Exception ex)
                {
                    return ResultFactory.CreateFailure("Failure during index update: " + ex.Message);
                }
            }

            var aliasExists = await _client.Indices.AliasExistsAsync(ElasticConfiguration.UserDocument.IndexName + "-alias");

            if (!aliasExists.Exists)
            {
                var isValid = await _client.Indices.PutAliasAsync(ElasticConfiguration.UserDocument.IndexName, ElasticConfiguration.UserDocument.IndexName + "-alias");

                if(!isValid.IsValid)
                    return ResultFactory.CreateFailure("Failure during alias creation: " + ElasticConfiguration.UserDocument.IndexName +"-alias");
                
            }
            return ResultFactory.CreateSuccess();
        }
    }
}
