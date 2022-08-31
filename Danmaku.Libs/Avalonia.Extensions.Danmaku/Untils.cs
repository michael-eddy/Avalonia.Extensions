using Avalonia.Media;
using System;
using System.Linq;

namespace Avalonia.Extensions.Danmaku
{
    internal static class Untils
    {
        public static Color ToColor(this string obj, bool x2 = false)
        {
            obj = obj.Replace("#", "");
            if (x2)
                obj = Convert.ToInt32(obj).ToString("X2");
            return obj.Length switch
            {
                4 => Color.Parse($"#00{obj}"),
                6 or 8 => Color.Parse($"#{obj}"),
                _ => Colors.Gray,
            };
        }
        internal static int ToInt32(this object obj)
        {
            try
            {
                if (obj is int result)
                    return result;
                else if (obj is double d && double.IsNaN(d))
                    return 0;
                else
                {
                    if (int.TryParse(obj.ToString(), out result))
                        return result;
                    else
                        return Convert.ToInt32(obj.ToString());
                }
            }
            catch
            {
                try
                {
                    var num = obj.ToString().Split('.').FirstOrDefault();
                    return string.IsNullOrEmpty(num) ? 0 : int.Parse(num);
                }
                catch
                {
                    return 0;
                }
            }
        }
    }
}