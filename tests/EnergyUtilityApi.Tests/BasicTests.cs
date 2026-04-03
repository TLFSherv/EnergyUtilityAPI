using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using EnergyUtilityApi;
using System.Net;
namespace EnergyUtilityApi.Tests;

public class BasicTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public BasicTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetEnergyCostScotland_ReturnsOkResult()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/energy-utility/cost?Postcode=AB10AG&PropertyType=6&PropertyAge=11&NumberOfAdults=3&FloorArea=4");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetEnergyData_ReturnsOkResult()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/energy-utility?Postcode=B330AA");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetEnergyCost_BadRequest()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/energy-utility/cost?Postcode=A");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

}