using Microsoft.EntityFrameworkCore;
using SmartPOS.API.Data;
using SmartPOS.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Text;
using SmartPOS.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on port 7086 with HTTPS.
// This uses the default dev certificate when available in Development.
var port = Environment.GetEnvironmentVariable("PORT");

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(
        int.Parse(port ?? "8080")
    );
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var connectionString = ResolveConnectionString(builder.Configuration);
var connectionInfo = new NpgsqlConnectionStringBuilder(connectionString);
Console.WriteLine($"Database host: {connectionInfo.Host}, DB: {connectionInfo.Database}, SSL mode: {connectionInfo.SslMode}");
// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
        npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy
            .WithOrigins(
                "https://point-of-sale-wapf.onrender.com",
                "https://smartpos-retail.onrender.com",
                "http://localhost:5173",
                "http://localhost:3000"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithExposedHeaders("Content-Disposition", "Content-Type", "X-Pagination"));
});

// Configure JWT authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection.GetValue<string>("Key");
var issuer = jwtSection.GetValue<string>("Issuer");
var audience = jwtSection.GetValue<string>("Audience");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
        
        // Handle CORS preflight and 401 responses
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
                }
                return System.Threading.Tasks.Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return System.Threading.Tasks.Task.CompletedTask;
            }
        };
    });

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddHostedService<NotificationSchedulerService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter: Bearer {your token}"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();
app.UseForwardedHeaders();

// Automatically apply pending migrations on startup
try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        Console.WriteLine("✓ Database migrations applied successfully");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"✗ Error applying migrations: {ex.Message}");
}

// Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        DbSeeder.SeedAsync(context).Wait();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
// Enable Swagger in all environments for testing
app.UseSwagger();
app.UseSwaggerUI();

// Add global exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Apply CORS middleware BEFORE authentication/authorization
app.UseCors("AllowReactApp");

// Comment out HTTPS redirect for now to avoid the warning
// app.UseHttpsRedirection();

// Authentication and Authorization after CORS
app.UseAuthentication();
app.UseAuthorization();

// Add default route to redirect to Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapControllers();

app.Run();

static string ResolveConnectionString(IConfiguration configuration)
{
    var rawConnectionString =
        Environment.GetEnvironmentVariable("DATABASE_URL") ??
        Environment.GetEnvironmentVariable("POSTGRES_URL") ??
        Environment.GetEnvironmentVariable("POSTGRESQL_URL") ??
        configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrWhiteSpace(rawConnectionString))
        throw new InvalidOperationException("Database connection string is not configured. Set DATABASE_URL or ConnectionStrings:DefaultConnection.");

    if (rawConnectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
        rawConnectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
    {
        return ConvertPostgresUrlToConnectionString(rawConnectionString);
    }

    var builder = new NpgsqlConnectionStringBuilder(rawConnectionString);
    ApplySslDefaults(builder);
    return builder.ConnectionString;
}

static string ConvertPostgresUrlToConnectionString(string databaseUrl)
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':', 2);
    var builder = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port > 0 ? uri.Port : 5432,
        Username = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : string.Empty,
        Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty,
        Database = uri.AbsolutePath.TrimStart('/'),
        Pooling = true
    };

    var query = uri.Query.TrimStart('?')
        .Split('&', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    foreach (var queryEntry in query)
    {
        var keyValue = queryEntry.Split('=', 2);
        if (keyValue.Length != 2)
            continue;

        var key = keyValue[0];
        var value = Uri.UnescapeDataString(keyValue[1]);

        if (key.Equals("sslmode", StringComparison.OrdinalIgnoreCase) &&
            Enum.TryParse<SslMode>(value, true, out var sslMode))
        {
            builder.SslMode = sslMode;
        }
    }

    ApplySslDefaults(builder);
    return builder.ConnectionString;
}

static void ApplySslDefaults(NpgsqlConnectionStringBuilder builder)
{
    var host = builder.Host?.Trim().ToLowerInvariant();
    var isLocalHost = host is "localhost" or "127.0.0.1" or "::1";
    if (isLocalHost)
        return;

    if (builder.SslMode is SslMode.Disable or SslMode.Prefer)
        builder.SslMode = SslMode.Require;
}
