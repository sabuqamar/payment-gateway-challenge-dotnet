using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Moq;
using Moq.Protected;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Tests;

public class BankServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHandler;
    private readonly HttpClient _httpClient;
    private readonly BankService _bankService;

    public BankServiceTests()
    {
        _mockHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHandler.Object){
            BaseAddress = new Uri("http://localhost:8080")
        };
        _bankService = new BankService(_httpClient);
    }

    [Fact]
    public async Task GetAuthorisationSuccessfuly()
    {
        // Arrange
        var expectedResponse = new GetAuthResponse 
        { 
            Authorized = true, 
            AuthorizationCode = "0bb07405-6d44-4b50-a14f-7ae0beff13ad"
        };
        var model = new GetAuthRequest
        {
            CardNumber = "2222405343248877",
            ExpiryDate = "4/2025",
            Amount = 100,
            Cvv = "123",
            Currency = "GBP"
        };
        var json = JsonSerializer.Serialize(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedResponse), System.Text.Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _bankService.GetAuthorisation(model);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Authorized, result.Authorized);
        Assert.Equal(expectedResponse.AuthorizationCode, result.AuthorizationCode);
    }

    [Fact]
    public async Task ThrowsHttpRequestExceptionIfRequestFails()
    {
        // Arrange
        var model = new GetAuthRequest
        {
            CardNumber = "2222405343248877",
            ExpiryDate = "4/2025",
            Amount = 100,
            Cvv = "123",
            Currency = "GBP"
        };
        var json = JsonSerializer.Serialize(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                ReasonPhrase = "Internal Server Error"
            });

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _bankService.GetAuthorisation(model));
    }
}