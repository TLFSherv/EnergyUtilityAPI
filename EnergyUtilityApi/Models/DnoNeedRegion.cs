using System;
using System.Collections.Generic;

namespace EnergyUtilityApi;

public partial class DnoNeedRegion
{
    public int DnoId { get; set; }

    public int NeedRegionSourceId { get; set; }

    public virtual Dno Dno { get; set; } = null!;
}
