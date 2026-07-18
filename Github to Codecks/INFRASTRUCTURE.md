# Github to Codecks - Infrastructure Document

## Overview

Github to Codecks is a Windows Forms desktop application that migrates GitHub issues to the Codecks project management platform. It presents a three-step wizard where the user selects a GitHub repository, an issue, and a Codecks project, then creates a card in the project's "Awaiting Triage" deck with the issue's title, body, and labels preserved as tags.

- **Author:** Hunter Industries / Toby Hunter
- **Version:** 1.0.1.0
- **Repository:** https://github.com/LegendarySpork9/Other-Projects

## Technology Stack

| Component | Technology | Version |
|---|---|---|
| Framework | .NET Framework | 4.7.2 |
| Language | C# | - |
| Application Type | Windows Forms (WinExe) | - |
| HTTP Client | RestSharp | 111.3.0 |
| JSON Serialisation | Newtonsoft.Json | 13.0.3 |
| Build System | MSBuild + NuGet | - |

## Solution Structure

```
Github to Codecks/
+-- Github To Codecks/                  # Main Windows Forms application
|   +-- Content/                        # Static assets (icons, images)
|   +-- Models/                         # Data models
|   +-- Properties/                     # Assembly info, resources, settings
|   +-- Services/                       # API service classes
+-- packages/                           # NuGet packages
```

## Application Architecture

### Application Type

The application is a **.NET Framework 4.7.2 Windows Forms** desktop application with a single-form wizard UI for migrating GitHub issues to Codecks cards.

### Services

| Service | Responsibility |
|---|---|
| `GithubService` | Fetches user-owned repositories and issues from the GitHub REST API |
| `CodeckService` | Fetches projects, decks, and existing cards from Codecks; creates new cards |

### Models

| Model | Properties | Purpose |
|---|---|---|
| `GitModel` | Token, User | GitHub authentication credentials |
| `CodeckModel` | Token, SubDomain | Codecks authentication credentials |
| `RepoModel` | Name, FormattedName | GitHub repository (FormattedName replaces dashes with spaces) |
| `IssueModel` | Number, Title, Body, Tags | GitHub issue with labels as tags |
| `ProjectModel` | Id, Name | Codecks project |
| `CardModel` | Title, Content, DeckId, Tags | Codecks card to be created |

## User Interface

### Form Controls

| Control | Type | Purpose |
|---|---|---|
| lblTitle | Label | Title: "Hunter Industries: Github to Codecks" |
| cmbRepo | ComboBox | GitHub repository selector |
| cmbIssue | ComboBox | Issue selector (filtered to exclude already-migrated issues) |
| cmbProject | ComboBox | Codecks project selector |
| ptbRepo | PictureBox | Status indicator for repository loading |
| ptbIssue | PictureBox | Status indicator for issue loading |
| ptbProject | PictureBox | Status indicator for project loading |
| btnConfirm | Button | "Confirm and Send" — triggers card creation |
| ptbConfirm | PictureBox | Status indicator for card creation |

### Wizard Flow

1. **Form loads** — Fetches user-owned GitHub repositories and populates the repository dropdown
2. **Repository selected** — Fetches open issues for the selected repository and all existing Codecks cards; filters out issues whose titles already exist as cards; populates the issue dropdown
3. **Issue selected** — Fetches Codecks projects and populates the project dropdown
4. **Project selected** — Enables the "Confirm and Send" button
5. **Confirm clicked** — Finds the "Awaiting Triage" deck in the selected project and creates a card with the issue's title, body, and labels

Each step provides visual feedback with a loading spinner, tick (success), or cross (failure) icon. All dropdowns include a "--Back--" option to return to the previous step.

### Duplicate Prevention

Before populating the issue dropdown, the application fetches all existing Codecks cards and filters out any GitHub issues whose titles already match a card title. This prevents duplicate migration.

## External Integrations

### GitHub REST API

- **Base URL:** `https://api.github.com`
- **Authentication:** Bearer token (Personal Access Token)
- **API Version:** `2022-11-28`

| Endpoint | Purpose |
|---|---|
| `GET /user/repos?affiliation=owner` | Fetch user-owned repositories |
| `GET /repos/{user}/{repo}/issues?per_page=100` | Fetch open issues for a repository |

### Codecks API

- **Base URL:** `https://api.codecks.io`
- **Authentication:** `X-Auth-Token` header + `X-Account` header

| Endpoint | Method | Purpose |
|---|---|---|
| `GET /` (with query) | GET | Fetch all cards with title, status, tags, and content |
| `GET /` (with query) | GET | Fetch all projects with names |
| `GET /` (with query) | GET | Fetch decks filtered by "Awaiting Triage" title |
| `POST /dispatch/cards/create` | POST | Create a new card in a deck |

### Card Creation

When a card is created, the following data is mapped:

| Card Field | Source |
|---|---|
| Title | GitHub issue title |
| Content | `{issue title}\n{issue body}` |
| DeckId | ID of the "Awaiting Triage" deck in the selected project |
| Tags | GitHub issue labels |
| MasterTags | GitHub issue labels (duplicated) |

## Configuration

### App.config Structure

```xml
<appSettings>
  <add key="GithubToken" value="<GitHub Personal Access Token>" />
  <add key="GithubLogin" value="<GitHub username>" />
  <add key="CodeckToken" value="<Codecks API token>" />
  <add key="CodeckSubDomain" value="<Codecks account subdomain>" />
</appSettings>
```

| Setting | Required | Purpose |
|---|---|---|
| `GithubToken` | Yes | GitHub Personal Access Token for API authentication |
| `GithubLogin` | Yes | GitHub username for repository and issue API calls |
| `CodeckToken` | Yes | Codecks API authentication token |
| `CodeckSubDomain` | Yes | Codecks account subdomain |

## CI/CD

### GitHub Actions Workflows

The Other-Projects repository has a single shared workflow:

| Workflow | Trigger | Steps |
|---|---|---|
| **Check for Linked Issue** (`PR Linked Issue.yml`) | PR opened/edited/reopened/synchronised | Verifies PR has linked GitHub issues via description, comments, or Development section |

No build or test workflows are configured for this project.

## Static Assets

| File | Purpose |
|---|---|
| `Logo.ico` | Application icon |
| `LoadingSpinner.gif` | Loading indicator animation |
| `Tick.png` | Success status indicator |
| `Cross.png` | Error status indicator |

## Hosting Requirements

### Runtime Prerequisites

- .NET Framework 4.7.2 Runtime
- Windows (required for Windows Forms UI)

### Network Requirements

- Outbound HTTPS to `api.github.com` for GitHub API requests
- Outbound HTTPS to `api.codecks.io` for Codecks API requests
