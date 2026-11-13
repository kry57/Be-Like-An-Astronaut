# Mars Base Alpha - Backend

ASP.NET Core Web API (.NET 10) for the 3-day Mars survival decision game, built from the UI screens
(Dust Storm -> Toxic Soil -> Cooling Loop Leak -> Mission Complete).

## Architecture (Clean Architecture)

```
src/
  MarsBase.Domain          Entities + game rules (GameSession aggregate), enums. No dependencies.
  MarsBase.Application     GameService (use cases), DTOs, Result pattern, repository interfaces.
  MarsBase.Infrastructure  EF Core + SQL Server, repositories, unit of work, seeding.
  MarsBase.Api             Controllers, global exception handler (ProblemDetails), CORS, OpenAPI.
```

## Run

```bash
dotnet restore
dotnet run --project src/MarsBase.Api
```

- Connection string: `src/MarsBase.Api/appsettings.json` (defaults to SQL Server LocalDB).
- On startup the schema is created (`EnsureCreated`) and the 3 events are seeded.
- OpenAPI document (Development): `GET /openapi/v1.json`. Quick tests: `src/MarsBase.Api/MarsBase.Api.http`.

### Switch to EF Core migrations (recommended for production)

1. Remove the `EnsureCreatedAsync` call in `Infrastructure/DependencyInjection.cs` and use `MigrateAsync`.
2. Create the first migration:
   ```bash
   dotnet tool install --global dotnet-ef
   dotnet ef migrations add Initial -p src/MarsBase.Infrastructure -s src/MarsBase.Api -o Persistence/Migrations
   dotnet ef database update -p src/MarsBase.Infrastructure -s src/MarsBase.Api
   ```

## Endpoints

| Method | Route | Purpose (UI screen) |
|---|---|---|
| POST | `/api/sessions` | Start mission, Day 01/03. Body (optional): `{ "playerName": "..." }` |
| GET | `/api/sessions/{id}` | Dashboard: resources, crew 4/4, vitals, build points, active emergency + countdown, or final result |
| POST | `/api/sessions/{id}/choices` | "Choose" button. Body: `{ "optionId": 1 }` |
| POST | `/api/sessions/{id}/restart` | "Play Again" (new session, same player) |
| GET | `/api/sessions/{id}/history` | Decisions taken so far |
| GET | `/health` | Health check |

Errors are returned as RFC 7807 ProblemDetails: 400 (option not in current event), 404 (session not found),
409 (mission finished, not enough Build Points, concurrent update).

## Game rules

- Start values (from the screens): Oxygen 94, Water 88, Solar Grid 91, Hydroponics 76, Radiation 98, Crew Vitals 92. Build Points: 5.
- Every option applies deltas to resources (clamped 0-100) and may cost Build Points.
- Dust Storm has a 108 s countdown ("IMPACT IN 01:48"). Answering late applies `TimeoutPenalty` to Water, Solar Grid and Hydroponics.
- After the choice the day advances. Oxygen or Crew Vitals at 0 -> `Failed`. After Day 3 -> `Completed` with score (average of resources) and the 4 lessons.
- Optimistic concurrency via `RowVersion` on the session.

## Assumptions to review

The screens do not define numeric effects, so the deltas, the 5 starting Build Points and the costs
(Grow Without Soil = 3, Install New Pipes = 4) are my balance assumptions. Edit them in
`Infrastructure/Persistence/DbSeeder.cs`. The screens mention Build Points only on "Install New Pipes",
so the starting amount and the cost of "Grow Without Soil" are my guesses.

## Not included yet

Authentication, unit tests, Docker. Easy next steps: xUnit tests for `GameSession` (pure domain logic), JWT auth, leaderboard.
