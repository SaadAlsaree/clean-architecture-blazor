namespace Application.Features.ValueFeature.Queries.GetListValue;

internal class GetListValueValidator : AbstractValidator<GetListValueQuery>
{
    public GetListValueValidator()
    {
        RuleFor(v => v.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(v => v.PageSize)
            .InclusiveBetween((byte)1, (byte)100)
            .WithMessage("PageSize must be between 1 and 100");

        RuleFor(v => v.CreatedFrom)
            .LessThanOrEqualTo(v => v.CreatedTo ?? DateTime.MaxValue)
            .When(v => v.CreatedFrom.HasValue && v.CreatedTo.HasValue)
            .WithMessage("CreatedFrom must be less than or equal to CreatedTo");

        RuleFor(v => v.Name)
            .MaximumLength(100)
            .When(v => !string.IsNullOrEmpty(v.Name))
            .WithMessage("Name cannot exceed 100 characters");
    }
}
