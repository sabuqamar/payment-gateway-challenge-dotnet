using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests;

public class PaymentsControllerTests
{
    private readonly Random _random = new();
    
    [Fact]
    public async Task RetrievesAPaymentSuccessfully()
    {
        // Arrange
        var payment = new PostPaymentResponse
        {
            Id = Guid.NewGuid(),
            ExpiryYear = _random.Next(2023, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = _random.Next(1111, 9999).ToString(),
            Currency = "GBP"
        };

        var paymentsRepository = new PaymentsRepository();
        paymentsRepository.Add(payment);

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton(paymentsRepository)))
            .CreateClient();

        // Act
        var response = await client.GetAsync($"/api/Payments/{payment.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
        paymentResponse.Id.Should().NotBeEmpty();
        paymentResponse.CardNumberLastFour.Should().Be(payment.CardNumberLastFour);
        paymentResponse.Status.Should().Be(payment.Status);
        paymentResponse.Amount.Should().Be(payment.Amount);
        paymentResponse.Currency.Should().Be(payment.Currency);
        paymentResponse.ExpiryMonth.Should().Be(payment.ExpiryMonth);
        paymentResponse.ExpiryYear.Should().Be(payment.ExpiryYear);
    }

    [Fact]
    public async Task Returns404IfPaymentNotFound()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();
        
        // Act
        var response = await client.GetAsync($"/api/Payments/{Guid.NewGuid()}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreatesAnAuthorisedPaymentSuccessfully()
    {
        // Arrange
        var request = new PostPaymentRequest
        {
            CardNumber = "2222405343248877",
            ExpiryMonth = 4,
            ExpiryYear = 2025,
            Amount = 100,
            Cvv = "123",
            Currency = "GBP"
        };
        var expectedResponse = new PostPaymentResponse
        {
            Id = Guid.NewGuid(),
            CardNumberLastFour = "8877",
            Status = PaymentStatus.Authorized,
            Amount = 100,
            Currency = "GBP",
            ExpiryMonth = 4,
            ExpiryYear = 2025
        };
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();
        
        // Act
        var response = await client.PostAsync($"/api/Payments", content);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
        paymentResponse.Id.Should().NotBeEmpty();
        paymentResponse.CardNumberLastFour.Should().Be(expectedResponse.CardNumberLastFour);
        paymentResponse.Status.Should().Be(expectedResponse.Status);
        paymentResponse.Amount.Should().Be(expectedResponse.Amount);
        paymentResponse.Currency.Should().Be(expectedResponse.Currency);
        paymentResponse.ExpiryMonth.Should().Be(expectedResponse.ExpiryMonth);
        paymentResponse.ExpiryYear.Should().Be(expectedResponse.ExpiryYear);
    }
    
    [Fact]
    public async Task CreatesARejectedPaymentSuccessfully()
    {
        // Arrange
        var request = new PostPaymentRequest
        {
            CardNumber = "2222405343248112",
            ExpiryMonth = 1,
            ExpiryYear = 2026,
            Amount = 60000,
            Cvv = "456",
            Currency = "USD"
        };
        var expectedResponse = new PostPaymentResponse
        {
            Id = Guid.NewGuid(),
            CardNumberLastFour = "8112",
            Status = PaymentStatus.Rejected,
            Amount = 60000,
            Currency = "USD",
            ExpiryMonth = 1,
            ExpiryYear = 2026
        };
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();
        
        // Act
        var response = await client.PostAsync($"/api/Payments", content);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
        paymentResponse.Id.Should().NotBeEmpty();
        paymentResponse.CardNumberLastFour.Should().Be(expectedResponse.CardNumberLastFour);
        paymentResponse.Status.Should().Be(expectedResponse.Status);
        paymentResponse.Amount.Should().Be(expectedResponse.Amount);
        paymentResponse.Currency.Should().Be(expectedResponse.Currency);
        paymentResponse.ExpiryMonth.Should().Be(expectedResponse.ExpiryMonth);
        paymentResponse.ExpiryYear.Should().Be(expectedResponse.ExpiryYear);
    }
    
    [Fact]
    public async Task Returns400IfCardNumberTooShort()
    {
        // Arrange
        var request = new PostPaymentRequest
        {
            CardNumber = "2222",
            ExpiryMonth = 4,
            ExpiryYear = 2025,
            Amount = 100,
            Cvv = "123",
            Currency = "GBP"
        };
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();
        
        // Act
        var response = await client.PostAsync($"/api/Payments", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Returns400IfCardNumberTooLong()
    {
        // Arrange
        var request = new PostPaymentRequest
        {
            CardNumber = "2222405343248877820302",
            ExpiryMonth = 4,
            ExpiryYear = 2025,
            Amount = 100,
            Cvv = "123",
            Currency = "GBP"
        };
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();
        
        // Act
        var response = await client.PostAsync($"/api/Payments", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Returns400IfExpiryMonthTooLarge()
    {
        // Arrange
        var request = new PostPaymentRequest
        {
            CardNumber = "2222405343248877",
            ExpiryMonth = 14,
            ExpiryYear = 2025,
            Amount = 100,
            Cvv = "123",
            Currency = "GBP"
        };
        
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();
        
        // Act
        var response = await client.PostAsync($"/api/Payments", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Returns400IfExpiryDateInPast()
    {
        // Arrange
        var request = new PostPaymentRequest
        {
            CardNumber = "2222405343248877",
            ExpiryMonth = 4,
            ExpiryYear = 2021,
            Amount = 100,
            Cvv = "123",
            Currency = "GBP"
        };
        
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();
        
        // Act
        var response = await client.PostAsync($"/api/Payments", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Returns400IfCvvTooShort()
    {
        // Arrange
        var request = new PostPaymentRequest
        {
            CardNumber = "2222405343248877",
            ExpiryMonth = 4,
            ExpiryYear = 2025,
            Amount = 100,
            Cvv = "12",
            Currency = "GBP"
        };
        
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();
        
        // Act
        var response = await client.PostAsync($"/api/Payments", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Returns400IfCvvTooLong()
    {
        // Arrange
        var request = new PostPaymentRequest
        {
            CardNumber = "2222405343248877",
            ExpiryMonth = 4,
            ExpiryYear = 2025,
            Amount = 100,
            Cvv = "123456",
            Currency = "GBP"
        };
        
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();
        
        // Act
        var response = await client.PostAsync($"/api/Payments", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Returns400IfCurrencyTooShort()
    {
        // Arrange
        var request = new PostPaymentRequest
        {
            CardNumber = "2222405343248877",
            ExpiryMonth = 4,
            ExpiryYear = 2025,
            Amount = 100,
            Cvv = "123",
            Currency = "GB"
        };
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();
        
        // Act
        var response = await client.PostAsync($"/api/Payments", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}