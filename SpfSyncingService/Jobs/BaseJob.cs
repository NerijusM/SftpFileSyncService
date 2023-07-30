using Quartz;

namespace SpfSyncingService.Jobs;

[DisallowConcurrentExecution]
public abstract class BaseJob<T> : IJob where T : IJob
{
    protected readonly ILogger<T> _logger;
    protected BaseJob(ILogger<T> logger)
    {
        _logger = logger;
    }
    public abstract Task ExecuteJob(IJobExecutionContext context);

    public Task Execute(IJobExecutionContext context)
    {
        try
        {
            _logger.LogInformation($"Job started");
            return ExecuteJob(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Task.CompletedTask;
        }
    }
}