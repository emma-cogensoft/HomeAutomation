using HomeAutomation.Application.Services.Inverter;
using HomeAutomation.Domain.Inverter;
using MediatR;

namespace HomeAutomation.Application.InverterSettings;

public class GetInverterSettings : IRequest<CurrentInverterSettings>
{
    internal class GetInverterSettingsHandler : IRequestHandler<GetInverterSettings, CurrentInverterSettings>
    {
        private readonly IInverterSettingsDataReader _inverterSettingsDataReader;

        public GetInverterSettingsHandler(IInverterSettingsDataReader inverterSettingsDataReader)
        {
            _inverterSettingsDataReader = inverterSettingsDataReader;
        }

        public async Task<CurrentInverterSettings> Handle(GetInverterSettings request, CancellationToken cancellationToken)
        {
            var settings = await _inverterSettingsDataReader.GetCurrentSettingsAsync(cancellationToken);

            return settings;
        }
    }
}