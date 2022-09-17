using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Logging;
using System;
using System.Reflection;

namespace Avalonia.Extensions.Controls
{
    [PseudoClasses(":empty")]
    [TemplatePart("PART_TextView", typeof(TextView))]
    public class EditTextView : TextBox
    {
        private TextView presenter;
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            var presenter = ResetResource(e);
            if (IsFocused)
                presenter?.ShowCaret();
        }
        private TextView ResetResource(TemplateAppliedEventArgs e)
        {
            presenter = e.NameScope.Get<TextView>("PART_TextView");
            try
            {
                var type = GetType().BaseType;
                type.GetField("_presenter", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(this, presenter);
                var imClient = type.GetField("_imClient", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);
                imClient.GetType().GetMethod("SetPresenter", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public)?.Invoke(imClient, new object[] { presenter, this });
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
            return presenter;
        }
    }
}