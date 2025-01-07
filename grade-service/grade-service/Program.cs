using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using grade_service.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"])),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });

// Add controllers
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "GradeAPI", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// Configure SQLite as the database
var connectionString = builder.Environment.IsDevelopment() 
    ? builder.Configuration.GetConnectionString("DefaultConnection")
    : builder.Configuration.GetSection("DockerConnectionStrings")["DefaultConnection"];

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Apply migrations on startup with retry logic
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    int retryCount = 0;
    const int maxRetries = 5;
    const int retryDelay = 5000; // 5 seconds
    
    while (retryCount < maxRetries)
    {
        try
        {
            logger.LogInformation("Attempting to apply database migrations (Attempt {RetryCount})", retryCount + 1);
            
            // Verify database file exists
            var dbPath = "/app/data/GradeDb.sqlite";
            if (!File.Exists(dbPath))
            {
                logger.LogInformation("Database file not found, creating new database at {DbPath}", dbPath);
            }
            
            dbContext.Database.Migrate();
            logger.LogInformation("Database migrations applied successfully");
            break;
        }
        catch (Microsoft.Data.Sqlite.SqliteException ex) when (ex.SqliteErrorCode == 10) // SQLITE_IOERR
        {
            retryCount++;
            logger.LogError(ex, "Database I/O error occurred. Retrying in {RetryDelay}ms...", retryDelay);
            
            if (retryCount >= maxRetries)
            {
                logger.LogCritical("Failed to apply database migrations after {MaxRetries} attempts", maxRetries);
                throw;
            }
            
            Thread.Sleep(retryDelay);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Critical error applying database migrations");
            throw;
        }
    }
}

// Configure the rest of the pipeline
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAnyOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
