using System.ComponentModel.DataAnnotations;

public record GetConsumptionRequest
{
    [Required]
    [StringLength(8)]
    [MinLength(6)]
    public string Postcode { get; set; }
    [Range(1, 7)]
    public string PropertyType { get; set; }
    [Range(1, 8)]
    public string PropertyAge { get; set; }
    [Range(1, 5)]
    public string FloorArea { get; set; }
    [Range(1, 3)]
    public string HouseholdSize { get; set; }
    [Range(1, 5)]
    public string NumberOfAdults { get; set; }
    [Range(1, 5)]
    public string NumberOfBedrooms { get; set; }
}