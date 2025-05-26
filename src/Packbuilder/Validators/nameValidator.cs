using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Packbuilder.Validators;
public class NameAttribute : ValidationAttribute
{
    private const int Min = 3;
    private const int Max = 20;
    private const string Pattern = @"^\p{L}+(?:[ '\-]\p{L}+)*$";

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        var name = value as string;

        if (string.IsNullOrWhiteSpace(name))
            return new ValidationResult("Name is required.");

        name = name.Trim();

        if (name.Length < Min)
            return new ValidationResult("Name must be at least 3 characters long");

        if (name.Length > Max)
            return new ValidationResult("Name cannot be longer than 20 characters");

        if (!Regex.IsMatch(name, Pattern))
            return new ValidationResult("Invalid name detected, name validation failed.");

        return ValidationResult.Success;
    }
}