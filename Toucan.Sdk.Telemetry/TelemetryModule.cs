using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Toucan.Sdk.Telemetry;


/*
 // Custom metrics for the application
var greeterMeter = new Meter("OtPrGrYa.Example", "1.0.0");
var countGreetings = greeterMeter.CreateCounter<int>("greetings.count", description: "Counts the number of greetings");

// Custom ActivitySource for the application
var greeterActivitySource = new ActivitySource("OtPrGrJa.Example");*/
public static class TelemetryModule
{

    //public static void ExposeTelemetry(this WebApplication app) => app.MapPrometheusScrapingEndpoint();
    public static IOpenTelemetryBuilder AddTelemetry(this IServiceCollection services, string applicationName/*, string? tracingOtlpEndpoint = null*/)
    {
        IOpenTelemetryBuilder otel = services.AddOpenTelemetry();

        // Configure OpenTelemetry Resources with the application name
        otel.ConfigureResource(resource => resource
            .AddService(serviceName: applicationName));

        // Add Metrics for ASP.NET Core and our custom metrics and export to Prometheus
        otel.WithMetrics(metrics => metrics
            // Metrics provider from OpenTelemetry
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            //.AddMeter(greeterMeter.Name)
            // Metrics provides by ASP.NET Core in .NET 8
            .AddMeter("Microsoft.AspNetCore.Hosting")
            .AddMeter("Microsoft.AspNetCore.Routing")
            .AddMeter("Microsoft.AspNetCore.Diagnostics")
            .AddMeter("Microsoft.AspNetCore.HeaderParsing")
            .AddMeter("Microsoft.AspNetCore.HeaderParsing")
            .AddMeter("System.Net.NameResolution")
            .AddMeter("System.Net.Http")
            .AddMeter("Microsoft.Extensions.Diagnostics.HealthChecks")
            .AddMeter("Microsoft.Extensions.Diagnostics.ResourceMonitoring")
            .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
            .AddView("http.server.request.duration",
                new ExplicitBucketHistogramConfiguration
                {
                    Boundaries = new double[] { 0, 0.005, 0.01, 0.025, 0.05,
                           0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10 }
                })
            //.AddPrometheusExporter()
            );
        //// Add Tracing for ASP.NET Core and our custom ActivitySource and export to Jaeger
        //otel.WithTracing(tracing =>
        //{
        //    tracing.AddAspNetCoreInstrumentation();
        //    tracing.AddHttpClientInstrumentation();
        //    //tracing.AddSource(greeterActivitySource.Name);
        //    if (tracingOtlpEndpoint != null)
        //    {
        //        tracing.AddOtlpExporter(otlpOptions =>
        //        {
        //            otlpOptions.Endpoint = new Uri(tracingOtlpEndpoint);
        //        });
        //    }
        //    else
        //    {
        //        tracing.AddConsoleExporter();
        //    }
        //});
        return otel;
    }

}
