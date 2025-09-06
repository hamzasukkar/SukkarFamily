# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

SukkarFamily is an ASP.NET Core 8.0 MVC web application for managing a family tree. The application uses Entity Framework Core with SQLite database for data persistence and provides functionality for managing family members (Persone entities) and family news.

## Architecture

### Core Models
- **Persone** (`Models/Persone.cs`): Represents family members with hierarchical relationships (Parent/children)
- **News** (`Models/News.cs`): Family news articles with Title, Text, and ImgUrl
- **DB** (`Models/DB.cs`): Entity Framework DbContext with `persones` and `News` DbSets

### Controllers
- **HomeController**: Main controller handling Index (shows news) and News views
- **PersoneController**: CRUD operations for family members
- **NewsController**: CRUD operations for news articles  
- **AdminController**: Administrative functions
- **api/TreeController**: API endpoints for family tree data (partially implemented)

### Database
- SQLite database located at `./data/FamilyTree.db`
- Connection string: `"Filename=./data/FamilyTree.db"`
- Uses Entity Framework Core migrations (current: AddDateToNews)

## Common Development Commands

### Build and Run
```bash
# Build the project (run from SukkarFamily subdirectory)
dotnet build

# Run the application  
dotnet run

# Run with watch (auto-restart on changes)
dotnet watch run
```

### Database Operations
```bash
# Add new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Generate migration script
dotnet ef migrations script
```

### Entity Framework Tools
- Global tool installed: `dotnet-ef` version 9.0.8
- Run EF commands from the `SukkarFamily/SukkarFamily` directory

### Project Structure
```
SukkarFamily/
├── SukkarFamily.sln          # Solution file
└── SukkarFamily/             # Main project directory
    ├── Controllers/          # MVC Controllers
    ├── Models/              # Data models and DbContext
    ├── Views/               # Razor views
    ├── Migrations/          # EF Core migrations
    ├── data/                # SQLite database files
    ├── appsettings.json     # Configuration
    └── SukkarFamily.csproj  # Project file
```

## Current Branch Status
- Working branch: `2025-codex`
- Main branch: `master`
- Recent migration pending: `AddDateToNews`
- Untracked layout file: `Views/Shared/_Layout2.cshtml`

## Dependencies
- .NET 8.0 (though .NET 9.0 SDK is available)
- Entity Framework Core 8.0.7 with SQLite and SQL Server providers
- ASP.NET Core MVC with Razor views
- Newtonsoft.Json for API serialization