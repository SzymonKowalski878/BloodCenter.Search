using Autofac;
using BloodCenter.Search.Infrastructure.UserIndex;

namespace BloodCenter.Search.Infrastructure
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterIndexCreators(builder);
        }

        private static void RegisterIndexCreators(ContainerBuilder builder)
        {
            builder.RegisterType<UserIndexCreator>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<UserIndexUpdater>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<UserQueryBuilder>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
