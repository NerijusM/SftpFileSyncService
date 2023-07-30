namespace SpfSyncingWorkerCore.Entities;

public class SyncedFile: EntityBase
{
    public string FileName { get; private set; } = string.Empty;
    public string LocalPath { get; private set; } = string.Empty;
    public string RemotePath { get; private set; } = string.Empty;
    public DateTime LastWriteTime { get; private set; } = DateTime.MinValue;
    public DateTime RecordDate { get; private set; } = DateTime.Now;


    public void SetFileInfo(string fileName, string localPath, string remotePath, DateTime lastWriteTime)
    {
        FileName = fileName;
        LocalPath = localPath;
        RemotePath = remotePath;
        LastWriteTime = lastWriteTime;
    }
}
