using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
public record DataInput
{
    [FromQuery(Name = "postcode")]
    public string[]? Postcodes { get; set; }
    public int? PropertyType { get; set; }
    public int? PropertyAge { get; set; }
    public int? FloorArea { get; set; }
    public int? HouseholdSize { get; set; }
    public int? NumberOfAdults { get; set; }
    public int? NumberOfBedrooms { get; set; }
    public int PaymentMethod { get; set; } = 1;
    public int MeterType { get; set; } = 1;

}