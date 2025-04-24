using Bogus;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Evently.Modules.Users.IntegrationTests.Abstractions;

[Collection(nameof(IntegrationTestCollection))]
public class BaseIntegrationTest : IDisposable
{
    protected static readonly Faker Faker = new();
    private readonly IServiceScope _scope;
    protected readonly ISender Sender;
    // this dependency will be resolved similar to the way of using
    // dependency inject in ASP.Net Core
    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
    }

    public void Dispose()
    {
        _scope.Dispose();
    }
}
