using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace BloodCenter.Search.IntegrationTests.Infrastructure
{
    public class CustomAutofacServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        private readonly Action<ContainerBuilder> _configurationOverride;

        public CustomAutofacServiceProviderFactory(Action<ContainerBuilder> configurationOverride)
        {
            _configurationOverride = configurationOverride;
        }

        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            var builder = new ContainerBuilder();

            builder.Populate(services);

            return builder;
        }

        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            _configurationOverride(containerBuilder);
            var container = containerBuilder.Build();

            return new AutofacServiceProvider(container);
        }
    }
}
