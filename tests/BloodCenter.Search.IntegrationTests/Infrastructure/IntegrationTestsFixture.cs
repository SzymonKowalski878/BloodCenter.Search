using Autofac;
using BloodCenter.Search.Client;
using BloodCenter.Search.WebApi;
using BloodCenter.Search.Domain.Interfaces;
using BloodCenter.Search.Domain.Models;
using BloodCenter.Search.Infrastructure.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nest;
using NSubstitute;
using Xunit;

namespace BloodCenter.Search.IntegrationTests.Infrastructure
{
    public class IntegrationTestsFixture<TDataProvider, TClient> : IntegrationTestsFixture<TDataProvider>
        where TDataProvider : BaseDataProvider, new()
        where TClient : CustomHttpClient
    {
        public readonly TClient ApiClient;

        public IntegrationTestsFixture()
            : base()
        {
            ApiClient = GetApiClient();
        }

        private TClient GetApiClient()
        {
            var client = CreateClient();
            var c = Activator.CreateInstance(typeof(TClient), client, client.BaseAddress ?? new Uri("http://localhost/")) as TClient;
            client.BaseAddress = null;
            return c!;
        }
    }

    public class IntegrationTestsFixture<TDataProvider> : WebApplicationFactory<Program>, IAsyncLifetime
        where TDataProvider : BaseDataProvider, new()
    {
        public TDataProvider DataProvider;
        public Identity.Client.IIdentityClient IdentityClientMock = Substitute.For<Identity.Client.IIdentityClient>();
        public readonly Seeder<UserDocument> UserSeeder;

        public IntegrationTestsFixture()
        {
            DataProvider = new TDataProvider();
            UserSeeder = new Seeder<UserDocument>(
                Services.GetRequiredService<IElasticClient>(),
                ElasticConfiguration.UserDocument.IndexName);
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseEnvironment("IntegrationTests");
            builder.ConfigureAppConfiguration(b => b.AddJsonFile("appsettings.IntegrationTests.json"));
            builder.UseServiceProviderFactory(new CustomAutofacServiceProviderFactory(Register));

            return base.CreateHost(builder);
        }

        private void Register(ContainerBuilder container)
        {
            container.Register(_ => IdentityClientMock).AsImplementedInterfaces().SingleInstance();
        }

        public virtual async Task InitializeAsync()
        {
            var indexCreators = Services.GetServices<IIndexCreator>();
            foreach (var indexCreator in indexCreators)
            {
                await indexCreator.Create(default);
            }
        }

        public new virtual async Task DisposeAsync()
        {
            await UserSeeder.DeleteAll();
            await base.DisposeAsync();
        }
    }
}
