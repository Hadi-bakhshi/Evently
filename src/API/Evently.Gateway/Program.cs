using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using Serilog;
using Evently.Gateway.OpenTelemetry;
using Evently.Gateway.Authentication;
using Evently.Gateway.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));


builder.Services
           .AddOpenTelemetry()
           .ConfigureResource(resource => resource.AddService(DiagnosticsConfig.ServiceName))
           .WithTracing(tracing =>
           {
               tracing
                   .AddAspNetCoreInstrumentation()
                   .AddHttpClientInstrumentation()
                   .AddSource("Yarp.ReverseProxy");

               tracing.AddOtlpExporter();
           });

builder.Services.AddAuthorization();

builder.Services.AddAuthentication().AddJwtBearer();

builder.Services.ConfigureOptions<JwtBearerConfigureOptions>();

WebApplication app = builder.Build();

app.UseLogContextTraceLogging();

app.UseSerilogRequestLogging();

app.UseAuthentication();

app.UseAuthorization();

app.MapReverseProxy();

app.Run();
