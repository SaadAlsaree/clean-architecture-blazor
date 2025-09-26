using Cortex.Mediator.Commands;

namespace Application.Features.ValueFeature.Commands.CreateValue;

public class CreateValueCommand : ICommand<Response<Guid>>
{
    public string Name { get; set; } = string.Empty;
    public int ValueNumber { get; set; }
}
