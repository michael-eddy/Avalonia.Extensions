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
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            var presenter = e.NameScope.Get<TextView>("PART_TextView");
            SetPresenter(presenter);
            if (IsFocused)
                presenter?.ShowCaret();
        }
        private void SetPresenter(TextView presenter)
        {
            try
            {
                var type = GetType().BaseType;
                type.GetField("_presenter", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.SetValue(this, presenter);
                var _imClient = type.GetField("_imClient", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(this);
                var meth = _imClient.GetType().GetMethod("SetPresenter", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                meth?.Invoke(_imClient, new object[] { presenter, this });
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
        }
    }
}