using authentification_service.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<AUTHDB>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Build the app
var app = builder.Build();

// Apply migrations and create database if not exists
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AUTHDB>();
    dbContext.Database.Migrate();
    
    // Seed admin user if not exists
    if (!dbContext.Users.Any(u => u.Username == "admin"))
    {
        var adminUsername = "admin";
        var adminPassword = BCrypt.Net.BCrypt.HashPassword("admin");
        
        var adminUser = new User
        {
            Username = adminUsername,
            PasswordHash = adminPassword,
            IsAdmin = true
        };
        
        try 
        {
            dbContext.Users.Add(adminUser);
            await dbContext.SaveChangesAsync();
            
            Console.WriteLine("Admin user created successfully");
            Console.WriteLine($"Username: {adminUsername}");
            Console.WriteLine($"Password: admin");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating admin user: {ex.Message}");
        }
    }
    else
    {
        Console.WriteLine("Admin user already exists");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAnyOrigin");
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
