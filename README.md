# 📋 Job Application Tracker

A full-stack web application for tracking job applications and financial options pricing, built as a demonstration of C#/.NET development. The project features a layered ASP.NET Core backend with a repository and service pattern, AI-powered job description analysis via the Groq API, and an American options pricing calculator using the Cox-Ross-Rubinstein Binomial Tree model.

---

## Features

**Applications**
- Add, edit, and delete job applications
- Track status per application — Applied, Interview, Offer, Rejected
- Status transition validation — enforces logical progression between states
- Search by company or role, filter by status, sort by date or company name
- AI-powered job description analysis returning a role summary, key skills, and interview talking points

**Options Calculator**
- American-style options pricing via the Cox-Ross-Rubinstein Binomial Tree model
- Early exercise premium calculation — direct comparison between American and European pricing
- Full tree parameter output — up/down factors, risk neutral probability, time step size
- Delta approximation from the first step of the tree

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core Minimal API (.NET 9) |
| Architecture | Repository pattern, Service layer, Dependency Injection |
| Validation | FluentValidation |
| Database | SQLite via Entity Framework Core |
| Frontend | HTML + Vanilla JavaScript + CSS |
| AI Integration | Groq API (Llama 3.1 8B) |
| Deployment | Railway |

---

## Project Structure

```
JobTracker/
└── JobTracker.Api/
    ├── Exceptions/
    │   ├── ApplicationNotFoundException.cs
    │   ├── InvalidStatusTransitionException.cs
    │   └── ValidationException.cs
    ├── Interfaces/
    │   ├── IApplicationRepository.cs
    │   ├── IApplicationService.cs
    │   ├── IAnalysisService.cs
    │   └── IBinomialCalculatorService.cs
    ├── Middleware/
    │   └── ExceptionHandlingMiddleware.cs
    ├── Models/
    │   ├── Application.cs
    │   ├── AppDbContext.cs
    │   ├── AnalyseRequest.cs
    │   ├── GroqResponse.cs
    │   ├── BinomialRequest.cs
    │   └── BinomialResult.cs
    ├── Repositories/
    │   └── ApplicationRepository.cs
    ├── Services/
    │   ├── ApplicationService.cs
    │   ├── AnalysisService.cs
    │   └── BinomialCalculatorService.cs
    ├── Validators/
    │   ├── ApplicationValidator.cs
    │   └── BinomialRequestValidator.cs
    ├── wwwroot/
    │   ├── index.html            # Entry point, redirects to applications
    │   ├── pages/
    │   │   ├── applications.html # Job tracker page
    │   │   └── options.html      # Options calculator page
    │   ├── js/
    │   │   ├── applications.js   # Applications page logic
    │   │   └── options.js        # Options calculator logic
    │   └── css/
    │       └── styles.css        # Shared styling
    ├── Properties/
    │   └── launchSettings.json
    ├── Program.cs                # App configuration and API routes
    ├── JobTracker.Api.csproj     # Project dependencies
    ├── appsettings.json
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

**Applications**

| Method | Endpoint | Description |
|---|---|---|
| GET | `/applications` | Fetch all applications |
| POST | `/applications` | Add a new application |
| PUT | `/applications/{id}` | Update an existing application |
| DELETE | `/applications/{id}` | Delete an application |
| POST | `/analyse` | Analyse a job description with AI |

**Options**

| Method | Endpoint | Description |
|---|---|---|
| POST | `/options/binomial` | Price American options via CRR Binomial Tree |

---

## Architecture

The backend follows a layered architecture separating concerns across interfaces, repositories, and services:

- **Interfaces** define contracts for all services and repositories following the C# `I` prefix convention
- **Repositories** are the only layer with direct database access via Entity Framework Core
- **Services** contain all business logic including status transition rules and input validation
- **Middleware** provides centralised exception handling, mapping typed exceptions to consistent HTTP responses
- **FluentValidation** validates all incoming requests before they reach the service layer

---

## Options Pricing — Binomial Tree (CRR)

The Cox-Ross-Rubinstein model prices American-style options, which are the dominant contract type on US exchanges such as the CBOE and CME. Unlike the Black-Scholes model which is limited to European options, the binomial tree accounts for early exercise at every node by comparing the discounted continuation value against the intrinsic value of immediate exercise — taking whichever is greater.

The model outputs both American and European prices for direct comparison, with the difference representing the early exercise premium.

---

## AI Analysis

The `/analyse` endpoint accepts a job description and returns:

1. A 2 sentence summary of the role
2. Top 5 key skills required
3. Three suggested talking points for an interview

The API call is made server-side from the C# backend — the Groq API key is never exposed to the browser.

---

## Deployment

The application is deployed on [Railway](https://jobtracker-production-11d2.up.railway.app/).

The deployed version will not persist or have saved elements due to privacy concerns — it is intended to be used as a live demonstration of the application and its features, including the AI analysis tool and options calculator. For personal use and to keep application data private, the project is best run locally following the Getting Started steps above.

Railway is connected to this GitHub repository. Every push to `main` triggers an automatic rebuild and redeploy, which also resets any data entered into the live version.

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
- For a production deployment with persistent data, SQLite would be replaced with PostgreSQL — EF Core supports this with a single configuration change.

---

## Author

Built by [akframe1](https://github.com/akframe1) as a learning project demonstrating C#/.NET, financial modelling, and JavaScript skills.