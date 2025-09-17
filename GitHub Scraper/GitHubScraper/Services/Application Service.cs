using GitHubScraper.Converters;
using GitHubScraper.Models;
using GitHubScraper.Models.Related;

namespace GitHubScraper.Services
{
    internal class ApplicationService
    {
        LoggerService Logger = new();

        // Checks the application settings are present.
        public bool Setup()
        {
            bool configured = true;

            if (string.IsNullOrWhiteSpace(AppSettingsModel.Owner))
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, "Valid owner not found. Please provide one in the app settings with the tag \"Owner\"");

                configured = false;
            }

            else
            {
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Owner: {AppSettingsModel.Owner}");
            }

            if (string.IsNullOrWhiteSpace(AppSettingsModel.Repositories))
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, "Valid repositories not found. Please provide one in the app settings with the tag \"Repositories\"");

                configured = false;
            }

            else
            {
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Repositories: {AppSettingsModel.Repositories.Split(',').Length}");
            }

            if (string.IsNullOrWhiteSpace(AppSettingsModel.Workflows))
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, "Valid workflows not found. Please provide one in the app settings with the tag \"Workflows\"");

                configured = false;
            }

            else
            {
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Workflows: {AppSettingsModel.Workflows.Split(',').Length}");
            }

            if (string.IsNullOrWhiteSpace(AppSettingsModel.BearerToken))
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, "Valid authentication token not found. Please provide one in the app settings with the tag \"BearerToken\"");

                configured = false;
            }

            else
            {
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Bearer Token: {AppSettingsModel.BearerToken}");
            }

            if (string.IsNullOrWhiteSpace(AppSettingsModel.ConnectionString))
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, "Valid connection string not found. Please provide one in the app settings with the tag \"SQLConnectionString\"");

                configured = false;
            }

            else
            {
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Connection String: {AppSettingsModel.ConnectionString}");
            }

            if (string.IsNullOrWhiteSpace(AppSettingsModel.SQLFiles))
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, "Valid sql files not found. Please provide one in the app settings with the tag \"SQLFiles\"");

                configured = false;
            }

            else
            {
                Logger.LogMessage(StandardValues.LoggerValues.Debug, $"SQL Files: {AppSettingsModel.SQLFiles}");
            }

            return configured;
        }

        // Runs the application.
        public void Run()
        {
            DatabaseService _databaseService = new();
            GitHubService _gitHubService = new();

            foreach (string repository in AppSettingsModel.Repositories.Split(','))
            {
                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Running Scraper for {repository}");

                DateTime lastRunDate = _databaseService.GetLastRunDate(repository);

                List<IssueModel> issues = _gitHubService.GetIssues(repository, lastRunDate);
                List<CommitModel> commits = _gitHubService.GetCommits(repository, lastRunDate);
                List<PullRequestModel> pullRequests = _gitHubService.GetPullRequests(repository, lastRunDate);
                List<WorkflowModel> workflows = [];

                foreach (string workflow in AppSettingsModel.Workflows.Split(','))
                {
                    List<WorkflowRunModel>? workflowRuns = _gitHubService.GetWorkflowRuns(repository, workflow, lastRunDate);

                    if (workflowRuns != null)
                    {
                        workflows.Add(new()
                        {
                            Name = workflow,
                            WorkflowRuns = workflowRuns
                        });
                    }
                }

                List<ReleaseModel> releases = _gitHubService.GetReleases(repository, lastRunDate);

                _databaseService.OutputIssues(repository, issues);
                _databaseService.OutputCommits(repository, commits);
                _databaseService.OutputPullRequests(repository, pullRequests);
                
                foreach (WorkflowModel workflow in workflows)
                {
                    _databaseService.OutputWorkflowRuns(repository, workflow);
                }

                _databaseService.OutputReleases(repository, releases);

                Logger.LogMessage(StandardValues.LoggerValues.Info, $"Ran Scraper for {repository}");
            }
        }
    }
}
