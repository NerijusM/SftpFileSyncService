using Autofac;
using SpfSyncingWorkerCore.Interfaces;
using SpfSyncingWorkerInfrastructure.Data.Repositories;

namespace SpfSyncingWorkerInfrastructure;

public class DefaultInfrastructureModule: Module
{   
    protected override void Load(ContainerBuilder builder)
    {
        // TODO: register repositories
        builder.RegisterType<SyncedFileRepository>()
           .As<ISyncedFileRepository>().InstancePerLifetimeScope();
      
    }

}
