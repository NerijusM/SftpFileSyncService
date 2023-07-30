using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpfSyncingWorkerCore.Entities;

namespace SpfSyncingWorkerInfrastructure.Data.Configuration;

public class SyncedFileConfiguration : IEntityTypeConfiguration<SyncedFile>
{
    public void Configure(EntityTypeBuilder<SyncedFile> builder)
    {
        builder.ToTable("SyncedFiles").HasKey(x => x.Id);
    }
}
