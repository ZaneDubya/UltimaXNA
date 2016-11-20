/***************************************************************************
 *   Afont.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using System.IO;
#endregion

namespace UltimaXNA.Core.UI.Fonts
{
    abstract class AFont : IFont
    {
        public bool HasBuiltInOutline
        {
            set;
            get;
        }

        public int Height
        {
            get;
            set;
        }

        public int Baseline
        {
            get
            {
                return GetCharacter('M').Height + GetCharacter('M').YOffset;
            }
        }

        public abstract ICharacter GetCharacter(char character);

        public abstract void Initialize(BinaryReader reader);
    }
}
