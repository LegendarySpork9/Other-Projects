// Copyright © - Unpublished - Toby Hunter
using GitHubScraper.Functions;
using GitHubScraper.Models;
using Moq;

namespace GitHubScraper.Tests.Functions
{
    [TestClass]
    public class DatabaseFunctionTest
    {
        // Checks whether the CreateAggregates method returns one record for an empty list.
        [TestMethod]
        public void TestCreateAggregatesEmpty()
        {
            Mock<DatabaseFunction> _mockDatabaseFunction = new();

            List<IssueAggregateModel> issueAggregates = _mockDatabaseFunction.Object.CreateAggregates("Unit-Test", [], []);

            Assert.AreEqual(1, issueAggregates.Count);
            Assert.IsTrue(issueAggregates[0].Date == DateTime.UtcNow.Date);
            Assert.IsTrue(issueAggregates[0].Created == 0);
            Assert.IsTrue(issueAggregates[0].Solved == 0);
        }

        // Checks whether the CreateAggregates method returns one record for an issue created today.
        [TestMethod]
        public void TestCreateAggregatesCreated()
        {
            Mock<DatabaseFunction> _mockDatabaseFunction = new();

            List<IssueModel> mockIssue = new()
            {
                new()
                {
                    Repository = "Unit-Test",
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
                    Labels = new()
                    {
                        new()
                        {
                            Name = "bug"
                        }
                    }
                }
            };

            List<IssueAggregateModel> issueAggregates = _mockDatabaseFunction.Object.CreateAggregates("Unit-Test", mockIssue, []);

            Assert.AreEqual(1, issueAggregates.Count);
            Assert.IsTrue(issueAggregates[0].Date == DateTime.UtcNow.Date);
            Assert.IsTrue(issueAggregates[0].Created == 1);
            Assert.IsTrue(issueAggregates[0].Solved == 0);
        }

        // Checks whether the CreateAggregates method returns two records for an issue created yesterday and solved today.
        [TestMethod]
        public void TestCreateAggregatesCreatedSolved()
        {
            Mock<DatabaseFunction> _mockDatabaseFunction = new();

            List<IssueAggregateModel> expected =
            [
                new()
                {
                    Date = DateTime.UtcNow.AddDays(-1).Date,
                    Created = 1,
                    Solved = 0
                },
                new()
                {
                    Date = DateTime.UtcNow.Date,
                    Created = 0,
                    Solved = 1
                }
            ];
            List<IssueModel> mockIssue =
            [
                new()
                {
                    Repository = "Unit-Test",
                    Id = 46578346587688,
                    Number = 1,
                    Title = "Test",
                    Assignee = new()
                    {
                        Login = "UnitTester"
                    },
                    Type = "Bug",
                    State = "Closed",
                    Created_At = DateTime.UtcNow.AddDays(-1),
                    Closed_At = DateTime.UtcNow,
                    Labels = new()
                    {
                        new()
                        {
                            Name = "bug"
                        }
                    }
                }
            ];

            List<IssueAggregateModel> actual = _mockDatabaseFunction.Object.CreateAggregates("Unit-Test", mockIssue, []);

            Assert.AreEqual(2, actual.Count);

            if (actual.Count == expected.Count)
            {
                for (int index = 0; index < actual.Count; index++)
                {
                    Assert.AreEqual(expected[index].Date, actual[index].Date);
                    Assert.AreEqual(expected[index].Created, actual[index].Created);
                    Assert.AreEqual(expected[index].Solved, actual[index].Solved);
                }
            }
        }

        // Checks whether the CreateAggregates method fills the gap between the issue dates and now.
        [TestMethod]
        public void TestCreateAggregatesFill()
        {
            Mock<DatabaseFunction> _mockDatabaseFunction = new();

            List<IssueModel> mockIssue = new()
            {
                new()
                {
                    Repository = "Unit-Test",
                    Id = 46578346587688,
                    Number = 1,
                    Title = "Test",
                    Assignee = new()
                    {
                        Login = "UnitTester"
                    },
                    Type = "Bug",
                    State = "Open",
                    Created_At = DateTime.UtcNow.AddDays(-15),
                    Closed_At = DateTime.UtcNow.AddDays(-5),
                    Labels = new()
                    {
                        new()
                        {
                            Name = "bug"
                        }
                    }
                }
            };

            List<IssueAggregateModel> issueAggregates = _mockDatabaseFunction.Object.CreateAggregates("Unit-Test", mockIssue, []);

            Assert.AreEqual(16, issueAggregates.Count);

            foreach (IssueAggregateModel issueAggregate in issueAggregates)
            {
                if (issueAggregate.Date == DateTime.UtcNow.AddDays(-15).Date)
                {
                    Assert.IsTrue(issueAggregate.Created == 1);
                    Assert.IsTrue(issueAggregate.Solved == 0);
                }

                else if (issueAggregate.Date == DateTime.UtcNow.AddDays(-5).Date)
                {
                    Assert.IsTrue(issueAggregate.Created == 0);
                    Assert.IsTrue(issueAggregate.Solved == 1);
                }

                else
                {
                    Assert.IsTrue(issueAggregate.Created == 0);
                    Assert.IsTrue(issueAggregate.Solved == 0);
                }
            }
        }

        // Checks whether the CreateAggregates method returns one record for an existing issue.
        [TestMethod]
        public void TestCreateAggregatesExcludeIssue()
        {
            Mock<DatabaseFunction> _mockDatabaseFunction = new();

            List<IssueModel> mockIssue = new()
            {
                new()
                {
                    Repository = "Unit-Test",
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

            List<IssueModel> mockExistingIssue = new()
            {
                new()
                {
                    Id = 46578346587688,
                    Number = 0,
                    Title = "UnLoaded",
                    State = "UnLoaded",
                    Created_At = mockIssue[0].Created_At,
                    Closed_At = mockIssue[0].Closed_At,
                    Labels = []
                }
            };

            List<IssueAggregateModel> issueAggregates = _mockDatabaseFunction.Object.CreateAggregates("Unit-Test", mockIssue, mockExistingIssue);

            Assert.AreEqual(1, issueAggregates.Count);
            Assert.IsTrue(issueAggregates[0].Date == DateTime.UtcNow.Date);
            Assert.IsTrue(issueAggregates[0].Created == 0);
            Assert.IsTrue(issueAggregates[0].Solved == 0);
        }

        // Checks whether the CreateAggregates method returns one record for an existing issue with no closed date.
        [TestMethod]
        public void TestCreateAggregatesExcludeIssueClosed()
        {
            Mock<DatabaseFunction> _mockDatabaseFunction = new();

            List<IssueModel> mockIssue = new()
            {
                new()
                {
                    Repository = "Unit-Test",
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

            List<IssueModel> mockExistingIssue = new()
            {
                new()
                {
                    Id = 46578346587688,
                    Number = 0,
                    Title = "UnLoaded",
                    State = "UnLoaded",
                    Created_At = mockIssue[0].Created_At,
                    Labels = []
                }
            };

            List<IssueAggregateModel> issueAggregates = _mockDatabaseFunction.Object.CreateAggregates("Unit-Test", mockIssue, mockExistingIssue);

            Assert.AreEqual(1, issueAggregates.Count);
            Assert.IsTrue(issueAggregates[0].Date == DateTime.UtcNow.Date);
            Assert.IsTrue(issueAggregates[0].Created == 0);
            Assert.IsTrue(issueAggregates[0].Solved == 1);
        }

        // Checks whether the CreateAggregates method returns one record for an existing issue with no created date.
        [TestMethod]
        public void TestCreateAggregatesExcludeIssueCreated()
        {
            Mock<DatabaseFunction> _mockDatabaseFunction = new();

            List<IssueModel> mockIssue = new()
            {
                new()
                {
                    Repository = "Unit-Test",
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

            List<IssueModel> mockExistingIssue = new()
            {
                new()
                {
                    Id = 46578346587688,
                    Number = 0,
                    Title = "UnLoaded",
                    State = "UnLoaded",
                    Created_At = DateTime.Parse("01/01/1900 00:00:00").ToUniversalTime(),
                    Closed_At = mockIssue[0].Closed_At,
                    Labels = []
                }
            };

            List<IssueAggregateModel> issueAggregates = _mockDatabaseFunction.Object.CreateAggregates("Unit-Test", mockIssue, mockExistingIssue);

            Assert.AreEqual(1, issueAggregates.Count);
            Assert.IsTrue(issueAggregates[0].Date == DateTime.UtcNow.Date);
            Assert.IsTrue(issueAggregates[0].Created == 1);
            Assert.IsTrue(issueAggregates[0].Solved == 0);
        }
    }
}
