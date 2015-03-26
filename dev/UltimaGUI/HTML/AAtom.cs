/***************************************************************************
 *   AAtom.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using UltimaXNA.UltimaGUI.HTML.Atoms;
#endregion

namespace UltimaXNA.UltimaGUI.HTML
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

        public bool Style_IsBold = false;
        public bool Style_IsItalic = false;
        public bool Style_IsOutlined = false;
        bool m_isUnderlined = false;
        public bool Style_IsUnderlined
        {
            get
            {
                if (HREFAttributes != null)
                {
                    return HREFAttributes.Underline;
                }
                else
                {
                    return m_isUnderlined;
                }
            }
            set { m_isUnderlined = value; }
        }
        public Fonts Font = Fonts.Default;
        public Alignments Alignment = Alignments.Default;
        public Color Color = Color.White;
        public HREF_Attributes HREFAttributes = null;
        public bool IsHREF { get { return HREFAttributes != null; } }

        public AAtom()
        {

        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}
