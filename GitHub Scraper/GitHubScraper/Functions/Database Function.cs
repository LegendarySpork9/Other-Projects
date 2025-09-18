using GitHubScraper.Converters;
using GitHubScraper.Models;
using GitHubScraper.Services;

namespace GitHubScraper.Functions
{
    internal class DatabaseFunction
    {
        // Creates the issue aggregates.
        public List<IssueAggregateModel> CreateAggregates(string repository, List<IssueModel> issues)
        {
            LoggerService _logger = new();

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Creating aggregates for repository {repository}");

            List<IssueAggregateModel> issueAggregates = [];

            foreach (IssueModel issue in issues)
            {
                if (issue.Closed_At != null)
                {
                    DateTime date = (DateTime)issue.Closed_At;
                    DateTime closedDate = DateTime.Parse(date.ToString("dd/MM/yyyy"));

                    int index = issueAggregates.FindIndex(ia => ia.Date == closedDate);

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

                else
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
                }
            }

            _logger.LogMessage(StandardValues.LoggerValues.Info, $"Created aggregates for repository {repository}");
            return issueAggregates;
        }
    }
}
