using Evently.Api.Extensions;
using Evently.Common.Application;
using Evently.Common.Infrastructure;
using Evently.Modules.Events.Infrastructure;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddApplication([Evently.Modules.Events.Application.AssemblyReference.Assembly]);
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("Database")!);

builder.Configuration.AddModuleConfiguration(["events"]);

builder.Services.AddEventsModule(builder.Configuration);

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

EventsModule.MapEndpoints(app);

#pragma warning disable S6966
app.Run();

