namespace HomeAutomation.Domain.ValueObjects;

/// <summary>
/// WattHours is a value object that represents a value in WattHours. It can not be less than 0.
/// </summary>
public class WattHours: ValueObject<WattHours>
{
    public const string Unit = "Wh";
    public int Value { get; }

    public WattHours(int value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "WattHours value must be greater than 0");
        }

        Value = value;
    }
    
    public static implicit operator int(WattHours value) => value.Value;
    public static implicit operator WattHours(int value) => new(value);

    protected override bool EqualsCore(WattHours other) => Value == other.Value;

    protected override int GetHashCodeCore() => nameof(WattHours).GetHashCode() + Value.GetHashCode();
    
    public override string ToString()
    {
        return $"{Value}{Unit}";
    }
}