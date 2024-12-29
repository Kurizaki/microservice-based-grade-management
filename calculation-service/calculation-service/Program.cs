var builder = WebApplication.CreateBuilder(args);

/////////////////////////////////////////////////////////////
// 1. Register Services
/////////////////////////////////////////////////////////////
builder.Services.AddControllers();               // Adds controller support
builder.Services.AddEndpointsApiExplorer();      // Enables minimal APIs explorer
builder.Services.AddSwaggerGen();                // Adds Swagger/OpenAPI generation

// CORS: Allow any origin, method, and header
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", policyBuilder =>
        policyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
});

/////////////////////////////////////////////////////////////
// 2. Build the Application
/////////////////////////////////////////////////////////////
var app = builder.Build();

/////////////////////////////////////////////////////////////
// 3. Configure Middleware (HTTP pipeline)
/////////////////////////////////////////////////////////////
if (app.Environment.IsDevelopment())
{
    // Provide detailed exception page and enable Swagger in development
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply the "AllowAnyOrigin" CORS policy to incoming requests
app.UseCors("AllowAnyOrigin");

// Use routing middleware to direct requests to the correct endpoints
app.UseRouting();

// Use authorization (if configured in the controllers)
app.UseAuthorization();

// Map incoming requests to controllers
app.MapControllers();

// Run the app
app.Run();