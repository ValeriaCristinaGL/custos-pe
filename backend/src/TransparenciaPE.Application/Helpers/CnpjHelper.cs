using System.Text.RegularExpressions;

namespace TransparenciaPE.Application.Helpers;

public static partial class CnpjHelper
{
    public static string Sanitize(string? cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return string.Empty;

        return NonDigitRegex().Replace(cnpj.Trim(), "");
    }

    public static bool IsValid(string? cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;

        var sanitized = Sanitize(cnpj);

        if (sanitized.Length != 14)
            return false;

        // All same digits are invalid
        if (sanitized.Distinct().Count() == 1)
            return false;

        // Validate check digits
        var digits = sanitized.Select(c => c - '0').ToArray();

        int[] firstWeights = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] secondWeights = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        var firstCheck = CalculateCheckDigit(digits, firstWeights);
        if (digits[12] != firstCheck)
            return false;

        var secondCheck = CalculateCheckDigit(digits, secondWeights);
        return digits[13] == secondCheck;
    }

    private static int CalculateCheckDigit(int[] digits, int[] weights)
    {
        var sum = 0;
        for (var i = 0; i < weights.Length; i++)
            sum += digits[i] * weights[i];

        var remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }

    [GeneratedRegex(@"\D")]
    private static partial Regex NonDigitRegex();
}
