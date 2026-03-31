using Microsoft.EntityFrameworkCore;
using EnergyUtilityApi;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Numerics;

public class EnergyUtilityService
{
    readonly EnergyUtilityDbContext _context;
    public EnergyUtilityService(EnergyUtilityDbContext context)
    {
        _context = context;
    }
    public async Task<PostcodeData> GetPostcodeData(string postcode)
    {
        ElectricityConsumptionStatistics postcodeStats = await GetElectricityConsumptionStatistics(postcode);
        DNORegionData regionData = await GetDNORegionData(postcode);
        return new PostcodeData
        {
            postcode = postcode,
            MeanCons = postcodeStats.MeanCons,
            MedianCons = postcodeStats.MedianCons,
            RegionCode = regionData.RegionCode,
            Region = regionData.Region,
            Operator = regionData.Operator
        };
    }
    public async Task<DNORegionData> GetDNORegionData(string postcode)
    {
        DNORegionData result = await _context.AllPostcodeDnos
        .Where(a => a.Postcode.Replace(" ", "") == postcode)
        .Join(_context.Dnos,
        a => a.DnoId,
        dno => dno.Id,
        (a, dno) => new DNORegionData
        {
            Postcode = postcode,
            RegionCode = dno.RegionCode,
            Region = dno.Region,
            Operator = dno.Operator
        })
        .SingleOrDefaultAsync();

        if (result == null)
        {
            string shortPostcode = Regex.Replace(postcode, "[^0-9a-zA-Z]+", "")[..3];
            result = await _context.AllPostcodeDnos
            .Where(a => a.Postcode.Replace(" ", "").StartsWith(shortPostcode))
            .Join(_context.Dnos,
            a => a.DnoId,
            dno => dno.Id,
            (a, dno) => new DNORegionData
            {
                Postcode = shortPostcode,
                RegionCode = dno.RegionCode,
                Region = dno.Region,
                Operator = dno.Operator
            })
            .FirstOrDefaultAsync();
        }
        return result;
    }
    public async Task<decimal> GetElectricityConsumption(GetConsumptionRequest req)
    {
        ElectricityConsumptionStatistics postcodeStats = await GetElectricityConsumptionStatistics(req.Postcode);
        decimal result = postcodeStats.MedianCons ?? 2700;
        int regionId = await GetNeedRegionId(req.Postcode);
        Dictionary<string, decimal?> multiplierDictionary = await GetHouseholdFeatureMultipliers(regionId);

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
    private async Task<ElectricityConsumptionStatistics> GetElectricityConsumptionStatistics(string postcode)
    {
        ElectricityConsumptionStatistics result = await _context.ElecConsPostcodes
        .Where(x => x.Postcode.Replace(" ", "") == postcode)
        .Select(x => new ElectricityConsumptionStatistics { MeanCons = x.MeanCons, MedianCons = x.MedianCons })
        .SingleOrDefaultAsync();

        if (result == null)
        {
            // get the first three characters from the postcode
            string shortPostcode = postcode.Replace(" ", "")[..3];
            result = await _context.ElecConsPostcodes
            .Where(x => x.Postcode == shortPostcode)
            .Select(x => new ElectricityConsumptionStatistics { MedianCons = x.MedianCons, MeanCons = x.MeanCons })
            .FirstOrDefaultAsync();

            Console.WriteLine($"No match in DB for {postcode} using {shortPostcode}");
        }
        return result;
    }
    private async Task<int> GetNeedRegionId(string postcode)
    {
        int result = await _context.AllPostcodeDnos
        .Where(d => d.Postcode.Replace(" ", "") == postcode)
        .Join(_context.DnoNeedRegions,
        d => d.DnoId,
        dnr => dnr.DnoId,
        (d, dnr) => dnr.NeedRegionSourceId)
        .SingleOrDefaultAsync();

        if (result == 0)
        {
            // get the first three characters from the postcode
            string shortPostcode = postcode.Replace(" ", "")[..3];
            result = await _context.AllPostcodeDnos
            .Where(d => d.Postcode.StartsWith(shortPostcode))
            .Join(_context.DnoNeedRegions,
            d => d.DnoId,
            dnr => dnr.DnoId,
            (d, dnr) => dnr.NeedRegionSourceId)
            .FirstOrDefaultAsync();

            Console.WriteLine($"No match in DB for {postcode} using {shortPostcode}");
        }
        Console.WriteLine(result);
        return result;
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