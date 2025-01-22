using System.Diagnostics;

namespace Toucan.Sdk.EventSourcing;

public static class EventSourcingTelemetry
{
    public const string SourceName = "event_store";

    private static readonly ActivitySource source = new(SourceName);

    internal static Activity? Start(string name) => source.StartActivity(name);
}
