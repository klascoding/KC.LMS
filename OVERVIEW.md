Learning Management System (LMS) 
A software application for the administration, documentation, tracking, reporting, automation, and delivery of educational courses, training programs, or learning and development programs.

Backend — .NET 10 API.
Frontend — React + Vite, built with the Bootstrap design system.
Authentication — JWT-based authentication with refresh tokens. Users can register, log in, and log out. Passwords are hashed using bcrypt.

Prerequisites
.NET SDK 10
Node.js 20+ and npm
Bootstrap 5.3

Project layout
docs/               # markdown documentation for users of the LMS
KC.LMS.Server/     # minimal API
    Controllers/   # TUnit tests (unit + endpoint)
kc.lms.client/     # React + Vite + Bootstrap SPA

Backend
cd KC.LMS.Server

# Run the API (serves https://localhost:65373)
dotnet run --project KC.LMS.Server

Frontend
cd KC.LMS.Client
npm install        # pulls Bootstrap (see Prerequisites)
npm run dev        # Vite dev server on http://localhost:65373
With both the API and the dev server running, open http://localhost:65373 to add, edit,

Documentation
The repository includes client-facing documentation that explains what the LMS does and how users can work with it day to day.

Product overview: docs/lms/index.md
Feature summary: docs/lms/features.md