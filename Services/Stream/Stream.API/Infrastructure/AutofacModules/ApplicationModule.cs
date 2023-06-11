using System.Reflection;
using Autofac;
using Stream.API.Application.IntegrationEvents.EventHandling;
using Stream.API.Application.Queries;
using Stream.Domain.AggregatesModel.StreamerAggregate;
using Stream.Domain.AggregatesModel.StreamSessionAggregate;
using Stream.Infrastructure.Idempotency;
using Stream.Infrastructure.Repositories;
using Team5.BuildingBlocks.MessageBroker.EventBus.Abstractions;
using Module = Autofac.Module;

namespace Stream.API.Infrastructure.AutofacModules
{
    public class ApplicationModule
        : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StreamSessionQueries>()
                .As<IStreamSessionQueries>()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<StreamerQueries>()
                .As<IStreamerQueries>()
                .InstancePerLifetimeScope();

            builder.RegisterType<StreamerRepository>()
                .As<IStreamerRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<StreamSessionRepository>()
                .As<IStreamSessionRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RequestManager>()
                .As<IRequestManager>()
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(NewUserCreatedIntegrationEventHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));
        }
    }
}