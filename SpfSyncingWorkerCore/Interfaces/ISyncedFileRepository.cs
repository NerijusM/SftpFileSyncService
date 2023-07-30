using SFTP.Wrapper.Models;
using SpfSyncingWorkerCore.Entities;

namespace SpfSyncingWorkerCore.Interfaces;

public interface ISyncedFileRepository: IRepository<SyncedFile>
{
    IEnumerable<SftpFileInformation> FilesNotInDb(IEnumerable<SftpFileInformation> files);
    IEnumerable<SftpFileInformation> FilesWithChangedresoRedingTimes(IEnumerable<SftpFileInformation> files);
    Task DeleteChangedFilesFromDb(IEnumerable<SftpFileInformation> files);
}
