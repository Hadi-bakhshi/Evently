namespace Evently.Common.Application.Clock;

public interface IDateTimeProvider
{
#pragma warning disable IDE0040 // Add accessibility modifiers
    public DateTime UtcNow { get; }
#pragma warning restore IDE0040 // Add accessibility modifiers
}
