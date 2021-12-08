namespace Avalonia.Extensions.Model
{
    internal sealed class BindingModel
    {
        public BindingModel(string content)
        {
            Content = content;
        }
        public string Content { get; set; }
    }
}