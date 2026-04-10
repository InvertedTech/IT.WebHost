using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProtoValidate;

namespace IT.WebHost.Core.Extensions
{
    public static class ProtoValidateExtensions
    {
        public static void AddProtoViolations(this ModelStateDictionary modelState, ValidationResult result)
        {
            foreach (var violation in result.Violations)
            {
                var field = violation.Field?.Elements.FirstOrDefault()?.FieldName ?? string.Empty;
                modelState.AddModelError(field, violation.Message);
            }
        }
    }
}
