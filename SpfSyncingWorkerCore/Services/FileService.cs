using Ardalis.Result;
using SFTP.Wrapper.Responses;
using SpfSyncingWorkerCore.Interfaces;

namespace SpfSyncingWorkerCore.Services;
/// <summary>
///A file service is designed to perform file operations
/// </summary>
public class FileService: IFileService
{   
    /// <summary>
    /// Saves file
    /// </summary>
    /// <param name="to"></param>
    /// <param name="response"></param>
    /// <returns></returns>
    public async Task<Result<string>> SafeFileAsync(string to, DownloadFileResponse response)
    {
        if (to == null)
            return Result.Error("Path to save file is null");

        if (response == null)
            return Result.Error($"Download file response is null");

        var isFolderExists = CheckIfFolderExistOrCreate(to);
        if (!isFolderExists.IsSuccess)
            return isFolderExists;


        var localPath = Path.Combine(to, response.FileName);
        return await SaveFileFromstream(response.Stream, localPath);
    }

    /// <summary>
    /// Saves the file to the specified location from the stream
    /// </summary>
    /// <param name="memoryStream">Stream</param>
    /// <param name="localPath">location</param>
    /// <returns></returns>
    private static async Task<Result<string>> SaveFileFromstream(Stream? memoryStream, string localPath)
    {
        //TODO: delete if files can be modified
        //if (IsFileExists(localPath))
        //    return Result.Error($"File allready exist in current path: {localPath}.");

        if (memoryStream == null)
            return Result.Error("Files stream is empty");

        try
        {
            using FileStream file = new FileStream(localPath, FileMode.Create, FileAccess.Write);
            byte[] bytes = new byte[memoryStream.Length];
            await memoryStream.ReadAsync(bytes, 0, bytes.Length);
            await file.WriteAsync(bytes, 0, bytes.Length);
            memoryStream.Close();
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
        return Result.Success(localPath);
    }

    /// <summary>
    /// Checks if the directory exists and creates it if not
    /// </summary>
    /// <param name="pathToFolder"></param>
    /// <returns></returns>
    private Result CheckIfFolderExistOrCreate(string pathToFolder)
    {
        bool isFolderExists = Directory.Exists(pathToFolder);
        if (isFolderExists)
            return Result.Success();

        try
        {
            Directory.CreateDirectory(pathToFolder);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);

        }
        return Result.Success();
    }
    /// <summary>
    /// Checks if file exists
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private bool IsFileExists(string path)
    {
        if (File.Exists(path))
            return true;

        return false;
    }
}
