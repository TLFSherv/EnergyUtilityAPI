using System;
using System.Collections.Generic;

namespace EnergyUtilityApi;

public partial class Dno
{
    public int Id { get; set; }

    public char? RegionCode { get; set; }

    public string? Region { get; set; }

    public string? Operator { get; set; }
}
