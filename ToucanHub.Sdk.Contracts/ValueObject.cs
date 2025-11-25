namespace ToucanHub.Sdk.Contracts;

public delegate T Change<T>(T sourceValueObject) where T : ValueObject;

[ExcludeFromCodeCoverage]
public abstract record class ValueObject { };