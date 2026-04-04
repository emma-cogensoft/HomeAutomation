namespace HomeAutomation.Application.BatteryData;

/// <summary>
/// Tracks consecutive inverter failures across requests. Once the failure threshold
/// is reached the circuit opens and no further calls are attempted until the
/// application restarts.
/// </summary>
public class InverterCircuitBreaker
{
    private readonly int _failureThreshold;
    private volatile int _consecutiveFailures;

    public InverterCircuitBreaker(int failureThreshold = 3)
    {
        _failureThreshold = failureThreshold;
    }

    public bool IsOpen => _consecutiveFailures >= _failureThreshold;

    public void RecordSuccess() => Interlocked.Exchange(ref _consecutiveFailures, 0);

    public void RecordFailure() => Interlocked.Increment(ref _consecutiveFailures);
}
