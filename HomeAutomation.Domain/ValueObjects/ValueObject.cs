namespace HomeAutomation.Domain.ValueObjects;

public abstract class ValueObject<T> : IEquatable<ValueObject<T>> where T : ValueObject<T>
{
    public override bool Equals(object? obj)
    {
        if (obj is not T valueObject) return false;
        return Equals(valueObject);
    }

    public bool Equals(ValueObject<T>? other)
    {
        if (other is not T valueObject) return false;
        return EqualsCore(valueObject);
    }

    public override int GetHashCode()
    {
        return GetHashCodeCore();
    }

    protected abstract bool EqualsCore(T other);
    protected abstract int GetHashCodeCore();

    public static bool operator ==(ValueObject<T>? a, ValueObject<T>? b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;

        return a.Equals(b);
    }

    public static bool operator !=(ValueObject<T>? a, ValueObject<T>? b) => !(a == b);
    
}
