using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.Extensions.Controls
{
    public partial class EditTextView
    {
        readonly struct UndoRedoState : IEquatable<UndoRedoState>
        {
            public string? Text { get; }
            public int CaretPosition { get; }
            public UndoRedoState(string? text, int caretPosition)
            {
                Text = text;
                CaretPosition = caretPosition;
            }
            public bool Equals(UndoRedoState other) => ReferenceEquals(Text, other.Text) || Equals(Text, other.Text);
            public override bool Equals(object? obj) => obj is UndoRedoState other && Equals(other);
            public override int GetHashCode() => Text?.GetHashCode() ?? 0;
        }
    }
}