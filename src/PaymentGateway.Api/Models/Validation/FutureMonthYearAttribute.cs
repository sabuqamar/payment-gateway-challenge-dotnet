using System;
using System.ComponentModel.DataAnnotations;

public class FutureDateAttribute : ValidationAttribute
{
    private readonly string _monthPropertyName;

    public FutureDateAttribute(string monthPropertyName)
    {
        _monthPropertyName = monthPropertyName;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var year = (int)value;
        var monthProperty = validationContext.ObjectType.GetProperty(_monthPropertyName);

        if (monthProperty == null)
        {
            return new ValidationResult($"Property '{_monthPropertyName}' not found.");
        }
        
        var month = (int)monthProperty.GetValue(validationContext.ObjectInstance);
        if (month < 1 || month > 12)
        {
            return new ValidationResult("Invalid month. Must be between 1 and 12.");
        }

        var currentDate = DateTime.Now;
        var expiryDate = new DateTime(year, month, 1);

        if (expiryDate > currentDate)
        {
            return ValidationResult.Success;
        }

        return new ValidationResult("The expiry date must be in the future.");
    }
}