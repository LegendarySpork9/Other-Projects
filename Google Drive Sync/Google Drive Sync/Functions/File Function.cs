using GoogleDriveSync.Models;

namespace GoogleDriveSync.Functions
{
    public class FileFunction
    {
        /*private List<ChangeModel> Changes = new();

        public List<ChangeModel> GetChanges() => Changes;

        public void CheckForChangesUp(FolderModel googleDrive, FolderModel localDrive)
        {
            foreach (FileModel file in localDrive.Files)
            {
                FileModel remoteFile = FindFile(googleDrive, file.Name);

                if (remoteFile == new FileModel())
                {
                    Changes.Add(new()
                    {
                        File = file.Name
                    });

                    continue;
                }

                if (file.Path != remoteFile.Path)
                {
                    file.Changes.Add(new()
                    {
                        Field = "Path",
                        Match = false
                    });
                }

                if (file.LastModified != remoteFile.LastModified)
                {
                    file.Changes.Add(new()
                    {
                        Field = "LastModified",
                        Match = false
                    });
                }
            }

            foreach (FolderModel subFolder in localDrive.Folders)
            {
                CheckForChanges(googleDrive, subFolder);
            }
        }

        private FileModel FindFile(FolderModel googleDrive, string matchName)
        {
            foreach (FileModel file in googleDrive.Files)
            {
                if (file.Name == matchName)
                {
                    return file;
                }
            }

            foreach (FolderModel subFolder in googleDrive.Folders)
            {
                FileModel returnedFile = FindFile(subFolder, matchName);

                if (returnedFile != new FileModel())
                {
                    return returnedFile;
                }
            }

            return new FileModel();
        }*/
    }
}
