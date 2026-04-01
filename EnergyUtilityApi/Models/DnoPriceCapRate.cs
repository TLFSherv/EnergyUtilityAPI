using System;
using System.Collections.Generic;

namespace EnergyUtilityApi;

public partial class DnoPriceCapRate
{
    public int Id { get; set; }

    public string? PaymentMethod { get; set; }

    public int? PaymentMethodId { get; set; }

    public string? MeterType { get; set; }

    public int? MeterTypeId { get; set; }

    public decimal? AnnualStandingCharge { get; set; }

    public decimal? UnitRatePence { get; set; }

    public int? DnoId { get; set; }
}
