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
    public async Task<IActionResult> GetAsync()
    {
        try
        {
            var batteryInfo = await _mediator.Send(new GetBatteryData());

            _logger.LogDebug("Battery data retrieved: {PercentageCharged}% charged, source: {Source}",
                batteryInfo.BatteryState.PercentageCharged, batteryInfo.BatteryState.TimeStamp);

            return Ok(new BatteryResponse
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
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve battery data");
            return StatusCode(503, "Battery data is temporarily unavailable.");
        }
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