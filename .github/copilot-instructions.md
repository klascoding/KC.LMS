# KC.LMS — Copilot Instructions

## Overview
KC.LMS is a Learning Management System solution (`KC.LMS.slnx`) with:

- **KC.LMS.Server** — ASP.NET Core Web API (.NET 10), controller-based (`Controllers/`). Entry point: `Program.cs`. HTTP samples in `KC.LMS.Server.http`.
- **kc.lms.client** — React 19 + TypeScript SPA built with Vite 8 (`kc.lms.client/src`). Linted with oxlint.
- **KC.LMS.Storage** — .NET 10 class library for data/storage abstractions (currently being built out).

## Build & Run
- Build solution: `dotnet build KC.LMS.slnx`
- Run API: `dotnet run --project KC.LMS.Server`
- Client dev server: `npm run dev` (from `kc.lms.client/`)
- Client build: `npm run build`; lint: `npm run lint`

## Conventions
- C#: file-scoped namespaces, nullable enabled, async/await with `CancellationToken` on API endpoints.
- API controllers live in `KC.LMS.Server/Controllers` and use attribute routing (`[Route("[controller]")]`).
- Storage/data access code belongs in `KC.LMS.Storage`, not in the Server project.
- Client code: functional React components with hooks, TypeScript strict mode.
- Keep changelogs (`CHANGELOG.md`) updated when adding notable features.

## When Making Changes
- Prefer minimal, focused changes; follow existing patterns in each project.
- Validate C# changes with `dotnet build`; validate client changes with `npm run lint` and `npm run build`.
