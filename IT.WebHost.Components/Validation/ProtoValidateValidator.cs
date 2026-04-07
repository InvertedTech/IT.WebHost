using Google.Protobuf;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ProtoValidate;

namespace IT.WebHost.Components.Validation
{
    public class ProtoValidateValidator<TMessage> : ComponentBase, IDisposable
        where TMessage : IMessage<TMessage>
    {
        [CascadingParameter] private EditContext? EditContext { get; set; }
        [Inject] private IValidator Validator { get; set; } = null!;
        [Parameter] public Func<TMessage>? MessageFactory { get; set; }

        private ValidationMessageStore? _messageStore;

        protected override void OnInitialized()
        {
            if (EditContext == null)
                throw new InvalidOperationException($"{nameof(ProtoValidateValidator<TMessage>)} must be used inside an EditForm.");

            _messageStore = new ValidationMessageStore(EditContext);
            EditContext.OnValidationRequested += OnValidationRequested;
            EditContext.OnFieldChanged += OnFieldChanged;
        }

        private void OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
        {
            _messageStore!.Clear();
            if (MessageFactory == null) return;

            var result = Validator.Validate(MessageFactory(), false);

            foreach (var violation in result.Violations)
            {
                var fieldName = violation.Field?.Elements.FirstOrDefault()?.FieldName ?? string.Empty;
                if (string.IsNullOrEmpty(fieldName)) continue;
                _messageStore!.Add(new FieldIdentifier(EditContext!.Model, fieldName), violation.Message);
            }

            EditContext!.NotifyValidationStateChanged();
        }

        private void OnFieldChanged(object? sender, FieldChangedEventArgs e)
        {
            _messageStore!.Clear(e.FieldIdentifier);
            if (MessageFactory != null)
            {
                var result = Validator.Validate(MessageFactory(), false);
                foreach (var violation in result.Violations)
                {
                    var fieldName = violation.Field?.Elements.FirstOrDefault()?.FieldName ?? string.Empty;
                    if (fieldName != e.FieldIdentifier.FieldName) continue;
                    _messageStore!.Add(e.FieldIdentifier, violation.Message);
                }
            }
            EditContext!.NotifyValidationStateChanged();
        }

        public void Dispose()
        {
            if (EditContext == null) return;
            EditContext.OnValidationRequested -= OnValidationRequested;
            EditContext.OnFieldChanged -= OnFieldChanged;
        }
    }
}
