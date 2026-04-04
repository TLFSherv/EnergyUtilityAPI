public record SendPostcodeData
{
    public required string Postcode { get; set; }
    public int? NumOfMeters { get; set; }
    public decimal? MeanCons { get; set; }
    public decimal? MedianCons { get; set; }
    public decimal? TotalCons { get; set; }
    public char? RegionCode { get; set; }
    public string? Region { get; set; }
    public string? Operator { get; set; }

};