using System.ComponentModel.DataAnnotations;

namespace Packbuilder.Validators;
public class PasswordAttribute : ValidationAttribute
{
    private const int Min = 8;
    private const int Max = 128;

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        var password = value as string;

        if (string.IsNullOrWhiteSpace(password))
            return new ValidationResult("Password is required.");

        if (password.Length < Min)
            return new ValidationResult("Password must be at least 8 characters long.");

        if (password.Length > Max)
            return new ValidationResult("Password cannot be longer than 128 characters.");

        return ValidationResult.Success;
    }
}