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
using UltimaXNA.Configuration;
using UltimaXNA.Ultima.Data.Accounts;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.LoginGumps;
#endregion

namespace UltimaXNA.Ultima.Login.States
{
    public class CharacterListState : AState
    {
        GUIManager m_UserInterface;
        LoginModel m_Login;

        CharacterListGump m_CharListGump;

        public CharacterListState()
            : base()
        {

        }

        public override void Intitialize()
        {
            base.Intitialize();

            m_UserInterface = UltimaServices.GetService<GUIManager>();
            m_Login = UltimaServices.GetService<LoginModel>();

            m_CharListGump = (CharacterListGump)m_UserInterface.AddControl(new CharacterListGump(), 0, 0);
            m_CharListGump.OnBackToSelectServer += OnBackToSelectServer;
            m_CharListGump.OnLoginWithCharacter += OnLoginWithCharacter;
            m_CharListGump.OnDeleteCharacter += OnDeleteCharacter;
            m_CharListGump.OnNewCharacter += OnNewCharacter;
        }

        private bool m_autoSelectedCharacter;

        public override void Update(double totalTime, double frameTime)
        {
            base.Update(totalTime, frameTime);

            if (SceneState == SceneState.Active)
            {
                switch (m_Login.Client.Status)
                {
                    case LoginClientStatus.GameServer_CharList:
                        if (!m_autoSelectedCharacter && Settings.Game.AutoSelectLastCharacter && !string.IsNullOrWhiteSpace(Settings.Game.LastCharacterName))
                        {
                            m_autoSelectedCharacter = true;

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
                    case LoginClientStatus.WorldServer_LoginComplete:
                        // Almost completed logging in, just waiting for our client object.
                        // break;
                    case LoginClientStatus.WorldServer_InWorld:
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
            Manager.CurrentState = new LoginState();
        }

        public void OnLoginWithCharacter(int index)
        {
            m_CharListGump.ActivePage = 2;
            m_Login.Client.LoginWithCharacter(index);
        }

        public void OnDeleteCharacter(int index)
        {
            m_Login.Client.DeleteCharacter(index);
        }

        public void OnNewCharacter()
        {
            Manager.CurrentState = new CreateCharacterState();
        }
    }
}
