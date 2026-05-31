public class ApplicationService : IApplicationService
{
    private readonly IApplicationRepository _repository;

    public ApplicationService(IApplicationRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Application>> GetAllAsync()
        => _repository.GetAllAsync();

    public Task<Application?> GetByIdAsync(int id)
        => _repository.GetByIdAsync(id);

    public Task<Application> AddAsync(Application application)
        => _repository.AddAsync(application);

    public Task<Application?> UpdateAsync(int id, Application updated)
        => _repository.UpdateAsync(id, updated);

    public Task<bool> DeleteAsync(int id)
        => _repository.DeleteAsync(id);
}