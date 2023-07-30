using System;

namespace SFTP.Wrapper.Models
{
    public class SftpFileInformation
    {
        public SftpFileInformation(string name, string fullName, long length, DateTime lastWriteTime)
        {
            Name = name;
            FullName = fullName;
            Length = length;
            LastWriteTime = lastWriteTime;
        }

        public string Name { get; }
        public string FullName { get; }
        public long Length { get; }
        public DateTime LastWriteTime { get; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(FullName) && Length > 0;
        }
    }
}