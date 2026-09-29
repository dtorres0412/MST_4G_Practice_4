using MST_4G_Self_Practice_4.Data;

namespace MST_4G_Self_Practice_4.Extensions;

public class Validations
{
    public static void ValidateBusinessRules(params(string? value, int maxLength, bool allowSpaces)[] fields)
    {
        char[] specialSymbols = ['^', '<', '>', '|', '&', '"', '/', '\'', ','];

        foreach(var(value, maxLength, allowSpaces) in fields)
        {
            if(string.IsNullOrWhiteSpace(value)) continue;

            string valueTrimmed = value.Trim();

            if (valueTrimmed.IndexOfAny(specialSymbols) >= 0)
            {
                throw new ArgumentException("Special symbols are not accepted.");
            }

            if (valueTrimmed.Length > maxLength)
            {
                throw new ArgumentException($"The input exceeds on the maxlength of {maxLength}");
            }

            if(!allowSpaces && valueTrimmed.Contains(' '))
            {
                throw new ArgumentException("Spaces are not allowed in this field.");
            }
        }
    }
}