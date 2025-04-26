using GoogleDriveSync.Models;
using GoogleDriveSync.Services;
using localDriveSync.Services;
using System.Collections.Generic;

namespace GoogleDriveSync.Functions
{
    public class FolderFunction
    {
        /*public void TraverseLocalFolders(FolderModel folder, string path)
        {
            DocumentService _documentService = new();

            folder.Folders = _documentService.GetFolders(folder.Id);
            folder.Files = _documentService.GetFiles(folder.Id, path);
            Folders += folder.Folders.Count;
            Files += folder.Files.Count;

            foreach (FolderModel subfolder in folder.Folders)
            {
                path += $@"\{subfolder.Name}";

                TraverseLocalFolders(subfolder, path);

                path = path.Replace($@"\{subfolder.Name}", "");
            }
        }*/
    }
}
