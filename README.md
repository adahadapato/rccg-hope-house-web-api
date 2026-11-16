# RCCG Hope House API

The back end for the RCCG Hope House website (`rccghopehouse.org.uk`) and its admin panel. It stores the church's content (services, sermons, events, gallery, devotionals), handles forms from the public site (prayer requests, contact, birthday wishes, the first-time visitor form), and runs background jobs such as the YouTube and Open Heavens syncs.

The website lives in a separate repository, `rccg-hope-house-web`.

## Tech stack

- .NET 10, ASP.NET Core minimal APIs
- Entity Framework Core with SQL Server
- ASP.NET Core Identity with JWT sign-in
- MediatR (commands and queries) and FluentValidation
- Serilog (rolling log file in production)
- Twilio (SMS), SMTP email, OpenAI (moderation and content helpers), YouTube Data API, API.Bible

## Project layout

The five projects sit directly in the repository root, next to `RccgHopeHouse.sln`.

| Project | What it holds |
|---|---|
| `RccgHopeHouse.Api` | Endpoints, middleware, start-up, rate limits, authorisation policies |
| `RccgHopeHouse.Application` | Features: commands, queries, validators and DTOs, one folder per feature |
| `RccgHopeHouse.Core` | Entities, value objects, enums, interfaces, constants |
| `RccgHopeHouse.Infrastructure` | Database (EF Core configurations, repositories, migrations), email, SMS, storage, AI and other outside services |
| `RccgHopeHouse.Worker` | Background services: YouTube sync, Open Heavens devotional sync, Thanksgiving sync, email queue, cache warm-up |

Dependencies point inwards: `Api` → `Application` → `Core`, with `Infrastructure` implementing the interfaces defined in `Core`.

## Getting started

You need the .NET 10 SDK and a SQL Server database (LocalDB is fine).

Run everything from the repository root.

```powershell
dotnet restore
dotnet build
dotnet run --project RccgHopeHouse.Api
```

The API listens on `http://localhost:5208` (and `https://localhost:7016`). In Development, Swagger is at `http://localhost:5208/swagger`. A health check is at `/health`.

On start-up the API applies any pending database migrations and seeds the roles and the first admin user, so a new empty database is ready after the first run.

## Configuration and secrets

`appsettings.json` lists every setting with the secret values left blank. **Never put real keys in it or in any file in this repository.**

- **Locally:** use .NET User Secrets.
  ```powershell
  dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your connection string>" --project RccgHopeHouse.Api
  ```
- **In production:** values come from GitHub Actions repository secrets, injected by the deploy workflow.

| Section | Settings | Notes |
|---|---|---|
| `ConnectionStrings` | `DefaultConnection` | SQL Server |
| `Jwt` | `Key`, `Issuer`, `Audience`, `ExpiryMinutes` | `Key` is secret |
| `AppSettings` | `ApiBaseUrl`, `WebsiteBaseUrl` | Used in links inside emails and texts |
| `Cors` | `AllowedOrigins` | The website's addresses |
| `EmailSettings` | SMTP host and port, admin and info mailboxes | The two passwords are secret |
| `SeedData` | `AdminEmail`, `AdminPassword` | First admin user; password is secret |
| `OpenAI` | `ApiKey`, `BaseUrl`, `Model` | `ApiKey` is secret |
| `Twilio` | `AccountSid`, `AuthToken`, `From` | All three are needed for SMS. If blank, no texts are sent; the first-time visitor form still saves and logs a warning |
| `YouTube` | `ApiKey`, `ThanksgivingPlaylistId`, `Channels` | `ApiKey` is secret |
| `ApiBible` | `BaseUrl`, `ApiKey` | `ApiKey` is secret |
| `OpenHeavens` | `BaseUrl` | Devotional source |

## Database migrations

Migrations live in `RccgHopeHouse.Infrastructure/Migrations`.

```powershell
# Create a migration after changing an entity or its configuration
dotnet ef migrations add <Name> --project RccgHopeHouse.Infrastructure --startup-project RccgHopeHouse.Api

# Apply to your local database
dotnet ef database update --project RccgHopeHouse.Infrastructure --startup-project RccgHopeHouse.Api

# Undo the last migration (roll the database back to the one before it first)
dotnet ef database update <PreviousMigrationName> --project RccgHopeHouse.Infrastructure --startup-project RccgHopeHouse.Api
dotnet ef migrations remove --project RccgHopeHouse.Infrastructure --startup-project RccgHopeHouse.Api
```

The live database is migrated automatically when the deployed API starts.

## API overview

All routes start with `/api`. The full, current list with request and response shapes is in Swagger. `RCCG-HopeHouse-API.postman_collection.json` has a starter Postman collection.

| Area | Route groups |
|---|---|
| Sign-in and accounts | `/auth`, `/account`, `/admin` |
| Church content | `/church-info`, `/services`, `/service-broadcasts`, `/sermons`, `/events`, `/announcements`, `/pastor-posts`, `/devotionals` |
| Annual content | `/themes-of-the-year`, `/prophecies`, `/annual-prayers` |
| Gallery | `/gallery`, `/gallery-categories`, `/birthday-wishes` |
| People | `/members`, `/first-timers`, `/celebrations`, `/invitations` |
| Connect | `/prayer-requests`, `/contact` |
| Giving | `/giving-types`, `/offerings` |
| Tools | `/bible`, `/seo`, `/ai-prompts` |

Public endpoints serve the website. Admin endpoints require sign-in, and in many groups they sit under `/<group>/admin`.

### Roles

`Admin`, `ContentEditor`, `MediaManager`, `PrayerTeam`, `UserManager`. `Admin` can do everything. Prayer requests are readable only by `Admin` and `PrayerTeam`.

### Public forms

Public form endpoints are rate-limited per IP address. Birthday wishes (`BirthdayWishSubmit`) and the first-time visitor form (`FirstTimerSubmit`) also carry a hidden honeypot field, `website`, to keep bots out. Celebration invitation links have their own limits.

### First-time visitors

`POST /api/first-timers` receives the welcome form that visitors open from the QR code. Visitors are stored in their own table, not the members list; an admin promotes one to a member later. A prayer request on the form is saved as a normal prayer request and linked by ID, so its text is never returned by the first-timer endpoints.

## Deployment

Pushing to `main` runs `.github/workflows/backend-ci.yml`, which restores, builds, publishes `RccgHopeHouse.Api` and deploys it. Production secrets are added after the build artifact is uploaded, so they are never stored in the artifact.

The workflow needs these repository secrets (Settings → Secrets and variables → Actions):

`DEFAULT_CONNECTION`, `JWT_KEY`, `OPENAI_API_KEY`, `YOUTUBE_API_KEY`, `API_BIBLE_KEY`, `EMAIL_ADMIN_PASSWORD`, `EMAIL_INFO_PASSWORD`, `SERVER_COMPUTER_NAME`, `SERVER_USERNAME`, `SERVER_PASSWORD`, `WEBSITE_NAME`

Deploy the API before the website when a website change depends on a new endpoint.

```powershell
git status
git add -A
git commit -m "Api: <what changed>"
git push origin main
```

## Keeping secrets out of the repository

- Run `git status` before every commit and read the list. Nothing outside the five project folders, `.github`, `.gitignore`, the solution file and this README should appear.
- `.gitignore` excludes `appsettings.Development.json`, `.env` files, `AppSettings-keep.txt`, build output and local uploads. Keep it in the repository root.
- If a secret is ever pushed, removing the commit is not enough: replace the secret at its provider straight away.
