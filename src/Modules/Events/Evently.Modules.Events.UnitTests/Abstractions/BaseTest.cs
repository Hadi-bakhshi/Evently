using Bogus;
using Evently.Common.Domain;

namespace Evently.Modules.Events.UnitTests.Abstractions;

#pragma warning disable CA1515
public abstract class BaseTest
{
    protected static readonly Faker Faker = new();

    public static T AssertDomainEventWasPublished<T>(Entity entity)
    {
        T? domainEvent = entity.DomainEvents.OfType<T>().SingleOrDefault();

        if (domainEvent is null)
        {
            throw new Exception($"Domain event of type {typeof(T).Name} was not published.");
        }

        return domainEvent;
    }
}
