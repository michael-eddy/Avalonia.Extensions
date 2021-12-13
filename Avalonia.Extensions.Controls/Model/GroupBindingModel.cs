namespace Avalonia.Extensions
{
    public sealed class GroupBindingModel
    {
        public GroupBindingModel(string content)
        {
            Content = content;
        }
        public GroupBindingModel(string content, object tag)
        {
            Tag = tag;
            Content = content;
        }
        public GroupBindingModel(string content, object tag, bool check)
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