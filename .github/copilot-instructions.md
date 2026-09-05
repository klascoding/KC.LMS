# KC.LMS — Copilot Instructions

## Overview
KC.LMS is a Learning Management System solution (`KC.LMS.slnx`) with:

- **KC.LMS.Server** — ASP.NET Core Web API (.NET 10), controller-based (`Controllers/`). Entry point: `Program.cs`. HTTP samples in `KC.LMS.Server.http`.
- **kc.lms.client** — React 19 + TypeScript SPA built with Vite 8 (`kc.lms.client/src`). Linted with oxlint.
- **KC.LMS.Service** — .NET 10 class library for business logic/services (e.g., `AuthService`). Namespace: `KC.LMS.Service`. DTO/model records (e.g., RegisterRequest, LoginRequest, AuthUserResult) live in the Models/ folder under the `KC.LMS.Service.Models` namespace, while service interfaces and implementations stay at the project root under `KC.LMS.Service`.
- **KC.LMS.Storage** — .NET 10 class library for data access: `ApplicationDbContext` (EF Core + PostgreSQL), entities, multi-tenant query filters. Tests in **KC.LMS.Storage.Test** (MSTest + SQLite in-memory).

## Build & Run
- Build solution: `dotnet build KC.LMS.slnx`
- Run API: `dotnet run --project KC.LMS.Server`
- Client dev server: `npm run dev` (from `kc.lms.client/`)
- Client build: `npm run build`; lint: `npm run lint`

## Conventions
- C#: file-scoped namespaces, nullable enabled, async/await with `CancellationToken` on API endpoints.
- API controllers live in `KC.LMS.Server/Controllers` and use attribute routing (`[Route("[controller]")]`).
- Layering: **Server → Service → Storage**. Business logic goes in `KC.LMS.Service`; entities, DbContext, and data access in `KC.LMS.Storage`; controllers stay thin.
- All tenant-owned entities implement `ITenantOwned` and are filtered by EF global query filters via `ITenantProvider`.
- Client code: functional React components with hooks, TypeScript strict mode.
- Keep changelogs (`CHANGELOG.md`) updated when adding notable features.

## When Making Changes
- Prefer minimal, focused changes; follow existing patterns in each project.
- Validate C# changes with `dotnet build`; run `dotnet test KC.LMS.Storage.Test` when touching Storage or Service code; validate client changes with `npm run lint` and `npm run build`.
