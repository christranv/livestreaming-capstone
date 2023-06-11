using System.Reflection;
using Autofac;
using Notification.API.IntegrationEvents.EventHandling;
using Team5.BuildingBlocks.MessageBroker.EventBus.Abstractions;

namespace Notification.API.AutofacModules
{
    public class ApplicationModule
        : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(NewStreamCreatedEventHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));
        }
    }
}
