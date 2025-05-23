using AuthenticationApp.API.Extensions; // Contains Swagger & JWT extensions for API
using AuthenticationApp.Infrastructure.Extensions;
using AuthenticationApp.Infrastructure.Persistence;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args); // Creates a new WebApplication with default configuration

// ------------------ SERVICE REGISTRATIONS ------------------

// Adds support for minimal API controllers
builder.Services.AddControllers();

// ✅ Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "ready" })
    .AddDbContextCheck<ApplicationDbContext>("Database", tags: new[] { "ready" });

// ✅ Health Checks UI
builder.Services.AddHealthChecksUI(options =>
{
    options.SetEvaluationTimeInSeconds(15); // check interval
    options.MaximumHistoryEntriesPerEndpoint(60);
    options.AddHealthCheckEndpoint("Authentication API", "/health");
    // ✅ Webhook
    options.AddWebhookNotification("Webhook.site Alert", uri: "https://webhook.site/89b40eeb-74f9-4b25-9bf3-0ffda448054c",
        payload: "{\"text\": \"🚨 Failure detected at {{TIMESTAMP}}\"}",
        restorePayload: "{\"text\": \"✅ Recovery at {{TIMESTAMP}}\"}");
})
.AddInMemoryStorage(); // or use AddSqlServerStorage if needed


// ✅ Swagger Documentation
// Registers Swagger generator and configures it from appsettings (title, security, XML docs, etc.)
builder.Services.AddSwaggerDocumentation(builder.Configuration);

// ✅ JWT Authentication Setup
// Configures JWT Bearer authentication using settings from appsettings.json
builder.Services.AddJwtAuthentication(builder.Configuration);

// ✅ Infrastructure Services
// Registers:
// - DbContext (EF Core)
// - Identity (UserManager, SignInManager, RoleManager)
// - Application services (AuthService, EmailSender, JwtTokenGenerator, etc.)
// - Domain event dispatching and repositories
builder.Services.AddInfrastructure(builder.Configuration);

// ------------------ BUILDING APPLICATION ------------------

var app = builder.Build(); // Builds the WebApplication from the builder (services + middleware pipeline)

// ✅ Seed Default Admin User and Roles
// This will check if "Admin" role and "admin@yourdomain.com" user exist, and create them if not.
await app.SeedDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    // Enable Swagger UI in development environment
    app.UseSwaggerDocumentation();
}

// ------------------ MIDDLEWARE PIPELINE ------------------

// Redirects all HTTP requests to HTTPS
app.UseHttpsRedirection();

// Adds authentication middleware to check JWT tokens
app.UseAuthentication();

// Adds authorization middleware to enforce role/policy-based access
app.UseAuthorization();

// ✅ Health Check endpoint
app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
        });
        await context.Response.WriteAsync(result);
    }
});

// ✅ HealthChecks UI dashboard
app.UseHealthChecksUI(config =>
{
    config.UIPath = "/health-ui"; // open in browser
});

// Maps attribute-based controllers (e.g., [HttpGet], [Route])
app.MapControllers();

// Starts the application
app.Run();
