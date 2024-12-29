using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using grade_service.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

/////////////////////////////////////////////////////////////
// 1. Configure JWT Authentication
/////////////////////////////////////////////////////////////
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Validates that the token is signed with a valid key
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"])
            ),
            ValidateIssuer = false,     // Disable issuer validation
            ValidateAudience = false,   // Disable audience validation
        };
    });

/////////////////////////////////////////////////////////////
// 2. Add Controllers
/////////////////////////////////////////////////////////////
builder.Services.AddControllers();

/////////////////////////////////////////////////////////////
// 3. Configure Swagger/OpenAPI
/////////////////////////////////////////////////////////////
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "GradeAPI", Version = "v1" });

    // Bearer token authentication for Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Require the "Bearer" scheme for any protected endpoints
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

/////////////////////////////////////////////////////////////
// 4. Configure Database (SQLite)
/////////////////////////////////////////////////////////////
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

/////////////////////////////////////////////////////////////
// 5. Configure CORS
/////////////////////////////////////////////////////////////
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", policyBuilder =>
        policyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
    );
});

/////////////////////////////////////////////////////////////
// 6. Build the Application
/////////////////////////////////////////////////////////////
var app = builder.Build();

/////////////////////////////////////////////////////////////
// 7. Apply Database Migrations on Startup
/////////////////////////////////////////////////////////////
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    dbContext.Database.Migrate();
}

/////////////////////////////////////////////////////////////
// 8. Configure Middleware in the HTTP Pipeline
/////////////////////////////////////////////////////////////
app.UseSwagger();               // Enable Swagger endpoint
app.UseSwaggerUI();             // Enable Swagger UI
app.UseCors("AllowAnyOrigin");  // Apply the CORS policy
app.UseAuthentication();        // Enable JWT-based authentication
app.UseAuthorization();         // Enable authorization checks
app.MapControllers();           // Map controller endpoints

/////////////////////////////////////////////////////////////
// 9. Run the Application
/////////////////////////////////////////////////////////////
app.Run();
