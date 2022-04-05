using System;
using System.Linq;

namespace Avalonia.Extensions.Danmaku
{
    internal static class Untils
    {
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