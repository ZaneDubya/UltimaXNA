/***************************************************************************
 *   HTMLParserAtoms.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using UltimaXNA.Diagnostics;
using UltimaXNA.UltimaData.Fonts;

namespace UltimaXNA.UltimaGUI.HTML
{
    public class HTMLImageGump : AHTMLAtom
    {
        public HTMLImage AssociatedImage;

        private int m_overrideWidth = -1;
        public override int Width
        {
            set
            {
                m_overrideWidth = value;
            }
            get
            {
                if (m_overrideWidth != -1)
                    return m_overrideWidth + 1;
                Texture2D gump = UltimaData.GumpData.GetGumpXNA(Value);
                return gump.Width + 1;
            }
        }

        private int m_overrideHeight = -1;
        public override int Height
        {
            set
            {
                m_overrideHeight = value;
            }
            get
            {
                if (m_overrideHeight != -1)
                    return m_overrideHeight;
                Texture2D gump = UltimaData.GumpData.GetGumpXNA(Value);
                return gump.Height;
            }
        }

        public Texture2D Texture
        {
            get
            {
                return UltimaData.GumpData.GetGumpXNA(Value);
            }
        }

        public int Value = -1;
        public int ValueDown = -1;
        public int ValueOver = -1;

        public HTMLImageGump(int value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("<gImg {0}>", Value);
        }
    }

    public class HTMLAtomCharacter : AHTMLAtom
    {
        public override int Width
        {
            get
            {
                if (Character < 32)
                    return 0;
                return UniText.Fonts[(int)Font].GetCharacter(Character).Width + (Style_IsBold ? 1 : 0) + 1;
            }
        }

        public override int Height
        {
            get
            {
                return UniText.Fonts[(int)Font].Lineheight;
            }
        }

        public char Character = ' ';

        public HTMLAtomCharacter(char c)
        {
            Character = c;
        }

        public override string ToString()
        {
            return Character.ToString();
        }
    }

    public class HTMLAtomSpan : AHTMLAtom
    {
        private int m_width = 0;
        public override int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        private int m_height = 0;
        public override int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        public HTMLAtomSpan()
        {
            m_height = UniText.Fonts[(int)Font].Lineheight;
        }
    }

    public abstract class AHTMLAtom
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
                if (this is HTMLAtomCharacter)
                {
                    HTMLAtomCharacter atom = (HTMLAtomCharacter)this;
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
                if (this is HTMLAtomCharacter)
                {
                    HTMLAtomCharacter atom = (HTMLAtomCharacter)this;
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
                if (this is HTMLAtomCharacter)
                {
                    HTMLAtomCharacter atom = (HTMLAtomCharacter)this;
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

        public AHTMLAtom()
        {

        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}
