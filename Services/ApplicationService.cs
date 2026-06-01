public class ApplicationService : IApplicationService
{
    private readonly IApplicationRepository _repository;
    private readonly ApplicationValidator _validator;
    private readonly ILogger<ApplicationService> _logger;

    private static readonly Dictionary<string, string[]> AllowedTransitions = new()
    {
        { "Applied",   new[] { "Interview", "Rejected" } },
        { "Interview", new[] { "Offer", "Rejected" } },
        { "Offer",     new[] { "Rejected" } },
        { "Rejected",  Array.Empty<string>() }
    };

    public ApplicationService(
        IApplicationRepository repository,
        ApplicationValidator validator,
        ILogger<ApplicationService> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public Task<IEnumerable<Application>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all applications");
        return _repository.GetAllAsync();
    }

    public Task<Application?> GetByIdAsync(int id)
        => _repository.GetByIdAsync(id);

    public async Task<Application> AddAsync(Application application)
    {
        var validation = await _validator.ValidateAsync(application);
        if (!validation.IsValid)
        {
            var errors = validation.Errors.Select(e => e.ErrorMessage);
            throw new ValidationException(errors);
        }

        var created = await _repository.AddAsync(application);
        _logger.LogInformation("Application created: ID {Id}, Company {Company}", created.Id, created.Company);
        return created;
    }

    public async Task<Application?> UpdateAsync(int id, Application updated)
    {
        var validation = await _validator.ValidateAsync(updated);
        if (!validation.IsValid)
        {
            var errors = validation.Errors.Select(e => e.ErrorMessage);
            throw new ValidationException(errors);
        }

        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
            throw new ApplicationNotFoundException(id);

        if (existing.Status != updated.Status)
        {
            var allowed = AllowedTransitions[existing.Status];
            if (!allowed.Contains(updated.Status))
            {
                _logger.LogWarning("Invalid transition attempted: {From} -> {To}", existing.Status, updated.Status);
                throw new InvalidStatusTransitionException(existing.Status, updated.Status);
            }
        }

        var result = await _repository.UpdateAsync(id, updated);
        _logger.LogInformation("Application updated: ID {Id}", id);
        return result;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
            throw new ApplicationNotFoundException(id);

        var deleted = await _repository.DeleteAsync(id);
        _logger.LogInformation("Application deleted: ID {Id}", id);
        return deleted;
    }
}