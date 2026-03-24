public record GetConsumptionRequest
{
    public required string Postcode { get; set; }
    public string PropertyType { get; set; }
    public string PropertyAge { get; set; }
    public string FloorArea { get; set; }
    public string HouseholdSize { get; set; }
    public string NumberOfAdults { get; set; }
    public string NumberOfRooms { get; set; }
}