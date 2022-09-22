using Avalonia.Controls;
using System.Collections.Generic;
using System.IO;
using System;

namespace Avalonia.Extensions
{
    internal static class Untils
    {
        public static TabControl FindTabControl(this Control tab)
        {
            IControl control = tab;
            while (control != null)
            {
                if (control is TabControl tabControl)
                    return tabControl;
                control = control.Parent;
            }
            return null;
        }
        public static bool MoveFolder(string sourcePath, string destPath)
        {
            bool result = false;
            if (Directory.Exists(sourcePath))
            {
                DirectoryInfo folder = new DirectoryInfo(sourcePath);
                if (!Directory.Exists(destPath))
                {
                    try
                    {
                        Directory.CreateDirectory(destPath);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("创建目标目录失败：" + ex.Message);
                    }
                }
                List<string> files = new List<string>(Directory.GetFiles(sourcePath));
                files.ForEach(c =>
                {
                    string destFile = Path.Combine(new[] { destPath, Path.GetFileName(c) });
                    if (File.Exists(destFile))
                        File.Delete(destFile);
                    File.Move(c, destFile);
                });
                List<string> folders = new List<string>(Directory.GetDirectories(sourcePath));
                folders.ForEach(c =>
                {
                    string destDir = Path.Combine(new string[] { destPath, Path.GetFileName(c) });
                    MoveFolder(c, destDir);
                });
                Directory.Delete(sourcePath);
                result = true;
            }
            else
            {
                result = false;
                throw new DirectoryNotFoundException("源目录不存在！");
            }
            return result;
        }
    }
}