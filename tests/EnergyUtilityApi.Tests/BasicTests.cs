using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using EnergyUtilityApi;
using System.Net;
using System.Net.Http.Json;

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
    public async Task GetEnergyCost_ReturnsBadRequestWhenPostcode()
    {
        // Arrange
        var client = _factory.CreateClient();
        // Act
        var response = await client.GetAsync("/api/energy-utility/cost?Postcode=A");
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Contains("Postcode", problemDetails.Errors.Keys);
    }

    [Fact]
    public async Task GetEnergyData_ReturnsBadRequestWhenPostcode()
    {
        // Arrange
        var client = _factory.CreateClient();
        // Act
        var response = await client.GetAsync("/api/energy-utility?Postcode=B33&Postcode=B330AB&Postcode=B330AD");
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Contains("Postcodes[0]", problemDetails.Errors.Keys);
    }

}