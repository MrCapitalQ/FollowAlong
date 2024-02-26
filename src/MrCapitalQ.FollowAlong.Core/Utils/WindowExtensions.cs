﻿using Microsoft.UI.Xaml;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MrCapitalQ.FollowAlong.Core.Utils
{
    [ExcludeFromCodeCoverage]
    public static class WindowExtensions
    {
        const uint WDA_NONE = 0x00000000;
        const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;

        [DllImport("user32.dll")]
        private static extern uint SetWindowDisplayAffinity(IntPtr hwnd, uint dwAffinity);

        public static void SetIsExcludedFromCapture(this Window window, bool isExcludedFromCapture)
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            _ = SetWindowDisplayAffinity(hWnd, isExcludedFromCapture ? WDA_EXCLUDEFROMCAPTURE : WDA_NONE);
        }
    }
}
