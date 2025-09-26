namespace Application.Features.ValueFeature.Queries.ExportExcelValue;

internal class ExportExcelValueValidator : AbstractValidator<ExportExcelValueQuery>
{
    public ExportExcelValueValidator()
    {
        RuleFor(v => v.CreatedFrom)
            .LessThanOrEqualTo(v => v.CreatedTo ?? DateTime.MaxValue)
            .When(v => v.CreatedFrom.HasValue && v.CreatedTo.HasValue)
            .WithMessage("CreatedFrom must be less than or equal to CreatedTo");
    }
}
