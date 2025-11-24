namespace Toucan.Infrastructure.Pipeline;

public class PipelineBuilder
{
    private readonly List<IMessageMiddleware> middlewares = new();

    private PipelineBuilder() { }

    public static PipelineBuilder Setup() => new();

    public PipelineBuilder Handle<T>(T eventHandler)
        where T : class, IMessageMiddleware
    {
        middlewares.Add(eventHandler);
        return this;
    }

    public PipelineBuilder Handle<TMessage>(
        Action<TMessage> handler) =>
        Handle(new MessageHandlerWrapper<TMessage>(handler));

    public PipelineBuilder Handle<TMessage>(
        Func<TMessage, CancellationToken, ValueTask> handler) =>
        Handle(new MessageHandlerWrapper<TMessage>(handler));

    public PipelineBuilder Handle<TMessage>(
        IMessageMiddleware<TMessage> handler) =>
        Handle(handler as IMessageMiddleware);

    public PipelineBuilder Transform<TMessage, TTransformedMessage>(
        Func<TMessage, TTransformedMessage> handler) =>
        Handle(new MessageTransformationWrapper<TMessage, TTransformedMessage>(handler));


    public PipelineBuilder Transform<TMessage>(
        Func<TMessage, object> handler) =>
        Handle(new MessageTransformationWrapper<TMessage>(handler));

    public PipelineBuilder Transform<TMessage, TTransformedMessage>(
        Func<TMessage, CancellationToken, ValueTask<TTransformedMessage>> handler) =>
        Handle(new MessageTransformationWrapper<TMessage, TTransformedMessage>(handler));


    public PipelineBuilder Transform<TMessage>(
        Func<TMessage, CancellationToken, ValueTask<object>> handler) =>
        Handle(new MessageTransformationWrapper<TMessage>(handler));

    public PipelineBuilder Transform<TMessage, TTransformedMessage>(
        IMessageTransformation<TMessage, TTransformedMessage> handler) =>
        Handle(handler);

    public PipelineBuilder Filter<TMessage>(
        Func<TMessage, bool> handler) =>
        Handle(new MessageFilterWrapper<TMessage>(handler));

    public PipelineBuilder Filter<TMessage>(
        Func<TMessage, CancellationToken, ValueTask<bool>> handler) =>
        Handle(new MessageFilterWrapper<TMessage>(handler));

    public PipelineBuilder Filter<TMessage>(
        IMessageFilter<TMessage> handler) =>
        Handle(handler);

    public IEnumerable<IMessageMiddleware> Build() => middlewares;
}