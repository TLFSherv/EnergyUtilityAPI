using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using EnergyUtilityApi;
namespace EnergyUtilityApi.Tests;

public class BasicTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public BasicTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetEnergyData_ReturnsOkResult()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/energy-utility/cost?Postcode=AB10AG&PrpertyType=4&PropertyAge=1&HouseholdSize=3");
        response.EnsureSuccessStatusCode();
    }
}