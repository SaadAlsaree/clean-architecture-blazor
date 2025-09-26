namespace Application.Features.ValueFeature.Commands.UpdateValue;

internal class UpdateValueValidator : AbstractValidator<UpdateValueCommand>
{
    public UpdateValueValidator()
    {
        RuleFor(v => v.Id).NotEmpty().WithMessage("Id is required");
        RuleFor(v => v.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(v => v.ValueNumber).NotEmpty().WithMessage("Value number is required");
    }
}
