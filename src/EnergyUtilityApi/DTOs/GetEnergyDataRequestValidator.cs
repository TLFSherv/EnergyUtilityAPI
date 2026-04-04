using FluentValidation;
public class GetEnergyDataRequestValidator : AbstractValidator<GetEnergyDataRequest>
{
    public GetEnergyDataRequestValidator()
    {
        RuleForEach(x => x.Postcodes).NotEmpty()
        .WithMessage("Postcode is required")
        .Length(6, 8).WithMessage("Postcode length must be between 6 and 8 characters");
    }
}