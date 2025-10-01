using Microsoft.EntityFrameworkCore;
using reservation_tracker_backend_dotnet.Data;
using reservation_tracker_backend_dotnet.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

    // Seed database if in development
    if (builder.Environment.IsDevelopment())
    {
        options.UseSeeding((context, _) => DbSeeder.Seed(context))
            .UseAsyncSeeding((context, _, ct) => DbSeeder.SeedAsync(context, ct));
    }
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Quick DB connection check
app.MapGet("/db-check", async (AppDbContext db) =>
{
    try
    {
        await db.Database.OpenConnectionAsync();
        await db.Database.CloseConnectionAsync();
        return Results.Ok("Database connection successful");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Database connection failed: {ex.Message}");
    }
});


app.Run();
