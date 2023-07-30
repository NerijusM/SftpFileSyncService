using Quartz;
using SpfSyncingWorkerCore.Extensions;
using SpfSyncingWorkerCore.Interfaces;

namespace SpfSyncingService.Jobs;

public class SyncJob : BaseJob<SyncJob>
{
    private readonly IFileSynchroniseService _fileSynchroniseService;
    private readonly IConfiguration _configuration;

    private readonly string _pathToLoad;
    private readonly string _pathToSave;

    public SyncJob(ILogger<SyncJob> logger, IFileSynchroniseService fileSynchroniseService, IConfiguration configuration) : base(logger)
    {
        _fileSynchroniseService = fileSynchroniseService;
        _configuration = configuration;

        _pathToLoad = _configuration.GetValue<string>("SynchroniseConfig:pathToLoad") ?? string.Empty;
        _pathToSave = _configuration.GetValue<string>("SynchroniseConfig:pathToSave") ?? string.Empty;

        if (string.IsNullOrEmpty(_pathToLoad) ||
            string.IsNullOrEmpty(_pathToSave))
            throw new ArgumentNullException("There no file remote or local path");

    }

    public override async Task ExecuteJob(IJobExecutionContext context)
    {
        var result = await _fileSynchroniseService.SynchronizeAsync(_pathToLoad, _pathToSave);
        if (result.IsSuccess)
            _logger.LogInformation(result.SuccessMessage);

        _logger.LogError(result.Errors.ToErrorString());
    }
}
