using IT.WebHost.CMS.Models;

namespace IT.WebHost.CMS.Services
{
    public class WidgetRegistryService
    {
        private readonly List<WidgetDescriptor> _widgets = [];

        public void Register(WidgetDescriptor descriptor) => _widgets.Add(descriptor);
        public IReadOnlyList<WidgetDescriptor> GetAll() => _widgets;
        public WidgetDescriptor? Get(string id) => _widgets.FirstOrDefault(w => w.Id == id);
    }
}
