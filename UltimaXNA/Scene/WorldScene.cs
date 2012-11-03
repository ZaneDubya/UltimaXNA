/***************************************************************************
 *   WorldScene.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
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
#region usings
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UltimaXNA.Network;
using UltimaXNA.UILegacy;
using UltimaXNA.UILegacy.ClientsideGumps;
using UltimaXNA.Interface.Input;
#endregion

namespace UltimaXNA.Scene
{
    public class WorldScene : BaseScene
    {
        public WorldScene(Game game)
            : base(game)
        {
        }

        public override void  Intitialize()
        {
            base.Intitialize();
            UserInterface.AddGump_Local(new TopMenu(0), 0, 0);
            UserInterface.AddGump_Local(new ChatWindow(), 0, 0);

            // this is the login sequence for 0.6.1.10
            Interaction.GetMySkills();
            Interaction.SendClientVersion();
            Interaction.SendClientScreenSize();
            Interaction.SendClientLocalization();
            // Packet: BF 00 0A 00 0F 0A 00 00 00 1F
            // Packet: 09 00 00 00 02  
            // Packet: 06 80 00 00 17
            Interaction.GetMyBasicStatus();
            // Packet: D6 00 0B 00 00 00 02 00 00 00 17
            // Packet: D6 00 37 40 00 00 FB 40 00 00 FD 40 00 00 FE 40
            //         00 00 FF 40 00 01 00 40 00 01 02 40 00 01 03 40
            //         00 01 04 40 00 01 05 40 00 01 06 40 00 01 07 40
            //         00 01 24 40 00 01 26 
            IsometricRenderer.LightDirection = -0.6f;
            ClientVars.EngineVars.InWorld = true;
        }

        public override void Dispose()
        {
            base.Dispose();
            UltimaClient.Disconnect();
            ClientVars.EngineVars.InWorld = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (SceneState == SceneState.Active)
            {
                if (!UltimaClient.IsConnected)
                {
                    if (UserInterface.IsModalMsgBoxOpen == false)
                    {
                        MsgBox g = UserInterface.MsgBox("You have lost your connection with the server.", MsgBoxTypes.OkOnly);
                        g.OnClose = onCloseLostConnectionMsgBox;
                    }
                }
                else
                {
                    IsometricRenderer.CenterPosition = Entities.GetPlayerObject().Position;
                    IsometricRenderer.Update(gameTime);

                    // Toggle for logout
                    if (InputState.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.Q, false, false, true))
                        Interaction.DisconnectToLoginScreen();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (SceneState == SceneState.Active)
            {
                IsometricRenderer.Draw(gameTime);
            }
        }

        void onCloseLostConnectionMsgBox()
        {
            Interaction.DisconnectToLoginScreen();
        }
    }
}
