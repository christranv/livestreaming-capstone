using System.Reflection;
using Autofac;
using Payment.API.Application.IntegrationEvents;
using Payment.API.Application.IntegrationEvents.EventHandling;
using Team5.BuildingBlocks.MessageBroker.EventBus.Abstractions;
using Module = Autofac.Module;

namespace Payment.API.Infrastructure.AutofacModules
{
    public class ApplicationModule
        : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(NewUserCreatedIntegrationEventHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));
        }
    }
}