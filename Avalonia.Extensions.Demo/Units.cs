using System;
using System.Linq;

namespace Avalonia.Controls.Demo
{
    internal static class Units
    {
        internal static int ToInt32(this object obj)
        {
            try
            {
                return obj is int result ? result
                    : obj is double d && double.IsNaN(d) ? 0 :
                    int.TryParse(obj.ToString(), out result) ? result : Convert.ToInt32(obj.ToString());
            }
            catch
            {
                try
                {
                    if (obj == null)
                        return default;
                    else
                    {
                        var num = obj.ToString()?.Split('.').FirstOrDefault();
                        return string.IsNullOrEmpty(num) ? 0 : int.Parse(num);
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }
    }
}