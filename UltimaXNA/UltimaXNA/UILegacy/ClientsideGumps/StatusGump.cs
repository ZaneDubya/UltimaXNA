/***************************************************************************
 *   StatusGump.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *   
 *   Thanks surcouf94!
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Input;
using UltimaXNA.UILegacy.Gumplings;
using UltimaXNA.Entities;

namespace UltimaXNA.UILegacy.ClientsideGumps
{
    class StatusGump : Gump
    {
        List<Control> gumplingsToUpdate = new List<Control>();
        PlayerMobile m = (PlayerMobile)EntitiesCollection.GetPlayerObject();
        GameTime _refreshTime = new GameTime();

        public StatusGump()
            : base(0, 0)
        {
            IsMovable = true;
            AddGumpling(new GumpPic(this, 0, 0, 0, 0x2A6C, 0));
            LastGumpling.MakeADragger(this);
            LastGumpling.OnMouseClick += onGumpClick;


            AddGumpling(new TextLabelAscii(this, 0, 54, 44, 1, 6, "" + m.Name.ToString()));
            gumplingsToUpdate.Add(LastGumpling);
            AddGumpling(new TextLabelAscii(this, 0, 88, 71, 1, 6, "" + m.Strength));
            gumplingsToUpdate.Add(LastGumpling);
            AddGumpling(new TextLabelAscii(this, 0, 88, 99, 1, 6, "" + m.Dexterity));
            gumplingsToUpdate.Add(LastGumpling);
            AddGumpling(new TextLabelAscii(this, 0, 87, 127, 1, 6, "" + m.Intelligence));
            gumplingsToUpdate.Add(LastGumpling);

            AddGumpling(new TextLabelAscii(this, 0, 147, 67, 1, 6, "" + m.Health.Current));
            gumplingsToUpdate.Add(LastGumpling);
            AddGumpling(new TextLabelAscii(this, 0, 147, 77, 1, 6, "" + m.Health.Max));
            gumplingsToUpdate.Add(LastGumpling);


            AddGumpling(new TextLabelAscii(this, 0, 147, 94, 1, 6, "" + m.Stamina.Current));
            gumplingsToUpdate.Add(LastGumpling);
            AddGumpling(new TextLabelAscii(this, 0, 147, 105, 1, 6, "" + m.Stamina.Max));
            gumplingsToUpdate.Add(LastGumpling);

            AddGumpling(new TextLabelAscii(this, 0, 148, 122, 1, 6, "" + m.Mana.Current));
            gumplingsToUpdate.Add(LastGumpling);
            AddGumpling(new TextLabelAscii(this, 0, 148, 133, 1, 6, "" + m.Mana.Max));
            gumplingsToUpdate.Add(LastGumpling);

            AddGumpling(new TextLabelAscii(this, 0, 289, 127, 1, 6, "" + m.Followers.Current + "/" + m.Followers.Max));
            gumplingsToUpdate.Add(LastGumpling);

            AddGumpling(new TextLabelAscii(this, 0, 212, 121, 1, 6, "" + m.Weight.Current));
            gumplingsToUpdate.Add(LastGumpling);
            AddGumpling(new TextLabelAscii(this, 0, 212, 132, 1, 6, "" + m.Weight.Max));
            gumplingsToUpdate.Add(LastGumpling);

            AddGumpling(new TextLabelAscii(this, 0, 219, 71, 1, 6, "" + m.StatCap));
            gumplingsToUpdate.Add(LastGumpling);
            AddGumpling(new TextLabelAscii(this, 0, 218, 99, 1, 6, "" + m.Luck));
            gumplingsToUpdate.Add(LastGumpling);
            AddGumpling(new TextLabelAscii(this, 0, 282, 99, 1, 6, "" + m.Gold));
            gumplingsToUpdate.Add(LastGumpling);

            AddGumpling(new TextLabelAscii(this, 0, 352, 70, 1, 6, "" + m.ArmorRating));
            gumplingsToUpdate.Add(LastGumpling);
            AddGumpling(new TextLabelAscii(this, 0, 352, 85, 1, 6, "" + m.ResistFire));
            gumplingsToUpdate.Add(LastGumpling);
            AddGumpling(new TextLabelAscii(this, 0, 352, 100, 1, 6, "" + m.ResistCold));
            gumplingsToUpdate.Add(LastGumpling);
            AddGumpling(new TextLabelAscii(this, 0, 352, 114, 1, 6, "" + m.ResistPoison));
            gumplingsToUpdate.Add(LastGumpling);
            AddGumpling(new TextLabelAscii(this, 0, 352, 129, 1, 6, "" + m.ResistEnergy));
            gumplingsToUpdate.Add(LastGumpling);

            AddGumpling(new TextLabelAscii(this, 0, 277, 70, 1, 6, "" + m.DamageMin + "/" + m.DamageMax));
            gumplingsToUpdate.Add(LastGumpling);

        }

        void onGumpClick(int x, int y, MouseButtons button)
        {
            if (button == MouseButtons.RightButton)
            {
                Dispose();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (_refreshTime.TotalRealTime.TotalSeconds + 0.5 < gameTime.TotalRealTime.TotalSeconds) //need to update
            {
                _refreshTime = new GameTime(gameTime.TotalRealTime, gameTime.ElapsedRealTime, gameTime.TotalGameTime, gameTime.ElapsedGameTime);

                foreach (Control c in gumplingsToUpdate)
                    _controls.Remove(c);

                AddGumpling(new TextLabelAscii(this, 0, 54, 44, 1, 6, "" + m.Name.ToString()));
                gumplingsToUpdate.Add(LastGumpling);
                AddGumpling(new TextLabelAscii(this, 0, 88, 71, 1, 6, "" + m.Strength));
                gumplingsToUpdate.Add(LastGumpling);
                AddGumpling(new TextLabelAscii(this, 0, 88, 99, 1, 6, "" + m.Dexterity));
                gumplingsToUpdate.Add(LastGumpling);
                AddGumpling(new TextLabelAscii(this, 0, 87, 127, 1, 6, "" + m.Intelligence));
                gumplingsToUpdate.Add(LastGumpling);

                AddGumpling(new TextLabelAscii(this, 0, 147, 67, 1, 6, "" + m.Health.Current));
                gumplingsToUpdate.Add(LastGumpling);
                AddGumpling(new TextLabelAscii(this, 0, 147, 77, 1, 6, "" + m.Health.Max));
                gumplingsToUpdate.Add(LastGumpling);


                AddGumpling(new TextLabelAscii(this, 0, 147, 94, 1, 6, "" + m.Stamina.Current));
                gumplingsToUpdate.Add(LastGumpling);
                AddGumpling(new TextLabelAscii(this, 0, 147, 105, 1, 6, "" + m.Stamina.Max));
                gumplingsToUpdate.Add(LastGumpling);

                AddGumpling(new TextLabelAscii(this, 0, 148, 122, 1, 6, "" + m.Mana.Current));
                gumplingsToUpdate.Add(LastGumpling);
                AddGumpling(new TextLabelAscii(this, 0, 148, 133, 1, 6, "" + m.Mana.Max));
                gumplingsToUpdate.Add(LastGumpling);

                AddGumpling(new TextLabelAscii(this, 0, 289, 127, 1, 6, "" + m.Followers.Current + "/" + m.Followers.Max));
                gumplingsToUpdate.Add(LastGumpling);

                AddGumpling(new TextLabelAscii(this, 0, 212, 121, 1, 6, "" + m.Weight.Current));
                gumplingsToUpdate.Add(LastGumpling);
                AddGumpling(new TextLabelAscii(this, 0, 212, 132, 1, 6, "" + m.Weight.Max));
                gumplingsToUpdate.Add(LastGumpling);

                AddGumpling(new TextLabelAscii(this, 0, 219, 71, 1, 6, "" + m.StatCap));
                gumplingsToUpdate.Add(LastGumpling);
                AddGumpling(new TextLabelAscii(this, 0, 218, 99, 1, 6, "" + m.Luck));
                gumplingsToUpdate.Add(LastGumpling);
                AddGumpling(new TextLabelAscii(this, 0, 282, 99, 1, 6, "" + m.Gold));
                gumplingsToUpdate.Add(LastGumpling);

                AddGumpling(new TextLabelAscii(this, 0, 352, 70, 1, 6, "" + m.ArmorRating));
                gumplingsToUpdate.Add(LastGumpling);
                AddGumpling(new TextLabelAscii(this, 0, 352, 85, 1, 6, "" + m.ResistFire));
                gumplingsToUpdate.Add(LastGumpling);
                AddGumpling(new TextLabelAscii(this, 0, 352, 100, 1, 6, "" + m.ResistCold));
                gumplingsToUpdate.Add(LastGumpling);
                AddGumpling(new TextLabelAscii(this, 0, 352, 114, 1, 6, "" + m.ResistPoison));
                gumplingsToUpdate.Add(LastGumpling);
                AddGumpling(new TextLabelAscii(this, 0, 352, 129, 1, 6, "" + m.ResistEnergy));
                gumplingsToUpdate.Add(LastGumpling);

                AddGumpling(new TextLabelAscii(this, 0, 277, 70, 1, 6, "" + m.DamageMin + "/" + m.DamageMax));
                gumplingsToUpdate.Add(LastGumpling);

            }

            base.Update(gameTime);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        void logout_OnClose()
        {
            _manager.RequestLogout();
        }

    }
}
