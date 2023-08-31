namespace HomeAutomation.Domain.ValueObjects;

/// <summary>
/// Percentage is a value type that represents a value in percentage. It can not be less than 0 or more than 100.
/// </summary>
public class Percentage: ValueObject<Percentage>
{
    public const string Unit = "%";
  
    public int Value { get; }

    public Percentage(int value)
    {
        if (value is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Percentage value must be between 0 and 100");
        }

        Value = value;
    }

    public static implicit operator int(Percentage value) => value.Value;
    public static implicit operator Percentage(int value) => new(value);
    
    protected override bool EqualsCore(Percentage other) => Value == other.Value;

    protected override int GetHashCodeCore() => nameof(Percentage).GetHashCode() + Value.GetHashCode();
}
