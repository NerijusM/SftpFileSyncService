﻿using Quartz;
using SpfSyncingService.Jobs;

namespace SpfSyncingService.Extensions
{
    public static class ServiceCollectionQuartzConfiguratorExtensions
    {
        public static void AddJobAndTrigger<T>(this IServiceCollectionQuartzConfigurator quartz, IConfiguration config)
            where T : IJob
        {
            string jobName = typeof(T).Name;

            var configKey = $"Quartz:{jobName}";
            var cronSchedule = config[configKey];

            if (string.IsNullOrEmpty(cronSchedule))
            {
                throw new Exception($"No Quartz.NET Cron schedule found for job in configuration at {configKey}");
            }

            var jobKey = new JobKey(jobName);
            quartz.AddJob<T>(opts => opts.WithIdentity(jobKey));

            quartz.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity(jobName + "-trigger")
                .WithCronSchedule(cronSchedule));
        }

        public static void AddAllJobAndTrigger(this IServiceCollectionQuartzConfigurator quartz, IConfiguration config)
        {
            // TODO: Add Job and register in appsettings.json in Quartz section
            quartz.AddJobAndTrigger<SyncJob>(config);
        }
    }
}
