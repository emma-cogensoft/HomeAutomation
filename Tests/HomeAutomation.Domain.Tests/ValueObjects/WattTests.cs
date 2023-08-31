using HomeAutomation.Domain.ValueObjects;

namespace HomeAutomation.Domain.Tests.ValueObjects;

[TestOf(nameof(Watt))]
public class WattTests
{
    [Test]
    [TestCase(0)]
    [TestCase(100)]
    [TestCase(10_000)]
    [TestCase(-50)]
    [TestCase(-10_000)]
    public void WhenWattIsCreated_HasExpectedValues(int value)
    {
        // Act
        var sut = new Watt(value);

        // Assert
        Assert.That(sut.Value, Is.EqualTo(value));
    }

    [Test]
    [TestCase(0)]
    [TestCase(100)]
    [TestCase(10_000)]
    [TestCase(-50)]
    [TestCase(-10_000)]
    public void WhenWattIsCreated_HasExpectedToString(int value)
    {
        // Act
        var sut = new Watt(value);

        // Assert
        Assert.That(sut.ToString(), Is.EqualTo($"{value}W"));
    }

    [Test]
    [TestCase(0)]
    [TestCase(100)]
    [TestCase(10_000)]
    [TestCase(-50)]
    [TestCase(-10_000)]
    public void WhenWattIsCreated_HasExpectedImplicitConversionToWatt(int value)
    {
        // Act
        Watt sut = value;

        // Assert
        Assert.That(sut.Value, Is.EqualTo(value));
    }

    [Test]
    [TestCase(0)]
    [TestCase(100)]
    [TestCase(10_000)]
    [TestCase(-50)]
    [TestCase(-10_000)]
    public void WhenWattIsCreated_HasExpectedImplicitConversionToInt(int value)
    {
        // Arrange
        var sut = new Watt(value);

        // Act
        int result = sut;

        // Assert
        Assert.That(result, Is.EqualTo(value));
    }

    [Test]
    [TestCase(0)]
    [TestCase(100)]
    [TestCase(10_000)]
    [TestCase(-50)]
    [TestCase(-10_000)]
    public void WhenWattIsCreated_HasExpectedEqualsOperator(int value)
    {
        // Arrange
        var sut = new Watt(value);

        // Act
        var result = sut == value;

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    [TestCase(0)]
    [TestCase(100)]
    [TestCase(10_000)]
    [TestCase(-50)]
    [TestCase(-10_000)]
    public void WhenWattIsCreated_HasExpectedNotEqualsOperator(int value)
    {
        // Arrange
        var sut = new Watt(value);

        // Act
        var result = sut != value;

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    [TestCase(0, 0)]
    [TestCase(100, 100)]
    [TestCase(10_000, 10_000)]
    [TestCase(-50, -50)]
    [TestCase(-10_000, -10_000)]
    public void WhenWattIsCreated_HasExpectedEqualsOperator(int value1, int value2)
    {
        // Arrange
        var sut1 = new Watt(value1);
        var sut2 = new Watt(value2);

        // Act
        var result = sut1 == sut2;

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    [TestCase(0, 1)]
    [TestCase(100, 101)]
    [TestCase(10_000, 10_001)]
    [TestCase(-50, -49)]
    [TestCase(-10_000, -9_999)]
    public void WhenWattIsCreated_HasExpectedNotEqualsOperator(int value1, int value2)
    {
        // Arrange
        var sut1 = new Watt(value1);
        var sut2 = new Watt(value2);

        // Act
        var result = sut1 != sut2;

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    [TestCase(0, 0)]
    [TestCase(100, 100)]
    [TestCase(10_000, 10_000)]
    [TestCase(-50, -50)]
    [TestCase(-10_000, -10_000)]
    public void WhenWattIsCreated_HasExpectedEquals(int value1, int value2)
    {
        // Arrange
        var sut1 = new Watt(value1);
        var sut2 = new Watt(value2);

        // Act
        var result = sut1.Equals(sut2);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    [TestCase(0, 1)]
    [TestCase(100, 101)]
    [TestCase(10_000, 10_001)]
    [TestCase(-50, -49)]
    [TestCase(-10_000, -9_999)]
    public void WhenWattIsCreated_HasExpectedNotEquals(int value1, int value2)
    {
        // Arrange
        var sut1 = new Watt(value1);
        var sut2 = new Watt(value2);

        // Act
        var result = sut1.Equals(sut2);

        // Assert
        Assert.That(result, Is.False);
    }
    
    [Test]
    [TestCase(0, 100)]
    [TestCase(100, 10_000)]
    [TestCase(10_000, 0)]
    public void WhenWattIsCreatedWithDifferentValue_HaveDifferentHashCode(int value1, int value2)
    {
        // Act
        var sut1 = new Watt(value1);
        var sut2 = new Watt(value2);

        // Assert
        Assert.That(sut1.GetHashCode(), Is.Not.EqualTo(sut2.GetHashCode()));
    }
    
    [Test]
    [TestCase(0, 0)]
    [TestCase(100, 100)]
    [TestCase(10_000, 10_000)]
    public void WhenWattIsCreatedWithSameValue_HaveSameHashCode(int value1, int value2)
    {
        // Act
        var sut1 = new Watt(value1);
        var sut2 = new Watt(value2);

        // Assert
        Assert.That(sut1.GetHashCode(), Is.EqualTo(sut2.GetHashCode()));
    }
}