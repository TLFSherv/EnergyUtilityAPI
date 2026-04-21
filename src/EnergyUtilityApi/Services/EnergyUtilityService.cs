using Microsoft.EntityFrameworkCore;
using EnergyUtilityApi;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using System.Xml;

public class EnergyUtilityService
{
    private int _regionIdScotland { get; }
    private readonly EnergyUtilityDbContext _context;
    private readonly ILogger<EnergyUtilityService> _logger;
    public EnergyUtilityService(EnergyUtilityDbContext context, ILogger<EnergyUtilityService> logger, IOptions<EnergyUtilityServiceSettings> options)
    {
        _context = context;
        _logger = logger;
        _regionIdScotland = options.Value.ScotlandRegionId;
    }
    public async Task<DataOutput> GetData(DataRequest req)
    {
        EnergyMeterData meterData = await GetPostcodeMeterData(req.Postcode);
        EnergyRegionData regionData = await GetDNORegionData(req.Postcode);

        ConsumptionRequest consumptionRequest = new ConsumptionRequest
        {
            Postcode = req.Postcode,
            MedianConsumption = meterData.MedianCons ?? 2700,
            PropertyType = req.PropertyType,
            PropertyAge = req.PropertyAge,
            FloorArea = req.FloorArea,
            HouseholdSize = req.HouseholdSize,
            NumberOfAdults = req.NumberOfAdults,
            NumberOfBedrooms = req.NumberOfBedrooms
        };
        decimal energyConsumption = await GetEnergyConsumption(consumptionRequest);

        CostRequest costRequest = new CostRequest
        {
            Postcode = req.Postcode,
            EnergyConsumption = energyConsumption,
            PaymentMethodId = req.PaymentMethodId,
            MeterTypeId = req.MeterTypeId
        };
        decimal energyCost = await GetEnergyCost(costRequest);

        return new DataOutput
        {
            Postcode = req.Postcode,
            EnergyConsumption = energyConsumption,
            EnergyCost = energyCost,
            MeterData = meterData,
            RegionData = regionData
        };
    }

    private async Task<decimal> GetEnergyCost(CostRequest req)
    {
        try
        {
            decimal? energyCost = 0;
            // calculate the energy cost
            if (req.PaymentMethodId != null && req.MeterTypeId != null)
            {
                _logger.LogDebug("Fetching price cap and rate data from database for postcode: {Postcode}", req.Postcode);
                var result = await _context.AllPostcodeDnos
                .Where(a => a.Postcode.Replace(" ", "") == req.Postcode)
                .Join(_context.DnoPriceCapRates,
                a => a.DnoId,
                d => d.DnoId,
                (a, d) => new
                {
                    AnnualStandingCharge = d.AnnualStandingCharge,
                    UnitRatePence = d.UnitRatePence,
                    PaymentMethodId = d.PaymentMethodId,
                    MeterTypeId = d.MeterTypeId
                })
                .Where(x => x.PaymentMethodId == req.PaymentMethodId
                && x.MeterTypeId == req.MeterTypeId)
                .SingleOrDefaultAsync();
                // use consumption and db values to calculate the cost
                energyCost = req.EnergyConsumption * result.UnitRatePence + 365 * result.AnnualStandingCharge;
                _logger.LogInformation("Successfully calculated energy cost for postcode: {Postcode}", req.Postcode);
            }
            else _logger.LogWarning("PaymentMethodId or MeterTypeId was not provided, energy consumption successfully calculated");

            return energyCost ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while calculating the energy cost for postcode: {Postcode}", req.Postcode);
            throw;
        }
    }

    private async Task<decimal> GetEnergyConsumption(ConsumptionRequest req)
    {
        try
        {
            decimal energyConsumption = req.MedianConsumption;
            int regionId = await GetNeedRegionId(req.Postcode);
            Dictionary<string, decimal?> multiplierDictionary = await GetHouseholdFeatureMultipliers(regionId);

            // Get the type of the object
            Type type = req.GetType();

            // Get all public properties of the object's type
            PropertyInfo[] properties = type.GetProperties();
            // Get property name and values from the record (GetEnergyConsumptionRequest) where value is not null
            foreach (PropertyInfo property in properties)
            {
                // get the name and value of each property
                string name = property.Name;
                object value = property.GetValue(req);

                if (value is int val && val != 0)
                {
                    string key = $"{name}-{value}";
                    if (multiplierDictionary.TryGetValue(key, out decimal? foundMultiplier))
                    {
                        decimal m = foundMultiplier ?? 1;
                        energyConsumption *= m;
                    }
                    else
                    {
                        _logger.LogWarning("No match in DB for {Key} (using 1.0)", key);
                        energyConsumption *= 1;
                    }
                }
            }
            _logger.LogInformation("Successfully calculated the energy consumption for postcode: {Postcode}", req.Postcode);

            return energyConsumption;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating the energy consumption for postcode: {Postcode}", req.Postcode);
            throw;
        }
    }

    private async Task<EnergyRegionData> GetDNORegionData(string postcode)
    {
        try
        {
            _logger.LogDebug("Fetching DNO region data for postcode: {Postcode}", postcode);
            EnergyRegionData result = await _context.AllPostcodeDnos
            .Where(a => a.Postcode.Replace(" ", "") == postcode)
            .Join(_context.Dnos,
            a => a.DnoId,
            dno => dno.Id,
            (a, dno) => new EnergyRegionData
            {
                RegionCode = dno.RegionCode,
                Region = dno.Region,
                Operator = dno.Operator
            })
            .SingleOrDefaultAsync();

            if (result == null)
            {
                string shortPostcode = Regex.Replace(postcode, "[^0-9a-zA-Z]+", "")[..4];
                _logger.LogWarning("No match in DB for {Postcode} using {shortPostcode}", postcode, shortPostcode);
                result = await _context.AllPostcodeDnos
                .Where(a => a.Postcode.Replace(" ", "").StartsWith(shortPostcode))
                .Join(_context.Dnos,
                a => a.DnoId,
                dno => dno.Id,
                (a, dno) => new EnergyRegionData
                {
                    RegionCode = dno.RegionCode,
                    Region = dno.Region,
                    Operator = dno.Operator
                })
                .FirstOrDefaultAsync();

                _logger.LogInformation("Successfully fetched DNO region data for postcode: {Postcode}", shortPostcode);
            }
            else _logger.LogInformation("Successfully fetched DNO region data for postcode: {Postcode}", postcode);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching DNO region data from database for postcode: {Postcode}", postcode);
            throw;
        }

    }

    private async Task<EnergyMeterData> GetPostcodeMeterData(string postcode)
    {
        try
        {
            _logger.LogDebug("Fetching consumption statistics for postcode: {Postcode}", postcode);
            EnergyMeterData result = await _context.PostcodeMeters
            .Where(x => x.Postcode.Replace(" ", "") == postcode)
            .Select(x => new EnergyMeterData
            {
                NumOfMeters = x.NumMeters,
                TotalCons = x.TotalCons,
                MeanCons = x.MeanCons,
                MedianCons = x.MedianCons
            })
            .SingleOrDefaultAsync();

            if (result == null)
            {
                // get the first three characters from the postcode
                string shortPostcode = Regex.Replace(postcode, "[^0-9a-zA-Z]+", "")[..4];
                _logger.LogWarning("No match in DB for {Postcode} using {ShortPostcode}", postcode, shortPostcode);
                result = await _context.PostcodeMeters
                .Where(x => x.Postcode.Replace(" ", "") == shortPostcode)
                .Select(x => new EnergyMeterData
                {
                    NumOfMeters = x.NumMeters,
                    TotalCons = x.TotalCons,
                    MedianCons = x.MedianCons,
                    MeanCons = x.MeanCons
                })
                .FirstOrDefaultAsync();
                _logger.LogInformation("Successfully fetched consumption statistics for postcode: {Postcode}", shortPostcode);
            }
            else _logger.LogInformation("Successfully fetched consumption statistics for postcode: {Postcode}", postcode);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving consumption statistics for postcode: {Postcode}", postcode);
            throw;
        }
    }

    private async Task<int> GetNeedRegionId(string postcode)
    {
        try
        {
            _logger.LogDebug("Fetching NEED data from DB for postcode: {Postcode}", postcode);
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
                string shortPostcode = Regex.Replace(postcode, "[^0-9a-zA-Z]+", "")[..4];
                _logger.LogWarning("No match in DB for {Postcode} using {shortPostcode}", postcode, shortPostcode);
                result = await _context.AllPostcodeDnos
                .Where(d => d.Postcode.StartsWith(shortPostcode))
                .Join(_context.DnoNeedRegions,
                d => d.DnoId,
                dnr => dnr.DnoId,
                (d, dnr) => dnr.NeedRegionSourceId)
                .FirstOrDefaultAsync();
                _logger.LogInformation("Successfully fetched NEED data for postcode: {Postcode}", shortPostcode);
            }
            else _logger.LogInformation("Successfully fetched NEED data for postcode: {Postcode}", postcode);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving NEED data for postcode: {Postcode}", postcode);
            throw;
        }
    }

    private async Task<Dictionary<string, decimal?>> GetHouseholdFeatureMultipliers(int regionId)
    {
        try
        {
            _logger.LogDebug("Fetching household feature weights for regionId: {RegionId}", regionId);
            var result = await _context.RegionalWeights
            .Where(rw => rw.RegionId == regionId || (regionId != _regionIdScotland && rw.RegionId == null))
            .Join(_context.WeightCategories,
            rw => rw.CategoryId,
            wc => wc.Id,
            (rw, wc) => new
            {
                Key = $"{wc.CategoryName}-{rw.ValueId}",
                Value = rw.Multiplier
            })
            .ToDictionaryAsync(x => x.Key, x => x.Value);
            _logger.LogInformation("Successfully fetched household feature weights for regionId: {RegionId}", regionId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving household feature weights for regionId: {RegionId}", regionId);
            throw;
        }
    }

}