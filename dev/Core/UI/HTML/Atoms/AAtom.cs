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
using UltimaXNA.Core.UI.HTML.Atoms;
using UltimaXNA.Core.UI.HTML.Styles;
#endregion

namespace UltimaXNA.Core.UI.HTML.Atoms
{
    public abstract class AAtom
    {
        public virtual int Width
        {
            set { }
            get { return 0; }
        }

        public virtual int Height
        {
            set { }
            get { return 0; }
        }

        public bool CanBreakAtThisAtom
        {
            get
            {
                if (this is CharacterAtom)
                {
                    CharacterAtom atom = (CharacterAtom)this;
                    if (atom.Character == ' ' || atom.Character == '\n')
                        return true;
                    else
                        return false;
                }
                return true;
            }
        }

        public bool IsThisAtomABreakingSpace
        {
            get
            {
                if (this is CharacterAtom)
                {
                    CharacterAtom atom = (CharacterAtom)this;
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
                if (this is CharacterAtom)
                {
                    CharacterAtom atom = (CharacterAtom)this;
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
        public AAtom(StyleState style)
        {
            Style = new StyleState(style);
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}
