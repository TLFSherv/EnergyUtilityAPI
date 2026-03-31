public record DNORegionData
{
    public required string Postcode { get; set; }
    public char? RegionCode { get; set; }
    public string? Region { get; set; }
    public string? Operator { get; set; }
};