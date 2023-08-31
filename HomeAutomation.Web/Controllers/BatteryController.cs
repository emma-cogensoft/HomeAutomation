using HomeAutomation.Application.BatteryData;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeAutomation.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BatteryController : ControllerBase
{
    private readonly ILogger<BatteryController> _logger;
    private readonly IMediator _mediator;

    public BatteryController(ILogger<BatteryController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<BatteryResponse> GetAsync()
    {
        var batteryInfo = await _mediator.Send(new GetBatteryData());
        
        return new BatteryResponse
        {
            StateDescription = batteryInfo.BatteryState.Description,
            ActivityDescription = batteryInfo.BatteryActivity.Description,
            TimeToCompleteInH = batteryInfo.BatteryActivity.TimeToComplete,
            PercentageCharged = batteryInfo.BatteryState.PercentageCharged,
            PercentageUncharged = batteryInfo.BatteryState.PercentageUncharged,
            AvailablePercentageCharged = batteryInfo.BatteryState.AvailablePercentageCharged,
            TotalBatteryCapacityInWh = batteryInfo.BatteryCapacity,
            AvailableChargeInBattery = batteryInfo.BatteryState.AvailableChargeInBattery,
            RemainingBatteryCapacityInWh = batteryInfo.BatteryState.RemainingBatteryCapacity,
            BatteryPowerUsageInW = batteryInfo.BatteryActivity.BatteryPowerUsage,
            TimeStamp = batteryInfo.BatteryState.TimeStamp
        };
    }
    
    public record BatteryResponse
    {
        public string StateDescription { get; init; } = string.Empty;
        public string ActivityDescription { get; init; } = string.Empty;
        public int TimeToCompleteInH { get; init; }
        public int PercentageCharged { get; init; }
        public int PercentageUncharged { get; init; }
        public int AvailablePercentageCharged { get; init; }
        public int TotalBatteryCapacityInWh { get; init; }
        public int AvailableChargeInBattery { get; init; }
        public int RemainingBatteryCapacityInWh { get; init; }
        public int BatteryPowerUsageInW { get; init; }
        public DateTime TimeStamp { get; init; }
    }
}