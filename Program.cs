using MapelRestAPI.DI;
using MapelRestAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendLocalhost",
        policy =>
        {
            policy.WithOrigins("http://localhost:8080") // Your AngularJS app URL
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Bind configuration
var azureAdOptions = builder.Configuration.GetSection("AzureAd");
string authority = $"{azureAdOptions["Instance"]}{azureAdOptions["TenantId"]}/v2.0";
string audience = $"api://{azureAdOptions["FrontAppClientId"]!}";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Clean Architecture Dependencies
builder.Services.AddInfrastructure();
builder.Services.AddScoped<UserCreationHandler>();
builder.Services.AddScoped<UserInvitaionHandler>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authority;
        options.Audience = audience; // App Registration for backend API
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowFrontendLocalhost"); // Enable CORS before authentication
app.MapControllers();
app.Run();
