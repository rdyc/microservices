using FluentValidation.Results;

namespace FW.Core.Validation;

public static class FluentValidationExtensions
{
    public static IDictionary<string, string[]> ToDictionary(this IEnumerable<ValidationFailure> validationResults)
    {
        return validationResults
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray()
            );
    }
}