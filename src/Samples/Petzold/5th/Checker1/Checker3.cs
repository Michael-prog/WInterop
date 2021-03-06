﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Drawing;
using WInterop.Gdi;
using WInterop.Windows;

namespace Checker
{
    class Checker3 : WindowClass
    {
        const int DIVISIONS = 5;
        WindowHandle[,] hwndChild = new WindowHandle[DIVISIONS, DIVISIONS];

        int cxBlock, cyBlock;
        Checker3Child _childClass = (Checker3Child)(new Checker3Child().Register());

        protected override LResult WindowProcedure(WindowHandle window, MessageType message, WParam wParam, LParam lParam)
        {
            switch (message)
            {
                case MessageType.Create:
                    for (int x = 0; x < DIVISIONS; x++)
                        for (int y = 0; y < DIVISIONS; y++)
                            hwndChild[x, y] = _childClass.CreateWindow(
                                style: WindowStyles.ChildWindow | WindowStyles.Visible,
                                parentWindow: window);
                    return 0;
                case MessageType.Size:
                    cxBlock = lParam.LowWord / DIVISIONS;
                    cyBlock = lParam.HighWord / DIVISIONS;
                    for (int x = 0; x < DIVISIONS; x++)
                        for (int y = 0; y < DIVISIONS; y++)
                            hwndChild[x, y].MoveWindow(
                                new Rectangle(x * cxBlock, y * cyBlock, cxBlock, cyBlock),
                                repaint: true);
                    return 0;
                case MessageType.LeftButtonDown:
                    Windows.MessageBeep(BeepType.Ok);
                    return 0;
            }

            return base.WindowProcedure(window, message, wParam, lParam);
        }
    }

    class Checker3Child : WindowClass
    {
        public Checker3Child()
            : base(windowExtraBytes: IntPtr.Size)
        {
        }

        protected override LResult WindowProcedure(WindowHandle window, MessageType message, WParam wParam, LParam lParam)
        {
            switch (message)
            {
                case MessageType.Create:
                    window.SetWindowLong(0, IntPtr.Zero); // on/off flag
                    return 0;
                case MessageType.LeftButtonDown:
                    window.SetWindowLong(0, (IntPtr)(1 ^ (int)window.GetWindowLong(0)));
                    window.Invalidate(false);
                    return 0;
                case MessageType.Paint:
                    using (DeviceContext dc = window.BeginPaint())
                    {
                        Rectangle rect = window.GetClientRectangle();
                        dc.Rectangle(rect);

                        if (window.GetWindowLong(0) != IntPtr.Zero)
                        {
                            dc.MoveTo(default);
                            dc.LineTo(new Point(rect.Right, rect.Bottom));
                            dc.MoveTo(new Point(0, rect.Bottom));
                            dc.LineTo(new Point(rect.Right, 0));
                        }
                    }
                    return 0;
            }

            return base.WindowProcedure(window, message, wParam, lParam);
        }
    }
}
