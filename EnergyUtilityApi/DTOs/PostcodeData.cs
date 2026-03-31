public record PostcodeData
{
    public required string Postcode { get; set; }
    public decimal? MeanCons { get; set; }
    public decimal? MedianCons { get; set; }
    public char? RegionCode { get; set; }
    public string? Region { get; set; }
    public string? Operator { get; set; }
};