using Autofac;
using BloodCenter.Search.Application.Commands.AddUserCommand;
using BloodCenter.Search.Application.Queries.GetUsers;

namespace BloodCenter.Search.Application
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterCommandHandlers(builder);
            RegisterQurydHandlers(builder);
        }

        private static void RegisterCommandHandlers(ContainerBuilder builder)
        {
            builder.RegisterType<AddUserCommandHandler>().AsImplementedInterfaces();
        }

        private static void RegisterQurydHandlers(ContainerBuilder builder)
        {
            builder.RegisterType<GetUsersQueryHandler>().AsImplementedInterfaces();
        }
    }
}
