public record EnergyMeterData
{
    public int? NumOfMeters { get; set; }
    public decimal? MeanCons { get; set; }
    public decimal? MedianCons { get; set; }
    public decimal? TotalCons { get; set; }
}