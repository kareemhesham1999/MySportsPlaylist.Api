# MySportsPlaylist.Api

MySportsPlaylist.Api is a robust ASP.NET Core Web API that allows users to explore, manage, and curate playlists of sports matches. It features secure authentication, real-time notifications via SignalR, playlist management, and administrative endpoints for managing sports matches. This API is designed to serve as the backend for a sports streaming/curation application.

[Frontend URL](https://github.com/kareemhesham1999/MySportsPlaylist.Frontend)

---

## Table of Contents

- [Features](#features)
- [Architecture Overview](#architecture-overview)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Configuration](#configuration)
  - [Database](#database)
  - [Running the Application](#running-the-application)
- [API Endpoints](#api-endpoints)
  - [Authentication](#authentication)
  - [Matches](#matches)
  - [Playlists](#playlists)
  - [Notifications (SignalR)](#notifications-signalr)
- [Background Services](#background-services)
- [Project Structure](#project-structure)
- [Security](#security)
- [Development and Contribution](#development-and-contribution)
- [License](#license)

---

## Features

- **User Registration and Login** with JWT authentication.
- **Role-based Authorization** (Admin and User roles).
- **Browse, Search, and Manage Sports Matches** (Live and Replay).
- **Personal Playlists**: Users can add or remove matches from their playlists.
- **Real-Time Notifications**: Users receive live updates via SignalR when matches are added/removed from playlists or when match statuses change.
- **Admin Endpoints** for match CRUD operations.
- **Extensible and Modular Architecture** using repositories and services.
- **Background Services** for demo and real-time match status updates.
- **Swagger UI** for easy API exploration and testing.

---

## Architecture Overview

- **ASP.NET Core 6 Web API**.
- **Entity Framework Core** for data access (SQL Server).
- **JWT (JSON Web Tokens)** for authentication and authorization.
- **SignalR** for real-time notifications.
- **Background Services** for periodic and demo updates.
- **Separation of Concerns**: Controllers, Services, Repositories, and Models.

---

## Getting Started

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/) (local or Docker)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- (Optional) [Angular Frontend](https://angular.io/) at `http://localhost:4200` (for CORS)

### Configuration

1. **Clone the repository:**

   ```bash
   git clone https://github.com/kareemhesham1999/MySportsPlaylist.Api.git
   cd MySportsPlaylist.Api
   ```

2. **Set up `appsettings.json`:**

   - The default configuration uses:

     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost,1433;Database=MySportsPlaylistDb;User Id=sa;Password=YourStr0ngP@ssw0rd;TrustServerCertificate=True;Encrypt=False"
     },
     "Jwt": {
       "Key": "MySecureJwtSecretKeyForMySportsPlaylistApp2025",
       "Issuer": "MySportsPlaylist.Api",
       "Audience": "MySportsPlaylistClient"
     }
     ```

   - Update the connection string and JWT settings as needed.

3. **Database**

   - On first run, the database and seed data (admin/user accounts, matches) are created automatically.

   - Default users:
     - **Admin:** `admin / Admin123!`
     - **User:** `user1 / User123!`

### Running the Application

#### With Visual Studio

- Open `MySportsPlaylist.sln`
- Set `MySportsPlaylist.Api` as the startup project.
- Press `F5` or click **Run**.

#### With CLI

```bash
dotnet build
dotnet run --project MySportsPlaylist.Api
```

#### Swagger

- Navigate to [https://localhost:7161/swagger](https://localhost:7161/swagger) (configured in `launchSettings.json`).

---

## API Endpoints

### Authentication

- **POST /api/Auth/register**
  - Register a new user.
- **POST /api/Auth/login**
  - User login (returns JWT token).

### Matches

- **GET /api/Matches**
  - Get all matches.
- **GET /api/Matches/{id}**
  - Get match by ID.
- **GET /api/Matches/live**
  - List all live matches.
- **GET /api/Matches/replay**
  - List all replay matches.
- **GET /api/Matches/search?query=...**
  - Search matches by title or competition.

**Admin-only:**
- **POST /api/Matches**
  - Add a new match.
- **PUT /api/Matches/{id}**
  - Update an existing match.
- **DELETE /api/Matches/{id}**
  - Delete a match.

### Playlists (Requires Authentication)

- **GET /api/Playlists**
  - Get the current user's playlist.
- **POST /api/Playlists/{matchId}**
  - Add a match to the user's playlist.
- **DELETE /api/Playlists/{matchId}**
  - Remove a match from the user's playlist.
- **GET /api/Playlists/contains/{matchId}**
  - Check if a match is in the user's playlist.

### Notifications (SignalR)

- **SignalR Hub:** `/hubs/notifications`
- Authenticate with JWT as a query string: `?access_token=YOUR_TOKEN`
- Client listens on the `ReceiveNotification` event for real-time updates.

---

## Background Services

- **LiveMatchStatusService**: Monitors (but doesn't update) live matches in production.
- **MatchDemoService** (Development only): Randomly toggles match statuses for demonstration, sending notifications to all users.

---

## Project Structure

```
MySportsPlaylist.Api/
├── Controllers/        # API Controllers
├── Data/               # EF Core DbContext and Seeder
├── Hubs/               # SignalR Hubs
├── Models/             # Entity and DTO Models
├── Repositories/       # Data access abstractions and implementations
├── Services/           # Business logic and background services
├── Properties/         # launchSettings.json
├── appsettings*.json   # Configuration
├── Program.cs          # Entry point and DI setup
├── MySportsPlaylist.Api.csproj
└── ...
```

---

## Security

- **JWT Authentication**: Protects endpoints; tokens required for all playlist and admin actions.
- **Role-based Authorization**: Only Admins can create/update/delete matches.
- **CORS**: Only allows requests from `http://localhost:4200` (Angular frontend).
- **Password Storage**: Passwords are hashed using SHA256.

---

## Development and Contribution

1. Fork this repository and clone your fork.
2. Install dependencies and set up your SQL Server.
3. Make your changes and test thoroughly.
4. Submit a pull request with a clear description of your changes.

---

## License

This project is licensed under the MIT License.

---

## Contact

For questions, suggestions, or issues, please open an issue in the repository.
