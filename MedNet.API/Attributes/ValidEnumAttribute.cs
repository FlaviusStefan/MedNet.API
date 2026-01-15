using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Attributes
{
    /// <summary>
    /// Validates that an enum value is defined in the enum type
    /// </summary>
    public class ValidEnumAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Let [Required] handle null validation
            }

            var enumType = value.GetType();
            
            if (!enumType.IsEnum)
            {
                return new ValidationResult($"The field {validationContext.DisplayName} must be an enum type.");
            }

            // Check if the value is defined in the enum
            if (!Enum.IsDefined(enumType, value))
            {
                var validValues = string.Join(", ", Enum.GetNames(enumType));
                return new ValidationResult(
                    $"The field {validationContext.DisplayName} must be one of: {validValues}. " +
                    $"Received invalid value: {value}");
            }

            return ValidationResult.Success;
        }
    }
}
