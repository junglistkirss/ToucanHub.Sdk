using Toucan.Sdk.Contracts.Query;

namespace Toucan.Sdk.Contracts.Wrapper;

public record class Results<T> : Result
{
    public static new Results<T> Error(params string[] messages) => new()
    {
        Status = ResultStatus.Error,
        Messages = [.. messages.Where(x => !string.IsNullOrWhiteSpace(x))]
    };
    public static new Results<T> Warn(params string[] messages) => new()
    {
        Status = ResultStatus.Warn,
        Messages = [.. messages.Where(x => !string.IsNullOrWhiteSpace(x))]
    };
    public static Results<T> Warn(IEnumerable<T>? models = default, params string[] messages) => new()
    {
        Values = models?.ToArray(),
        Status = ResultStatus.Warn,
        Messages = [.. messages.Where(x => !string.IsNullOrWhiteSpace(x))]
    };

    public static new Results<T> Success(params string[] messages) => Success([], messages);

    public static Results<T> Success(IEnumerable<T>? models, params string[] messages)
        => new()
        {
            Values = models?.ToList(),
            Status = ResultStatus.Success,
            Messages = [.. messages.Where(x => !string.IsNullOrWhiteSpace(x))]
        };
    public static Results<T> Success(PartialCollection<T>? models, params string[] messages)
           => new()
           {
               DomainCount = models?.DomainCount,
               Values = models?.ToList(),
               Status = ResultStatus.Success,
               Messages = [.. messages.Where(x => !string.IsNullOrWhiteSpace(x))]
           };
    public static Results<T> Success(IEnumerable<T>? models, long domainCount, params string[] messages)
       => new()
       {
           DomainCount = domainCount,
           Values = models?.ToList(),
           Status = ResultStatus.Success,
           Messages = [.. messages.Where(x => !string.IsNullOrWhiteSpace(x))]
       };

    public static Results<T> Success<TIn>(PartialCollection<TIn>? models, Converter<TIn, T> converter, params string[] messages)
         => new()
         {
             DomainCount = models?.DomainCount,
             Values = models?.ToList().ConvertAll(converter),
             Status = ResultStatus.Success,
             Messages = [.. messages.Where(x => !string.IsNullOrWhiteSpace(x))]
         };
    public static Results<T> Success<TIn>(IEnumerable<TIn>? models, Converter<TIn, T> converter, params string[] messages)
             => new()
             {
                 Values = models?.ToList().ConvertAll(converter),
                 Status = ResultStatus.Success,
                 Messages = [.. messages.Where(x => !string.IsNullOrWhiteSpace(x))]
             };
    public static Results<T> Success<TIn>(IEnumerable<TIn>? models, long domaincount, Converter<TIn, T> converter, params string[] messages)
             => new()
             {
                 DomainCount = domaincount,
                 Values = models?.ToList().ConvertAll(converter),
                 Status = ResultStatus.Success,
                 Messages = [.. messages.Where(x => !string.IsNullOrWhiteSpace(x))]
             };

    public static Results<T> Warn<TIn>(PartialCollection<TIn>? models, Converter<TIn, T> converter, params string[] messages)
        => new()
        {
            DomainCount = models?.DomainCount,
            Values = models?.ToList().ConvertAll(converter),
            Status = ResultStatus.Warn,
            Messages = [.. messages.Where(x => !string.IsNullOrWhiteSpace(x))]
        };
    public static Results<T> Warn<TIn>(IEnumerable<TIn>? models, Converter<TIn, T> converter, params string[] messages)
             => new()
             {
                 Values = models?.ToList().ConvertAll(converter),
                 Status = ResultStatus.Warn,
                 Messages = [.. messages.Where(x => !string.IsNullOrWhiteSpace(x))]
             };
    public static Results<T> Warn<TIn>(IEnumerable<TIn>? models, long domainCount, Converter<TIn, T> converter, params string[] messages)
         => new()
         {
             DomainCount = domainCount,
             Values = models?.ToList().ConvertAll(converter),
             Status = ResultStatus.Warn,
             Messages = [.. messages.Where(x => !string.IsNullOrWhiteSpace(x))]
         };

    public long? DomainCount { get; private init; }
    public IReadOnlyCollection<T>? Values { get; private init; }
}