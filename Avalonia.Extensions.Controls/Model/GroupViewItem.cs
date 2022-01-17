namespace Avalonia.Extensions
{
    public sealed class GroupViewItem
    {
        public GroupViewItem(string content)
        {
            Content = content;
        }
        public GroupViewItem(string content, bool check) : this(content)
        {
            Check = check;
        }
        public GroupViewItem(string content, object tag) : this(content)
        {
            Tag = tag;
        }
        public GroupViewItem(string content, object tag, bool check) : this(content, tag)
        {
            Check = check;
        }
        internal string Id { get; set; }
        public object Tag { get; set; }
        public bool Check { get; set; }
        public string Content { get; set; }
    }
}