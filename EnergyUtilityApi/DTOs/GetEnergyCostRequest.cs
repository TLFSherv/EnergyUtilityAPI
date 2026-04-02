public record GetEnergyCostRequest
{
    public string? Postcode { get; set; }
    public int? PaymentMethodId { get; set; }
    public int? MeterTypeId { get; set; }
    public int? PropertyType { get; set; }
    public int? PropertyAge { get; set; }
    public int? FloorArea { get; set; }
    public int? HouseholdSize { get; set; }
    public int? NumberOfAdults { get; set; }
    public int? NumberOfBedrooms { get; set; }

}