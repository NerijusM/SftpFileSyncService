using Microsoft.EntityFrameworkCore;
using SFTP.Wrapper.Models;
using SpfSyncingWorkerCore.Entities;
using SpfSyncingWorkerCore.Interfaces;

namespace SpfSyncingWorkerInfrastructure.Data.Repositories;

/// <summary>
/// The repository class is responsible for actions of the SyncedFile entity with the database
/// </summary>
public class SyncedFileRepository : Repository<SyncedFile>, ISyncedFileRepository
{
    public SyncedFileRepository(ServiceDbContext context) : base(context) { }

    /// <summary>
    /// Compares and returns an IEnumerable<SftpFileInformation> type list of files that are not in the database
    /// </summary>
    /// <param name="files">list odf files</param>
    /// <returns></returns>
    public IEnumerable<SftpFileInformation> FilesNotInDb(IEnumerable<SftpFileInformation> files)
    {
        var filesInDb = files.Where(f => Context.Set<SyncedFile>()
                                          .Any(x => x.FileName == f.Name && x.LastWriteTime == f.LastWriteTime));

        var filesNotInDb = files.Except(filesInDb).ToList(); 

        return filesNotInDb;
    }

    /// <summary>
    ///  Compares and returns an IEnumerable<SftpFileInformation> type list of files that are in the database but have different last save times
    /// </summary>
    /// <param name="files"></param>
    /// <returns></returns>
    public IEnumerable<SftpFileInformation> FilesWithChangedresoRedingTimes(IEnumerable<SftpFileInformation> files)
    {
        var result = files.Where(f => Context.Set<SyncedFile>()
                                          .Any(x => x.FileName == f.Name && x.LastWriteTime != f.LastWriteTime));
        return result;

    }

    /// <summary>
    /// Deletes entries of the given IEnumerable<SftpFileInformation> list type from the database by filename
    /// </summary>
    /// <param name="files"></param>
    /// <returns></returns>
    public async Task DeleteChangedFilesFromDb(IEnumerable<SftpFileInformation> files)
    {
        if (files.Count() == 0)
            return;

        foreach(var file in files)
        {
            await DeletechangedFile(file);
        }
    }

    /// <summary>
    /// Deletes records of the given SftpFileInformation type from the database by file name
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    private async Task DeletechangedFile(SftpFileInformation file)
    {
        var entity = await SingleOrDefaultAsync(x => x.FileName == file.Name);
        
        if (entity == null)
            return;

        await DeleteAsync(entity);
    }

}
