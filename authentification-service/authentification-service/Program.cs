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

// Apply migrations and seed admin user if not exists
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AUTHDB>();
    dbContext.Database.Migrate();

    // Seed the admin user
    SeedAdminUser(dbContext);
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

app.UseCors("AllowAnyOrigin");
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
