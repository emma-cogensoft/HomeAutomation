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
                batteryInfo.BatteryInfo.BatteryState.PercentageCharged, batteryInfo.DataSource);

            return Ok(new BatteryResponse
            {
                StateDescription = batteryInfo.BatteryInfo.BatteryState.Description,
                ActivityDescription = batteryInfo.BatteryInfo.BatteryActivity.Description,
                TimeToCompleteInH = batteryInfo.BatteryInfo.BatteryActivity.TimeToComplete,
                PercentageCharged = batteryInfo.BatteryInfo.BatteryState.PercentageCharged,
                PercentageUncharged = batteryInfo.BatteryInfo.BatteryState.PercentageUncharged,
                AvailablePercentageCharged = batteryInfo.BatteryInfo.BatteryState.AvailablePercentageCharged,
                TotalBatteryCapacityInWh = batteryInfo.BatteryInfo.BatteryCapacity,
                AvailableChargeInBattery = batteryInfo.BatteryInfo.BatteryState.AvailableChargeInBattery,
                RemainingBatteryCapacityInWh = batteryInfo.BatteryInfo.BatteryState.RemainingBatteryCapacity,
                BatteryPowerUsageInW = batteryInfo.BatteryInfo.BatteryActivity.BatteryPowerUsage,
                TimeStamp = batteryInfo.BatteryInfo.BatteryState.TimeStamp,
                DataSource = batteryInfo.DataSource,
                SolarInputInW = batteryInfo.SolarInputInW,
                HomeUsageInW = batteryInfo.HomeUsageInW,
                FeedInW = batteryInfo.FeedInW
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
        public string DataSource { get; init; } = string.Empty;
        public int SolarInputInW { get; init; }
        public int HomeUsageInW { get; init; }
        public int FeedInW { get; init; }
    }
}