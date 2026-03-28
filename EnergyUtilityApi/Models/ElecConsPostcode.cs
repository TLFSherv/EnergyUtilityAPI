using System;
using System.Collections.Generic;

namespace EnergyUtilityApi;

public partial class ElecConsPostcode
{
    public string Postcode { get; set; } = null!;

    public int? NumMeters { get; set; }

    public decimal? MeanCons { get; set; }

    public decimal? MedianCons { get; set; }
}
