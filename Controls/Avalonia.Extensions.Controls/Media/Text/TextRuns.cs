using Avalonia.Collections;
using System.Collections.Generic;

namespace Avalonia.Extensions.Media
{
    public sealed class TextRuns : AvaloniaList<AvaloniaObject>
    {
        public TextRuns()
        {
            ResetBehavior = ResetBehavior.Remove;
        }
        public TextRuns(IEnumerable<AvaloniaObject> items) : base(items)
        {
            ResetBehavior = ResetBehavior.Remove;
        }
    }
}