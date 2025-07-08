using GoogleDriveSync.Converters;
using GoogleDriveSync.Models;
using GoogleDriveSync.Services;
using System;
using System.Collections.Generic;

namespace GoogleDriveSync.Functions
{
    public class FileFunction
    {
        // Compares the files and records the changes.
        public List<FileModel> CompareForChanges(List<FileModel> googleDrive, List<FileModel> localDrive)
        {
            LoggerService _logger = new LoggerService();

            _logger.LogMessage(StandardValues.LoggerValues.Info, "Comparing Google Drive and the Local Drive for changes");

            List<FileModel> files = new List<FileModel>();
            int differences = 0;

            foreach (FileModel localFile in localDrive)
            {
                List<ChangeModel> changes = new List<ChangeModel>();
                FileModel googleFile = googleDrive.Find(c => c.Name == localFile.Name && c.Type == localFile.Type);

                if (googleFile != null)
                {
                    DateTime lastModified = DateTime.Parse("01/01/1900");
                    
                    if (localFile.Path != googleFile.Path)
                    {
                        changes.Add(new ChangeModel
                        {
                            Field = "Path",
                            OldValue = localFile.Path,
                            NewValue = googleFile.Path,
                            Stream = "Up"
                        });

                        differences++;
                    }

                    if (localFile.LastModified.ToString("dd/MM/yyyy HH:mm:ss") != googleFile.LastModified.ToString("dd/MM/yyyy HH:mm:ss"))
                    {
                        if (googleFile.LastModified > localFile.LastModified)
                        {
                            lastModified = googleFile.LastModified;

                            changes.Add(new ChangeModel
                            {
                                Field = "Modified",
                                OldValue = localFile.LastModified.ToString(),
                                NewValue = googleFile.LastModified.ToString(),
                                Stream = "Up"
                            });
                        }

                        else
                        {
                            lastModified = localFile.LastModified;

                            changes.Add(new ChangeModel
                            {
                                Field = "Modified",
                                OldValue = googleFile.LastModified.ToString(),
                                NewValue = localFile.LastModified.ToString(),
                                Stream = "Down"
                            });
                        }

                        differences++;
                    }

                    if (lastModified == DateTime.Parse("01/01/1900"))
                    {
                        files.Add(new FileModel
                        {
                            Id = $"{googleFile.Id},{localFile.Id}",
                            Name = googleFile.Name,
                            Type = googleFile.Type,
                            PathIds = googleFile.PathIds,
                            Path = $"{googleFile.Path},{localFile.Path}",
                            Hidden = localFile.Hidden,
                            Created = localFile.Created,
                            LastModified = googleFile.LastModified,
                            Changes = changes
                        });
                    }

                    else
                    {
                        files.Add(new FileModel
                        {
                            Id = $"{googleFile.Id},{localFile.Id}",
                            Name = googleFile.Name,
                            Type = googleFile.Type,
                            PathIds = googleFile.PathIds,
                            Path = $"{googleFile.Path},{localFile.Path}",
                            Hidden = localFile.Hidden,
                            Created = localFile.Created,
                            LastModified = lastModified,
                            Changes = changes
                        });
                    }
                }

                else
                {
                    changes.Add(new ChangeModel
                    {
                        Field = "Path",
                        OldValue = "Not Uploaded",
                        NewValue = localFile.Path,
                        Stream = "Down"
                    });

                    changes.Add(new ChangeModel
                    {
                        Field = "Last Modified",
                        OldValue = "Not Uploaded",
                        NewValue = localFile.LastModified.ToString(),
                        Stream = "Down"
                    });

                    files.Add(new FileModel
                    {
                        Id = localFile.Id,
                        Name = localFile.Name,
                        Type = localFile.Type,
                        Path = localFile.Path,
                        Hidden = localFile.Hidden,
                        Created = localFile.Created,
                        LastModified = localFile.LastModified,
                        Changes = changes
                    });

                    differences += 2;
                }
            }

            _logger.LogMessage(StandardValues.LoggerValues.Debug, "Compared Local Drive");

            foreach (FileModel googleFile in googleDrive)
            {
                List<ChangeModel> changes = new List<ChangeModel>();
                FileModel file = files.Find(c => c.Name == googleFile.Name && c.Type == googleFile.Type);

                if (file == null)
                {
                    changes.Add(new ChangeModel
                    {
                        Field = "Path",
                        OldValue = "Not Downloaded",
                        NewValue = googleFile.Path,
                        Stream = "Up"
                    });

                    changes.Add(new ChangeModel
                    {
                        Field = "Last Modified",
                        OldValue = "Not Downloaded",
                        NewValue = googleFile.LastModified.ToString(),
                        Stream = "Up"
                    });

                    files.Add(new FileModel
                    {
                        Id = googleFile.Id,
                        Name = googleFile.Name,
                        Type = googleFile.Type,
                        PathIds = googleFile.PathIds,
                        Path = googleFile.Path,
                        Created = googleFile.Created,
                        LastModified = googleFile.LastModified,
                        Changes = changes
                    });

                    differences += 2;
                }
            }

            _logger.LogMessage(StandardValues.LoggerValues.Debug, "Compared Google Drive");
            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Found {differences} change(s) between Google Drive and the Local Drive");

            return files;
        }
    }
}
