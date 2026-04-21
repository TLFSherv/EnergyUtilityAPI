using FluentValidation;
using EnergyUtilityApi;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
public class DataInputValidator : AbstractValidator<DataInput>
{
    private readonly EnergyUtilityDbContext _context;
    public DataInputValidator(EnergyUtilityDbContext context)
    {
        _context = context;

        RuleForEach(x => x.Postcodes).NotEmpty()
        .WithMessage("Postcode is required")
        .Length(6, 8).WithMessage("Postcode length must be between 6 and 8 characters")
        .MustAsync(BeAValidPostcode)
        .WithMessage("Invalid postcode");

        RuleFor(x => x.PaymentMethodId).InclusiveBetween(1, 3)
        .WithMessage("Payment method values must be between 1 and 3");

        RuleFor(x => x.MeterTypeId).InclusiveBetween(1, 2)
        .WithMessage("Meter type values must be 1 or 2");

        RuleFor(x => x.PropertyType).InclusiveBetween(1, 7)
        .WithMessage("Property type values must be between 1 and 7");

        RuleFor(x => x.PropertyAge).InclusiveBetween(1, 12)
        .WithMessage("Property age values must be between 1 and 12");

        RuleFor(x => x.FloorArea).InclusiveBetween(1, 5)
        .WithMessage("Floor area values must be between 1 and 5");

        RuleFor(x => x.HouseholdSize).InclusiveBetween(1, 3)
        .WithMessage("Household size values must be between 1 and 3");

        RuleFor(x => x.NumberOfAdults).InclusiveBetween(1, 5)
        .WithMessage("Number of adult values must be between 1 and 5");

        RuleFor(x => x.NumberOfBedrooms).InclusiveBetween(1, 5)
        .WithMessage("Number of bedroom values must be between 1 and 5");

        RuleFor(x => x.PaymentMethodId).InclusiveBetween(1, 3)
        .WithMessage("Payment method id must be between 1 and 3");

        RuleFor(x => x.MeterTypeId).InclusiveBetween(1, 2)
        .WithMessage("Meter type id must be either 1 or 2");

        // payment method id and meter type id should be provided together
        // When(r => r.PaymentMethodId != null || r.MeterTypeId != null, () =>
        // {
        //     RuleFor(x => x.PaymentMethodId).NotEmpty();
        //     RuleFor(x => x.MeterTypeId).NotEmpty();
        // });
    }

    private async Task<bool> BeAValidPostcode(string postcode, CancellationToken token)
    {
        try
        {
            if (postcode == null) return false;
            string shortPostcode = Regex.Replace(postcode, "[^0-9a-zA-Z]+", "")[..4];
            string? result = await _context.PostcodeMeters
                .Where(x => x.Postcode.Replace(" ", "") == shortPostcode)
                .Select(x => x.Postcode)
                .SingleOrDefaultAsync();
            return result != null;
        }
        catch (Exception)
        {
            return false;
        }
    }

}