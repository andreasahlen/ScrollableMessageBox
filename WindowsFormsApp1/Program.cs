using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;
using static WindowsFormsApp1.ScrollableMessageBox;

namespace WindowsFormsApp1
{
    

    internal static class Program
    {
        private static Dictionary<ScrollableMsgBoxButtonType, string> locales = new Dictionary<ScrollableMsgBoxButtonType, string>
        {
            {  ScrollableMsgBoxButtonType.OkButton, "&Ok" },
            {  ScrollableMsgBoxButtonType.CancelButton, "&Cancel" },
            {  ScrollableMsgBoxButtonType.YesButton, "&Yes" },
            {  ScrollableMsgBoxButtonType.NoButton, "&No" },
            {  ScrollableMsgBoxButtonType.AbortButton, "&Abort" },
            {  ScrollableMsgBoxButtonType.RetryButton, "&Retry" },
            {  ScrollableMsgBoxButtonType.IgnoreButton, "&Ignore" }
        };

        //private static Keys GetHotKeyFromString(string value)
        //{
        //    char hotkey;

        //    if (!string.IsNullOrWhiteSpace(value))
        //    {
        //        if (value.Contains("&"))
        //        {
        //            int detectedHotKeyPrefix = 0;
        //            foreach (char item in value)
        //            {
        //                if (item == '&')
        //                {
        //                    detectedHotKeyPrefix = value.IndexOf(item);
        //                    if (detectedHotKeyPrefix < value.Length - 1)
        //                    {
        //                        hotkey = value[detectedHotKeyPrefix + 1];
        //                        return (Keys)char.ToUpper(hotkey);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return Keys.None;
        //}

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new Form1());

            //Keys a = GetHotKeyFromString("Ok");
            //Keys b = GetHotKeyFromString("&Ok");
            //Keys c = GetHotKeyFromString("Bearbe&iten");
            //Keys d = GetHotKeyFromString("&");
            //return;
        }
    }
}