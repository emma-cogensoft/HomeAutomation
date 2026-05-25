using HomeAutomation.Application.Services.Inverter;
using HomeAutomation.Domain.Inverter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HomeAutomation.Application.BatteryData;

/// <summary>
/// Tries the local inverter first; falls back to the cloud inverter if unreachable.
/// Once both readers have failed consecutively beyond the circuit breaker threshold,
/// no further network calls are made until the application restarts.
/// </summary>
public class FallbackInverterRealtimeDataReader : IInverterRealtimeDataReader
{
    private readonly IInverterRealtimeDataReader _localReader;
    private readonly IInverterRealtimeDataReader _cloudReader;
    private readonly InverterCircuitBreaker _circuitBreaker;
    private readonly ILogger<FallbackInverterRealtimeDataReader> _logger;

    public FallbackInverterRealtimeDataReader(
        [FromKeyedServices("local")] IInverterRealtimeDataReader localReader,
        [FromKeyedServices("cloud")] IInverterRealtimeDataReader cloudReader,
        InverterCircuitBreaker circuitBreaker,
        ILogger<FallbackInverterRealtimeDataReader> logger)
    {
        _localReader = localReader;
        _cloudReader = cloudReader;
        _circuitBreaker = circuitBreaker;
        _logger = logger;
    }

    public async Task<InverterRealtimeData> GetInverterRealtimeDataAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetInverterRealtimeDataAsync called");

        if (_circuitBreaker.IsOpen)
        {
            _logger.LogWarning("Inverter circuit breaker is open — skipping inverter calls");
            throw new InvalidOperationException("Inverter circuit breaker is open after repeated failures.");
        }

        try
        {
            var result = await TryGetDataAsync(cancellationToken);
            _circuitBreaker.RecordSuccess();
            _logger.LogInformation("Inverter data retrieved successfully");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Both local and cloud inverter readers failed");
            _circuitBreaker.RecordFailure();
            throw;
        }
    }

    private async Task<InverterRealtimeData> TryGetDataAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Attempting to get inverter data from local reader");
        try
        {
            var result = await _localReader.GetInverterRealtimeDataAsync(cancellationToken);
            _logger.LogInformation("✓ Local inverter succeeded");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "✗ Local inverter failed ({ExceptionType}), attempting cloud inverter fallback", ex.GetType().Name);
            try
            {
                var result = await _cloudReader.GetInverterRealtimeDataAsync(cancellationToken);
                _logger.LogInformation("✓ Cloud inverter fallback succeeded");
                return result;
            }
            catch (Exception cloudEx)
            {
                _logger.LogError(cloudEx, "✗ Cloud inverter fallback also failed ({ExceptionType})", cloudEx.GetType().Name);
                throw;
            }
        }
    }
}
