namespace Toucan.Sdk.Contracts.JsonData;

public enum JsonDataValueType
{
    Unspecified = 0,
    Null,
    Object,
    Array,
    Number,
    String,
    Boolean,
    DateTime,
    DateTimeOffset,
    DateOnly,
    TimeSpan,
    TimeOnly,
}

public enum JsonDataNumberType
{
    Unprecised,
    Byte, SByte,
    Short, Int32, Int64,
    UShort, UInt32, UInt64,
    Double, Float, Decimal,
}
