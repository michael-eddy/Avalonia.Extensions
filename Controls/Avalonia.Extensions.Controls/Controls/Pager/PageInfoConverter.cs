using Avalonia.Data.Converters;
using Avalonia.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Avalonia.Extensions.Controls
{
    public sealed class PageInfoConverter : IMultiValueConverter
    {
        private string format;
        private static PageInfoConverter instance;
        public static PageInfoConverter Instance
        {
            get
            {
                instance ??= new PageInfoConverter();
                return instance;
            }
        }
        public void SetPageInfoFormat(string format) => this.format = format;
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (values[0] is int showingPageDataStartNumber && values[1] is int showingPageDataEndNumber &&
                    values[2] is int totalPageCount && values[3] is int totalDataCount)
                {
                    if (string.IsNullOrEmpty(format))
                        format = Core.Instance.IsEnglish ? "{0}-{1},{2} pages,{3} records" : "{0}-{1}条,共{2}页";
                    return string.Format(format, showingPageDataStartNumber, showingPageDataEndNumber, totalPageCount);
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(this, ex.Message);
            }
            return AvaloniaProperty.UnsetValue;
        }
    }
}