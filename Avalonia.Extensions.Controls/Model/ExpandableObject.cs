using Avalonia.Collections;
using System.Collections;

namespace Avalonia.Extensions.Model
{
    public sealed class ExpandableObject
    {
        public ExpandableObject()
        {
            Items = new AvaloniaList<object>();
        }
        public ExpandableObject(string header, IEnumerable objs)
        {
            this.Header = header;
            this.Items = objs ?? new AvaloniaList<object>();
        }
        public string Header { get; set; }
        public IEnumerable Items { get; set; }
    }
}