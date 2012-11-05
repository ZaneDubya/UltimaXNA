﻿/***************************************************************************
 *   StatusGump.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code by surcouf94
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UltimaXNA.Entity;
using UltimaXNA.Graphics;
using UltimaXNA.GUI;
using UltimaXNA.UltimaGUI.Controls;

namespace UltimaXNA.UltimaGUI.ClientsideGumps
{
    class StatusGump : Gump
    {
        List<Control> ControlsToUpdate = new List<Control>();
        PlayerMobile m = (PlayerMobile)Entities.GetPlayerObject();
        GameTime _refreshTime = new GameTime();

        public StatusGump()
            : base(0, 0)
        {
            IsMovable = true;
            AddControl(new GumpPic(this, 0, 0, 0, 0x2A6C, 0));
            LastControl.MakeDragger(this);
            LastControl.MakeCloseTarget(this);


            AddControl(new TextLabelAscii(this, 0, 54, 44, 1, 6, "" + m.Name.ToString()));
            ControlsToUpdate.Add(LastControl);
            AddControl(new TextLabelAscii(this, 0, 88, 71, 1, 6, "" + m.Strength));
            ControlsToUpdate.Add(LastControl);
            AddControl(new TextLabelAscii(this, 0, 88, 99, 1, 6, "" + m.Dexterity));
            ControlsToUpdate.Add(LastControl);
            AddControl(new TextLabelAscii(this, 0, 87, 127, 1, 6, "" + m.Intelligence));
            ControlsToUpdate.Add(LastControl);

            AddControl(new TextLabelAscii(this, 0, 147, 67, 1, 6, "" + m.Health.Current));
            ControlsToUpdate.Add(LastControl);
            AddControl(new TextLabelAscii(this, 0, 147, 77, 1, 6, "" + m.Health.Max));
            ControlsToUpdate.Add(LastControl);


            AddControl(new TextLabelAscii(this, 0, 147, 94, 1, 6, "" + m.Stamina.Current));
            ControlsToUpdate.Add(LastControl);
            AddControl(new TextLabelAscii(this, 0, 147, 105, 1, 6, "" + m.Stamina.Max));
            ControlsToUpdate.Add(LastControl);

            AddControl(new TextLabelAscii(this, 0, 148, 122, 1, 6, "" + m.Mana.Current));
            ControlsToUpdate.Add(LastControl);
            AddControl(new TextLabelAscii(this, 0, 148, 133, 1, 6, "" + m.Mana.Max));
            ControlsToUpdate.Add(LastControl);

            AddControl(new TextLabelAscii(this, 0, 289, 127, 1, 6, "" + m.Followers.Current + "/" + m.Followers.Max));
            ControlsToUpdate.Add(LastControl);

            AddControl(new TextLabelAscii(this, 0, 212, 121, 1, 6, "" + m.Weight.Current));
            ControlsToUpdate.Add(LastControl);
            AddControl(new TextLabelAscii(this, 0, 212, 132, 1, 6, "" + m.Weight.Max));
            ControlsToUpdate.Add(LastControl);

            AddControl(new TextLabelAscii(this, 0, 219, 71, 1, 6, "" + m.StatCap));
            ControlsToUpdate.Add(LastControl);
            AddControl(new TextLabelAscii(this, 0, 218, 99, 1, 6, "" + m.Luck));
            ControlsToUpdate.Add(LastControl);
            AddControl(new TextLabelAscii(this, 0, 282, 99, 1, 6, "" + m.Gold));
            ControlsToUpdate.Add(LastControl);

            AddControl(new TextLabelAscii(this, 0, 352, 70, 1, 6, "" + m.ArmorRating));
            ControlsToUpdate.Add(LastControl);
            AddControl(new TextLabelAscii(this, 0, 352, 85, 1, 6, "" + m.ResistFire));
            ControlsToUpdate.Add(LastControl);
            AddControl(new TextLabelAscii(this, 0, 352, 100, 1, 6, "" + m.ResistCold));
            ControlsToUpdate.Add(LastControl);
            AddControl(new TextLabelAscii(this, 0, 352, 114, 1, 6, "" + m.ResistPoison));
            ControlsToUpdate.Add(LastControl);
            AddControl(new TextLabelAscii(this, 0, 352, 129, 1, 6, "" + m.ResistEnergy));
            ControlsToUpdate.Add(LastControl);

            AddControl(new TextLabelAscii(this, 0, 277, 70, 1, 6, "" + m.DamageMin + "/" + m.DamageMax));
            ControlsToUpdate.Add(LastControl);

        }

        public override void Update(GameTime gameTime)
        {
            if (_refreshTime.TotalGameTime.TotalSeconds + 0.5 < gameTime.TotalGameTime.TotalSeconds) //need to update
            {
                _refreshTime = new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime);

                foreach (Control c in ControlsToUpdate)
                    Controls.Remove(c);

                AddControl(new TextLabelAscii(this, 0, 54, 44, 1, 6, "" + m.Name.ToString()));
                ControlsToUpdate.Add(LastControl);
                AddControl(new TextLabelAscii(this, 0, 88, 71, 1, 6, "" + m.Strength));
                ControlsToUpdate.Add(LastControl);
                AddControl(new TextLabelAscii(this, 0, 88, 99, 1, 6, "" + m.Dexterity));
                ControlsToUpdate.Add(LastControl);
                AddControl(new TextLabelAscii(this, 0, 87, 127, 1, 6, "" + m.Intelligence));
                ControlsToUpdate.Add(LastControl);

                AddControl(new TextLabelAscii(this, 0, 147, 67, 1, 6, "" + m.Health.Current));
                ControlsToUpdate.Add(LastControl);
                AddControl(new TextLabelAscii(this, 0, 147, 77, 1, 6, "" + m.Health.Max));
                ControlsToUpdate.Add(LastControl);


                AddControl(new TextLabelAscii(this, 0, 147, 94, 1, 6, "" + m.Stamina.Current));
                ControlsToUpdate.Add(LastControl);
                AddControl(new TextLabelAscii(this, 0, 147, 105, 1, 6, "" + m.Stamina.Max));
                ControlsToUpdate.Add(LastControl);

                AddControl(new TextLabelAscii(this, 0, 148, 122, 1, 6, "" + m.Mana.Current));
                ControlsToUpdate.Add(LastControl);
                AddControl(new TextLabelAscii(this, 0, 148, 133, 1, 6, "" + m.Mana.Max));
                ControlsToUpdate.Add(LastControl);

                AddControl(new TextLabelAscii(this, 0, 289, 127, 1, 6, "" + m.Followers.Current + "/" + m.Followers.Max));
                ControlsToUpdate.Add(LastControl);

                AddControl(new TextLabelAscii(this, 0, 212, 121, 1, 6, "" + m.Weight.Current));
                ControlsToUpdate.Add(LastControl);
                AddControl(new TextLabelAscii(this, 0, 212, 132, 1, 6, "" + m.Weight.Max));
                ControlsToUpdate.Add(LastControl);

                AddControl(new TextLabelAscii(this, 0, 219, 71, 1, 6, "" + m.StatCap));
                ControlsToUpdate.Add(LastControl);
                AddControl(new TextLabelAscii(this, 0, 218, 99, 1, 6, "" + m.Luck));
                ControlsToUpdate.Add(LastControl);
                AddControl(new TextLabelAscii(this, 0, 282, 99, 1, 6, "" + m.Gold));
                ControlsToUpdate.Add(LastControl);

                AddControl(new TextLabelAscii(this, 0, 352, 70, 1, 6, "" + m.ArmorRating));
                ControlsToUpdate.Add(LastControl);
                AddControl(new TextLabelAscii(this, 0, 352, 85, 1, 6, "" + m.ResistFire));
                ControlsToUpdate.Add(LastControl);
                AddControl(new TextLabelAscii(this, 0, 352, 100, 1, 6, "" + m.ResistCold));
                ControlsToUpdate.Add(LastControl);
                AddControl(new TextLabelAscii(this, 0, 352, 114, 1, 6, "" + m.ResistPoison));
                ControlsToUpdate.Add(LastControl);
                AddControl(new TextLabelAscii(this, 0, 352, 129, 1, 6, "" + m.ResistEnergy));
                ControlsToUpdate.Add(LastControl);

                AddControl(new TextLabelAscii(this, 0, 277, 70, 1, 6, "" + m.DamageMin + "/" + m.DamageMax));
                ControlsToUpdate.Add(LastControl);

            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
