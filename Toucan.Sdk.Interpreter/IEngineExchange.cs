namespace Toucan.Sdk.Interpreter;

public delegate string ContextEncoder(object contextValue);
public delegate object? ResponseDecoder(string responseValue, Type type);

public interface IEngineExchange
{
    public ContextEncoder Encoder { get; }
    public ResponseDecoder Decoder { get; }
}
