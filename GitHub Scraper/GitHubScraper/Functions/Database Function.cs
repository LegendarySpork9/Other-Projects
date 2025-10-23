// Copyright © - Unpublished - Toby Hunter
using GitHubScraper.Converters;
using GitHubScraper.Models;
using GitHubScraper.Services;

namespace GitHubScraper.Functions
{
    public class DatabaseFunction
    {
        // Creates the issue aggregates.
        public List<IssueAggregateModel> CreateAggregates(string repository, List<IssueModel> issues, List<IssueModel> existingIssues)
        {
            LoggerService _logger = new();

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Creating aggregates for repository {repository}");

            List<IssueAggregateModel> issueAggregates = [];

            issues = FilterIssues(repository, issues, existingIssues);

            foreach (IssueModel issue in issues)
            {
                IssueModel? existingIssue = existingIssues.Find(c => c.Id == issue.Id);

                DateTime createdDate = issue.Created_At.Date.ToUniversalTime();
                int index = -1;

                if (existingIssue == null || issue.Created_At != existingIssue.Created_At)
                {
                    index = issueAggregates.FindIndex(ia => ia.Date == createdDate);

                    if (index != -1)
                    {
                        issueAggregates[index].Created += 1;
                    }

                    else
                    {
                        issueAggregates.Add(new()
                        {
                            Date = createdDate,
                            Created = 1,
                            Solved = 0
                        });
                    }
                }

                if (issue.Closed_At != null && (existingIssue == null || issue.Closed_At != existingIssue.Closed_At))
                {
                    DateTime date = (DateTime)issue.Closed_At;
                    DateTime closedDate = date.Date.ToUniversalTime();

                    index = issueAggregates.FindIndex(ia => ia.Date == closedDate);

                    if (index != -1)
                    {
                        issueAggregates[index].Solved += 1;
                    }

                    else
                    {
                        issueAggregates.Add(new()
                        {
                            Date = closedDate,
                            Created = 0,
                            Solved = 1
                        });
                    }
                }
            }

            issueAggregates = [.. issueAggregates.OrderBy(ia => ia.Date)];

            if (issueAggregates.Count > 0)
            {
                DateTime previousdate = issueAggregates[0].Date;

                for (int index = 1; index < issueAggregates.Count; index++)
                {
                    int days = (issueAggregates[index].Date - previousdate).Days;

                    if (days != 1)
                    {
                        issueAggregates.Insert(index, new()
                        {
                            Date = previousdate.AddDays(1),
                            Created = 0,
                            Solved = 0
                        });

                        index -= 1;
                    }

                    else
                    {
                        previousdate = issueAggregates[index].Date;
                    }
                }

                while (true)
                {
                    if (previousdate != DateTime.UtcNow.Date)
                    {
                        issueAggregates.Add(new()
                        {
                            Date = previousdate.AddDays(1),
                            Created = 0,
                            Solved = 0
                        });

                        previousdate = previousdate.AddDays(1);
                    }

                    else
                    {
                        break;
                    }
                }
            }

            else
            {
                issueAggregates.Add(new()
                {
                    Date = DateTime.UtcNow.Date,
                    Created = 0,
                    Solved = 0
                });
            }

            _logger.LogMessage(StandardValues.LoggerValues.Debug, $"{issueAggregates.Count} aggregate(s) created");
            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Created aggregates for repository {repository}");
            return issueAggregates;
        }

        // Filters out existing issues from the given list of issues.
        private List<IssueModel> FilterIssues(string repository, List<IssueModel> issues, List<IssueModel> existingIssues)
        {
            LoggerService _logger = new();

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Filtering issues for repository {repository}");

            List<IssueModel> removedIssues = [];

            foreach (IssueModel issue in issues)
            {
                IssueModel? existingIssue = existingIssues.Find(c => c.Id == issue.Id);

                if (existingIssue != null)
                {
                    if (issue.Created_At == existingIssue.Created_At && issue.Closed_At == existingIssue.Closed_At)
                    {
                        removedIssues.Add(issue);
                    }
                }
            }

            foreach (IssueModel issue in removedIssues)
            {
                issues.Remove(issue);
            }

            _logger.LogMessage(StandardValues.LoggerValues.Debug, $"{removedIssues.Count} issue(s) removed");
            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Filtered issues for repository {repository}");
            return issues;
        }
    }
}
