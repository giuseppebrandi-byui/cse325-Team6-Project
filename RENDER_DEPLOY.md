Render deployment notes for this Blazor app

This project is prepared to run on Render. If Render's dashboard does not
offer a native .NET option in your plan, choose "Docker" and deploy using
the included `Dockerfile` (recommended). The repo now includes a
multi-stage `Dockerfile` and a small `/health` endpoint to assist with
health checks.

Key notes

- The app binds to the port provided by the environment variable named
  `PORT`. Render sets this automatically. If `PORT` is not present the
  app falls back to `5000`.
- TLS is terminated by Render's edge proxy. The app is configured NOT to
  enforce HTTPS or HSTS so it works properly behind Render.
- The database connection string should be provided via environment
  variables (or Render's dashboard). This project reads the connection
  string from the usual ASP.NET Core configuration (appsettings\*.json)
  and also falls back to the environment variable `DEFAULT_CONNECTION`.

Using Docker on Render (recommended when .NET isn't listed)

1. In the Render dashboard create a new service → Web Service and select
   the Docker option.
2. Connect your repository and choose the branch to deploy.
3. Render will build the image using the `Dockerfile` in the repository.
   No explicit Build Command is required when using Docker — Render will
   run `docker build` based on the file.
4. Start Command: leave blank — the Docker image's ENTRYPOINT runs the
   app. Render will provide the `PORT` env var into the container.
5. Environment variables to set in the Render service settings:
   - `DEFAULT_CONNECTION` (your full Npgsql connection string) or set
     your own config name used by the app.
   - `Jwt:Issuer`, `Jwt:Audience`, `Jwt:Key` if your app requires them.

Health checks

- A simple health endpoint is available at `/health` which returns HTTP
  200 and a small JSON payload { status: "ok" }.

Troubleshooting

- Check Render logs if startup fails. Common causes are missing
  environment variables or runtime mismatches. Using the provided
  Dockerfile avoids runtime mismatch problems because it builds with
  the .NET 9 SDK.

If you'd like I can also add a Render YAML or further examples (Docker
compose, custom health checks, ready-made Render dashboard screenshots).
