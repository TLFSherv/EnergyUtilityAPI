using System.Diagnostics.CodeAnalysis;
public record ConsumptionRequest
{
    public required string Postcode { get; set; }
    public decimal MedianConsumption { get; set; } = 2700;
    public int? PropertyType { get; set; }
    public int? PropertyAge { get; set; }
    public int? FloorArea { get; set; }
    public int? HouseholdSize { get; set; }
    public int? NumberOfAdults { get; set; }
    public int? NumberOfBedrooms { get; set; }
}