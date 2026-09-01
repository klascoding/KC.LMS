This file explains how Visual Studio created the project.

The following steps were used to generate this project:
- Create new ASP\.NET Core Web API project.
- Update project file to add a reference to the frontend project and set SPA properties.
- Update `launchSettings.json` to register the SPA proxy as a startup assembly.
- Add project to the startup projects list.
- Write this file.

## Changes

- Added multi-tenant storage model in `KC.LMS.Storage` (Tenant, Organization, User, UserAccess, Employee, Department, Position) with EF Core (PostgreSQL) and TenantId global query filters.
- Added JWT-based authentication: `AuthController` (`/auth/register`, `/auth/login`, `/auth/logout`), bcrypt password hashing, and tenant resolution from the `tenant_id` JWT claim.
