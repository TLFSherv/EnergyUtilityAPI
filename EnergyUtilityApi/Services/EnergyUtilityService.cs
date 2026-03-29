using Microsoft.EntityFrameworkCore;
using EnergyUtilityApi;
public class EnergyUtilityService
{
    readonly EnergyUtilityDbContext _context;
    public EnergyUtilityService(EnergyUtilityDbContext context)
    {
        _context = context;
    }

    public async Task<decimal?> GetMedianElectricityCosumption(string postcode)
    {
        return await _context.ElecConsPostcodes
        .Where(x => x.Postcode == postcode)
        .Select(x => x.MedianCons)
        .SingleOrDefaultAsync();
    }

}