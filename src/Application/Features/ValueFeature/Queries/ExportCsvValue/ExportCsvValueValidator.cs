namespace Application.Features.ValueFeature.Queries.ExportCsvValue;

internal class ExportCsvValueValidator : AbstractValidator<ExportCsvValueQuery>
{
    public ExportCsvValueValidator()
    {
        RuleFor(v => v.CreatedFrom)
            .LessThanOrEqualTo(v => v.CreatedTo ?? DateTime.MaxValue)
            .When(v => v.CreatedFrom.HasValue && v.CreatedTo.HasValue)
            .WithMessage("CreatedFrom must be less than or equal to CreatedTo");
    }
}
