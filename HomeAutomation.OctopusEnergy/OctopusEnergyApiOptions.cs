using System.ComponentModel.DataAnnotations;

namespace HomeAutomation.OctopusEnergy;

public class OctopusEnergyApiOptions
{
    [Required]
    public required string Tariff { get; init; }

    [Required]
    public required string RegionCode { get; init; }

    public int HoursAhead { get; init; } = 24;
}
