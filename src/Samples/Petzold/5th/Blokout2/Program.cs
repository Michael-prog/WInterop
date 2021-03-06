﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Drawing;
using WInterop.Gdi;
using WInterop.Windows;

namespace Blokout2
{
    /// <summary>
    /// Sample from Programming Windows, 5th Edition.
    /// Original (c) Charles Petzold, 1998
    /// Figure 7-11, Pages 314-317.
    /// </summary>
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Windows.CreateMainWindowAndRun(new Blockout2(), "Mouse Button & Capture Demo");
        }
    }

    class Blockout2 : WindowClass
    {
        bool fBlocking, fValidBox;
        Point ptBeg, ptEnd, ptBoxBeg, ptBoxEnd;

        void DrawBoxOutline(WindowHandle window, Point ptBeg, Point ptEnd)
        {
            using (DeviceContext dc = window.GetDeviceContext())
            {
                dc.SetRasterOperation(PenMixMode.Not);
                dc.SelectObject(StockBrush.Null);
                dc.Rectangle(Rectangle.FromLTRB(ptBeg.X, ptBeg.Y, ptEnd.X, ptEnd.Y));
            }
        }

        protected override LResult WindowProcedure(WindowHandle window, MessageType message, WParam wParam, LParam lParam)
        {
            switch (message)
            {
                case MessageType.LeftButtonDown:
                    ptBeg.X = ptEnd.X = lParam.LowWord;
                    ptBeg.Y = ptEnd.Y = lParam.HighWord;
                    DrawBoxOutline(window, ptBeg, ptEnd);
                    window.SetCapture();
                    Windows.SetCursor(CursorId.Cross);
                    fBlocking = true;
                    return 0;
                case MessageType.MouseMove:
                    if (fBlocking)
                    {
                        Windows.SetCursor(CursorId.Cross);
                        DrawBoxOutline(window, ptBeg, ptEnd);
                        ptEnd.X = lParam.LowWord;
                        ptEnd.Y = lParam.HighWord;
                        DrawBoxOutline(window, ptBeg, ptEnd);
                    }
                    return 0;
                case MessageType.LeftButtonUp:
                    if (fBlocking)
                    {
                        DrawBoxOutline(window, ptBeg, ptEnd);
                        ptBoxBeg = ptBeg;
                        ptBoxEnd.X = lParam.LowWord;
                        ptBoxEnd.Y = lParam.HighWord;
                        Windows.ReleaseCapture();
                        Windows.SetCursor(CursorId.Arrow);
                        fBlocking = false;
                        fValidBox = true;
                        window.Invalidate(true);
                    }
                    return 0;
                case MessageType.Paint:
                    using (DeviceContext dc = window.BeginPaint())
                    {
                        if (fValidBox)
                        {
                            dc.SelectObject(StockBrush.Black);
                            dc.Rectangle(Rectangle.FromLTRB(ptBoxBeg.X, ptBoxBeg.Y, ptBoxEnd.X, ptBoxEnd.Y));
                        }
                        if (fBlocking)
                        {
                            dc.SetRasterOperation(PenMixMode.Not);
                            dc.SelectObject(StockBrush.Null);
                            dc.Rectangle(Rectangle.FromLTRB(ptBeg.X, ptBeg.Y, ptEnd.X, ptEnd.Y));
                        }
                    }
                    return 0;
                }

                return base.WindowProcedure(window, message, wParam, lParam);
        }
    }
}
