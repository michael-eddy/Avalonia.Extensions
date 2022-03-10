using Avalonia.Media.TextFormatting.Unicode;

namespace Avalonia.Extensions.Utils
{
    internal static class StringUtils
    {
        private enum CharClass
        {
            CharClassUnknown,
            CharClassWhitespace,
            CharClassAlphaNumeric,
        }
        public static bool IsEol(char c) => c == '\r' || c == '\n';
        public static bool IsStartOfWord(string text, int index)
        {
            if (index >= text.Length)
                return false;
            var codepoint = new Codepoint(text[index]);
            if (index > 0)
            {
                var previousCodepoint = new Codepoint(text[index - 1]);
                if (!previousCodepoint.IsWhiteSpace)
                    return false;
                if (previousCodepoint.IsBreakChar)
                    return true;
            }
            return codepoint.GeneralCategory switch
            {
                GeneralCategory.LowercaseLetter or GeneralCategory.TitlecaseLetter or GeneralCategory.UppercaseLetter or GeneralCategory.DecimalNumber or GeneralCategory.LetterNumber or GeneralCategory.OtherNumber or GeneralCategory.DashPunctuation or GeneralCategory.InitialPunctuation or GeneralCategory.OpenPunctuation or GeneralCategory.CurrencySymbol or GeneralCategory.MathSymbol => true,
                _ => false,
            };
        }
        public static int PreviousWord(string text, int cursor)
        {
            if (string.IsNullOrEmpty(text))
                return 0;
            int begin;
            int i;
            int cr;
            int lf;
            lf = LineBegin(text, cursor) - 1;
            if (lf > 0 && text[lf] == '\n' && text[lf - 1] == '\r')
                cr = lf - 1;
            else
                cr = lf;
            if (cursor - 1 == lf)
                return (cr > 0) ? cr : 0;
            CharClass cc = GetCharClass(text[cursor - 1]);
            begin = lf + 1;
            i = cursor;
            while (i > begin && GetCharClass(text[i - 1]) == cc)
                i--;
            if (cc == CharClass.CharClassWhitespace && i > begin)
            {
                cc = GetCharClass(text[i - 1]);
                while (i > begin && GetCharClass(text[i - 1]) == cc)
                    i--;
            }
            return i;
        }
        public static int NextWord(string text, int cursor)
        {
            int i, lf, cr;
            cr = LineEnd(text, cursor);
            if (cursor >= text.Length)
                return cursor;
            if (cr < text.Length && text[cr] == '\r' && cr + 1 < text.Length && text[cr + 1] == '\n')
                lf = cr + 1;
            else
                lf = cr;
            if (cursor == cr || cursor == lf)
            {
                if (lf < text.Length)
                    return lf + 1;
                return cursor;
            }
            i = cursor;
            while (i < cr && char.IsWhiteSpace(text[i]))
                i++;
            if (i >= cr)
                return i;
            var cc = GetCharClass(text[i]);
            while (i < cr && GetCharClass(text[i]) == cc)
                i++;
            return i;
        }
        private static CharClass GetCharClass(char c)
        {
            if (char.IsWhiteSpace(c))
                return CharClass.CharClassWhitespace;
            else if (char.IsLetterOrDigit(c))
                return CharClass.CharClassAlphaNumeric;
            else
                return CharClass.CharClassUnknown;
        }
        private static int LineBegin(string text, int pos)
        {
            while (pos > 0 && !IsEol(text[pos - 1]))
                pos--;
            return pos;
        }
        private static int LineEnd(string text, int cursor, bool include = false)
        {
            while (cursor < text.Length && !IsEol(text[cursor]))
                cursor++;
            if (include && cursor < text.Length)
            {
                if (text[cursor] == '\r' && text[cursor + 1] == '\n')
                    cursor += 2;
                else
                    cursor++;
            }
            return cursor;
        }
    }
}