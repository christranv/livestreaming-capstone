using System.Reflection;
using Autofac;
using Stream.SignalrHub.IntegrationEvents.EventHandling;
using Team5.BuildingBlocks.MessageBroker.EventBus.Abstractions;

namespace Stream.SignalrHub.AutofacModules
{
    public class ApplicationModule
        : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(NewDonateRecordAddedIntegrationEventHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));
        }
    }
}
