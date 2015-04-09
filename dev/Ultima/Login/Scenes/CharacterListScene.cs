/***************************************************************************
 *   LoginScene.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using UltimaXNA.Core.Network;
using UltimaXNA.Data;
using UltimaXNA.UltimaGUI.LoginGumps;
using UltimaXNA.UltimaVars;

#endregion

namespace UltimaXNA.UltimaLogin.Scenes
{
    public class CharacterListScene : AScene
    {
        CharacterListGump m_CharListGump;

        public CharacterListScene()
        {

        }

        public override void Intitialize(UltimaEngine engine)
        {
            base.Intitialize(engine);
            m_CharListGump = (CharacterListGump)Engine.UserInterface.AddControl(new CharacterListGump(), 0, 0);
            m_CharListGump.OnBackToSelectServer += OnBackToSelectServer;
            m_CharListGump.OnLoginWithCharacter += OnLoginWithCharacter;
            m_CharListGump.OnDeleteCharacter += OnDeleteCharacter;
            m_CharListGump.OnNewCharacter += OnNewCharacter;
        }

        private bool _autoSelectedCharacter;

        public override void Update(double totalTime, double frameTime)
        {
            base.Update(totalTime, frameTime);

            if (SceneState == SceneState.Active)
            {
                switch (Engine.Client.Status)
                {
                    case UltimaClientStatus.GameServer_CharList:
                        if (!_autoSelectedCharacter && Settings.Game.AutoSelectLastCharacter && !string.IsNullOrWhiteSpace(Settings.Game.LastCharacterName))
                        {
                            _autoSelectedCharacter = true;

                            for(int i = 0; i < Characters.List.Length; i++)
                            {
                                if(Characters.List[i].Name == Settings.Game.LastCharacterName)
                                {
                                    OnLoginWithCharacter(i);
                                }
                            }
                        }
                        // This is where we're supposed to be while waiting to select a character.
                        break;
                    case UltimaClientStatus.WorldServer_LoginComplete:
                        // Almost completed logging in, just waiting for our client object.
                        // break;
                    case UltimaClientStatus.WorldServer_InWorld:
                        // we've connected! Client takes us into the world and disposes of this Model.
                        break;
                    default:
                        // what's going on here? Add additional error handlers.
                        throw (new Exception("Unknown UltimaClientStatus in CharacterListScene:Update"));
                }
            }
        }

        public override void Dispose()
        {
            m_CharListGump.Dispose();
            base.Dispose();
        }

        public void OnBackToSelectServer()
        {
            // !!! This SHOULD take us back to the 'logging in' screen,
            // which automatically logs in again. But we can't do that,
            // since I have UltimaClient clear your account/password data
            // once connected (is this really neccesary?) Have to fix ..
            Manager.CurrentScene = new LoginScene();
        }

        public void OnLoginWithCharacter(int index)
        {
            m_CharListGump.ActivePage = 2;
            Engine.Client.LoginWithCharacter(index);
        }

        public void OnDeleteCharacter(int index)
        {
            Engine.Client.DeleteCharacter(index);
        }

        public void OnNewCharacter()
        {
            Manager.CurrentScene = new CreateCharacterScene();
        }
    }
}
