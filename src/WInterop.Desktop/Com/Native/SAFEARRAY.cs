﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace WInterop.Com.Native
{
    public unsafe struct SAFEARRAY
    {
        public ushort cDims;
        public ushort fFeatures;
        public uint cbElements;
        public uint cLocks;
        public void* pvData;

        public SafeArrayBound _rgsabound;

        public ReadOnlySpan<SafeArrayBound> rgsabound => TrailingArray<SafeArrayBound>.GetBuffer(ref _rgsabound, cDims);
    }
}
