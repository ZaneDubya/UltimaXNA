/***************************************************************************
 *   CharacterAtom.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using UltimaXNA.Core.UI.HTML.Styles;

namespace UltimaXNA.Core.UI.HTML.Elements
{
    public class CharacterElement : AElement
    {
        public override int Width
        {
            get
            {
                if (Character < 32)
                {
                    return 0;
                }
                ICharacter ch = Style.Font.GetCharacter(Character);
                return ch.Width + ch.ExtraWidth + (Style.IsBold ? 1 : 0);
            }
            set
            {
                // does nothing
            }
        }

        public override int Height
        {
            get
            {
                return Style.Font.Height;
            }
            set
            {
                // does nothing
            }
        }

        public override bool CanBreakAtThisAtom
        {
            get
            {
                if (Character == ' ' || Character == '\n')
                {
                    return true;
                }
                return false;
            }
        }

        public override bool IsThisAtomABreakingSpace
        {
            get
            {
                if (Character == ' ')
                {
                    return true;
                }
                return false;
            }
        }

        public override bool IsThisAtomALineBreak
        {
            get
            {
                if (Character == '\n')
                {
                    return true;
                }
                return false;
            }
        }

        public char Character;

        public CharacterElement(StyleState style, char c)
            : base(style)
        {
            Character = c;
        }

        public override string ToString()
        {
            if (IsThisAtomALineBreak)
            {
                return @"\n";
            }
            return Character.ToString();
        }
    }
}
