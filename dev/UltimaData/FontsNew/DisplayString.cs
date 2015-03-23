using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UltimaData.FontsNew
{
    class DisplayString
    {
        internal Texture2D Texture;
        internal List<DisplayCharacter> Characters;

        internal DisplayString(Texture2D texture)
        {
            Texture = texture;
        }

        internal void AddCharacter(ACharacter character, Point loc, int hue)
        {
            Characters.Add(new DisplayCharacter(character, loc, hue));
        }
    }

    class DisplayCharacter
    {
        ACharacter Character;
        Point Location;
        int Hue;

        public DisplayCharacter(ACharacter character, Point loc, int hue)
        {
            Character = character;
            Location = loc;
            Hue = hue;
        }
    }
}
