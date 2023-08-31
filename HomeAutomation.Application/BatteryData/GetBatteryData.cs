using HomeAutomation.Application.Services.Inverter;
using HomeAutomation.Domain.Battery;
using MediatR;

namespace HomeAutomation.Application.BatteryData;

public class GetBatteryData : IRequest<BatteryInfo>
{
    internal class GetBatteryDataHandler : IRequestHandler<GetBatteryData, BatteryInfo>
    {
        private readonly IInverterRealtimeDataReader _inverterRealtimeDataReader;

        public GetBatteryDataHandler(IInverterRealtimeDataReader inverterRealtimeDataReader)
        {
            _inverterRealtimeDataReader = inverterRealtimeDataReader;
        }

        public async Task<BatteryInfo> Handle(GetBatteryData request, CancellationToken cancellationToken)
        {
            var batteryRealtimeData = await _inverterRealtimeDataReader.GetInverterRealtimeDataAsync(cancellationToken);

            var batteryInfo = new BatteryInfo((int)batteryRealtimeData.BatteryPowerUsage,
                (int)batteryRealtimeData.BatteryPercentage);

            return batteryInfo;
        }
    }
}