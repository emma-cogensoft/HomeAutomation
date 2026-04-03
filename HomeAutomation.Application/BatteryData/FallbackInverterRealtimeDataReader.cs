using HomeAutomation.Application.Services.Inverter;
using HomeAutomation.Domain.Inverter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HomeAutomation.Application.BatteryData;

/// <summary>
/// Tries the local inverter first; falls back to the cloud inverter if unreachable.
/// </summary>
public class FallbackInverterRealtimeDataReader : IInverterRealtimeDataReader
{
    private readonly IInverterRealtimeDataReader _localReader;
    private readonly IInverterRealtimeDataReader _cloudReader;
    private readonly ILogger<FallbackInverterRealtimeDataReader> _logger;

    public FallbackInverterRealtimeDataReader(
        [FromKeyedServices("local")] IInverterRealtimeDataReader localReader,
        [FromKeyedServices("cloud")] IInverterRealtimeDataReader cloudReader,
        ILogger<FallbackInverterRealtimeDataReader> logger)
    {
        _localReader = localReader;
        _cloudReader = cloudReader;
        _logger = logger;
    }

    public async Task<InverterRealtimeData> GetInverterRealtimeDataAsync(CancellationToken cancellationToken)
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
