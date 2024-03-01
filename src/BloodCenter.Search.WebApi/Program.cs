using BloodCenter.Search.WebApi;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetupConfiguration(builder.Configuration, builder.Environment)
    .Build();

var isIntegrationTests = builder.Environment.EnvironmentName.Equals("IntegrationTests");

builder
    .Services
    .SetupServices(configuration, isIntegrationTests);

builder
    .Host
    .SetupHost(isIntegrationTests);

var app = builder.Build();

await app
    .SetupApplication()
    .RunAsync();


namespace BloodCenter.Search.WebApi
{
    public partial class Program
    {

    }
}