using System;
using System.Collections.Generic;

namespace EnergyUtilityApi;

public partial class WeightCategory
{
    public int Id { get; set; }

    public string? CategoryName { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<RegionalWeight> RegionalWeights { get; set; } = new List<RegionalWeight>();
}
