using Microsoft.AspNetCore.Mvc;
public class EnergyUtilityController : EnergyUtilityBaseController
{
    private readonly EnergyUtilityService _service;
    public EnergyUtilityController(EnergyUtilityService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetEnergyConsumption([FromQuery] GetConsumptionRequest request)
    {
        try
        {
            //decimal result = await _service.GetElectricityConsumption(request);
            return Ok(request);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occured while retrieving consumption data");
        }
    }

    // get data for postcodes: region, dno, median and mean elec consumption
    // [HttpGet]
    // public async Task<IActionResult> GetDataByPostcode([FromQuery] string postcode)
    // {
    //     try
    //     {
    //         // var result = await _service.GetMedianElectricityCosumption(postcode);
    //         var result = await _service.GetNeedRegionId(postcode);
    //         return Ok(result);
    //     }
    //     catch (Exception)
    //     {
    //         return StatusCode(500, "An error occured while retrieving recipes");
    //     }
    // }
    // [HttpGet]
    // public IActionResult GetDataByPostcodes([FromQuery(Name = "postcode")] string[] postcodes)
    // {
    //     return Ok();
    // }


}