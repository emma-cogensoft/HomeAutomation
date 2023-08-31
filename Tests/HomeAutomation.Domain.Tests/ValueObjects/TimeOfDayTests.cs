using HomeAutomation.Domain.ValueObjects;

namespace HomeAutomation.Domain.Tests.ValueObjects;

[TestOf(nameof(TimeOfDay))]
public class TimeOfDayTests
{
    [Test]
    [TestCase(0)]
    [TestCase(5136)]
    [TestCase(1_000)]
    public void WhenTimeOfDayIsCreated_HasExpectedValues(int value)
    {
        // Act
        var sut = new TimeOfDay(value);

        // Assert
        Assert.That(sut.Value, Is.EqualTo(value));
    }

    [Test]
    [TestCase(0)]
    [TestCase(5136)]
    [TestCase(1_000)]
    public void WhenTimeOfDayIsCreated_HasExpectedToString(int value)
    {
        // Act
        var sut = new TimeOfDay(value);

        // Assert
        Assert.That(sut.ToString(), Is.EqualTo($"{value}"));
    }

    [Test]
    [TestCase(0)]
    [TestCase(5136)]
    [TestCase(1_000)]
    public void WhenTimeOfDayIsCreated_HasExpectedImplicitConversionToTimeOfDay(int value)
    {
        // Act
        TimeOfDay sut = value;

        // Assert
        Assert.That(sut.Value, Is.EqualTo(value));
    }

    [Test]
    [TestCase(0)]
    [TestCase(5136)]
    [TestCase(1_000)]
    public void WhenTimeOfDayIsCreated_HasExpectedImplicitConversionToInt(int value)
    {
        // Arrange
        var sut = new TimeOfDay(value);

        // Act
        int result = sut;

        // Assert
        Assert.That(result, Is.EqualTo(value));
    }

    [Test]
    [TestCase(0)]
    [TestCase(5136)]
    [TestCase(1_000)]
    public void WhenTimeOfDayIsCreated_HasExpectedEqualsOperator(int value)
    {
        // Arrange
        var sut = new TimeOfDay(value);

        // Act
        var result = sut == value;

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    [TestCase(0)]
    [TestCase(5136)]
    [TestCase(1_000)]
    public void WhenTimeOfDayIsCreated_HasExpectedNotEqualsOperator(int value)
    {
        // Arrange
        var sut = new TimeOfDay(value);

        // Act
        var result = sut != value;

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    [TestCase(0, 0)]
    [TestCase(5136, 5136)]
    [TestCase(1_000, 1_000)]
    public void WhenTimeOfDayIsCreated_HasExpectedEqualsOperator(int value1, int value2)
    {
        // Arrange
        var sut1 = new TimeOfDay(value1);
        var sut2 = new TimeOfDay(value2);

        // Act
        var result = sut1 == sut2;

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    [TestCase(0, 1)]
    [TestCase(5136, 5137)]
    [TestCase(1_000, 1_001)]
    public void WhenTimeOfDayIsCreated_HasExpectedNotEqualsOperator(int value1, int value2)
    {
        // Arrange
        var sut1 = new TimeOfDay(value1);
        var sut2 = new TimeOfDay(value2);

        // Act
        var result = sut1 != sut2;

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    [TestCase(0, 0)]
    [TestCase(5136, 5136)]
    [TestCase(1_000, 1_000)]
    public void WhenTimeOfDayIsCreated_HasExpectedEquals(int value1, int value2)
    {
        // Arrange
        var sut1 = new TimeOfDay(value1);
        var sut2 = new TimeOfDay(value2);

        // Act
        var result = sut1.Equals(sut2);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    [TestCase(0, 1)]
    [TestCase(5136, 5137)]
    [TestCase(1_000, 1_001)]
    public void WhenTimeOfDayIsCreated_HasExpectedNotEquals(int value1, int value2)
    {
        // Arrange
        var sut1 = new TimeOfDay(value1);
        var sut2 = new TimeOfDay(value2);

        // Act
        var result = sut1.Equals(sut2);

        // Assert
        Assert.That(result, Is.False);
    }
    
    [Test]
    [TestCase(0, 5136)]
    [TestCase(5136, 1_000)]
    [TestCase(1_000, 0)]
    public void WhenTimeOfDayIsCreatedWithDifferentValue_HaveDifferentHashCode(int value1, int value2)
    {
        // Act
        var sut1 = new TimeOfDay(value1);
        var sut2 = new TimeOfDay(value2);

        // Assert
        Assert.That(sut1.GetHashCode(), Is.Not.EqualTo(sut2.GetHashCode()));
    }
    
    [Test]
    [TestCase(0, 0)]
    [TestCase(5136, 5136)]
    [TestCase(1_000, 1_000)]
    public void WhenTimeOfDayIsCreatedWithSameValue_HaveSameHashCode(int value1, int value2)
    {
        // Act
        var sut1 = new TimeOfDay(value1);
        var sut2 = new TimeOfDay(value2);

        // Assert
        Assert.That(sut1.GetHashCode(), Is.EqualTo(sut2.GetHashCode()));
    }
}