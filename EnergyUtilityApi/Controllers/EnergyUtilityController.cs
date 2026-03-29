using Microsoft.AspNetCore.Mvc;
public class EnergyUtilityController : EnergyUtilityBaseController
{
    private readonly EnergyUtilityService _service;
    public EnergyUtilityController(EnergyUtilityService service)
    {
        _service = service;
    }

    // [HttpGet]
    // public IActionResult GetEnergyConsumption([AsParameters] GetConsumptionRequest request)
    // {
    //     return Ok();
    // }

    // get data for postcodes: region, dno, median and mean elec consumption
    [HttpGet]
    public async Task<IActionResult> GetDataByPostcode([FromQuery] string postcode)
    {
        try
        {
            var result = await _service.GetMedianElectricityCosumption(postcode);
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occured while retrieving recipes");
        }
    }
    // [HttpGet]
    // public IActionResult GetDataByPostcodes([FromQuery(Name = "postcode")] string[] postcodes)
    // {
    //     return Ok();
    // }


}