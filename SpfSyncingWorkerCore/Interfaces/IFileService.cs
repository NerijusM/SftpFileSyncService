using Ardalis.Result;
using SFTP.Wrapper.Responses;

namespace SpfSyncingWorkerCore.Interfaces;

public interface IFileService
{
    Task<Result<string>> SafeFileAsync(string to, DownloadFileResponse response);
}
