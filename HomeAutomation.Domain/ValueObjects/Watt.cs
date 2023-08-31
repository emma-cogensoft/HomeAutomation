namespace HomeAutomation.Domain.ValueObjects;

/// <summary>
/// Watt Value is a value object that represents a value in Watts.
/// </summary>
public class Watt: ValueObject<Watt>
{
    public const string Unit = "W";
    public int Value { get; }

    public Watt(int value)
    {
        Value = value;
    }

    public static implicit operator int(Watt value) => value.Value;
    public static implicit operator Watt(int value) => new(value);

    protected override bool EqualsCore(Watt other) => Value == other.Value;

    protected override int GetHashCodeCore() => nameof(Watt).GetHashCode() + Value.GetHashCode();
    
    public override string ToString()
    {
        return $"{Value}{Unit}";
    }
}