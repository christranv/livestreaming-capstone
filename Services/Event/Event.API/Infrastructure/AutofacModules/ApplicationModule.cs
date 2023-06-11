using System.Reflection;
using Autofac;
using Event.API.Application.IntegrationEvents.EventHandling;
using Team5.BuildingBlocks.MessageBroker.EventBus.Abstractions;

namespace Event.API.Infrastructure.AutofacModules
{
    public class ApplicationModule : Autofac.Module
    {
        public ApplicationModule()
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(SubEventStreamSessionFinishedIntegrationEventHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));
        }
    }
}