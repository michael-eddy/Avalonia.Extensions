namespace Avalonia.Extensions
{
    public sealed class GroupViewItem
    {
        public GroupViewItem(string content)
        {
            Content = content;
        }
        public GroupViewItem(string content, object tag)
        {
            Tag = tag;
            Content = content;
        }
        public GroupViewItem(string content, object tag, bool check)
        {
            Tag = tag;
            Check = check;
            Content = content;
        }
        internal string Id { get; set; }
        public object Tag { get; set; }
        public bool Check { get; set; }
        public string Content { get; set; }
    }
}