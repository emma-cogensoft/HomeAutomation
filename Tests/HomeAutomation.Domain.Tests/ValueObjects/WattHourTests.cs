using HomeAutomation.Domain.ValueObjects;

namespace HomeAutomation.Domain.Tests.ValueObjects;

[TestOf(nameof(WattHours))]
public class WattHourTests
{
    [Test]
    [TestCase(0)]
    [TestCase(100)]
    [TestCase(10_000)]
    public void WhenWattHourIsCreated_HasExpectedValues(int value)
    {
        // Act
        var sut = new WattHours(value);
        
        // Assert
        Assert.That(sut.Value, Is.EqualTo(value));
    }
    
    [Test]
    [TestCase(0)]
    [TestCase(100)]
    [TestCase(10_000)]
    public void Equals_CompareWattHourToInt_ReturnsFalse(int value)
    {
        // Arrange
        var sut = new WattHours(value);
        
        // Act
        var result = sut.Equals(value);
        
        // Assert
        Assert.That(result, Is.False);
    }
     
    [Test]
    [TestCase(0)]
    [TestCase(100)]
    [TestCase(10_000)]
    public void Equals_CompareIntToWattHour_ReturnsTrue(int value)
    {
        // Arrange
        var sut = new WattHours(value);
        
        // Act
        var result = value.Equals(sut);
        
        // Assert
        Assert.That(result, Is.True);
    }
    
    [Test]
    [TestCase(0)]
    [TestCase(100)]
    [TestCase(10_000)]
    public void ToString_IsExpected(int value)
    {
        // Act
        var sut = new WattHours(value);
        
        // Assert
        Assert.That(sut.ToString(), Is.EqualTo($"{value}Wh"));
    }
    
    [Test]
    [TestCase(0)]
    [TestCase(100)]
    [TestCase(10_000)]
    public void WhenCreatingWattHourFromInt_WithImplicitConversion_CreatesWattHour(int value)
    {
        // Act
        WattHours sut = value;
        
        // Assert
        Assert.That(sut.Value, Is.EqualTo(value));
    }
    
    [Test]
    [TestCase(0)]
    [TestCase(100)]
    [TestCase(10_000)]
    public void WhenCreatingWattHourFromInt_CreatesWattHour(int value)
    {
        // Act
        int sut = new WattHours(value);
        
        // Assert
        Assert.That(sut, Is.EqualTo(value));
    }
    
    [Test]
    [TestCase(-100)]
    [TestCase(-10_000)]
    public void WhenWattHourIsCreatedWithNegativeValue_ThrowsArgumentOutOfRangeException(int value)
    {
        // Act
        TestDelegate sut = () => new WattHours(value);
        
        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(sut);
    }
    
    [Test]
    [TestCase(0, 0)]
    [TestCase(100, 100)]
    [TestCase(10_000, 10_000)]
    public void WhenTwoWattHoursIsCreatedWithSameValue_AreEqual(int value1, int value2)
    {
        // Act
        var sut1 = new WattHours(value1);
        var sut2 = new WattHours(value2);
        
        // Assert
        Assert.That(sut1, Is.EqualTo(sut2));
    }
    
    [Test]
    [TestCase(0, 100)]
    [TestCase(100, 10_000)]
    [TestCase(10_000, 0)]
    public void WhenTwoWattHourIsCreatedWithDifferentValue_AreNotEqual(int value1, int value2)
    {
        // Act
        var sut1 = new WattHours(value1);
        var sut2 = new WattHours(value2);
        
        // Assert
        Assert.That(sut1, Is.Not.EqualTo(sut2));
    }
    
    [Test]
    [TestCase(0, 0)]
    [TestCase(100, 100)]
    [TestCase(10_000, 10_000)]
    public void WhenWattHourIsCreatedWithSameValue_HaveSameHashCode(int value1, int value2)
    {
        // Act
        var sut1 = new WattHours(value1);
        var sut2 = new WattHours(value2);
        
        // Assert
        Assert.That(sut1.GetHashCode(), Is.EqualTo(sut2.GetHashCode()));
    }
    
    [Test]
    [TestCase(0, 100)]
    [TestCase(100, 10_000)]
    [TestCase(10_000, 0)]
    public void WhenWattHourIsCreatedWithDifferentValue_HaveDifferentHashCode(int value1, int value2)
    {
        // Act
        var sut1 = new WattHours(value1);
        var sut2 = new WattHours(value2);
        
        // Assert
        Assert.That(sut1.GetHashCode(), Is.Not.EqualTo(sut2.GetHashCode()));
    }
    
    [Test]
    [TestCase(0, 0)]
    [TestCase(100, 100)]
    [TestCase(10_000, 10_000)]
    public void WhenTwoWattHourIsCreatedWithSameValue_EqualsOperator_ReturnsEqualsTrue(int value1, int value2)
    {
        // Act
        var sut1 = new WattHours(value1);
        var sut2 = new WattHours(value2);
        
        // Assert
        Assert.That(sut1 == sut2, Is.True);
    }
    
    [Test]
    [TestCase(0, 100)]
    [TestCase(100, 10_000)]
    [TestCase(10_000, 0)]
    public void WhenWattHourIsCreatedWithDifferentValue_EqualsOperator_ReturnsEqualsFalse(int value1, int value2)
    {
        // Act
        var sut1 = new WattHours(value1);
        var sut2 = new WattHours(value2);
        
        // Assert
        Assert.That(sut1 == sut2, Is.False);
    }
    
    [Test]
    [TestCase(0, 0)]
    [TestCase(100, 100)]
    [TestCase(10_000, 10_000)]
    public void WhenWattHourIsCreatedWithSameValue_NotEqualsOperator_ReturnsFalse(int value1, int value2)
    {
        // Act
        var sut1 = new WattHours(value1);
        var sut2 = new WattHours(value2);
        
        // Assert
        Assert.That(sut1 != sut2, Is.False);
    }
    
    [Test]
    [TestCase(0, 100)]
    [TestCase(100, 10_000)]
    [TestCase(10_000, 0)]
    public void WhenWattHourIsCreatedWithDifferentValue_NotEqualsOperator_ReturnsTrue(int value1, int value2)
    {
        // Act
        var sut1 = new WattHours(value1);
        var sut2 = new WattHours(value2);
        
        // Assert
        Assert.That(sut1 != sut2, Is.True);
    }
}