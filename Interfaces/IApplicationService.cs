public interface IApplicationService
{
    Task<IEnumerable<Application>> GetAllAsync();
    Task<Application?> GetByIdAsync(int id);
    Task<Application> AddAsync(Application application);
    Task<Application?> UpdateAsync(int id, Application updated);
    Task<bool> DeleteAsync(int id);
}