# Convertly

Convertly is a full stack SaaS-style document conversion project built for portfolio, interviews and technical practice.

The current MVP converts DOCX files to PDF with user accounts, plan limits, private storage, conversion history and simulated billing. The V2.0 evolution keeps the original MVP stable while improving product positioning, visual quality, security, privacy, i18n, accessibility and responsible content use.

## Stack

- Frontend: React, TypeScript, Vite, Tailwind CSS
- Backend: .NET 8, ASP.NET Core Web API
- Database: PostgreSQL, designed for Supabase PostgreSQL
- Storage: Supabase Storage
- Auth: JWT access token + refresh token
- Background jobs: Hangfire
- Conversion engine: LibreOffice headless
- Deploy target: Vercel for frontend, Render Docker service for backend/API + worker

## Main Features

- Account creation and login
- DOCX to PDF conversion
- Monthly conversion limits by plan
- File size and retention limits by plan
- Private upload and download flow
- Conversion history and detail pages
- Simulated Free, Pro and Business plans
- PT-BR and EN interface
- Upload validation, blocked extensions and safer error messages
- Basic rate limiting for sensitive endpoints
- Terms and copyright/content policy pages
- Accessibility polish for labels, keyboard navigation and feedback states

## V1 and V2.0

V1 is the stable MVP: authentication, plans, DOCX to PDF conversion, storage, history and download.

V2.0 is an incremental product evolution. It does not rewrite the project and does not change the core conversion rule. It adds:

- clearer SaaS positioning;
- visual polish;
- i18n PT-BR/EN;
- security and abuse-prevention improvements;
- privacy and retention communication;
- copyright and responsible upload guidance;
- accessibility and inclusive language improvements;
- portfolio-ready documentation.

## Project Structure

```txt
backend/
  src/
    Convertly.Api/
    Convertly.Application/
    Convertly.Domain/
    Convertly.Infrastructure/
  tests/
    Convertly.Tests/
frontend/
  src/
docs/
  v2.0/
README.md
```

## Running Locally

Requirements:

- Node.js compatible with the frontend toolchain
- .NET 8 SDK
- PostgreSQL connection string, preferably Supabase PostgreSQL
- Supabase Storage buckets for original and converted files
- LibreOffice available in the backend runtime for real conversions

### Backend

```bash
cd backend
dotnet restore
dotnet build
dotnet run --project src/Convertly.Api/Convertly.Api.csproj
```

The API is expected by the frontend at:

```txt
http://localhost:5000/api
```

### Frontend

```bash
cd frontend
npm install
npm run dev
```

Create `frontend/.env` from `frontend/.env.example` when needed:

```txt
VITE_API_BASE_URL=http://localhost:5000/api
```

## Tests

Backend:

```bash
cd backend
dotnet test
```

Frontend:

```bash
cd frontend
npm test
npm run build
```

## Environment Variables

Do not commit real secrets. Use local settings, Render variables or Supabase project settings.

Main backend settings:

```txt
ConnectionStrings__DefaultConnection
Jwt__Secret
Jwt__Issuer
Jwt__Audience
Jwt__AccessTokenMinutes
Jwt__RefreshTokenDays
Supabase__Url
Supabase__ServiceRoleKey
Supabase__OriginalsBucket
Supabase__ConvertedBucket
Conversion__LibreOfficePath
Conversion__LibreOfficeTimeoutSeconds
Frontend__BaseUrl
```

Main frontend setting:

```txt
VITE_API_BASE_URL
```

## Deployment Notes

- Frontend: Vercel, root directory `frontend`, build command `npm run build`, output directory `dist`.
- Backend: Render Docker service using `backend/Dockerfile`.
- Database and storage: Supabase PostgreSQL and Supabase Storage.
- Sensitive values such as JWT secret, database password and Supabase service role key must stay only in backend environment variables.

## V2.0 Documentation

The V2.0 documentation lives in [docs/v2.0](docs/v2.0/README.md).

Useful portfolio docs:

- [V2.0 demo script](docs/v2.0/docs/37-demo-script-v2.md)
- [Final review checklist](docs/v2.0/docs/38-final-review-checklist-v2.md)
- [Technical summary](docs/v2.0/docs/39-v2-technical-summary.md)

## Current Status

Convertly is portfolio-ready as a full stack MVP with V2.0 product polish. Payments are simulated, only DOCX to PDF conversion is implemented, and production-grade legal, abuse-reporting and payment workflows remain future improvements.
