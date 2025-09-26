namespace Application.Features.ValueFeature.Commands.InsertBulkValue;

internal class InsertBulkValueValidator : AbstractValidator<InsertBulkValueCommand>
{
    public InsertBulkValueValidator()
    {
        RuleFor(v => v.Values).NotEmpty().WithMessage("Values list cannot be empty");
        RuleForEach(v => v.Values).SetValidator(new BulkValueItemValidator());
    }
}

internal class BulkValueItemValidator : AbstractValidator<BulkValueItem>
{
    public BulkValueItemValidator()
    {
        RuleFor(v => v.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(v => v.ValueNumber).NotEmpty().WithMessage("Value number is required");
    }
}
