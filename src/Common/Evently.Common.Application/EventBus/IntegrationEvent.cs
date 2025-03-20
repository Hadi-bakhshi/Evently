namespace Evently.Common.Application.EventBus;

public abstract class IntegrationEvent( Guid id,DateTime occuredOnUtc) : IIntegrationEvent
{
    public Guid Id { get; init; } = id;
    public DateTime OccuredOnUtc { get; init; } = occuredOnUtc;
}
