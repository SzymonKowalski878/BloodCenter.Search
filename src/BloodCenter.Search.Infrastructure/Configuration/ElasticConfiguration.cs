namespace BloodCenter.Search.Infrastructure.Configuration
{
    public class ElasticConfiguration
    {
        public static IndexConfiguration UserDocument { get; } = new("user-index");
    }
}
