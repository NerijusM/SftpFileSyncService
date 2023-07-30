using Ardalis.Result;

namespace SpfSyncingWorkerCore.Interfaces;

public interface IFileSynchroniseService
{
    Task<Result> SynchronizeAsync(string remotePath, string localPath);
}