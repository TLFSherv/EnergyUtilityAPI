
public class EnergyUtilityController : BaseController
{
    [HttpGet]
    public IActionResult GetEnergyConsumption([AsParameters] GetConsumptionRequest request)
    {

    }

    // get data for postcodes: region, dno, median and mean elec consumption
    public IActionResult GetDataByPostcode([FromQuery] string postcode)
    {

    }

    public IActionResult GetDataByPostcodes([FromQuery(Name = "postcode")] string[] postcodes)
    {

    }


}