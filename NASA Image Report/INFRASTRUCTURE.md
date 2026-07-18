# NASA Image Report - Infrastructure Document

## Overview

NASA Image Report is a console application that aggregates NASA imagery into a Word document report. It fetches the Astronomy Picture of the Day (APOD) and all Mars Perseverance rover images from the current Martian sol date, downloads them locally, and generates a formatted `.docx` report with embedded images and metadata.

- **Author:** Hunter Industries / Toby Hunter
- **Version:** 1.0.1
- **Repository:** https://github.com/LegendarySpork9/Other-Projects

## Technology Stack

| Component | Technology | Version |
|---|---|---|
| Framework | .NET | 6.0 |
| Language | C# | Latest |
| Application Type | Console Application | - |
| HTTP Client | RestSharp | 111.4.1 |
| JSON Serialisation | Newtonsoft.Json | 13.0.3 |
| Configuration | System.Configuration.ConfigurationManager | 8.0.0 |
| Document Generation | Microsoft.Office.Interop.Word | 8.7 (COM) |

## Solution Structure

```
NASA Image Report/
+-- NASA Image Report/                  # Main console application
|   +-- Content/                        # Static assets (Logo.ico)
|   +-- Models/                         # Data models
|   +-- Services/                       # API, download, and document services
```

## Application Architecture

### Application Type

The application is a **.NET 6.0 console application** that runs as a one-shot process. It fetches NASA imagery, downloads the images, generates a Word document report, and exits.

### Services

| Service | Responsibility |
|---|---|
| `NASAAPIService` | Fetches rover images and APOD data from NASA APIs |
| `ImageDownloader` | Downloads images from URLs to a local temp directory |
| `WordDocumentService` | Generates a formatted Word document with embedded images and metadata |

### Models

| Model | Properties | Purpose |
|---|---|---|
| `APIModel` | APIKey, RoverURL, APODURL | API configuration and endpoint URLs |
| `APODModel` | Title, Description, Owner, ImageURL | Astronomy Picture of the Day data |
| `RoverImageModel` | ImageId, SolDate, Title, Site, DateReceived, DateTaken, ImageURL | Mars Perseverance rover image metadata |

## Application Pipeline

### Execution Flow

1. **Load configuration** — Read NASA API key from App.config
2. **Fetch rover images** — Paginate through the Mars rover API (100 images per page), collecting all images from the current sol date
3. **Download rover images** — Download all images in parallel (up to 10 concurrent threads) to a `temp/` directory
4. **Fetch APOD** — Retrieve the Astronomy Picture of the Day from the NASA APOD API
5. **Download APOD image** — Download the APOD image to the `temp/` directory
6. **Generate report** — Create a Word document with embedded images and metadata in the `Reports/` directory
7. **Clean up** — Delete the `temp/` directory

### Word Document Structure

The generated report follows this structure:

1. **APOD Section**
   - Introduction paragraph with the image title and photographer/owner
   - Description text (bold) explaining the image
   - Embedded APOD image

2. **Rover Images Section**
   - Introduction paragraph with the sol date
   - For each unique rover image (deduplicated by ImageId):
     - Metadata: Title, Site, Date Taken, Date Received
     - Embedded image

### Output

| Output | Location |
|---|---|
| Word Document | `{BaseDirectory}Reports\Image Report {dd-MM-yyyy}.docx` |
| Temporary Images | `{BaseDirectory}temp\` (deleted after report generation) |

## External Integrations

### Mars Rover API

- **Base URL:** `https://mars.nasa.gov/rss/api/`
- **Authentication:** None required
- **Pagination:** 100 images per page via `num` parameter

| Parameter | Value | Purpose |
|---|---|---|
| `feed` | `raw_images` | Image feed type |
| `category` | `mars2020` | Perseverance rover images |
| `feedtype` | `json` | Response format |
| `num` | `100` | Images per page |
| `order` | `sol` | Order by sol date |
| `page` | `{n}` | Page number |

The service paginates through results and filters to only include images from the most recent sol date, stopping when it encounters an earlier sol.

### APOD API

- **Base URL:** `https://api.nasa.gov/planetary/apod`
- **Authentication:** API key via `api_key` query parameter

| Response Field | Mapped To |
|---|---|
| `title` | APODModel.Title |
| `explanation` | APODModel.Description |
| `url` | APODModel.ImageURL |
| `copyright` | APODModel.Owner (defaults to "NASA" if absent) |

## Configuration

### App.config Structure

```xml
<appSettings>
  <add key="APIKey" value="<NASA API key>" />
</appSettings>
```

| Setting | Required | Purpose |
|---|---|---|
| `APIKey` | Yes | NASA API key for APOD endpoint authentication |

## CI/CD

### GitHub Actions Workflows

The Other-Projects repository has a single shared workflow:

| Workflow | Trigger | Steps |
|---|---|---|
| **Check for Linked Issue** (`PR Linked Issue.yml`) | PR opened/edited/reopened/synchronised | Verifies PR has linked GitHub issues via description, comments, or Development section |

No build or test workflows are configured for this project.

## Hosting Requirements

### Runtime Prerequisites

- .NET 6.0 Runtime
- Windows (required for Microsoft.Office.Interop.Word COM interop)
- Microsoft Word installed (required for Word document generation via COM automation)

### Network Requirements

- Outbound HTTPS to `mars.nasa.gov` for rover image API
- Outbound HTTPS to `api.nasa.gov` for APOD API
- Outbound HTTP/HTTPS to NASA image CDNs for image downloads

### File System Requirements

- Read/write access to the `temp/` directory (created at runtime for image downloads)
- Read/write access to the `Reports/` directory (created at runtime for document output)
