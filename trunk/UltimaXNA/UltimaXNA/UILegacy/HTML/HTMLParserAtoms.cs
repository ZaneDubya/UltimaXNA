using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using UltimaXNA.Diagnostics;

namespace UltimaXNA.UILegacy.HTML
{
    public class HTMLParser_AtomImageGump : HTMLParser_Atom
    {
        public HTMLImage AssociatedImage;

        private int _overrideWidth = -1;
        public override int Width
        {
            set
            {
                _overrideWidth = value;
            }
            get
            {
                if (_overrideWidth != -1)
                    return _overrideWidth + 1;
                Texture2D gump = Data.Gumps.GetGumpXNA(Value);
                return gump.Width + 1;
            }
        }

        private int _overrideHeight = -1;
        public override int Height
        {
            set
            {
                _overrideHeight = value;
            }
            get
            {
                if (_overrideHeight != -1)
                    return _overrideHeight;
                Texture2D gump = Data.Gumps.GetGumpXNA(Value);
                return gump.Height;
            }
        }

        public Texture2D Texture
        {
            get
            {
                return Data.Gumps.GetGumpXNA(Value);
            }
        }

        public int Value = -1;
        public int ValueDown = -1;
        public int ValueOver = -1;

        public HTMLParser_AtomImageGump(int value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("<gImg {0}>", Value);
        }
    }

    public class HTMLParser_AtomCharacter : HTMLParser_Atom
    {
        public override int Width
        {
            get
            {
                if (Character < 32)
                    return 0;
                return Data.UniText.Fonts[(int)Font].GetCharacter(Character).Width + (Style_IsBold ? 1 : 0) + 1;
            }
        }

        public override int Height
        {
            get
            {
                return Data.UniText.Fonts[(int)Font].Lineheight;
            }
        }

        public char Character = ' ';

        public HTMLParser_AtomCharacter(char c)
        {
            Character = c;
        }

        public override string ToString()
        {
            return Character.ToString();
        }
    }

    public class HTMLParser_AtomSpan : HTMLParser_Atom
    {
        private int _width = 0;
        public override int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        private int _height = 0;
        public override int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public HTMLParser_AtomSpan()
        {
            _height = Data.UniText.Fonts[(int)Font].Lineheight;
        }
    }

    public abstract class HTMLParser_Atom
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
                if (this is HTMLParser_AtomCharacter)
                {
                    HTMLParser_AtomCharacter atom = (HTMLParser_AtomCharacter)this;
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
                if (this is HTMLParser_AtomCharacter)
                {
                    HTMLParser_AtomCharacter atom = (HTMLParser_AtomCharacter)this;
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
                if (this is HTMLParser_AtomCharacter)
                {
                    HTMLParser_AtomCharacter atom = (HTMLParser_AtomCharacter)this;
                    if (atom.Character == '\n')
                        return true;
                }
                return false;
            }
        }

        public bool Style_IsBold = false;
        public bool Style_IsItalic = false;
        public bool Style_IsOutlined = false;
        bool _isUnderlined = false;
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
                    return _isUnderlined;
                }
            }
            set { _isUnderlined = value; }
        }
        public enumHTMLFonts Font = enumHTMLFonts.Default;
        public enumHTMLAlignments Alignment = enumHTMLAlignments.Default;
        public Color Color = Color.White;
        public HREF_Attributes HREFAttributes = null;
        public bool IsHREF { get { return HREFAttributes != null; } }

        public HTMLParser_Atom()
        {

        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}
