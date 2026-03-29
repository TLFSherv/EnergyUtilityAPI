using Microsoft.EntityFrameworkCore;
using EnergyUtilityApi;
public class EnergyUtilityService
{
    readonly EnergyUtilityDbContext _context;
    public record HouseholdFeatureMultipliers(string CategoryName, int? ValueId, decimal? Multiplier);
    public EnergyUtilityService(EnergyUtilityDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> GetElectricityConsumption(GetConsumptionRequest req)
    {
        decimal? medianCons = await GetMedianElectricityCosumption(req.Postcode);
        int regionId = await GetNeedRegionId(req.Postcode);
        var multipliers = await GetHouseholdFeatureMultipliers(regionId);
        return 0;
    }

    private async Task<decimal?> GetMedianElectricityCosumption(string postcode)
    {
        return await _context.ElecConsPostcodes
        .Where(x => x.Postcode == postcode)
        .Select(x => x.MedianCons)
        .SingleOrDefaultAsync();
    }

    private async Task<int> GetNeedRegionId(string postcode)
    {
        return await _context.AllPostcodeDnos
        .Where(d => d.Postcode == postcode)
        .Join(_context.DnoNeedRegions,
        d => d.DnoId,
        dnr => dnr.DnoId,
        (d, dnr) => dnr.NeedRegionSourceId)
        .SingleOrDefaultAsync();
    }

    private async Task<IEnumerable<HouseholdFeatureMultipliers>> GetHouseholdFeatureMultipliers(int regionId)
    {
        return await _context.RegionalWeights
        .Where(rw => rw.RegionId == regionId)
        .Join(_context.WeightCategories,
        rw => rw.CategoryId,
        wc => wc.Id,
        (rw, wc) => new HouseholdFeatureMultipliers(wc.CategoryName, rw.ValueId, rw.Multiplier))
        .ToListAsync();
    }

}