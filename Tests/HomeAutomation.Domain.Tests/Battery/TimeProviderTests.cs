namespace HomeAutomation.Domain.Tests.Battery;

[TestOf(nameof(TimeProvider))]
public class TimeProviderTests
{
    [Test]
    public void UtcNow_WhenCalled_ReturnsUtcNow()
    {
        // Arrange
        var expected = DateTime.UtcNow;
        
        // Act
        var actual = TimeProvider.UtcNow;
        
        // Assert
        Assert.That(actual, Is.EqualTo(expected).Within(10).Seconds);
    }
}