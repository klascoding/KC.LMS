# AGENTS.md — KC.LMS

Guidance for AI coding agents working in this repository.

## Project Layout
- `KC.LMS.Server/` — ASP.NET Core Web API (.NET 10). Controllers in `Controllers/`.
- `kc.lms.client/` — React 19 + TypeScript SPA (Vite 8, oxlint).
- `KC.LMS.Service/` — .NET 10 class library for business logic/services.
- `KC.LMS.Storage/` — .NET 10 class library for data access (EF Core + PostgreSQL, entities, migrations).
- `KC.LMS.Storage.Test/` — MSTest unit tests (SQLite in-memory).
- Solution file: `KC.LMS.slnx`.

## Commands
| Task | Command |
|---|---|
| Build solution | `dotnet build KC.LMS.slnx` |
| Run tests | `dotnet test KC.LMS.Storage.Test` |
| Run API | `dotnet run --project KC.LMS.Server` |
| Client dev server | `cd kc.lms.client; npm run dev` |
| Client lint | `cd kc.lms.client; npm run lint` |
| Client build | `cd kc.lms.client; npm run build` |

## Rules
- Follow `.github/copilot-instructions.md` and scoped rules in `.github/instructions/`.
- Layering: Server → Service → Storage. Business logic in `KC.LMS.Service`; data access in `KC.LMS.Storage`; controllers thin.
- Always validate: `dotnet build` + `dotnet test KC.LMS.Storage.Test` for C#, `npm run lint` + `npm run build` for client changes.
- Keep changes minimal and focused; update `CHANGELOG.md` for notable features.
