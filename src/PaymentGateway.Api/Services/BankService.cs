using System.Text.Json;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Controllers;

public class BankService
{
    private readonly HttpClient _httpClient;
    
    public BankService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<GetAuthResponse> GetAuthorisation(GetAuthRequest model, CancellationToken ctx = default)
    {
        var response = await _httpClient.PostAsJsonAsync("payments", model, ctx);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error calling simulator: {response.ReasonPhrase}");
        }
        
        var responseBody = await response.Content.ReadAsStringAsync(ctx);
        var result = JsonSerializer.Deserialize<GetAuthResponse>(responseBody);
        return result;
    }
}