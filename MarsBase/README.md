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


## Little Galaxy Explorer frontend API (new)

The supplied `THE_LITTLE_GALAXY_EXPLORER` HTML is a six-level experience (not the original
three-day prototype). The backend now exposes a matching persistent session API under
`/api/explorer/sessions`. The original `/api/sessions` endpoints are retained for compatibility.

### Explorer endpoints

| Method | Route | Purpose |
|---|---|---|
| POST | `/api/explorer/sessions` | Create a session with hero, play mode, difficulty and players |
| GET | `/api/explorer/sessions/{id}` | Restore profile, active player, level, XP, stars, telemetry and decision history |
| POST | `/api/explorer/sessions/{id}/decisions` | Record a level decision (`level` 1–6, `option` 1–3), apply the same telemetry/XP/stars data used by the HTML decision table |
| PATCH | `/api/explorer/sessions/{id}/screen` | Persist current screen and level; set `advanceTurn: true` when the frontend advances the player |
| POST | `/api/explorer/sessions/{id}/restart` | Start a fresh run using the previous profile |

### Start request example

```json
{
  "playerName": "Omar",
  "hero": "both",
  "playMode": "duo",
  "difficulty": "medium",
  "players": [
    { "name": "Omar", "age": 10, "role": "Mission Leader" },
    { "name": "Sara", "age": 9, "role": "Science Officer" }
  ]
}
```

The response contains `id`, `currentScreenId`, `currentLevel`, `activePlayer`,
`players`, `totalXp`, `totalStars`, `levelStars`, `telemetry` and `history`.

### Record a choice

```http
POST /api/explorer/sessions/{id}/decisions
Content-Type: application/json

{ "level": 1, "option": 2 }
```

The backend uses the six-level decision values from the supplied HTML's
`C_ENGINE_DECISIONS` table and applies the difficulty XP multiplier (easy 1.0,
medium 1.25, hard 1.5). This avoids maintaining a second, conflicting set of
game-balance values.

### Important frontend integration note

The supplied HTML currently runs its gameplay in JavaScript memory and contains no `fetch`,
`XMLHttpRequest`, or API calls. These endpoints are ready for it, but the HTML must call them
from its onboarding, decision and screen-navigation handlers before sessions will be saved
to SQL Server. The API does not attempt to persist canvas animation frames, local visual
effects, sound settings, NASA reference links, the base-builder layout or the separate
platformer mini-game; those are client-side features in the supplied file.

Database initialization still uses `EnsureCreated` for this starter project. If you already
created `MarsBaseDb` from the previous backend, `EnsureCreated` will not add the new table to
an existing database. For a clean test, use a fresh database name in the connection string,
or add and apply an EF Core migration before using the new Explorer endpoints.
