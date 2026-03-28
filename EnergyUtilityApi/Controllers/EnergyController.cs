using System.IO.Pipelines;
using Microsoft.AspNetCore.Mvc;
public class EnergyController : EnergyBaseController
{
    [HttpGet]
    public IActionResult GetEnergyConsumption([AsParameters] GetConsumptionRequest request)
    {
        return Ok();
    }

    // get data for postcodes: region, dno, median and mean elec consumption
    public IActionResult GetDataByPostcode([FromQuery] string postcode)
    {
        return Ok();
    }

    public IActionResult GetDataByPostcodes([FromQuery(Name = "postcode")] string[] postcodes)
    {
        return Ok();
    }


}