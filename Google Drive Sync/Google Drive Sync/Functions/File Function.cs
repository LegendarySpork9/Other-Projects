using GoogleDriveSync.Converters;
using GoogleDriveSync.Models;
using GoogleDriveSync.Services;
using System;
using System.Collections.Generic;

namespace GoogleDriveSync.Functions
{
    public class FileFunction
    {
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
                    
                    if (googleFile.Path != localFile.Path)
                    {
                        changes.Add(new ChangeModel
                        {
                            Field = "Path",
                            OldValue = googleFile.Path,
                            NewValue = localFile.Path,
                            Stream = "Down"
                        });

                        differences++;
                    }

                    if (googleFile.LastModified != localFile.LastModified)
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
                        OldValue = localFile.Path,
                        NewValue = "Not Uploaded",
                        Stream = "Down"
                    });

                    changes.Add(new ChangeModel
                    {
                        Field = "Last Modified",
                        OldValue = localFile.LastModified.ToString(),
                        NewValue = "Not Uploaded",
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

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Found {differences} change(s) between Google Drive and the Local Drive");

            return files;
        }
    }
}
