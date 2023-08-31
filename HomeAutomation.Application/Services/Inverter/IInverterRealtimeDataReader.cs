using HomeAutomation.Domain.Inverter;

namespace HomeAutomation.Application.Services.Inverter;

public interface IInverterRealtimeDataReader
{
    Task<InverterRealtimeData> GetInverterRealtimeDataAsync(CancellationToken cancellationToken);
}