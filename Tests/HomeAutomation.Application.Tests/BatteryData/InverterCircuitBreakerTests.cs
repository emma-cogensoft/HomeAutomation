using HomeAutomation.Application.BatteryData;

namespace HomeAutomation.Application.Tests.BatteryData;

[TestOf(nameof(InverterCircuitBreaker))]
public class InverterCircuitBreakerTests
{
    [Test]
    public void IsOpen_WhenNew_IsFalse()
    {
        var breaker = new InverterCircuitBreaker();

        Assert.That(breaker.IsOpen, Is.False);
    }

    [Test]
    [TestCase(1)]
    [TestCase(2)]
    public void IsOpen_BelowThreshold_IsFalse(int failures)
    {
        var breaker = new InverterCircuitBreaker(failureThreshold: 3);

        for (var i = 0; i < failures; i++)
            breaker.RecordFailure();

        Assert.That(breaker.IsOpen, Is.False);
    }

    [Test]
    public void IsOpen_AtThreshold_IsTrue()
    {
        var breaker = new InverterCircuitBreaker(failureThreshold: 3);

        breaker.RecordFailure();
        breaker.RecordFailure();
        breaker.RecordFailure();

        Assert.That(breaker.IsOpen, Is.True);
    }

    [Test]
    public void IsOpen_AboveThreshold_RemainsTrue()
    {
        var breaker = new InverterCircuitBreaker(failureThreshold: 3);

        breaker.RecordFailure();
        breaker.RecordFailure();
        breaker.RecordFailure();
        breaker.RecordFailure();

        Assert.That(breaker.IsOpen, Is.True);
    }

    [Test]
    public void RecordSuccess_AfterFailures_ClosesCircuit()
    {
        var breaker = new InverterCircuitBreaker(failureThreshold: 3);

        breaker.RecordFailure();
        breaker.RecordFailure();
        breaker.RecordSuccess();

        Assert.That(breaker.IsOpen, Is.False);
    }

    [Test]
    public void RecordSuccess_ResetsFailureCount_SoThresholdMustBeReachedAgain()
    {
        var breaker = new InverterCircuitBreaker(failureThreshold: 3);

        breaker.RecordFailure();
        breaker.RecordFailure();
        breaker.RecordSuccess();

        // Two more failures after reset should not open the circuit
        breaker.RecordFailure();
        breaker.RecordFailure();

        Assert.That(breaker.IsOpen, Is.False);
    }
}
