# 📋 Job Application Tracker

A minimal full-stack web application for tracking job applications, built as a hands-on introduction to the C#/.NET stack. The project demonstrates a RESTful API backend, a vanilla JavaScript frontend, SQLite database integration, and AI-powered job description analysis via the Groq API.

---

## Features

- Add, edit, and delete job applications
- Track status per application — Applied, Interview, Offer, Rejected
- Search applications by company or role
- Filter by status and sort by date or company name
- Analyse a job description using AI to get a role summary, key skills, and interview talking points

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core Minimal API (.NET 9) |
| Database | SQLite via Entity Framework Core |
| Frontend | HTML + Vanilla JavaScript + CSS |
| AI Integration | Groq API (Llama 3.1 8B) |
| Deployment | Railway |

---

## Project Structure

```
JobTracker/
└── JobTracker.Api/
    ├── Models/
    │   ├── Application.cs        # Job application entity
    │   ├── AppDbContext.cs       # Entity Framework database context
    │   ├── AnalyseRequest.cs     # Request model for AI analysis
    │   └── GroqResponse.cs       # Response models for Groq API
    ├── wwwroot/
    │   ├── index.html            # Application markup
    │   ├── app.js                # All JavaScript logic
    │   └── styles.css            # All styling
    ├── Properties/
    │   └── launchSettings.json
    ├── Program.cs                # App configuration and API routes
    ├── JobTracker.Api.csproj     # Project dependencies
    ├── appsettings.json          # App configuration
    └── appsettings.Development.json
```

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download)
- [Visual Studio Code](https://code.visualstudio.com/) with the C# Dev Kit extension
- [Git](https://git-scm.com/)
- A [Groq API key](https://console.groq.com) (free, no credit card required)

---

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/akframe1/jobTracker.git
cd jobTracker/JobTracker.Api
```

### 2. Install dependencies

```bash
dotnet restore
```

### 3. Add your Groq API key

Store it securely using .NET User Secrets — this keeps the key off your filesystem and out of source control:

```bash
dotnet user-secrets init
dotnet user-secrets set "Groq:ApiKey" "your-groq-api-key-here"
```

### 4. Run the app

```bash
dotnet watch run
```

Open your browser and navigate to `http://localhost:5011`.

The SQLite database file (`jobtracker.db`) is created automatically on first run.

---

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/applications` | Fetch all applications |
| POST | `/applications` | Add a new application |
| PUT | `/applications/{id}` | Update an existing application |
| DELETE | `/applications/{id}` | Delete an application |
| POST | `/analyse` | Analyse a job description with AI |

---

## AI Analysis

The `/analyse` endpoint accepts a job description and returns:

1. A 2 sentence summary of the role
2. Top 5 key skills required
3. Three suggested talking points for an interview

The API call is made from the C# backend — the Groq API key is never exposed to the browser.

---

## Deployment

The application is deployed on [Railway](https://railway.app). 

### Environment Variables on Railway

Set the following variable in your Railway service under **Variables**:

| Key | Value |
|---|---|
| `Groq__ApiKey` | Your Groq API key |

> Note the double underscore `__` — this is how Railway maps environment variables to nested .NET configuration keys (`Groq:ApiKey`).

### Auto-deploy

Railway is connected to this GitHub repository. Every push to `main` triggers an automatic rebuild and redeploy. The live URL remains unchanged between deployments.

---

## Useful Commands

```bash
dotnet run              # Run the app
dotnet watch run        # Run with hot reload (recommended for development)
dotnet build            # Compile without running
dotnet restore          # Install/restore NuGet packages
dotnet --version        # Check SDK version
```

---

## Git Workflow

This project uses a feature branch workflow:

```bash
# Create and switch to a new branch
git checkout -b feature/your-feature-name

# Stage and commit changes
git add .
git commit -m "Description of changes"

# Push branch to GitHub
git push -u origin feature/your-feature-name

# Merge into main when ready
git checkout main
git merge feature/your-feature-name
git push
```

---

## Notes

- SQLite is used for local development and is sufficient for demonstration purposes. The `.db` file is excluded from source control via `.gitignore`.
- The deployed version on Railway uses the same SQLite setup. For a production application with persistent data, this would be swapped to PostgreSQL — EF Core supports this with a single line change.

---

## Author

Built by [akframe1](https://github.com/akframe1) as a learning project to demonstrate C#/.NET and JavaScript skills.