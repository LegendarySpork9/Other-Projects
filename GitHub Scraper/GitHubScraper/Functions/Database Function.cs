using GitHubScraper.Converters;
using GitHubScraper.Models;
using GitHubScraper.Services;

namespace GitHubScraper.Functions
{
    public class DatabaseFunction
    {
        // Creates the issue aggregates.
        public List<IssueAggregateModel> CreateAggregates(string repository, List<IssueModel> issues)
        {
            LoggerService _logger = new();

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Creating aggregates for repository {repository}");

            List<IssueAggregateModel> issueAggregates = [];

            foreach (IssueModel issue in issues)
            {
                DateTime createdDate = DateTime.Parse(issue.Created_At.ToString("dd/MM/yyyy"));

                int index = issueAggregates.FindIndex(ia => ia.Date == createdDate);

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

                if (issue.Closed_At != null)
                {
                    DateTime date = (DateTime)issue.Closed_At;
                    DateTime closedDate = DateTime.Parse(date.ToString("dd/MM/yyyy"));

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
    }
}
