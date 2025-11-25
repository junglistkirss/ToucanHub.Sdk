namespace ToucanHub.Sdk.Pipeline.Tests;

public class CounterContext : PipelineContext
{
    public List<string> Counter = [];

}


public class CounterContextWithOutput : PipelineContext
{
    public List<string> Counter = [];
    public string? Output { get; set; } = null;
}
