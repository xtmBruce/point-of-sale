using Microsoft.EntityFrameworkCore;
using SmartPOS.API.Data;
using SmartPOS.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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
// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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
