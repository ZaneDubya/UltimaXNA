/***************************************************************************
 *   LoginScene.cs
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Network;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaGUI.Gumps;
using UltimaXNA.UltimaNetwork;
#endregion

namespace UltimaXNA.Scenes
{
    public class CharacterListScene : AScene
    {
        public CharacterListScene()
        {

        }

        public override void Intitialize()
        {
            Gump g = (Gump)UltimaEngine.UserInterface.AddControl(new CharacterListGump(), 0, 0);
            ((CharacterListGump)g).OnBackToSelectServer += this.OnBackToSelectServer;
            ((CharacterListGump)g).OnLoginWithCharacter += this.OnLoginWithCharacter;
            ((CharacterListGump)g).OnDeleteCharacter += this.OnDeleteCharacter;
            ((CharacterListGump)g).OnNewCharacter += this.OnNewCharacter;
        }

        public override void Update(double totalTime, double frameTime)
        {
            base.Update(totalTime, frameTime);

            if (SceneState == SceneState.Active)
            {
                switch (UltimaClient.Status)
                {
                    case UltimaClientStatus.GameServer_CharList:
                        // This is where we're supposed to be while waiting to select a character.
                        break;
                    case UltimaClientStatus.WorldServer_LoginComplete:
                        // Almost completed logging in, just waiting for our client object.
                        break;
                    case UltimaClientStatus.WorldServer_InWorld:
                        // We're in! Load the world.
                        Manager.CurrentScene = null;
                        UltimaEngine.ActiveModel = new UltimaXNA.UltimaWorld.WorldModel();
                        // Manager.CurrentScene = new WorldScene();
                        break;
                    default:
                        // what's going on here? Add additional error handlers.
                        throw (new Exception("Unknown UltimaClientStatus in CharacterListScene:Update"));
                }
            }
        }

        public override void Dispose()
        {
            UltimaEngine.UserInterface.GetControl<CharacterListGump>(0).Dispose();
            base.Dispose();
        }

        public void OnBackToSelectServer()
        {
            // !!! This SHOULD take us back to the 'logging in' screen,
            // which automatically logs in again. But we can't do that,
            // since I have UltimaClient clear your account/password data
            // once connected (is this really neccesary?) Have to fix this...
            Manager.CurrentScene = new LoginScene();
        }

        public void OnLoginWithCharacter(int index)
        {
            UltimaEngine.UserInterface.GetControl<CharacterListGump>(0).ActivePage = 2;
            UltimaClient.SelectCharacter(index);
        }

        public void OnDeleteCharacter(int index)
        {
            UltimaClient.DeleteCharacter(index);
        }

        public void OnNewCharacter()
        {
            Manager.CurrentScene = new CreateCharacterScene();
        }
    }
}
