namespace Application.Features.ValueFeature.Commands.CreateValue;

public class CreateValueValidator : AbstractValidator<CreateValueCommand>
{
    public CreateValueValidator()
    {
        RuleFor(v => v.Name).NotEmpty().MinimumLength(3).WithMessage("Name must be at least 3 characters").WithMessage("Name is required");
        RuleFor(v => v.ValueNumber).NotEmpty().WithMessage("Value number is required");
    }
}
