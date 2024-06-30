using PaymentGateway.Api.Models.Exceptions;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests;

public class PaymentsRepositoryTests
{
    private readonly Random _random = new();
    private readonly PaymentsRepository _repository;

    public PaymentsRepositoryTests()
    {
        _repository = new PaymentsRepository();
    }

    [Fact]
    public void AddSuccessfullyAddsPayment()
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

        // Act
        _repository.Add(payment);

        // Assert
        Assert.Contains(payment, _repository.Payments);
    }

    [Fact]
    public void GetReturnsPaymentSuccessfully()
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
        _repository.Payments = new List<PostPaymentResponse>([payment]);

        // Act
        var result = _repository.Get(payment.Id);

        // Assert
        Assert.Equal(payment, result);
    }

    [Fact]
    public void GetThrowsResourceNotFoundExceptionIfPaymentDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ResourceNotFoundException>(() => _repository.Get(nonExistentId));
    }
}