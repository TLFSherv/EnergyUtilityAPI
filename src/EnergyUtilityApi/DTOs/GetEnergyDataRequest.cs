using Microsoft.AspNetCore.Mvc;
public record GetEnergyDataRequest
{
    [FromQuery(Name = "postcode")]
    public string[]? Postcodes { get; set; }
}