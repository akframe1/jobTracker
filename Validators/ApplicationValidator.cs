using FluentValidation;

public class ApplicationValidator : AbstractValidator<Application>
{
    private static readonly string[] ValidStatuses = { "Applied", "Interview", "Offer", "Rejected" };

    public ApplicationValidator()
    {
        RuleFor(x => x.Company)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(100).WithMessage("Company name cannot exceed 100 characters.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .MaximumLength(100).WithMessage("Role cannot exceed 100 characters.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(s => ValidStatuses.Contains(s))
            .WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}.");

        RuleFor(x => x.CreatedAt)
            .Must(date => date == default || date <= DateTime.UtcNow)
            .WithMessage("Date cannot be in the future.");
    }
}