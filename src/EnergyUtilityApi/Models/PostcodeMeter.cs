using System;
using System.Collections.Generic;

namespace EnergyUtilityApi;

public partial class PostcodeMeter
{
    public string Postcode { get; set; } = null!;

    public int? NumMeters { get; set; }

    public decimal? TotalCons { get; set; }

    public decimal? MeanCons { get; set; }

    public decimal? MedianCons { get; set; }
}
