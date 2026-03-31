using Microsoft.AspNetCore.Mvc;
public class EnergyUtilityController : EnergyUtilityBaseController
{
    private readonly EnergyUtilityService _service;
    public EnergyUtilityController(EnergyUtilityService service)
    {
        _service = service;
    }

    [HttpGet("consumption/")]
    public async Task<IActionResult> GetEnergyConsumption([FromQuery] GetConsumptionRequest request)
    {
        try
        {
            decimal result = await _service.GetElectricityConsumption(request);
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occured while retrieving consumption data");
        }
    }

    // get data for postcodes: region, dno, median and mean elec consumption
    [HttpGet]
    public async Task<IActionResult> GetDataByPostcodes([FromQuery(Name = "postcode")] string[] postcodes)
    {
        try
        {
            PostcodeData[] results = new PostcodeData[postcodes.Length];
            for (int i = 0; i < postcodes.Length; i++)
            {
                results[i] = await _service.GetPostcodeData(postcodes[i]);
            }
            return Ok(results);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occured while retrieving recipes");
        }
    }

    [HttpGet("cost/")]
    public async Task<IActionResult> GetEnergyCost()
    {
        return Ok();
    }

}