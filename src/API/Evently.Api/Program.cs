using Evently.Api.Extensions;
using Evently.Api.Middleware;
using Evently.Api.OpenTelemetry;
using Evently.Common.Application;
using Evently.Common.Infrastructure;
using Evently.Common.Infrastructure.EventBus;
using Evently.Common.Presentation.Endpoints;
using Evently.Modules.Attendance.Infrastructure;
using Evently.Modules.Events.Infrastructure;
using Evently.Modules.Users.Infrastructure;
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
    Evently.Modules.Events.Application.AssemblyReference.Assembly,
    Evently.Modules.Users.Application.AssemblyReference.Assembly,
    Evently.Modules.Attendance.Application.AssemblyReference.Assembly,
]);

string databaseConnectionString = builder.Configuration.GetConnectionString("Database")!;
string redisConnectionString = builder.Configuration.GetConnectionString("Cache")!;
var rabbitMqSettings = new RabbitMqSettings(builder.Configuration.GetConnectionString("Queue")!);

builder.Services.AddInfrastructure(
    DiagnosticsConfig.ServiceName,
    [
        EventsModule.ConfigureConsumers(redisConnectionString),
        AttendanceModule.ConfigureConsumers,
        UsersModule.ConfigureConsumers
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

builder.Configuration.AddModuleConfiguration(["events", "users", "attendance"]);

builder.Services.AddEventsModule(builder.Configuration);

builder.Services.AddUsersModule(builder.Configuration);

builder.Services.AddAttendanceModule(builder.Configuration);

WebApplication app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.WithTitle("Evently");
    options.AddServer(new ScalarServer("https://localhost:5001"));
    options.AddServer(new ScalarServer("http://localhost:5000"));
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
