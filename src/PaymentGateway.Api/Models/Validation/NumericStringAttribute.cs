using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PaymentGateway.Api.Models.Validation;

public class NumericStringAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is string str && !Regex.IsMatch(str, @"^\d+$"))
        {
            return new ValidationResult("The field must be numeric.");
        }
        return ValidationResult.Success;
    }
}