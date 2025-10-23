// Copyright © - 14/05/2025 - Toby Hunter
using Github_To_Codecks.Models;
using Github_To_Codecks.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Github_To_Codecks
{
    public partial class Form : System.Windows.Forms.Form
    {
        readonly GitModel git = new GitModel();
        readonly CodeckModel codeck = new CodeckModel();
        readonly GithubService _github;
        readonly CodeckService _codeck;

        List<RepoModel> repos = new List<RepoModel>();
        List<IssueModel> issues = new List<IssueModel>();
        List<ProjectModel> projects = new List<ProjectModel>();

        public Form()
        {
            InitializeComponent();

            git.Token = ConfigurationManager.AppSettings["GithubToken"];
            git.User = ConfigurationManager.AppSettings["GithubLogin"];
            codeck.Token = ConfigurationManager.AppSettings["CodeckToken"];
            codeck.SubDomain = ConfigurationManager.AppSettings["CodeckSubDomain"];

            _github = new GithubService(git);
            _codeck = new CodeckService(codeck);
        }

        // Loads the repositories in GitHub.
        private async void LoadRepos(object sender, EventArgs e)
        {
            ptbRepo.Image = Properties.Resources.LoadingSpinner;

            repos = await _github.GetRepos();

            if (repos.Count == 0)
            {
                ptbRepo.Image = Properties.Resources.Cross;
            }

            else
            {
                foreach (RepoModel repo in repos)
                {
                    cmbRepo.Items.Add(repo.FormattedName);
                }

                cmbRepo.Enabled = true;
                ptbRepo.Image = Properties.Resources.Tick;
            }
        }

        // Loads the issues and allows the user to select one.
        private async void RepoSelected(object sender, EventArgs e)
        {
            cmbRepo.Enabled = false;
            cmbIssue.Items.Clear();
            cmbIssue.Text = "Choose an Issue";
            ptbConfirm.Image = null;
            ptbIssue.Image = Properties.Resources.LoadingSpinner;

            issues = await _github.GetIssues(repos[cmbRepo.SelectedIndex].Name);
            List<CardModel> cards = await _codeck.GetCards();

            if (issues.Count == 0 || cards.Count == 0)
            {
                cmbIssue.Items.Add("--Back--");
                cmbIssue.Enabled = true;
                ptbIssue.Image = Properties.Resources.Cross;
            }

            else
            {
                foreach (IssueModel issue in issues)
                {
                    if (!cards.Any(card => card.Title == issue.Title))
                    {
                        cmbIssue.Items.Add($"{issue.Number} - {issue.Title}");
                    }
                }

                cmbIssue.Items.Add("--Back--");
                cmbIssue.Enabled = true;
                ptbIssue.Image = Properties.Resources.Tick;
            }
        }

        // Loadsthe Codecks projects and allows the user to select one.
        private async void IssueSelected(object sender, EventArgs e)
        {
            cmbIssue.Enabled = false;
            cmbProject.Items.Clear();
            cmbProject.Text = "Choose a Codecks Project";

            if (cmbIssue.SelectedItem.ToString() == "--Back--")
            {
                cmbRepo.Enabled = true;
            }

            else
            {
                ptbProject.Image = Properties.Resources.LoadingSpinner;

                projects = await _codeck.GetProjects();

                if (projects.Count == 0)
                {
                    cmbProject.Items.Add("--Back--");
                    cmbProject.Enabled = true;
                    ptbProject.Image = Properties.Resources.Cross;
                }

                else
                {
                    foreach (ProjectModel project in projects)
                    {
                        cmbProject.Items.Add(project.Name);
                    }

                    cmbProject.Items.Add("--Back--");
                    cmbProject.Enabled = true;
                    ptbProject.Image = Properties.Resources.Tick;
                }
            }
        }

        // Allows the user to migrate the GitHub issue.
        private void ProjectSelected(object sender, EventArgs e)
        {
            if (cmbProject.SelectedItem.ToString() == "--Back--")
            {
                btnConfirm.Enabled = false;
                cmbProject.Enabled = false;
                cmbIssue.Enabled = true;
            }

            else
            {
                btnConfirm.Enabled = true;
            }
        }

        // Sends the GitHub issue to Codecks.
        private async void SendClick(object sender, EventArgs e)
        {
            cmbProject.Enabled = false;
            btnConfirm.Enabled = false;
            ptbConfirm.Image = Properties.Resources.LoadingSpinner;

            string deckId = await _codeck.GetDeck(projects[cmbProject.SelectedIndex].Id);
            IssueModel issue = issues[cmbIssue.SelectedIndex];

            CardModel card = new CardModel
            {
                Title = issue.Title,
                Content = $"{issue.Title}\n{issue.Body}",
                DeckId = deckId,
                Tags = issue.Tags
            };

            System.Net.HttpStatusCode code = await _codeck.CreateCard(card);

            if (code != System.Net.HttpStatusCode.OK)
            {
                ptbConfirm.Image = Properties.Resources.Cross;
            }

            else
            {
                ptbConfirm.Image = Properties.Resources.Tick;
                cmbRepo.Enabled = true;
            }
        }
    }
}
