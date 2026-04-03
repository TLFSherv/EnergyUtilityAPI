using System;
using System.Collections.Generic;

namespace EnergyUtilityApi;

public partial class AllPostcodeDno
{
    public string Postcode { get; set; } = null!;

    public int? DnoId { get; set; }
}
