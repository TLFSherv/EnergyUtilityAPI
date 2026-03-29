using System.ComponentModel.DataAnnotations;

public record GetConsumptionRequest
{
    [Required]
    [StringLength(8)]
    [MinLength(6)]
    public string Postcode { get; set; }
    [Range(1, 7)]
    public int? PropertyType { get; set; }
    [Range(1, 8)]
    public int? PropertyAge { get; set; }
    [Range(1, 5)]
    public int? FloorArea { get; set; }
    [Range(1, 3)]
    public int? HouseholdSize { get; set; }
    [Range(1, 5)]
    public int? NumberOfAdults { get; set; }
    [Range(1, 5)]
    public int? NumberOfBedrooms { get; set; }
}