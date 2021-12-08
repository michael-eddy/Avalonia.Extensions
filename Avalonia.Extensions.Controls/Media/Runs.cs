using Avalonia.Collections;
using System.Collections.Generic;

namespace Avalonia.Extensions.Media
{
    public sealed class Runs : AvaloniaList<Run>
    {
        public Runs()
        {
            ResetBehavior = ResetBehavior.Remove;
        }
        public Runs(IEnumerable<Run> items) : base(items)
        {
            ResetBehavior = ResetBehavior.Remove;
        }
    }
}