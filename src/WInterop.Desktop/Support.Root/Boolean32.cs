﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace WInterop
{
    /// <summary>
    /// Using bool in a struct prevents the struct from being blittable.
    /// This allows structs to marshal more effectively. [BOOL]
    /// </summary>
    public readonly struct Boolean32
    {
        public int RawValue { get; }

        public Boolean32(bool b) => RawValue = b ? 1 : 0;
        public Boolean32(int value) => RawValue = value;

        public readonly bool IsTrue => RawValue != 0;
        public readonly bool IsFalse => RawValue == 0;

        public static implicit operator bool(Boolean32 b) => b.IsTrue;
        public static implicit operator Boolean32(bool b) => new Boolean32(b);
        public static implicit operator uint(Boolean32 b) => (uint)b.RawValue;
        public static implicit operator Boolean32(uint b) => new Boolean32((int)b);

        public override string ToString() => IsTrue.ToString();
    }
}
