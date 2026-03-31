public record ElectricityConsumptionStatistics
{
    public required string Postcode { get; set; }
    public decimal? MeanCons { get; set; }
    public decimal? MedianCons { get; set; }
};