using Microsoft.EntityFrameworkCore;
using SpfSyncingWorkerCore.Entities;
using SpfSyncingWorkerInfrastructure.Data.Configuration;

namespace SpfSyncingWorkerInfrastructure.Data;

public class ServiceDbContext: DbContext
{
    public ServiceDbContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<SyncedFile> SyncedFiles => base.Set< SyncedFile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new SyncedFileConfiguration());
        //If there were more Entities, it would be worth creating an extension that would compile all configurations
    }
}
