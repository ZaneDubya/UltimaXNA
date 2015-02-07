/***************************************************************************
 *   Features.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;

namespace UltimaXNA.UltimaVars
{
    static class Features
    {
        static uint _featureFlags;
        public static bool EnableT2A { get { return ((_featureFlags & 0x1) != 0); } }
        public static bool EnableRen { get { return ((_featureFlags & 0x2) != 0); } }
        public static bool EnableThirdDawn { get { return ((_featureFlags & 0x4) != 0); } }
        public static bool EnableLBR { get { return ((_featureFlags & 0x8) != 0); } }
        public static bool EnableAOS { get { return ((_featureFlags & 0x10) != 0); } }
        public static bool Enable6CharSlots { get { return ((_featureFlags & 0x20) != 0); } }
        public static bool EnableSE { get { return ((_featureFlags & 0x40) != 0); } }
        public static bool EnableML { get { return ((_featureFlags & 0x80) != 0); } }
        public static bool Enable8thSplash { get { return ((_featureFlags & 0x100) != 0); } }
        public static bool Enable9thSplash { get { return ((_featureFlags & 0x200) != 0); } }
        public static bool Enable10thAge { get { return ((_featureFlags & 0x400) != 0); } }
        public static bool EnableMoreStorage { get { return ((_featureFlags & 0x800) != 0); } }
        public static bool Enable7CharSlots { get { return ((_featureFlags & 0x1000) != 0); } }
        public static bool Enable10thAgeFaces { get { return ((_featureFlags & 0x2000) != 0); } }
        public static bool EnableTrialAccount { get { return ((_featureFlags & 0x4000) != 0); } }
        public static bool Enable11thAge { get { return ((_featureFlags & 0x8000) != 0); } }
        public static bool EnableSA { get { return ((_featureFlags & 0x10000) != 0); } }

        public static void SetFlags(uint flags)
        {
            _featureFlags |= flags;
        }
    }
}
