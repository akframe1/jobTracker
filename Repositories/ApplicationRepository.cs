using Microsoft.EntityFrameworkCore;

public class ApplicationRepository : IApplicationRepository
{
    private readonly AppDbContext _context;

    public ApplicationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Application>> GetAllAsync()
        => await _context.Applications.ToListAsync();

    public async Task<Application?> GetByIdAsync(int id)
        => await _context.Applications.FindAsync(id);

    public async Task<Application> AddAsync(Application application)
    {
        application.CreatedAt = DateTime.UtcNow;
        _context.Applications.Add(application);
        await _context.SaveChangesAsync();
        return application;
    }

    public async Task<Application?> UpdateAsync(int id, Application updated)
    {
        var existing = await _context.Applications.FindAsync(id);
        if (existing is null) return null;

        existing.Company = updated.Company;
        existing.Role = updated.Role;
        existing.Status = updated.Status;
        existing.CreatedAt = updated.CreatedAt;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var application = await _context.Applications.FindAsync(id);
        if (application is null) return false;

        _context.Applications.Remove(application);
        await _context.SaveChangesAsync();
        return true;
    }
}