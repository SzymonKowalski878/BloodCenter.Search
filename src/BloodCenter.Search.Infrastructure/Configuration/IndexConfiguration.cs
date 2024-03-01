namespace BloodCenter.Search.Infrastructure.Configuration
{
    public class IndexConfiguration
    {
        public IndexConfiguration(string indexName)
        {
            IndexName = indexName;
        }

        public string IndexName { get; set; }
    }
}
