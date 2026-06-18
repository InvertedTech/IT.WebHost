using Buf.Validate;

namespace Core.Extensions
{
    public static class ProtovalidateExtensions
    {
        public static FieldValidationState ForField(
            this Violations? violations,
            string fieldPath)
        {
            if (violations is null || violations.Violations_.Count == 0)
            {
                return FieldValidationState.Valid;
            }

            return violations.Violations_.ToFieldValidationState(fieldPath);
        }

        public static FieldValidationState ForField(
            this IEnumerable<Violation>? violations,
            string fieldPath)
        {
            if (violations is null)
            {
                return FieldValidationState.Valid;
            }

            return violations.ToFieldValidationState(fieldPath);
        }

        private static FieldValidationState ToFieldValidationState(
            this IEnumerable<Violation> violations,
            string fieldPath)
        {
            var errors = violations
                .Where(v => MatchesFieldPath(v, fieldPath))
                .Select(v => v.Message)
                .Where(message => !string.IsNullOrWhiteSpace(message))
                .ToList();

            return errors.Count > 0
                ? new FieldValidationState(true, errors)
                : FieldValidationState.Valid;
        }

        private static bool MatchesFieldPath(Violation violation, string fieldPath)
        {
            if (violation.Field?.Elements is not { Count: > 0 } elements)
            {
                return false;
            }

            var names = elements
                .Select(e => e.FieldName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToList();

            if (names.Count == 0)
            {
                return false;
            }

            var fullPath = string.Join(".", names);
            var leafName = names[^1];

            return fullPath.Equals(fieldPath, StringComparison.OrdinalIgnoreCase)
                || leafName.Equals(fieldPath, StringComparison.OrdinalIgnoreCase);
        }
    }

    public readonly record struct FieldValidationState(
        bool IsInvalid,
        IReadOnlyList<string> Errors)
    {
        public static readonly FieldValidationState Valid =
            new(false, Array.Empty<string>());
    }
}