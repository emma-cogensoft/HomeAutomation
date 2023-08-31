using HomeAutomation.Domain.ValueObjects;

namespace HomeAutomation.Domain.Tests.ValueObjects;

[TestOf(nameof(Percentage))]
public class PercentageTests
{
    [Test]
    [TestCase(0)]
    [TestCase(50)]
    [TestCase(100)]
    public void WhenValueIsInRange_DoesNotThrowException(int value)
    {
        // Act
        var sut = new Percentage(value);

        // Assert
        Assert.That(sut.Value, Is.EqualTo(value));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(101)]
    public void WhenValueIsOutOfRange_ThrowsException(int value)
    {
        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Percentage(value));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception!.ParamName, Is.EqualTo("value"));
            Assert.That(exception.ActualValue, Is.EqualTo(value));
            Assert.That(exception.Message,
                Is.EqualTo(
                    $"Percentage value must be between 0 and 100 (Parameter 'value'){Environment.NewLine}Actual value was {value}."));
        });
    }

    [Test]
    [TestCase(0, 0)]
    [TestCase(50, 50)]
    [TestCase(100, 100)]
    public void WhenImplicitlyConvertingToInteger_ReturnsExpectedValue(int value, int expectedValue)
    {
        // Arrange
        var sut = new Percentage(value);

        // Act
        var result = (int)sut;

        // Assert
        Assert.That(result, Is.EqualTo(expectedValue));
    }

    [Test]
    [TestCase(0, 0)]
    [TestCase(50, 50)]
    [TestCase(100, 100)]
    public void WhenImplicitlyConvertingToPercentage_ReturnsExpectedValue(int value, int expectedValue)
    {
        // Act
        var result = (Percentage)value;

        // Assert
        Assert.That(result.Value, Is.EqualTo(expectedValue));
    }

    [Test]
    [TestCase(0, 0)]
    [TestCase(50, 50)]
    [TestCase(100, 100)]
    public void WhenComparingTwoInstancesWithSameValue_AreEqual(int value, int expectedValue)
    {
        // Arrange
        var sut1 = new Percentage(value);
        var sut2 = new Percentage(expectedValue);

        // Act
        var result = sut1 == sut2;

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    [TestCase(0, 1)]
    [TestCase(50, 51)]
    [TestCase(100, 55)]
    public void WhenComparingTwoInstancesWithDifferentValue_AreNotEqual(int value, int expectedValue)
    {
        // Arrange
        var sut1 = new Percentage(value);
        var sut2 = new Percentage(expectedValue);

        // Act
        var result = sut1 == sut2;

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    [TestCase(0, 1)]
    [TestCase(50, 51)]
    [TestCase(100, 55)]
    public void WhenComparingTwoInstancesWithDifferentValueUsingNotEqualsOperator_AreNotEqual(int value,
        int expectedValue)
    {
        // Arrange
        var sut1 = new Percentage(value);
        var sut2 = new Percentage(expectedValue);

        // Act
        var result = sut1 != sut2;

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    [TestCase(0, 1)]
    [TestCase(50, 51)]
    [TestCase(100, 55)]
    public void WhenComparingTwoInstancesWithDifferentValueUsingEqualsMethod_AreNotEqual(int value, int expectedValue)
    {
        // Arrange
        var sut1 = new Percentage(value);
        var sut2 = new Percentage(expectedValue);

        // Act
        var result = sut1.Equals(sut2);

        // Assert
        Assert.That(result, Is.False);
    }
    
    [Test]
    [TestCase(0, 0)]
    [TestCase(50, 50)]
    [TestCase(100, 100)]
    public void WhenComparingTwoInstancesWithSameValueButComparingIntToPercentageUsingEqualsMethod_AreNotEqual(int value1, int value2)
    {
        // Arrange
        var sut = new Percentage(value1);

        // Act
        var result = sut.Equals(value2);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    [TestCase(0, 1)]
    [TestCase(50, 51)]
    [TestCase(100, 55)]
    public void WhenComparingTwoInstancesWithDifferentValueUsingEqualsMethodAndNonNullableObject_AreNotEqual(int value,
        int expectedValue)
    {
        // Arrange
        var sut1 = new Percentage(value);
        var sut2 = new Percentage(expectedValue);

        // Act
        var result = sut1.Equals((object)sut2);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    [TestCase(0, 1)]
    [TestCase(50, 51)]
    [TestCase(100, 55)]
    public void WhenComparingTwoInstancesWithDifferentValueUsingNullableObject_AreNotEqual(int value, int expectedValue)
    {
        // Arrange
        var sut1 = new Percentage(value);
        var sut2 = new Percentage(expectedValue);

        // Act
        var result = sut1.Equals((object?)sut2);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    [TestCase(0, 0)]
    [TestCase(50, 50)]
    [TestCase(100, 100)]
    public void WhenComparingTwoInstancesWithSameValueUsingEqualsMethod_AreEqual(int value, int expectedValue)
    {
        // Arrange
        var sut1 = new Percentage(value);
        var sut2 = new Percentage(expectedValue);

        // Act
        var result = sut1.Equals(sut2);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    [TestCase(0, 0)]
    [TestCase(50, 50)]
    [TestCase(100, 100)]
    public void WhenComparingTwoInstancesWithSameValueUsingEqualsMethodAndObject_AreEqual(int value, int expectedValue)
    {
        // Arrange
        var sut1 = new Percentage(value);
        var sut2 = new Percentage(expectedValue);

        // Act
        var result = sut1.Equals((object)sut2);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    [TestCase(0, 0)]
    [TestCase(50, 50)]
    [TestCase(100, 100)]
    public void WhenComparingTwoInstancesWithSameValueUsingNullableObject_AreEqual(int value, int expectedValue)
    {
        // Arrange
        var sut1 = new Percentage(value);
        var sut2 = new Percentage(expectedValue);

        // Act
        var result = sut1.Equals((object?)sut2);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    [TestCase(0, 0)]
    [TestCase(50, 50)]
    [TestCase(100, 100)]
    public void WhenComparingTwoInstancesWithSameValueUsingEqualsOperator_AreEqual(int value, int expectedValue)
    {
        // Arrange
        var sut1 = new Percentage(value);
        var sut2 = new Percentage(expectedValue);

        // Act
        var result = sut1 == sut2;

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    [TestCase(0, 0)]
    [TestCase(50, 50)]
    [TestCase(100, 100)]
    public void WhenComparingTwoInstancesWithSameValueUsingNotEqualsOperator_AreNotEqual(int value, int expectedValue)
    {
        // Arrange
        var sut1 = new Percentage(value);
        var sut2 = new Percentage(expectedValue);

        // Act
        var result = sut1 != sut2;

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    [TestCase(0, 100)]
    [TestCase(5, 50)]
    [TestCase(55, 2)]
    public void WhenPercentageIsCreatedWithDifferentValue_HaveDifferentHashCode(int value1, int value2)
    {
        // Act
        var sut1 = new Percentage(value1);
        var sut2 = new Percentage(value2);

        // Assert
        Assert.That(sut1.GetHashCode(), Is.Not.EqualTo(sut2.GetHashCode()));
    }
    
    [Test]
    [TestCase(0, 0)]
    [TestCase(100, 100)]
    [TestCase(43, 43)]
    public void WhenPercentageIsCreatedWithSameValue_HaveSameHashCode(int value1, int value2)
    {
        // Act
        var sut1 = new Percentage(value1);
        var sut2 = new Percentage(value2);

        // Assert
        Assert.That(sut1.GetHashCode(), Is.EqualTo(sut2.GetHashCode()));
    }
}