/***************************************************************************
 *   CreateCharacterScene.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.Login.Accounts;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Ultima.UI.LoginGumps;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Core.Resources;
#endregion

namespace UltimaXNA.Ultima.Login.States
{
    enum CreateCharacterSceneStates
    {
        ChooseSkills,
        ChooseAppearance,
        Cancel,
        WaitingForResponse,
        Default = ChooseSkills,
    }
    public class CreateCharacterState : AState
    {
        CreateCharacterSceneStates m_Status;
        CreateCharSkillsGump m_CreateSkillsGump;
        CreateCharAppearanceGump m_CreateAppearanceGump;

        bool m_skillsSet = false;
        int[] m_attributes = new int[3];
        int[] m_skillIndexes= new int[3];
        int[] m_skillValues = new int[3];
        bool m_appearanceSet = false;
        string m_name;
        int m_gender, m_hairStyleID, m_facialHairStyleID;
        int m_skinHue, m_hairHue, m_facialHairHue;

        UserInterfaceService m_UserInterface;
        LoginModel m_Login;

        public CreateCharacterState()
            : base()
        {
            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
            m_Login = ServiceRegistry.GetService<LoginModel>();
        }

        public override void Intitialize()
        {
            base.Intitialize();

            m_Status = CreateCharacterSceneStates.Default;
            openSkillsGump();
        }

        void openSkillsGump()
        {
            m_CreateSkillsGump = (CreateCharSkillsGump)m_UserInterface.AddControl(new CreateCharSkillsGump(), 0, 0);
            m_CreateSkillsGump.OnForward += OnForward;
            m_CreateSkillsGump.OnBackward += OnBackward;
            m_Status = CreateCharacterSceneStates.ChooseSkills;
            // restore values
            if (m_skillsSet)
            {
                m_CreateSkillsGump.Strength = m_attributes[0];
                m_CreateSkillsGump.Dexterity = m_attributes[1];
                m_CreateSkillsGump.Intelligence = m_attributes[2];
                m_CreateSkillsGump.SkillIndex0 = m_skillIndexes[0];
                m_CreateSkillsGump.SkillIndex1 = m_skillIndexes[1];
                m_CreateSkillsGump.SkillIndex2 = m_skillIndexes[2];
                m_CreateSkillsGump.SkillPoints0 = m_skillValues[0];
                m_CreateSkillsGump.SkillPoints1 = m_skillValues[1];
                m_CreateSkillsGump.SkillPoints2 = m_skillValues[2];
            }
        }

        void openAppearanceGump()
        {
            m_CreateAppearanceGump = (CreateCharAppearanceGump)m_UserInterface.AddControl(new CreateCharAppearanceGump(), 0, 0);
            m_CreateAppearanceGump.OnForward += OnForward;
            m_CreateAppearanceGump.OnBackward += OnBackward;
            m_Status = CreateCharacterSceneStates.ChooseAppearance;
            // restore values
            if (m_appearanceSet)
            {
                m_CreateAppearanceGump.Name = m_name;
                m_CreateAppearanceGump.Gender = m_gender;
                m_CreateAppearanceGump.HairID = m_hairStyleID;
                m_CreateAppearanceGump.FacialHairID = m_facialHairStyleID;
                m_CreateAppearanceGump.SkinHue = m_skinHue;
                m_CreateAppearanceGump.HairHue = m_hairHue;
                m_CreateAppearanceGump.FacialHairHue = m_facialHairHue;
            }
        }

        bool validateSkills()
        {
            // we need to make sure that the stats add up to 80, skills add up to 100, and 3 unique skills are selected.
            // if not, pop up an appropriate error message.
            if (m_CreateSkillsGump.Strength + m_CreateSkillsGump.Dexterity + m_CreateSkillsGump.Intelligence != 80)
            {
                MsgBoxGump.Show("Error: your stat values did not add up to 80. Please logout and try to make another character.", MsgBoxTypes.OkOnly);
                return false;
            }
            if (m_CreateSkillsGump.SkillPoints0 + m_CreateSkillsGump.SkillPoints1 + m_CreateSkillsGump.SkillPoints2 != 100)
            {
                MsgBoxGump.Show("Error: your skill values did not add up to 100. Please logout and try to make another character.", MsgBoxTypes.OkOnly);
                return false;
            }
            if (m_CreateSkillsGump.SkillIndex0 == -1 || m_CreateSkillsGump.SkillIndex1 == -1 || m_CreateSkillsGump.SkillIndex2 == -1 ||
                (m_CreateSkillsGump.SkillIndex0 == m_CreateSkillsGump.SkillIndex1) ||
                (m_CreateSkillsGump.SkillIndex1 == m_CreateSkillsGump.SkillIndex2) ||
                (m_CreateSkillsGump.SkillIndex0 == m_CreateSkillsGump.SkillIndex2))
            {
                MsgBoxGump.Show("You must have three unique skills chosen!", MsgBoxTypes.OkOnly);
                return false;
            }
            // save the values;
            m_attributes[0] = m_CreateSkillsGump.Strength;
            m_attributes[1] = m_CreateSkillsGump.Dexterity;
            m_attributes[2] = m_CreateSkillsGump.Intelligence;
            m_skillIndexes[0] = m_CreateSkillsGump.SkillIndex0;
            m_skillIndexes[1] = m_CreateSkillsGump.SkillIndex1;
            m_skillIndexes[2] = m_CreateSkillsGump.SkillIndex2;
            m_skillValues[0] = m_CreateSkillsGump.SkillPoints0;
            m_skillValues[1] = m_CreateSkillsGump.SkillPoints1;
            m_skillValues[2] = m_CreateSkillsGump.SkillPoints2;
            m_skillsSet = true;
            return true;
        }

        bool validateAppearance()
        {
            // get the resource provider
            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();

            // save the values
            m_appearanceSet = true;
            m_name = m_CreateAppearanceGump.Name;
            m_gender = m_CreateAppearanceGump.Gender;
            m_hairStyleID = m_CreateAppearanceGump.HairID;
            m_facialHairStyleID = m_CreateAppearanceGump.FacialHairID;
            m_skinHue = m_CreateAppearanceGump.SkinHue;
            m_hairHue = m_CreateAppearanceGump.HairHue;
            m_facialHairHue = m_CreateAppearanceGump.FacialHairHue;
            // make sure name is long enough. 2+ Characters
            // if not, pop up an appropriate error message.
            if (m_name.Length < 2)
            {
                MsgBoxGump.Show(provider.GetString(1075458), MsgBoxTypes.OkOnly); // 1075458: Your character name is too short.
                return false;
            }
            if (m_name[m_name.Length - 1] == '.')
            {
                MsgBoxGump.Show(provider.GetString(1075457), MsgBoxTypes.OkOnly); // 1075457: Your character name cannot end with a period('.').
                return false;
            }
            return true;
        }

        public override void Update(double totalTime, double frameTime)
        {
            base.Update(totalTime, frameTime);

            if (SceneState == SceneState.Active)
            {
                switch (m_Status)
                {
                    case CreateCharacterSceneStates.ChooseSkills:
                        // do nothing
                        break;
                    case CreateCharacterSceneStates.ChooseAppearance:
                        // do nothing
                        break;
                    case CreateCharacterSceneStates.Cancel:
                        Manager.CurrentState = new CharacterListState();
                        break;
                    case CreateCharacterSceneStates.WaitingForResponse:
                        // do nothing, waiting for response to create character request.
                        break;
                }

                switch (m_Login.Client.Status)
                {
                    case LoginClientStatus.GameServer_CharList:
                        // This is where we're supposed to be while creating a character.
                        break;
                    case LoginClientStatus.WorldServer_LoginComplete:
                    case LoginClientStatus.WorldServer_InWorld:
                        // we've connected! Client takes us into the world and disposes of this Model.
                        break;
                    default:
                        // what's going on here? Add additional error handlers.
                        throw (new Exception("Unexpected UltimaClientStatus in CreateCharacterScene:Update"));
                }
            }
        }

        public override void Dispose()
        {
            if (m_CreateSkillsGump != null)
                m_CreateSkillsGump.Dispose();
            if (m_CreateAppearanceGump != null)
                m_CreateAppearanceGump.Dispose();
            base.Dispose();
        }

        public void OnBackward()
        {
            switch (m_Status)
            {
                case CreateCharacterSceneStates.ChooseSkills:
                    m_Status = CreateCharacterSceneStates.Cancel;
                    break;
                case CreateCharacterSceneStates.ChooseAppearance:
                    m_CreateAppearanceGump.Dispose();
                    openSkillsGump();
                    break;
            }
        }

        public void OnForward()
        {
            switch (m_Status)
            {
                case CreateCharacterSceneStates.ChooseSkills:
                    if (validateSkills())
                    {
                        m_CreateSkillsGump.Dispose();
                        openAppearanceGump();
                    }
                    break;
                case CreateCharacterSceneStates.ChooseAppearance:
                    if (validateAppearance())
                    {
                        m_Login.Client.CreateCharacter(new CreateCharacterPacket(
                            m_name, (Genders)m_gender, (Races)0, (byte)m_attributes[0], (byte)m_attributes[1], (byte)m_attributes[2], 
                            (byte)m_skillIndexes[0], (byte)m_skillValues[0], (byte)m_skillIndexes[1], (byte)m_skillValues[1], (byte)m_skillIndexes[2], (byte)m_skillValues[2],
                            (short)m_skinHue, (short)m_hairStyleID, (short)m_hairHue, (short)m_facialHairStyleID, (short)m_facialHairHue,
                            0, (short)Characters.FirstEmptySlot, Utility.IPAddress, 0, 0));
                        m_Status = CreateCharacterSceneStates.WaitingForResponse;
                    }
                    break;
            }
        }
    }
}
