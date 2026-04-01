using Microsoft.AspNetCore.Mvc;
public class EnergyUtilityController : EnergyUtilityBaseController
{
    private readonly EnergyUtilityService _service;
    public EnergyUtilityController(EnergyUtilityService service)
    {
        _service = service;
    }
    // get yearly energy consumption and cost data
    [HttpGet("cost/")]
    public async Task<IActionResult> GetEnergyCost([FromQuery] GetEnergyCostRequest request)
    {
        try
        {
            SendEnergyCostData result = await _service.GetEnergyCost(request);
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occured while retrieving consumption data");
        }
    }

    // get energy data for postcodes: region, dno, median and mean elec consumption
    [HttpGet]
    public async Task<IActionResult> GetDataByPostcodes([FromQuery(Name = "postcode")] string[] postcodes)
    {
        try
        {
            SendPostcodeData[] results = new SendPostcodeData[postcodes.Length];
            for (int i = 0; i < postcodes.Length; i++)
            {
                results[i] = await _service.GetEnergyDataByPostcode(postcodes[i]);
            }
            return Ok(results);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occured while retrieving recipes");
        }
    }

}