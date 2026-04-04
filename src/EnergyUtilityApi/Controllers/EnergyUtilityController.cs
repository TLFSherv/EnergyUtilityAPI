using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using System.Linq;
public class EnergyUtilityController : EnergyUtilityBaseController
{
    private readonly EnergyUtilityService _service;
    public EnergyUtilityController(EnergyUtilityService service)
    {
        _service = service;
    }
    // get yearly energy consumption and cost data
    [HttpGet("cost/")]
    public async Task<IActionResult> GetEnergyCost([FromQuery] GetEnergyCostRequest request, IValidator<GetEnergyCostRequest> validator)
    {
        try
        {
            var validationResults = await validator.ValidateAsync(request);
            if (!validationResults.IsValid)
            {
                return ValidationProblem(validationResults.ToModelStateDictionary());
            }

            SendEnergyCostData result = await _service.GetEnergyCost(request);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(new { Input = request, Output = result });
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occured while retrieving consumption data");
        }
    }

    // get energy data for postcodes: region, dno, median and mean elec consumption
    [HttpGet]
    public async Task<IActionResult> GetEnergyData(GetEnergyDataRequest request, IValidator<GetEnergyDataRequest> validator)
    {
        try
        {
            var validationResults = validator.Validate(request);
            if (!validationResults.IsValid)
            {
                return ValidationProblem(validationResults.ToModelStateDictionary());
            }

            int length = request.Postcodes.Length;
            SendPostcodeData[]? results = new SendPostcodeData[length];
            for (int i = 0; i < length; i++)
            {
                results[i] = await _service.GetEnergyDataByPostcode(request.Postcodes[i]);
            }

            if (results.Any(x => x != null))
            {
                return Ok(new { Input = request, Output = results });
            }
            else
            {
                return NotFound();
            }
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occured while retrieving recipes");
        }
    }

}