# LBQuiz

A real-time multiplayer quiz platform built with **Blazor Server** (.NET 10) and **MudBlazor**.

## Features

- **Create & host quizzes** – Build quizzes with multiple question types:
  - Multiple choice
  - Open-ended
  - Slider
  - Word cloud
  - Review (with average answer)
- **Lobby system** – Create or join lobbies via link or QR code
- **Real-time gameplay** – Live question progression and answer handling powered by SignalR
- **Scoring** – Automatic question scoring after each round
- **In-game chat** – Players can chat during a quiz session
- **User accounts** – Registration, login, and profile pictures via ASP.NET Core Identity

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core / Blazor Server (.NET 10) |
| UI | MudBlazor 8 |
| Real-time | ASP.NET Core SignalR |
| Database | SQL Server (Entity Framework Core) |
| Auth | ASP.NET Core Identity |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server or SQL Server Express

## Getting Started

1. **Clone the repository**
   ```bash
   git clone <repo-url>
   cd LBQuiz
   ```

2. **Configure the database connection**  
   Update the `DefaultConnection` string in `LBQuiz/appsettings.json` (or use User Secrets) to point at your SQL Server instance:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=.\\SQLExpress;Database=LBQuiz;Trusted_Connection=True;TrustServerCertificate=True"
   }
   ```

3. **Apply migrations**
   ```bash
   dotnet ef database update --project LBQuiz
   ```

4. **Run the app**
   ```bash
   dotnet run --project LBQuiz
   ```
   The app will be available at `https://localhost:5001` (or the port shown in the terminal).

## Project Structure

```
LBQuiz/
├── LBQuiz/                  # Main application
│   ├── Components/          # Blazor components & pages
│   │   ├── Pages/           # Top-level pages (Home, Lobby, Quiz, Chat, Auth…)
│   │   └── Quizes/          # Quiz-specific components
│   ├── Hubs/                # SignalR hubs (LobbyHub, ChatHub)
│   ├── Models/              # Domain models (Quiz, Question, Lobby…)
│   ├── Services/            # Business logic & hub connections
│   ├── Data/                # EF Core DbContext & migrations
│   └── wwwroot/             # Static assets
└── LBQuiz.Test/             # Unit tests
```

## Running Tests

```bash
dotnet test LBQuiz.Test
```
