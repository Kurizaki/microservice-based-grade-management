using authentification_service.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

/////////////////////////////////////////////////////////////
// 1. Add Services to the Container
//    - Registers controllers
//    - Configures the DbContext to use SQLite
//    - Enables a CORS policy that allows any origin, method, and header
/////////////////////////////////////////////////////////////
builder.Services.AddControllers();
builder.Services.AddDbContext<AUTHDB>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    // "AllowAnyOrigin" CORS policy
    options.AddPolicy("AllowAnyOrigin", builder =>
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
});

/////////////////////////////////////////////////////////////
// 2. Build the application
/////////////////////////////////////////////////////////////
var app = builder.Build();

/////////////////////////////////////////////////////////////
// 3. Database Migrations
//    - Creates a new scope to get the AUTHDB context
//    - Automatically applies any pending migrations 
//      and creates the database if it doesn't exist
/////////////////////////////////////////////////////////////
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AUTHDB>();
    dbContext.Database.Migrate();
}

/////////////////////////////////////////////////////////////
// 4. Configure the HTTP Request Pipeline
//    - Enables Developer Exception Page and Swagger in Development
//    - Applies the CORS policy
//    - Sets up Routing, Authorization, and Controller Endpoints
/////////////////////////////////////////////////////////////
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAnyOrigin");   // Apply CORS to requests

app.UseRouting();                // Enable endpoint routing
app.UseAuthorization();          // Use authorization middleware

app.MapControllers();            // Map controller routes

app.Run();                       // Run the application
