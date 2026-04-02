using BlazorBlueprint.Components;
using BlazorBlueprint.Primitives;
using Buf.Validate;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using ProtoFieldType = Google.Protobuf.Reflection.FieldType;
using FormFieldType = BlazorBlueprint.Components.FieldType;

namespace IT.WebHost.Components.Extensions
{
    public static class FormSchemaExtensions
    {
        public static string GetString(this Dictionary<string, object?> values, string key) =>
            values.TryGetValue(key, out var v) ? v?.ToString() ?? string.Empty : string.Empty;


        private const string UuidPattern = @"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$";

        public static FormSchema MapProtoToFormSchema<T>(T message) where T : IMessage<T>
        {
            var schema = new FormSchema();
            schema.Fields.AddRange(MapFields(message.Descriptor, prefix: null));
            return schema;
        }

        private static IEnumerable<FormFieldDefinition> MapFields(MessageDescriptor descriptor, string? prefix, int depth = 0)
        {
            if (depth > 5)
                yield break;

            foreach (var field in descriptor.Fields.InFieldNumberOrder())
            {
                if (field.FieldType == ProtoFieldType.Message)
                {
                    var nestedPrefix = prefix == null ? field.Name : $"{prefix}.{field.Name}";
                    foreach (var nested in MapFields(field.MessageType, nestedPrefix, depth + 1))
                        yield return nested;
                    continue;
                }

                if (field.FieldType == ProtoFieldType.Bytes)
                    continue;

                var opts = field.GetOptions();
                var rules = opts?.GetExtension(ValidateExtensions.Field);

                yield return BuildFieldDefinition(field, rules, prefix);
            }
        }

        private static FormFieldDefinition BuildFieldDefinition(FieldDescriptor field, FieldRules? rules, string? prefix)
        {
            var name = prefix == null ? field.Name : $"{prefix}.{field.Name}";
            var def = new FormFieldDefinition
            {
                Name = name,
                Label = field.Name,
                Type = ResolveFieldType(field, rules),
                Validations = new List<FieldValidation>()
            };

            if (field.FieldType == ProtoFieldType.Enum)
            {
                def.Options = new List<SelectOption<string>>();
                foreach (var value in field.EnumType.Values)
                    def.Options.Add(new SelectOption<string>(value.Name, value.Name));
            }

            if (rules == null)
                return def;

            if (rules.Required)
            {
                def.Required = true;
                def.Validations.Add(new FieldValidation { Type = ValidationType.Required });
            }

            if (rules.TypeCase == FieldRules.TypeOneofCase.String)
            {
                var s = rules.String;

                if (s.HasMinLen && s.MinLen > 0)
                {
                    if (!def.Required)
                    {
                        def.Required = true;
                        def.Validations.Add(new FieldValidation { Type = ValidationType.Required });
                    }
                    def.Validations.Add(new FieldValidation { Type = ValidationType.MinLength, Value = (int)s.MinLen });
                }

                if (s.HasMaxLen && s.MaxLen > 0)
                    def.Validations.Add(new FieldValidation { Type = ValidationType.MaxLength, Value = (int)s.MaxLen });

                switch (s.WellKnownCase)
                {
                    case StringRules.WellKnownOneofCase.Email:
                        def.Validations.Add(new FieldValidation { Type = ValidationType.Email });
                        break;
                    case StringRules.WellKnownOneofCase.Uri:
                    case StringRules.WellKnownOneofCase.UriRef:
                        def.Validations.Add(new FieldValidation { Type = ValidationType.Url });
                        break;
                    case StringRules.WellKnownOneofCase.Uuid:
                        def.Validations.Add(new FieldValidation { Type = ValidationType.Pattern, Value = UuidPattern });
                        break;
                }

                if (!string.IsNullOrEmpty(s.Pattern))
                    def.Validations.Add(new FieldValidation { Type = ValidationType.Pattern, Value = s.Pattern });
            }
            else if (rules.TypeCase == FieldRules.TypeOneofCase.Uint32)
            {
                var u = rules.Uint32;
                if (u.HasGte)
                    def.Validations.Add(new FieldValidation { Type = ValidationType.Min, Value = (int)u.Gte });
                if (u.HasLte)
                    def.Validations.Add(new FieldValidation { Type = ValidationType.Max, Value = (int)u.Lte });
            }
            else if (rules.TypeCase == FieldRules.TypeOneofCase.Int32)
            {
                var i = rules.Int32;
                if (i.HasGte)
                    def.Validations.Add(new FieldValidation { Type = ValidationType.Min, Value = i.Gte });
                if (i.HasLte)
                    def.Validations.Add(new FieldValidation { Type = ValidationType.Max, Value = i.Lte });
            }
            else if (rules.TypeCase == FieldRules.TypeOneofCase.Repeated)
            {
                var r = rules.Repeated;
                if (r.HasMinItems)
                {
                    var min = r.MinItems;
                    def.Validations.Add(new FieldValidation
                    {
                        Type = ValidationType.Custom,
                        Message = $"Must have at least {min} item(s).",
                        Value = (Func<object, string>)(val =>
                            val is System.Collections.ICollection col && (ulong)col.Count < min
                                ? $"Must have at least {min} item(s)."
                                : null!)
                    });
                }
                if (r.HasMaxItems)
                {
                    var max = r.MaxItems;
                    def.Validations.Add(new FieldValidation
                    {
                        Type = ValidationType.Custom,
                        Message = $"Must have at most {max} item(s).",
                        Value = (Func<object, string>)(val =>
                            val is System.Collections.ICollection col && (ulong)col.Count > max
                                ? $"Must have at most {max} item(s)."
                                : null!)
                    });
                }
            }

            return def;
        }

        private static FormFieldType ResolveFieldType(FieldDescriptor field, FieldRules? rules)
        {
            if (field.FieldType == ProtoFieldType.Enum)
                return FormFieldType.Select;

            if (field.FieldType == ProtoFieldType.Bool)
                return FormFieldType.Checkbox;

            if (IsNumericProtoType(field.FieldType))
                return FormFieldType.Number;

            if (field.FieldType == ProtoFieldType.String)
            {
                if (rules?.TypeCase == FieldRules.TypeOneofCase.String)
                {
                    return rules.String.WellKnownCase switch
                    {
                        StringRules.WellKnownOneofCase.Email   => FormFieldType.Email,
                        StringRules.WellKnownOneofCase.Uri     => FormFieldType.Url,
                        StringRules.WellKnownOneofCase.UriRef  => FormFieldType.Url,
                        _                                      => FormFieldType.Text
                    };
                }
                return FormFieldType.Text;
            }

            return FormFieldType.Text;
        }

        private static bool IsNumericProtoType(ProtoFieldType type) => type is
            ProtoFieldType.Int32 or ProtoFieldType.Int64 or
            ProtoFieldType.UInt32 or ProtoFieldType.UInt64 or
            ProtoFieldType.SInt32 or ProtoFieldType.SInt64 or
            ProtoFieldType.Fixed32 or ProtoFieldType.Fixed64 or
            ProtoFieldType.SFixed32 or ProtoFieldType.SFixed64 or
            ProtoFieldType.Float or ProtoFieldType.Double;
    }
}
