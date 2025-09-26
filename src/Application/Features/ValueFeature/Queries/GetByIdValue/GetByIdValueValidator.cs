namespace Application.Features.ValueFeature.Queries.GetByIdValue;

internal class GetByIdValueValidator : AbstractValidator<GetByIdValueQuery>
{
    public GetByIdValueValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty()
            .WithMessage("Id is required")
            .NotEqual(Guid.Empty)
            .WithMessage("Id cannot be empty GUID");
    }
}
