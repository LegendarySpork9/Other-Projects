// Copyright © - Unpublished - Toby Hunter
using System.IO;

namespace GoogleDriveSync.Abstractions
{
    // Interface for the file system operations.
    public interface IFileSystem
    {
        #region Files

        bool FileExists(string file);
        bool TryOpenRead(string file);
        FileAttributes GetAttributes(string file);
        void SetAttributes(string file, FileAttributes attributes);
        void DeleteFile(string file);

        #endregion

        #region Directories

        bool DirectoryExists(string fileDirectory);
        void CreateDirectory(string fileDirectory);
        string[] GetDirectories(string folder);
        string[] GetFiles(string folder);

        #endregion
    }
}
