using Domain.Common;
using Cortex.Mediator.Commands;

namespace Application.Features.ValueFeature.Commands.UpdateValue;

public class UpdateValueCommand : ICommand<Response<Guid>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ValueNumber { get; set; }
    public Status StatusId { get; set; }
}
