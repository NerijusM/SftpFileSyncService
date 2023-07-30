using Autofac;
using SpfSyncingWorkerCore.Interfaces;
using SpfSyncingWorkerCore.Services;

namespace SpfSyncingWorkerCore;

public class DefaultCoreModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // TODO: register services
        builder.RegisterType<FileSynchroniseService>()
           .As<IFileSynchroniseService>().InstancePerLifetimeScope();

        builder.RegisterType<FileService>()
          .As<IFileService>().InstancePerLifetimeScope();
    }
}
