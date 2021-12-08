using System.Collections;

namespace Avalonia.Extensions.Model
{
    public partial class ExpandableListView
    {
        public sealed class ExpandableArray
        {
            public string Header { get; set; }
            public IEnumerator Items { get; set; }
        }
    }
}