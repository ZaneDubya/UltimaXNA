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
        public Texture2D Texture;
        public List<DisplayCharacter> Characters;

        public DisplayString(Texture2D texture)
        {
            Texture = texture;
        }

        public void AddCharacter(ACharacter character, Point loc, int hue)
        {
            Characters.Add(new DisplayCharacter(character, loc, hue));
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
}
