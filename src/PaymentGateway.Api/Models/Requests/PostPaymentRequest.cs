using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using PaymentGateway.Api.Models.Validation;

namespace PaymentGateway.Api.Models.Requests;

public class PostPaymentRequest
{
    [Required]
    [NumericString]
    [StringLength(19, MinimumLength = 14)]
    [JsonPropertyName("card_number")]
    public string CardNumber { get; set; }
    [Required]
    [JsonPropertyName("expiry_month")]
    public int ExpiryMonth { get; set; }
    [Required]
    [JsonPropertyName("expiry_year")]
    [FutureDate(nameof(ExpiryMonth))]
    public int ExpiryYear { get; set; }
    [Required]
    [StringLength(3, MinimumLength = 3)]
    [JsonPropertyName("currency")]
    public string Currency { get; set; }
    [Required]
    [JsonPropertyName("amount")]
    public int Amount { get; set; }
    [Required]
    [NumericString]
    [StringLength(4, MinimumLength = 3)]
    [JsonPropertyName("cvv")]
    public string Cvv { get; set; }

    public static implicit operator GetAuthRequest(PostPaymentRequest request)
    {
        return new GetAuthRequest()
        {
            Amount = request.Amount,
            CardNumber = request.CardNumber,
            Currency = request.Currency,
            Cvv = request.Cvv,
            ExpiryDate =  new DateOnly(request.ExpiryYear, request.ExpiryMonth, 1).ToString("MM/yyyy")
        };
    }
}