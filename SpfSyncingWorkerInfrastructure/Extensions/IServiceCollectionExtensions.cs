using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SpfSyncingWorkerInfrastructure.Data;

namespace SpfSyncingWorkerInfrastructure.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ServiceDbContext>(options =>
          options.UseSqlite(connectionString));
        // TODO: if use PostgreSQL 
        //options.UseNpgsql(connectionString)); 
    }
}
