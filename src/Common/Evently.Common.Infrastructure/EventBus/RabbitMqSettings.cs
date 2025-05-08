namespace Evently.Common.Infrastructure.EventBus;

public sealed record RabbitMqSettings(string Host, string Username = "hadi", string Password = "hadi@123");

