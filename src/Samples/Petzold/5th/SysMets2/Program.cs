﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using WInterop.Gdi;
using WInterop.Gdi.Types;
using WInterop.Modules.Types;
using WInterop.Resources;
using WInterop.Resources.Types;
using WInterop.Windows;
using WInterop.Windows.Types;

namespace SysMets2
{
    /// <summary>
    /// Sample from Programming Windows, 5th Edition.
    /// Original (c) Charles Petzold, 1998
    /// Figure 4-10, Pages 103-106.
    /// </summary>
    static class Program
    {
        [STAThread]
        static void Main()
        {
            SafeModuleHandle module = Marshal.GetHINSTANCE(typeof(Program).Module);
            WindowClass wndclass = new WindowClass
            {
                Style = WindowClassStyle.CS_HREDRAW | WindowClassStyle.CS_VREDRAW,
                WindowProcedure = WindowProcedure,
                Instance = module,
                Icon = ResourceMethods.LoadIcon(IconId.IDI_APPLICATION),
                Cursor = ResourceMethods.LoadCursor(CursorId.IDC_ARROW),
                Background = GdiMethods.GetStockBrush(StockBrush.WHITE_BRUSH),
                ClassName = "SysMets2"
            };

            WindowMethods.RegisterClass(wndclass);

            WindowHandle window = WindowMethods.CreateWindow(
                module,
                "SysMets2",
                "Get System Metrics No. 2",
                WindowStyle.WS_OVERLAPPEDWINDOW | WindowStyle.WS_VSCROLL);

            WindowMethods.ShowWindow(window, ShowWindowCommand.SW_SHOWNORMAL);
            GdiMethods.UpdateWindow(window);

            while (WindowMethods.GetMessage(out MSG message, WindowHandle.Null, 0, 0))
            {
                WindowMethods.TranslateMessage(ref message);
                WindowMethods.DispatchMessage(ref message);
            }
        }

        static int cxChar, cxCaps, cyChar, cyClient, iVscrollPos;

        static LRESULT WindowProcedure(WindowHandle window, MessageType message, WPARAM wParam, LPARAM lParam)
        {
            switch (message)
            {
                case MessageType.WM_CREATE:
                    using (DeviceContext dc = GdiMethods.GetDeviceContext(window))
                    {
                        GdiMethods.GetTextMetrics(dc, out TEXTMETRIC tm);
                        cxChar = tm.tmAveCharWidth;
                        cxCaps = ((tm.tmPitchAndFamily & PitchAndFamily.TMPF_FIXED_PITCH) != 0 ? 3 : 2) * cxChar / 2;
                        cyChar = tm.tmHeight + tm.tmExternalLeading;
                    }

                    WindowMethods.SetScrollRange(window, ScrollBar.SB_VERT, 0, SysMets.sysmetrics.Count - 1, false);
                    WindowMethods.SetScrollPosition(window, ScrollBar.SB_VERT, iVscrollPos, true);

                    return 0;
                case MessageType.WM_SIZE:
                    cyClient = lParam.HighWord;
                    return 0;
                case MessageType.WM_VSCROLL:
                    switch ((ScrollBarCommand)wParam.LowWord)
                    {
                        case ScrollBarCommand.SB_LINEUP:
                            iVscrollPos -= 1;
                            break;
                        case ScrollBarCommand.SB_LINEDOWN:
                            iVscrollPos += 1;
                            break;
                        case ScrollBarCommand.SB_PAGEUP:
                            iVscrollPos -= cyClient / cyChar;
                            break;
                        case ScrollBarCommand.SB_PAGEDOWN:
                            iVscrollPos += cyClient / cyChar;
                            break;
                        case ScrollBarCommand.SB_THUMBPOSITION:
                            iVscrollPos = wParam.HighWord;
                            break;
                    }

                    iVscrollPos = Math.Max(0, Math.Min(iVscrollPos, SysMets.sysmetrics.Count - 1));

                    if (iVscrollPos != WindowMethods.GetScrollPosition(window, ScrollBar.SB_VERT))
                    {
                        WindowMethods.SetScrollPosition(window, ScrollBar.SB_VERT, iVscrollPos, true);
                        GdiMethods.InvalidateRectangle(window, true);
                    }
                    return 0;
                case MessageType.WM_PAINT:
                    using (DeviceContext dc = GdiMethods.BeginPaint(window))
                    {
                        int i = 0;
                        foreach (SystemMetric metric in SysMets.sysmetrics.Keys)
                        {
                            int y = cyChar * (i - iVscrollPos);

                            GdiMethods.TextOut(dc, 0, y, metric.ToString());
                            GdiMethods.TextOut(dc, 22 * cxCaps, y, SysMets.sysmetrics[metric]);
                            GdiMethods.SetTextAlignment(dc, TextAlignment.TA_RIGHT | TextAlignment.TA_TOP);
                            GdiMethods.TextOut(dc, 22 * cxCaps + 40 * cxChar, y, WindowMethods.GetSystemMetrics(metric).ToString());
                            GdiMethods.SetTextAlignment(dc, TextAlignment.TA_LEFT | TextAlignment.TA_TOP);
                            i++;
                        }
                    }
                    return 0;
                case MessageType.WM_DESTROY:
                    WindowMethods.PostQuitMessage(0);
                    return 0;
            }

            return WindowMethods.DefaultWindowProcedure(window, message, wParam, lParam);
        }
    }
}
