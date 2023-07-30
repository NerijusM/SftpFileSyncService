using SFTP.Wrapper;
using SFTP.Wrapper.Requests;
using SpfSyncingWorkerCore.Entities;
using SpfSyncingWorkerCore.Interfaces;
using SpfSyncingWorkerCore.Extensions;
using SFTP.Wrapper.Responses;
using Ardalis.Result;
using SFTP.Wrapper.Models;
using Microsoft.Extensions.Logging;

namespace SpfSyncingWorkerCore.Services;

public class FileSynchroniseService : IFileSynchroniseService
{
    private readonly ISyncedFileRepository _syncedFileRepository;
    private readonly IFileService _fileService;
    private readonly ISftpManager _sftpManager;
    private readonly ILogger<FileSynchroniseService> _logger;

    public FileSynchroniseService(ISyncedFileRepository syncedFileRepository,
        ISftpManager sftpManager,
        ILogger<FileSynchroniseService> logger,
        IFileService fileService)
    {
        _syncedFileRepository = syncedFileRepository;
        _sftpManager = sftpManager;
        _logger = logger;
        _fileService = fileService;
    }
    /// <summary>
    ///The method only synchronizes in the given sftp directory
    /// </summary>
    /// <param name="remotePath">Remote path string</param>
    /// <param name="localPath">Local path string</param>
    /// <returns></returns>
    public async Task<Result> SynchronizeAsync(string remotePath, string localPath)
    {

        var currentSftpFileList = await CurrentFilesInSftp(remotePath);       

        if (currentSftpFileList.Count() == 0)
            return Result.SuccessWithMessage("No files to sync");

        var filesNotInLocalResult = FilesNotInLocalArea(currentSftpFileList);
        if (!filesNotInLocalResult.IsSuccess)
            return Result.Error(filesNotInLocalResult.Errors.ToErrorString());

        //TODO: - The task does not specify whether to delete records if the file's last recording time has changed
        // If delete uncoment
        //var filesToDeletoFromDbResult = FilesWithChangedLastWritingTime(currentSftpFileList);
        //var fileDeleteResult = await DeleteFiles(filesToDeletoFromDbResult.Value);
        //if (!fileDeleteResult.IsSuccess)
        //    return Result.Error("Cant delete file record from Db");

        var result = await SyncFiles(localPath, filesNotInLocalResult.Value);
        if (!result.IsSuccess)
            return Result.Error(result.Errors.ToErrorString());

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="to"></param>
    /// <param name="filesToSync"></param>
    /// <returns></returns>
    private async Task<Result> SyncFiles(string to, IEnumerable<SftpFileInformation> filesToSync)
    {
        if (filesToSync.Count() == 0)
            return Result.NotFound("List of files to sync is empty");

        foreach (var file in filesToSync)
        {
            var result = await SyncFile(to, file);
            if (!result.IsSuccess)
            {
                OnError(result.Errors.ToErrorString());
                continue;
            }
        }
        return Result.SuccessWithMessage("Files synced");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="to"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    private async Task<Result> SyncFile(string to, SftpFileInformation file)
    {
        var fileResult = await LoadFile(file.FullName);
        if (!fileResult.IsSuccess)
            return Result.Error(fileResult.Errors.ToErrorString());

        var saveFileResult = await _fileService.SafeFileAsync(to, fileResult);
        if (!saveFileResult.IsSuccess)
            return Result.Error(saveFileResult.Errors.ToErrorString());

        var syncedFile = new SyncedFile();
        syncedFile.SetFileInfo(file.Name, file.FullName, saveFileResult.Value, file.LastWriteTime);

        var saveToDbResult = await SaveFileRecordToDb(to, syncedFile);
        if (!saveToDbResult.IsSuccess)
            return Result.Error(saveFileResult.Errors.ToErrorString());

        return Result.SuccessWithMessage($"File {file.FullName} succesfully synced");

    }

    /// <summary>
    /// The method returns the sftp list of currently existing files
    /// </summary>
    /// <param name="pathToLoad"></param>
    /// <returns></returns>
    private async Task<IEnumerable<SftpFileInformation>> CurrentFilesInSftp(string pathToLoad)
    {
        var result = await _sftpManager.GetAllFilesAsync(new GetAllFilesRequest(pathToLoad)).ConfigureAwait(false);

        if (result.Status == false)
            return Enumerable.Empty<SftpFileInformation>();

        if (result.Data == null)
            return Enumerable.Empty<SftpFileInformation>();

        var data = result.Data;
        var files = data.Files.Where(f => f.Length > 0);

        return files;
    }

    /// <summary>
    /// The method downloads the file
    /// </summary>
    /// <param name="fullFilePath"></param>
    /// <returns></returns>
    private async Task<Result<DownloadFileResponse>> LoadFile(string fullFilePath)
    {
        var downloadFileRequest = new DownloadFileRequest(fullFilePath);
        var response = await _sftpManager.DownloadFileAsync(downloadFileRequest);
        if (response.Status == false)
            return Result.Error(response.Message);

        return Result.Success(response.Data);
    }

    /// <summary>
    /// The method increments the case record in the db
    /// </summary>
    /// <param name="to"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    private async Task<Result> SaveFileRecordToDb(string to, SyncedFile file)
    {
        if (file == null)
            return Result.Error("SyncedFile is null");
        try
        {
            await _syncedFileRepository.AddAsync(file);

        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }

        return Result.Success();
    }

    /// <summary>
    /// The method selects files from the list that are not located in the location
    /// </summary>
    /// <param name="filesInSftp"></param>
    /// <returns></returns>
    private Result<IEnumerable<SftpFileInformation>> FilesNotInLocalArea(IEnumerable<SftpFileInformation> filesInSftp)
    {
        if (filesInSftp == null || filesInSftp.Count() == 0)
            return Result.Success(Enumerable.Empty<SftpFileInformation>());

               var filesNotInDbNames = _syncedFileRepository.FilesNotInDb(filesInSftp);
        return Result.Success(filesNotInDbNames);
    }

    /// <summary>
    /// The method selects files whose last recording time has changed from 
    /// the list (Used only if the record is deleted when the change occurs)
    /// </summary>
    /// <param name="filesInSftp"></param>
    /// <returns></returns>
    private Result<IEnumerable<SftpFileInformation>> FilesWithChangedLastWritingTime(IEnumerable<SftpFileInformation> filesInSftp)
    {
        if (filesInSftp == null || filesInSftp.Count() == 0)
            return Result.Success(Enumerable.Empty<SftpFileInformation>());

        var filesWithChangedWriteTime = _syncedFileRepository.FilesWithChangedresoRedingTimes(filesInSftp);
        return Result.Success(filesWithChangedWriteTime);
    }

    /// <summary>
    /// The method deletes files by list
    /// </summary>
    /// <param name="filesToDelete"></param>
    /// <returns></returns>
    private async Task<Result> DeleteFiles(IEnumerable<SftpFileInformation> filesToDelete)
    {
        try
        {
            await _syncedFileRepository.DeleteChangedFilesFromDb(filesToDelete);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    private void OnError(string message)
    {
        _logger.LogError(message);
    }
}


