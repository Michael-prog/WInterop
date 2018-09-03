﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using WInterop.Errors;
using WInterop.Windows.Unsafe;

namespace WInterop.Windows
{
    public class SystemParameters
    {
        private SystemParameters() { }

        public static SystemParameters Instance { get; } = new SystemParameters();

        public unsafe bool GetBeep()
        {
            Boolean32 beeperOn = new Boolean32();
            if (!Imports.SystemParametersInfoW(SystemParameterType.SPI_GETBEEP, 0, &beeperOn, 0))
                throw Error.GetExceptionForLastError();
            return beeperOn;
        }

        public unsafe void SetBeep(bool beeperOn, SystemParameterOptions options = 0)
        {
            if (!Imports.SystemParametersInfoW(SystemParameterType.SPI_SETBEEP, (Boolean32)beeperOn, null, 0))
                throw Error.GetExceptionForLastError();
        }

        public unsafe bool GetBlockSendInputResets()
        {
            Boolean32 simulatedInputBlocked = new Boolean32();
            if (!Imports.SystemParametersInfoW(SystemParameterType.SPI_GETBLOCKSENDINPUTRESETS, 0, &simulatedInputBlocked, 0))
                throw Error.GetExceptionForLastError();
            return simulatedInputBlocked;
        }

        public unsafe void SetBlockSendInputResets(bool simulatedInputBlocked, SystemParameterOptions options = 0)
        {
            if (!Imports.SystemParametersInfoW(SystemParameterType.SPI_SETBLOCKSENDINPUTRESETS, (Boolean32)simulatedInputBlocked, null, 0))
                throw Error.GetExceptionForLastError();
        }

        public unsafe uint GetWheelScrollLines()
        {
            uint linesToScroll;
            if (!Imports.SystemParametersInfoW(SystemParameterType.SPI_GETWHEELSCROLLLINES, 0, &linesToScroll, 0))
                throw Error.GetExceptionForLastError();
            return linesToScroll;
        }
    }
}
