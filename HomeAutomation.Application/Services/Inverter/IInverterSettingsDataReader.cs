using HomeAutomation.Domain.Inverter;

namespace HomeAutomation.Application.Services.Inverter;

public interface IInverterSettingsDataReader
{
    Task<CurrentInverterSettings> GetCurrentSettingsAsync(CancellationToken cancellationToken);
}