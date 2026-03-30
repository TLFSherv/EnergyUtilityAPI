using Microsoft.EntityFrameworkCore;
using EnergyUtilityApi;
using System.Reflection;
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
        decimal result = medianCons ?? 2700;
        int regionId = await GetNeedRegionId(req.Postcode);
        Dictionary<string, decimal?> multiplierDictionary = await GetHouseholdFeatureMultipliers(regionId);

        var keys = multiplierDictionary.Keys;
        foreach (var key in keys)
        {
            Console.WriteLine(key);
        }
        // Get the type of the object
        Type type = req.GetType();

        // Get all public properties of the object's type
        PropertyInfo[] properties = type.GetProperties();
        foreach (PropertyInfo property in properties)
        {
            // get the name and value of each property
            string name = property.Name;
            object value = property.GetValue(req);
            if (value != null)
            {
                string key = $"{name}-{value}";
                if (multiplierDictionary.TryGetValue(key, out decimal? foundMultiplier))
                {
                    decimal m = foundMultiplier ?? 1;
                    result *= m;
                }
                else
                {
                    Console.WriteLine($"No match in DB for {key} (using 1.0)");
                    result *= 1;
                }
            }

        }
        return result;
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

    private async Task<Dictionary<string, decimal?>> GetHouseholdFeatureMultipliers(int regionId)
    {
        return await _context.RegionalWeights
        .Where(rw => rw.RegionId == regionId)
        .Join(_context.WeightCategories,
        rw => rw.CategoryId,
        wc => wc.Id,
        (rw, wc) => new
        {
            Key = $"{wc.CategoryName}-{rw.ValueId}",
            Value = rw.Multiplier
        })
        .ToDictionaryAsync(x => x.Key, x => x.Value);
    }

}