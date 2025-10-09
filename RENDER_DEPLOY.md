Render deployment notes for this Blazor app

This project is prepared to run as a Render Web Service. Key notes:

- The app binds to the port provided by the environment variable named `PORT`.
  Render sets this automatically. If `PORT` is not present the app falls
  back to `5000`.

- TLS is terminated by Render's edge proxy. The app is configured NOT to
  enforce HTTPS or HSTS so it works properly behind Render.

- The database connection string should be provided via environment
  variables (or Render's dashboard). This project reads the connection
  string from the usual ASP.NET Core configuration (appsettings\*.json) and
  also falls back to the environment variable `DEFAULT_CONNECTION`.

Recommended Render service configuration

1. Create a new Web Service on Render.
2. Connect your repository and choose the branch you want to deploy (for
   example `main` or `Render-Test`).
3. Build Command (Render will detect dotnet automatically, but you can set explicitly):

   dotnet publish -c Release -o publish

4. Start Command (this uses the published dll and Render's provided PORT):

   dotnet publish/cse325-Team6-Project.dll

   (Adjust path if your publish output differs; Render's working directory
   will be the repository root after a build.)

5. Environment variables:
   - `PORT` (set by Render automatically)
   - `DEFAULT_CONNECTION` or configure the connection string in
     `appsettings.*.json` per your usual approach. If you use a managed
     Render Postgres DB, add the connection string in the service's
     environment variables.
   - Any `Jwt:*` values required by your app (Issuer/Audience/Key).

Health checks

- Render will consider the service healthy if it binds to the port and
  returns 200 on the root path. If you need a custom health endpoint, add
  a controller route (e.g. `/health`) that returns 200.

Troubleshooting

- If the app fails to start, check the Render service logs. Common issues
  are missing environment variables (connection string, JWT key) or
  runtime version mismatches. Ensure the service is using a compatible
  .NET runtime (the project targets net9.0).

Notes

- No changes were made to the `.env` file per request; the app already
  reads the connection string from environment/config.

- If you'd like, I can add a small `/health` controller and a sample
  Render `start` command that explicitly references the `PORT` env var.
