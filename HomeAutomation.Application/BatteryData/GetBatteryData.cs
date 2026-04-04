using HomeAutomation.Application.Services.Inverter;
using HomeAutomation.Domain.Battery;
using MediatR;
using Microsoft.Extensions.Options;

namespace HomeAutomation.Application.BatteryData;

public record BatteryDataResult(BatteryInfo BatteryInfo, string DataSource, int SolarInputInW, int HomeUsageInW, int FeedInW);

public class GetBatteryData : IRequest<BatteryDataResult>
{
    internal class GetBatteryDataHandler : IRequestHandler<GetBatteryData, BatteryDataResult>
    {
        private readonly IInverterRealtimeDataReader _inverterRealtimeDataReader;
        private readonly BatteryOptions _batteryOptions;

        public GetBatteryDataHandler(IInverterRealtimeDataReader inverterRealtimeDataReader,
            IOptions<BatteryOptions> batteryOptions)
        {
            _inverterRealtimeDataReader = inverterRealtimeDataReader;
            _batteryOptions = batteryOptions.Value;
        }

        public async Task<BatteryDataResult> Handle(GetBatteryData request, CancellationToken cancellationToken)
        {
            var batteryRealtimeData = await _inverterRealtimeDataReader.GetInverterRealtimeDataAsync(cancellationToken);

            var batteryInfo = new BatteryInfo((int)batteryRealtimeData.BatteryPowerUsage,
                (int)batteryRealtimeData.BatteryPercentage,
                _batteryOptions.CapacityInWh);

            return new BatteryDataResult(batteryInfo, batteryRealtimeData.Source, (int)batteryRealtimeData.SolarInput, (int)batteryRealtimeData.HomeUsage, (int)batteryRealtimeData.FeedIn);
        }
    }
}