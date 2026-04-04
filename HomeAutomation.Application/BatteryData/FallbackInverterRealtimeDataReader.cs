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
        if (_circuitBreaker.IsOpen)
        {
            _logger.LogWarning("Inverter circuit breaker is open — skipping inverter calls");
            throw new InvalidOperationException("Inverter circuit breaker is open after repeated failures.");
        }

        try
        {
            var result = await TryGetDataAsync(cancellationToken);
            _circuitBreaker.RecordSuccess();
            return result;
        }
        catch (Exception)
        {
            _circuitBreaker.RecordFailure();
            throw;
        }
    }

    private async Task<InverterRealtimeData> TryGetDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _localReader.GetInverterRealtimeDataAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Local inverter unreachable, falling back to cloud API");
            return await _cloudReader.GetInverterRealtimeDataAsync(cancellationToken);
        }
    }
}
