using authentification_service.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
var connectionString = builder.Environment.IsDevelopment() 
    ? builder.Configuration.GetConnectionString("DefaultConnection")
    : builder.Configuration.GetSection("DockerConnectionStrings")["DefaultConnection"];

builder.Services.AddDbContext<AUTHDB>(options =>
    options.UseSqlite(connectionString));

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Build the app
var app = builder.Build();

// Apply migrations with retry logic
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AUTHDB>();
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
            var dbPath = "/app/data/AUTHDatabase.sqlite";
            if (!File.Exists(dbPath))
            {
                logger.LogInformation("Database file not found, creating new database at {DbPath}", dbPath);
            }
            
            dbContext.Database.Migrate();
            logger.LogInformation("Database migrations applied successfully");
            
            // Seed the admin user
            SeedAdminUser(dbContext);
            break;
        }
        catch (Microsoft.Data.Sqlite.SqliteException ex) when (ex.SqliteErrorCode == 14) // SQLITE_CANTOPEN
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

void SeedAdminUser(AUTHDB dbContext)
{
    if (!dbContext.Users.Any(u => u.Username == "admin"))
    {
        var adminUser = new User
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"),
            IsAdmin = true
        };

        dbContext.Users.Add(adminUser);
        dbContext.SaveChanges();

        Console.WriteLine("Admin user created successfully: Username=admin, Password=admin");
    }
    else
    {
        Console.WriteLine("Admin user already exists.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add request logging middleware
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"Incoming {context.Request.Method} request to {context.Request.Path}");
    logger.LogInformation($"Request headers: {string.Join(", ", context.Request.Headers.Select(h => $"{h.Key}: {h.Value}"))}");
    
    await next();
    
    logger.LogInformation($"Response status code: {context.Response.StatusCode}");
});

app.UseCors("AllowAnyOrigin");
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
