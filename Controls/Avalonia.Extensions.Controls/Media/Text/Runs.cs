using Avalonia.Collections;
using System.Collections.Generic;

namespace Avalonia.Extensions.Media
{
    public sealed class Runs : AvaloniaList<AvaloniaObject>
    {
        public Runs()
        {
            ResetBehavior = ResetBehavior.Remove;
        }
        public Runs(IEnumerable<AvaloniaObject> items) : base(items)
        {
            ResetBehavior = ResetBehavior.Remove;
        }
    }
}