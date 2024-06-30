using PaymentGateway.Api.Models.Exceptions;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class PaymentsRepository
{
    public List<PostPaymentResponse> Payments = new();

    public void Add(PostPaymentResponse payment)
    {
        Payments.Add(payment);
    }

    public PostPaymentResponse Get(Guid id)
    {
        var payment = Payments.FirstOrDefault(p => p.Id == id);
        if (payment == null)
        {
            throw new ResourceNotFoundException("Resource not found.");
        }
        return payment;
    }
}