# AGENTS.md — KC.LMS

Guidance for AI coding agents working in this repository.

## Project Layout
- `KC.LMS.Server/` — ASP.NET Core Web API (.NET 10). Controllers in `Controllers/`.
- `kc.lms.client/` — React 19 + TypeScript SPA (Vite 8, oxlint).
- `KC.LMS.Storage/` — .NET 10 class library for data/storage abstractions.
- Solution file: `KC.LMS.slnx`.

## Commands
| Task | Command |
|---|---|
| Build solution | `dotnet build KC.LMS.slnx` |
| Run API | `dotnet run --project KC.LMS.Server` |
| Client dev server | `cd kc.lms.client; npm run dev` |
| Client lint | `cd kc.lms.client; npm run lint` |
| Client build | `cd kc.lms.client; npm run build` |

## Rules
- Follow `.github/copilot-instructions.md` and scoped rules in `.github/instructions/`.
- Data access belongs in `KC.LMS.Storage`, never directly in the Server project.
- Always validate: `dotnet build` for C#, `npm run lint` + `npm run build` for client changes.
- Keep changes minimal and focused; update `CHANGELOG.md` for notable features.
