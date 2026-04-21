using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using System.Linq;

[Consumes("application/json")]
public class EnergyUtilityController : EnergyUtilityBaseController
{
    private readonly EnergyUtilityService _service;
    public EnergyUtilityController(EnergyUtilityService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DataInput input, IValidator<DataInput> validator)
    {
        try
        {
            var validationResults = await validator.ValidateAsync(input);
            if (!validationResults.IsValid)
            {
                return ValidationProblem(validationResults.ToModelStateDictionary());
            }

            int length = input?.Postcodes?.Length ?? 0;
            DataOutput[]? output = new DataOutput[length];
            for (int i = 0; i < length; i++)
            {
                DataRequest request = DataInputToRequest(input.Postcodes[i], input);
                output[i] = await _service.GetData(request);
            }

            if (output.Any(x => x != null))
            {
                return Ok(new { Input = input, Output = output });
            }
            else
            {
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            return GetErrorResponse(ex);
        }
    }
    private DataRequest DataInputToRequest(string postcode, DataInput input)
    {
        return new DataRequest
        {
            Postcode = postcode,
            PropertyType = input.PropertyType,
            PropertyAge = input.PropertyAge,
            FloorArea = input.FloorArea,
            HouseholdSize = input.HouseholdSize,
            NumberOfAdults = input.NumberOfAdults,
            NumberOfBedrooms = input.NumberOfBedrooms,
            PaymentMethod = input.PaymentMethod,
            MeterType = input.MeterType
        };
    }
    private IActionResult GetErrorResponse(Exception ex)
    {
        var error = new ProblemDetails
        {
            Title = "An error occured",
            Detail = ex.Message,
            Status = 500,
            Type = "https://httpstatuses.com/500"
        };

        return new ObjectResult(error)
        {
            StatusCode = 500
        };
    }

}