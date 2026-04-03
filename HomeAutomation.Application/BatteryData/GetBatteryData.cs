using HomeAutomation.Application.Services.Inverter;
using HomeAutomation.Domain.Battery;
using MediatR;

namespace HomeAutomation.Application.BatteryData;

public record BatteryDataResult(BatteryInfo BatteryInfo, string DataSource);

public class GetBatteryData : IRequest<BatteryDataResult>
{
    internal class GetBatteryDataHandler : IRequestHandler<GetBatteryData, BatteryDataResult>
    {
        private readonly IInverterRealtimeDataReader _inverterRealtimeDataReader;

        public GetBatteryDataHandler(IInverterRealtimeDataReader inverterRealtimeDataReader)
        {
            _inverterRealtimeDataReader = inverterRealtimeDataReader;
        }

        public async Task<BatteryDataResult> Handle(GetBatteryData request, CancellationToken cancellationToken)
        {
            var batteryRealtimeData = await _inverterRealtimeDataReader.GetInverterRealtimeDataAsync(cancellationToken);

            var batteryInfo = new BatteryInfo((int)batteryRealtimeData.BatteryPowerUsage,
                (int)batteryRealtimeData.BatteryPercentage);

            return new BatteryDataResult(batteryInfo, batteryRealtimeData.Source);
        }
    }
}