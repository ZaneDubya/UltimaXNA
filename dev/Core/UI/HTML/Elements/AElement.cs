/***************************************************************************
 *   AAtom.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.UI.HTML.Styles;
#endregion

namespace UltimaXNA.Core.UI.HTML.Elements
{
    public abstract class AElement
    {
        public abstract int Width { get; set; }
        public abstract int Height { get; set; }

        public int Layout_X;
        public int Layout_Y;

        public bool CanBreakAtThisAtom
        {
            get
            {
                if (this is CharacterElement)
                {
                    CharacterElement atom = (CharacterElement)this;
                    if (atom.Character == ' ' || atom.Character == '\n')
                    {
                        return true;
                    }
                    return false;
                }
                return true;
            }
        }

        public bool IsThisAtomABreakingSpace
        {
            get
            {
                if (this is CharacterElement)
                {
                    CharacterElement atom = (CharacterElement)this;
                    if (atom.Character == ' ')
                        return true;
                }
                return false;
            }
        }

        public bool IsThisAtomALineBreak
        {
            get
            {
                if (this is CharacterElement)
                {
                    CharacterElement atom = (CharacterElement)this;
                    if (atom.Character == '\n')
                        return true;
                }
                return false;
            }
        }

        public StyleState Style;

        /// <summary>
        /// Creates a new atom.
        /// </summary>
        /// <param name="openTags">This atom will copy the styles from this parameter.</param>
        public AElement(StyleState style)
        {
            Style = new StyleState(style);
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}
