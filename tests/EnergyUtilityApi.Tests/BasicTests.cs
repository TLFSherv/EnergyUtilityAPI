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

    [Theory]
    [InlineData("/api/energy-utility?Postcode=AB10AG&PropertyType=6&PropertyAge=11&NumberOfAdults=3&FloorArea=4")]
    [InlineData("/api/energy-utility?Postcode=B330AA")]
    [InlineData("/api/energy-utility?Postcode=B330AB&Postcode=B330AD")]
    public async Task GetData_ReturnsOkResult(string request)
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync(request);
        response.EnsureSuccessStatusCode();
    }

    [Theory]
    [InlineData("/api/energy-utility?Postcode=A")]
    [InlineData("/api/energy-utility?Postcode=B33&Postcode=B330AB&Postcode=B330AD")]
    public async Task GetData_ReturnsBadRequestWhenPostcode(string request)
    {
        // Arrange
        var client = _factory.CreateClient();
        // Act
        var response = await client.GetAsync(request);
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Contains("Postcodes[0]", problemDetails.Errors.Keys);
    }

}