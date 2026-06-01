using FluentValidation;

public class BinomialRequestValidator : AbstractValidator<BinomialRequest>
{
    public BinomialRequestValidator()
    {
        RuleFor(x => x.SpotPrice)
            .GreaterThan(0).WithMessage("Spot price must be greater than zero.");

        RuleFor(x => x.StrikePrice)
            .GreaterThan(0).WithMessage("Strike price must be greater than zero.");

        RuleFor(x => x.TimeToExpiry)
            .GreaterThan(0).WithMessage("Time to expiry must be greater than zero.")
            .LessThanOrEqualTo(10).WithMessage("Time to expiry cannot exceed 10 years.");

        RuleFor(x => x.RiskFreeRate)
            .InclusiveBetween(0, 1).WithMessage("Risk free rate must be between 0 and 1 (e.g. 0.05 for 5%).");

        RuleFor(x => x.Volatility)
            .GreaterThan(0).WithMessage("Volatility must be greater than zero.")
            .LessThanOrEqualTo(5).WithMessage("Volatility cannot exceed 500%.");

        RuleFor(x => x.Steps)
            .InclusiveBetween(10, 1000).WithMessage("Steps must be between 10 and 1000.");
    }
}