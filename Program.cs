using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register the database context with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=jobtracker.db"));

// Allow the API to serve static files (our index.html)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseStaticFiles();
app.UseCors();

// Ensure the database is created on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// --- ROUTES ---

// GET all applications
app.MapGet("/applications", async (AppDbContext db) =>
    await db.Applications.ToListAsync());

// POST a new application
app.MapPost("/applications", async (Application app2, AppDbContext db) =>
{
    app2.CreatedAt = DateTime.UtcNow;
    db.Applications.Add(app2);
    await db.SaveChangesAsync();
    return Results.Created($"/applications/{app2.Id}", app2);
});

// DELETE an application by id
app.MapDelete("/applications/{id}", async (int id, AppDbContext db) =>
{
    var application = await db.Applications.FindAsync(id);
    if (application is null) return Results.NotFound();
    db.Applications.Remove(application);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Serve index.html at the root URL
app.MapFallbackToFile("index.html");

app.Run();

// --- MODELS ---

class Application
{
    public int Id { get; set; }
    public string Company { get; set; } = "";
    public string Role { get; set; } = "";
    public string Status { get; set; } = "Applied";
    public DateTime CreatedAt { get; set; }
}

class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Application> Applications => Set<Application>();
}