using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Exceptions;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly BankService _bankService;
    private readonly PaymentsRepository _paymentsRepository;

    public PaymentsController(BankService bankService, PaymentsRepository paymentsRepository)
    {
        _paymentsRepository = paymentsRepository;
        _bankService = bankService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        try
        {
            var payment = _paymentsRepository.Get(id);
            return new OkObjectResult(payment);
        }
        catch(ResourceNotFoundException)
        {
            return NotFound("Resource not found.");
        }
    }
    
    [HttpPost]
        public async Task<ActionResult<PostPaymentResponse?>> PostPaymentAsync([FromBody] PostPaymentRequest paymentRequest, CancellationToken ctx = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            GetAuthResponse authResponse = await _bankService.GetAuthorisation(paymentRequest, ctx);
            PostPaymentResponse payment = new PostPaymentResponse()
            {
                Id = Guid.NewGuid(),
                Status = (authResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Rejected),
                Amount = paymentRequest.Amount,
                Currency = paymentRequest.Currency,
                ExpiryMonth = paymentRequest.ExpiryMonth,
                ExpiryYear = paymentRequest.ExpiryYear,
                CardNumberLastFour = paymentRequest.CardNumber[^4..]
            };

            _paymentsRepository.Add(payment);
            return new OkObjectResult(payment);
        }
}