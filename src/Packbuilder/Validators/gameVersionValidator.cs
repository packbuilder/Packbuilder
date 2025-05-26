using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Packbuilder.Validators;
public class GameVersionAttribute : ValidationAttribute
{
    private const int Min = 1;
    private const int Max = 50;
    private const string Pattern = @"^\d+\.\d+(\.\d+)?$";

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        var version = value as string;

        if (string.IsNullOrWhiteSpace(version))
            return new ValidationResult("Game version is required.");

        version = version.Trim();

        if (version.Length < Min)
            return new ValidationResult("Game version is required.");

        if (version.Length > Max)
            return new ValidationResult("Game version should not be longer than 50 characters");

        if (!Regex.IsMatch(version, Pattern))
            return new ValidationResult("Invalid Minecraft version");

        return ValidationResult.Success;
    }
}