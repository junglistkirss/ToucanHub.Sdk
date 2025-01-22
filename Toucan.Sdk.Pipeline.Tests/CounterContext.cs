namespace Toucan.Sdk.Pipeline.Tests;

public record class CounterContext : PipelineContext
{
    public List<string> Counter = [];

}


public record class CounterContextWithOutput : PipelineContext
{
    public List<string> Counter = [];
    public string? Output { get; set; } = null;
}
