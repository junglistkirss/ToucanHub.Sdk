namespace Toucan.Sdk.Contracts;


public delegate T Change<T>(T sourceValueObject)
   where T : ValueObject;

public abstract record class ValueObject { };