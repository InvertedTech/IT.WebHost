using BlazorBlueprint.Components;
using Google.Protobuf;
using Buf.Validate;
using System;
using System.Collections.Generic;
using System.Text;

namespace IT.WebHost.Components.Extensions
{
    public static class FormSchemaExtensions
    {
        public static FormSchema MapProtoToFormSchema<T>(T message) where T : IMessage<T>
        {
            var schema = new FormSchema();

            foreach (var field in message.Descriptor.Fields.InFieldNumberOrder())
            {
                var opts = field.GetOptions();
                var rules = opts?.GetExtension(ValidateExtensions.Field);

            }

            return schema;
        }
    }
}
