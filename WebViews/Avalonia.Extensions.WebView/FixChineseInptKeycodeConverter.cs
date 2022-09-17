using CefNet.Input;
using CefNet;
using System;

namespace Avalonia.Extensions.WebView
{
    public class FixChineseInptKeycodeConverter : KeycodeConverter
    {
        public override VirtualKeys CharacterToVirtualKey(char character)
        {
            try
            {
                return base.CharacterToVirtualKey(character);
            }
            catch (Exception e)
            {
                if (PlatformInfo.IsWindows)
                {
                    if (e.Message.Equals("Incompatible input locale.", StringComparison.CurrentCultureIgnoreCase))
                        return VirtualKeys.None;
                }
                throw;
            }
        }
    }
}