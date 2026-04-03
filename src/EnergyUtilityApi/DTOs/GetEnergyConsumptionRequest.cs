using System.ComponentModel.DataAnnotations;
public record GetEnergyConsumptionRequest
{

    public string? Postcode { get; set; }
    public int? PropertyType { get; set; }
    public int? PropertyAge { get; set; }
    public int? FloorArea { get; set; }
    public int? HouseholdSize { get; set; }
    public int? NumberOfAdults { get; set; }
    public int? NumberOfBedrooms { get; set; }
}