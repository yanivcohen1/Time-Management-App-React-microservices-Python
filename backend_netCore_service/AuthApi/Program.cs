using System.Security.Claims;
using System.Text;
using AuthApi;
using AuthApi.Models;
using AuthApi.Options;
using AuthApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Parse environment from command line args, default to dev
string env = "dev";
if (args.Length >= 2 && args[0] == "--env")
{
    env = args[1];
}

builder.Configuration.AddYamlFile($"{env}.appsettings.yaml", optional: true, reloadOnChange: true);

// Configure server URLs from YAML
var urls = builder.Configuration["Server:Urls"];
if (!string.IsNullOrEmpty(urls))
{
    builder.WebHost.UseUrls(urls);
}

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

// Configure CORS from YAML
var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"]?.Split(',') ?? new[] { "http://localhost:3000", "http://localhost:3001" };
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(8, 0, 21)),
        mySqlOptions => mySqlOptions.EnableStringComparisonTranslations()));

// Register user service based on database provider
var databaseProvider = builder.Configuration["Database:Provider"];
if (databaseProvider == "MongoDB")
{
    var mongoConnection = builder.Configuration.GetConnectionString("MongoConnection");
    var databaseName = mongoConnection?.Split('/').Last() ?? "netcore_auth_xunit";
    builder.Services.AddScoped<IUserService>(sp => new MongoUserService(mongoConnection!, databaseName));
}
else
{
    builder.Services.AddScoped<IUserService, DatabaseUserService>(); // Use MySQL-backed user service
}

builder.Services.AddScoped<ITokenService, TokenService>();

var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT configuration is missing");

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("User", "Admin"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsEnvironment("Testing") && !app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/auth/login", (LoginRequest request, IUserService userService, ITokenService tokenService) =>
{
    var user = userService.ValidateCredentials(request.Username, request.Password);
    return user is null
        ? Results.Unauthorized()
        : Results.Ok(tokenService.CreateToken(user));
}).AllowAnonymous();

app.MapGet("/api/admin/reports", [Authorize(Roles = "Admin")] (ClaimsPrincipal principal, IUserService userService) =>
{
    var username = principal.Identity?.Name ?? "unknown";
    var user = userService.GetUser(username);
    var payload = new
    {
        Owner = username,
        Role = user?.Role,
        GeneratedAtUtc = DateTime.UtcNow,
        Items = new[] { "Quarterly financials", "Infrastructure status" }
    };

    return Results.Ok(payload);
});

app.MapGet("/api/users/profile", [Authorize] (ClaimsPrincipal principal, IUserService userService) =>
{
    var username = principal.Identity?.Name;
    if (username is null)
    {
        return Results.Problem("A valid identity was not found for this request.", statusCode: StatusCodes.Status400BadRequest);
    }

    var user = userService.GetUser(username);
    return user is null
        ? Results.NotFound()
        : Results.Ok(new { user.Username, user.Role });
});


app.MapGet("/api/reports/daily", [Authorize(Policy = "UserPolicy")] () => // UserPolicy allows both User and Admin roles
{
    var items = new[]
    {
        new { Id = 1, Title = "Daily sales summary" },
        new { Id = 2, Title = "Active sessions" }
    };

    return Results.Ok(items);
});

// Health check endpoint, no authentication required add async
app.MapGet("/api/health", async () =>
{
    await Task.Yield(); // Asynchronous hook for future health probes
    return Results.Ok(new { status = "Healthy" });
}); // .AllowAnonymous();

app.Run();

public partial class Program;
