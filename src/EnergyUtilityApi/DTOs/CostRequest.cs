public record CostRequest
{
    public required string Postcode { get; set; }
    public required decimal EnergyConsumption { get; set; }
    public required int PaymentMethod { get; set; }
    public required int MeterType { get; set; }
}