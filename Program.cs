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

// PUT - update an application
app.MapPut("/applications/{id}", async (int id, Application updated, AppDbContext db) =>
{
    var application = await db.Applications.FindAsync(id);
    if (application is null) return Results.NotFound();

    application.Company = updated.Company;
    application.Role = updated.Role;
    application.Status = updated.Status;
    application.CreatedAt = updated.CreatedAt;

    await db.SaveChangesAsync();
    return Results.Ok(application);
});

app.MapPost("/analyse", async (AnalyseRequest request, IConfiguration config) =>
{
    var apiKey = config["Groq:ApiKey"];

    var payload = new
    {
        model = "llama-3.1-8b-instant",
        messages = new[]
        {
            new {
                role = "user",
                content = $"Analyse this job description and return three things: 1. A 2 sentence summary of the role 2. Top 5 key skills required 3. Three suggested talking points for an interview. Job description: {request.JobDescription}"
            }
        }
    };

    using var http = new HttpClient();
    http.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

    var url = "https://api.groq.com/openai/v1/chat/completions";
    var response = await http.PostAsJsonAsync(url, payload);
    var rawJson = await response.Content.ReadAsStringAsync();

    var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    var result = System.Text.Json.JsonSerializer.Deserialize<GroqResponse>(rawJson, options);

    var text = result?.Choices?[0]?.Message?.Content ?? "No response received.";
    return Results.Ok(new { analysis = text });
});

// Serve index.html at the root URL
app.MapFallbackToFile("index.html");

app.Run();