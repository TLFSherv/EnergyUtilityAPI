public record DataOutput
{
    public required string Postcode { get; set; }
    public decimal EnergyConsumption { get; set; }
    public decimal EnergyCost { get; set; }
    public required EnergyMeterData MeterData { get; set; }
    public required EnergyRegionData RegionData { get; set; }
};