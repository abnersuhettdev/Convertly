# Convertly

Convertly is a full stack SaaS-style document conversion app. The current MVP lets users convert DOCX files to PDF with authentication, plan limits, private file handling, conversion history and a polished bilingual interface.

Live project: [convertly-theta.vercel.app](https://convertly-theta.vercel.app)

## Objective

The goal of Convertly is to demonstrate a realistic full stack product, beyond a simple CRUD app. It combines file upload, background processing, authentication, private storage, usage limits, account management and product-focused UX decisions such as accessibility, privacy, i18n and responsible content guidance.

## Stack

- Frontend: React, TypeScript, Vite, Tailwind CSS
- Backend: .NET 8, ASP.NET Core Web API
- Database: PostgreSQL
- Storage: Supabase Storage
- Auth: JWT access tokens and refresh tokens
- Background jobs: Hangfire
- Conversion engine: LibreOffice headless
- Deploy: Vercel frontend and Render backend

## Main Features

- Register and login
- DOCX to PDF conversion
- Drag and drop file upload
- Private conversion history
- Conversion status and detail page
- Protected PDF download
- Plan limits for monthly usage, file size and retention
- Simulated Free, Pro and Business plans
- Account page with password change and account deletion
- PT-BR and EN interface
- Terms and copyright/content policy pages
- Accessibility and visual polish for a more professional SaaS experience

## Running Locally

Requirements:

- Node.js
- .NET 8 SDK
- PostgreSQL database
- Supabase project and storage buckets
- LibreOffice installed for real DOCX to PDF conversion

### Backend

```bash
cd backend
dotnet restore
dotnet build Convertly.sln
dotnet run --project src/Convertly.Api/Convertly.Api.csproj
```

The API runs with the `/api` path base, usually at:

```txt
http://localhost:5000/api
```

### Frontend

```bash
cd frontend
npm install
npm run dev
```

Create `frontend/.env` with:

```txt
VITE_API_BASE_URL=http://localhost:5000/api
```

## Tests

Backend:

```bash
cd backend
dotnet test Convertly.sln
```

Frontend:

```bash
cd frontend
npm test
npm run build
```

## Environment Variables

Do not commit real secrets. The main settings are:

```txt
ConnectionStrings__DefaultConnection
Jwt__Secret
Jwt__Issuer
Jwt__Audience
Supabase__Url
Supabase__ServiceRoleKey
Supabase__OriginalsBucket
Supabase__ConvertedBucket
Conversion__LibreOfficePath
Frontend__BaseUrl
VITE_API_BASE_URL
```

## Status

Convertly is portfolio-ready as a full stack MVP. Payments are simulated and the implemented conversion flow is DOCX to PDF only.
