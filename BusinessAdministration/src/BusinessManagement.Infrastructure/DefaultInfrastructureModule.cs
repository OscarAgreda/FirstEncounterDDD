using System.Collections.Generic;
using System.Reflection;
using Autofac;
using BusinessManagement.Core.Aggregates;
using BusinessManagement.Core.Interfaces;
using BusinessManagement.Infrastructure.Data;
using BusinessManagement.Infrastructure.Messaging;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.ObjectPool;
using FirstEncounterDDD.SharedKernel.Interfaces;
using RabbitMQ.Client;
using Module = Autofac.Module;

namespace BusinessManagement.Infrastructure
{
  public class DefaultInfrastructureModule : Module
  {
    private readonly bool _isDevelopment = false;
    private readonly List<Assembly> _assemblies = new List<Assembly>();

    public DefaultInfrastructureModule(bool isDevelopment, Assembly callingAssembly = null)
    {
      _isDevelopment = isDevelopment;
      var coreAssembly = Assembly.GetAssembly(typeof(Room));
      var infrastructureAssembly = Assembly.GetAssembly(typeof(DefaultInfrastructureModule));
      _assemblies.Add(coreAssembly);
      _assemblies.Add(infrastructureAssembly);
      if (callingAssembly != null)
      {
        _assemblies.Add(callingAssembly);
      }
    }

    protected override void Load(ContainerBuilder builder)
    {
      if (_isDevelopment)
      {
        RegisterDevelopmentOnlyDependencies(builder);
      }
      else
      {
        RegisterProductionOnlyDependencies(builder);
      }
      RegisterCommonDependencies(builder);
    }

    private void RegisterCommonDependencies(ContainerBuilder builder)
    {
      builder.RegisterGeneric(typeof(EfRepository<>))
        .As(typeof(IRepository<>))
        .InstancePerLifetimeScope();

      builder
          .RegisterType<Mediator>()
          .As<IMediator>()
          .InstancePerLifetimeScope();

      builder.Register<ServiceFactory>(context =>
      {
        var c = context.Resolve<IComponentContext>();
        return t => c.Resolve(t);
      });

      var mediatrOpenTypes = new[]
      {
                typeof(IRequestHandler<,>),
                typeof(IRequestExceptionHandler<,,>),
                typeof(IRequestExceptionAction<,>),
                typeof(INotificationHandler<>),
            };

      foreach (var mediatrOpenType in mediatrOpenTypes)
      {
        builder
        .RegisterAssemblyTypes(_assemblies.ToArray())
        .AsClosedTypesOf(mediatrOpenType)
        .AsImplementedInterfaces();
      }

      builder.RegisterType<EmailSender>().As<IEmailSender>()
          .InstancePerLifetimeScope();

      builder.RegisterType<AppDbContextSeed>().InstancePerLifetimeScope();

      // register RabbitMQ types
      builder.RegisterType<RabbitMessagePublisher>()
        .As<IMessagePublisher>()
        .SingleInstance();
      builder.RegisterType<DefaultObjectPoolProvider>()
        .As<ObjectPoolProvider>()
        .SingleInstance();
      builder.RegisterType<RabbitModelPooledObjectPolicy>()
        .As<IPooledObjectPolicy<IModel>>()
        .SingleInstance();
    }

    private void RegisterDevelopmentOnlyDependencies(ContainerBuilder builder)
    {
      // TODO: Add development only services
    }

    private void RegisterProductionOnlyDependencies(ContainerBuilder builder)
    {
      // TODO: Add production only services
    }

  }
}
