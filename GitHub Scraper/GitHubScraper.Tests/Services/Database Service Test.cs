// Copyright © - Unpublished - Toby Hunter
using GitHubScraper.Models;
using GitHubScraper.Services;
using Microsoft.Data.SqlClient;
using Moq;
using System.Configuration;

namespace GitHubScraper.Tests.Services
{
    [TestClass]
    public class DatabaseServiceTest
    {
        // Sets the AppSettingsModel values to those in the test config file.
        [TestInitialize]
        public void Setup()
        {
            AppSettingsModel.Repositories = ConfigurationManager.AppSettings["Repositories"];
            AppSettingsModel.ConnectionString = ConfigurationManager.AppSettings["SQLConnectionString"];
            AppSettingsModel.SQLFiles = ConfigurationManager.AppSettings["SQLFiles"];

            using (SqlConnection connection = new(AppSettingsModel.ConnectionString))
            {
                connection.Open();

                string sql = @"if not exists (
	select RepositoryId from Repository with (nolock)
	where [Name] = @repository
)
begin

	insert into Repository ([Name])
	values (@repository)

end";

                using (SqlCommand command = new(sql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@repository", AppSettingsModel.Repositories));

                    int rowsAffected = command.ExecuteNonQuery();
                }
            }
        }

        // Checks whether the GetLastRunDate method returns 01/01/1900 if the given repository has never run before.
        [TestMethod]
        public void TestGetLastRunDateNew()
        {
            Mock<DatabaseService> _mockDatabaseServer = new();

            DateTime expected = DateTime.Parse("01/01/1900").ToUniversalTime();
            DateTime actual = _mockDatabaseServer.Object.GetLastRunDate(AppSettingsModel.Repositories);

            Assert.AreEqual(expected, actual);
        }

        // Checks whether the GetLastRunDate method returns the last time the given repository ran.
        [TestMethod]
        public void TestGetLastRunDate()
        {
            Mock<DatabaseService> _mockDatabaseService = new();

            DateTime expected = DateTime.UtcNow;

            using (SqlConnection connection = new(AppSettingsModel.ConnectionString))
            {
                connection.Open();

                using (SqlCommand commandOne = new(File.ReadAllText($@"{AppSettingsModel.SQLFiles}\LogRun.sql"), connection))
                {
                    commandOne.Parameters.Add(new SqlParameter("@repository", AppSettingsModel.Repositories));
                    commandOne.Parameters.Add(new SqlParameter("@issues", "0"));
                    commandOne.Parameters.Add(new SqlParameter("@commits", "0"));
                    commandOne.Parameters.Add(new SqlParameter("@pullRequests", "0"));
                    commandOne.Parameters.Add(new SqlParameter("@workflowRuns", "0"));
                    commandOne.Parameters.Add(new SqlParameter("@releases", "0"));

                    int rowsAffected = commandOne.ExecuteNonQuery();

                    if (rowsAffected == 1)
                    {
                        using (SqlCommand commandTwo = new(File.ReadAllText($@"{AppSettingsModel.SQLFiles}\GetLastRunDate.sql"), connection))
                        {
                            commandTwo.Parameters.Add(new SqlParameter("@repository", AppSettingsModel.Repositories));

                            using (SqlDataReader dataReader = commandTwo.ExecuteReader())
                            {
                                while (dataReader.Read())
                                {
                                    expected = dataReader.GetDateTime(0).ToUniversalTime();
                                }
                            }
                        }
                    }
                }
            }

            DateTime actual = _mockDatabaseService.Object.GetLastRunDate(AppSettingsModel.Repositories);

            using (SqlConnection connection = new(AppSettingsModel.ConnectionString))
            {
                connection.Open();

                string sql = @"delete from RunHistory
where RunDate = @date";

                using (SqlCommand command = new(sql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@date", actual));

                    int rowsAffected = command.ExecuteNonQuery();
                }
            }

            Assert.AreEqual(expected, actual);
        }

        // Checks whether the GetIssues method returns an empty list if there are no issues for the given repository.
        [TestMethod]
        public void TestGetIssuesNone()
        {
            Mock<DatabaseService> _mockDatabaseServer = new();

            List<IssueModel> issues = _mockDatabaseServer.Object.GetIssues(AppSettingsModel.Repositories);

            Assert.IsTrue(issues.Count == 0);
        }

        // Checks whether the GetIssues method returns an list of issues if there are some issues for the given repository.
        [TestMethod]
        public void TestGetIssues()
        {
            Mock<DatabaseService> _mockDatabaseService = new();

            List<IssueModel> expected = new()
            {
                new()
                {
                    Repository = AppSettingsModel.Repositories,
                    Id = 46578346587688,
                    Number = 1,
                    Title = "Test",
                    Assignee = new()
                    {
                        Login = "UnitTester"
                    },
                    Type = "Bug",
                    State = "Open",
                    Created_At = DateTime.UtcNow,
                    Closed_At = DateTime.UtcNow,
                    Labels = new()
                    {
                        new()
                        {
                            Name = "bug"
                        }
                    }
                }
            };

            using (SqlConnection connection = new(AppSettingsModel.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new(File.ReadAllText($@"{AppSettingsModel.SQLFiles}\OutputIssue.sql"), connection))
                {
                    int result = -1;

                    command.Parameters.Add(new SqlParameter("@repository", expected[0].Repository));
                    command.Parameters.Add(new SqlParameter("@issueId", expected[0].Id));
                    command.Parameters.Add(new SqlParameter("@number", expected[0].Number));
                    command.Parameters.Add(new SqlParameter("@title", expected[0].Title));
                    command.Parameters.Add(new SqlParameter("@assignee", expected[0].Assignee?.Login ?? "Unassigned"));
                    command.Parameters.Add(new SqlParameter("@type", expected[0].Type ?? "Undefined"));
                    command.Parameters.Add(new SqlParameter("@status", expected[0].State));
                    command.Parameters.Add(new SqlParameter("@dateCreated", expected[0].Created_At));
                    command.Parameters.Add(new SqlParameter("@dateSolved", expected[0].Closed_At ?? DateTime.Parse("01/01/1900 00:00:00").ToUniversalTime()));

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            result = dataReader.GetInt32(0);
                        }
                    }
                }
            }

            List<IssueModel> actual = _mockDatabaseService.Object.GetIssues(AppSettingsModel.Repositories);

            using (SqlConnection connection = new(AppSettingsModel.ConnectionString))
            {
                connection.Open();

                string sql = @"delete from Issue
where GitHubIssueId = @id";

                using (SqlCommand command = new(sql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@id", actual[0].Id));

                    int rowsAffected = command.ExecuteNonQuery();
                }
            }

            Assert.AreEqual(expected.Count, actual.Count);
            Assert.IsTrue(actual.Count == 1);
            Assert.IsTrue(actual[0].Id == expected[0].Id);
        }
    }
}
