// Copyright © - Unpublished - Toby Hunter
using GoogleDriveSync.Abstractions;
using System;
using System.IO;

namespace GoogleDriveSync.Implementations
{
    public class FileSystemWrapper : IFileSystem
    {
        #region Files

        // Returns whether the given file exists.
        public bool FileExists(string file) => File.Exists(file);

        // Returns the attributes for the given file.
        public FileAttributes GetAttributes(string file) => File.GetAttributes(file);

        // Changes the value of an attribute for the given file.
        public void SetAttributes(string file, FileAttributes attribute) => File.SetAttributes(file, attribute);

        // Deletes the given file.
        public void DeleteFile(string file) => File.Delete(file);

        // Returns whether the file is in use.
        public bool TryOpenRead(string file)
        {
            try
            {
                using (FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return true;
                }
            }

            catch
            {
                return false;
            }
        }

        // Writes bytes to the given file.
        public void WriteDataToFile(string file, byte[] data) => File.WriteAllBytes(file, data);

        // Sets the created time of the given file.
        public void SetCreatedTime(string file, DateTime created) => File.SetCreationTime(file, created);

        // Sets the modified time of the given file.
        public void SetModifiedTime(string file, DateTime modified) => File.SetLastWriteTime(file, modified);

        #endregion

        #region Directories

        // Returns whether the given directory exists.
        public bool DirectoryExists(string fileDirectory) => Directory.Exists(fileDirectory);

        // Creates the given directory.
        public void CreateDirectory(string fileDirectory) => Directory.CreateDirectory(fileDirectory);

        // Returns an array of the directories in a given folder.
        public string[] GetDirectories(string folder) => Directory.GetDirectories(folder);

        // Returns an array of the files in a given folder.
        public string[] GetFiles(string folder) => Directory.GetFiles(folder);

        #endregion

        #region Stream

        // Returns a new stream for the given file.
        public Stream Open(string file) => new FileStream(file, FileMode.Open);

        // Returns a new read stream for the given file.
        public Stream OpenRead(string file) => new FileStream(file, FileMode.Open, FileAccess.Read);

        #endregion
    }
}
