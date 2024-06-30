namespace PaymentGateway.Api.Models.Exceptions;

public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException(string message) : base(message) { }
}