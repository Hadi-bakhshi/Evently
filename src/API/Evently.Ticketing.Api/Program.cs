using Evently.Common.Infrastructure.EventBus;
using Evently.Common.Infrastructure;
using Evently.Common.Presentation.Endpoints;
using Evently.Common.Application;
using Evently.Modules.Ticketing.Infrastructure;
using Evently.Ticketing.Api.Extensions;
using Evently.Ticketing.Api.Middleware;
using Evently.Ticketing.Api.OpenTelemetry;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using RabbitMQ.Client;
using Scalar.AspNetCore;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddApplication([
    Evently.Modules.Ticketing.Application.AssemblyReference.Assembly,
]);

string databaseConnectionString = builder.Configuration.GetConnectionString("Database")!;
string redisConnectionString = builder.Configuration.GetConnectionString("Cache")!;
var rabbitMqSettings = new RabbitMqSettings(builder.Configuration.GetConnectionString("Queue")!);

builder.Services.AddInfrastructure(
    DiagnosticsConfig.ServiceName,
    [
        TicketingModule.ConfigureConsumers,
    ],
    rabbitMqSettings,
    databaseConnectionString,
    redisConnectionString);


builder.Services.AddHealthChecks()
    .AddNpgSql(databaseConnectionString)
    .AddRedis(redisConnectionString)
    .AddRabbitMQ(factory: _ =>
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(rabbitMqSettings.Host),
            UserName = "hadi",
            Password = "hadi@123"
        };
        return factory.CreateConnectionAsync().GetAwaiter().GetResult();
    })
    .AddUrlGroup(
        new Uri(builder.Configuration.GetValue<string>("KeyCloak:HealthUrl")!),
        HttpMethod.Get,
        "keycloak");

builder.Configuration.AddModuleConfiguration(["ticketing"]);

builder.Services.AddTicketingModule(builder.Configuration);

WebApplication app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.WithTitle("Evently.Ticketing.Api");
    options.AddServer(new ScalarServer("https://localhost:5101"));
    options.AddServer(new ScalarServer("http://localhost:5100"));
    options.AddServer(new ScalarServer("http://localhost:8080"));
    options.AddServer(new ScalarServer("https://localhost:8081"));
}); // scalar/v1

if (app.Environment.IsDevelopment())
{
    app.ApplyMigrations();
}

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseLogContextTraceLogging();
// logging middleware
app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapEndpoints();

app.Run();

public partial class Program;
