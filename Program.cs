using Microsoft.OpenApi.Models; // correct namespace for Swagger
using UserManagementAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 7252;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "User Management API", Version = "v1" });
});

var app = builder.Build();

// Middleware order matters
app.UseMiddleware<LoggingMiddleware>(); // 1. Logging (log all requests)
app.UseMiddleware<AuthenticationMiddleware>(); // 2. Authentication
app.UseMiddleware<ErrorHandlingMiddleware>(); // 3. Error handling

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API v1");
    });
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
