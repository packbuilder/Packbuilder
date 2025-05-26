using System.ComponentModel.DataAnnotations;

namespace Packbuilder.Validators;
public class MemoAttribute : ValidationAttribute
{
    private const int Min = 1;
    private const int Max = 300;
    private const string Pattern = @"^[\p{L}\p{N}.,!?'()\- ]+$";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var memo = value as string;

        if (string.IsNullOrWhiteSpace(memo))
        {
            return new ValidationResult("Memo is required.");
        }

        memo = memo.Trim();

        if (memo.Length < Min)
            return new ValidationResult("Memo must be at least 1 character long");

        if (memo.Length > Max)
            return new ValidationResult("Memo cannot be longer than 300 characters");

        if (!System.Text.RegularExpressions.Regex.IsMatch(memo, Pattern))
        {
            return new ValidationResult("Your memo contains invalid characters.");
        }

        return ValidationResult.Success;
    }
}