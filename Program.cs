using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=jobtracker.db"));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IAnalysisService, AnalysisService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseCors();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// --- ROUTES ---

app.MapGet("/applications", async (IApplicationService svc) =>
    await svc.GetAllAsync());

app.MapPost("/applications", async (Application application, IApplicationService svc) =>
{
    var created = await svc.AddAsync(application);
    return Results.Created($"/applications/{created.Id}", created);
});

app.MapPut("/applications/{id}", async (int id, Application updated, IApplicationService svc) =>
{
    var result = await svc.UpdateAsync(id, updated);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

app.MapDelete("/applications/{id}", async (int id, IApplicationService svc) =>
{
    var deleted = await svc.DeleteAsync(id);
    return deleted ? Results.NoContent() : Results.NotFound();
});

app.MapPost("/analyse", async (AnalyseRequest request, IAnalysisService svc) =>
{
    var analysis = await svc.AnalyseAsync(request.JobDescription);
    return Results.Ok(new { analysis });
});

app.MapFallbackToFile("index.html");

app.Run();