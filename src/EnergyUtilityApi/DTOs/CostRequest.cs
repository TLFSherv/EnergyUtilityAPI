public record CostRequest
{
    public required string Postcode { get; set; }
    public required decimal EnergyConsumption { get; set; }
    public required int PaymentMethodId { get; set; }
    public required int MeterTypeId { get; set; }
}