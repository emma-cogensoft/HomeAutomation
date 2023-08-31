namespace HomeAutomation.Domain.ValueObjects;

public class TimeOfDay : ValueObject<TimeOfDay>
{
    public int Value { get; }

    public TimeOfDay(int value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "TimeOfDay value must be greater than 0");
        }

        Value = value;
    }

    public static TimeOfDay Empty => new(0);
    
    public static implicit operator int(TimeOfDay value) => value.Value;
    public static implicit operator TimeOfDay(int value) => new(value);

    public override string ToString() => Value.ToString();

    protected override bool EqualsCore(TimeOfDay other) => Value == other.Value;

    protected override int GetHashCodeCore() => nameof(TimeOfDay).GetHashCode() + Value.GetHashCode();
}