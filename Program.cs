using MapelRestAPI.DI;
using MapelRestAPI.Entities;
using MapelRestAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json, environment variables, and user secrets
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>(); // For local dev secret storage

// Bind AzureAd configuration section to a strongly typed class
builder.Services.Configure<AzureAdSettings>(builder.Configuration.GetSection("AzureAd"));

// Enable CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendLocalhost", policy =>
    {
        policy.WithOrigins("http://localhost:8080", "https://thankful-island-0a228fb0f.2.azurestaticapps.net/") // AngularJS frontend
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Extract auth settings
var azureAdSection = builder.Configuration.GetSection("AzureAd");
string authority = $"{azureAdSection["Instance"]}{azureAdSection["TenantId"]}/v2.0";
string audience = $"api://{azureAdSection["FrontAppClientId"]}";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Clean Architecture dependencies
builder.Services.AddInfrastructure();
builder.Services.AddScoped<UserCreationHandler>();
builder.Services.AddScoped<UserInvitaionHandler>();

// Configure JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authority;
        options.Audience = audience;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowFrontendLocalhost");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
