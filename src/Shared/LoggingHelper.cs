using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Shared;

public static class LoggingHelper
{
    public static void ConfigureLogging(HostBuilderContext ctx, LoggerConfiguration lc)
    {
        lc.MinimumLevel.Debug()
        .Enrich.FromLogContext()
        .WriteTo.Console(theme: AnsiConsoleTheme.Code);

        //Fallback to console if file sink fails
        lc.WriteTo.Console();
    }

    public static IHostBuilder UseCentralLogging(this IHostBuilder hostBuilder, string servicename)
    {
        hostBuilder.UseSerilog((context, services, configuration) =>
        {
            configuration
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ServiceName", servicename)
                .WriteTo.Console()
                .WriteTo.File(
                    path: $"logs/{servicename}-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 14, //Keep 2 weeks
                    rollOnFileSizeLimit: true
                );
        });
        return hostBuilder;
    }

    public static IServiceCollection AddCentralTracing(this IServiceCollection services, string servicename)
    {
        services.AddOpenTelemetry().WithTracing(builder =>
        {
            builder
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(servicename))
                .AddAspNetCoreInstrumentation() // Trace dotnet stuff
                .AddHttpClientInstrumentation() // Trace http
                .AddSqlClientInstrumentation() // trace sql
                .AddEntityFrameworkCoreInstrumentation() // trace ef
                .AddSource(servicename)
                .AddZipkinExporter(o =>
                {
                    o.Endpoint = new Uri("http://zipkin:9411/api/v2/spans");
                });
        });
        return services;
    }
}
