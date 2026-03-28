using System;
using System.Collections.Generic;

namespace EnergyUtilityApi;

public partial class RegionalWeight
{
    public int Id { get; set; }

    public int? CategoryId { get; set; }

    public string? Region { get; set; }

    public string? Value { get; set; }

    public decimal? Multiplier { get; set; }

    public int? RegionId { get; set; }

    public int? ValueId { get; set; }

    public virtual WeightCategory? Category { get; set; }
}
