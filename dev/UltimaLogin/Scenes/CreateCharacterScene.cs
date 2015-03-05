/***************************************************************************
 *   CreateCharacterScene.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaGUI.LoginGumps;
using UltimaXNA.UltimaPackets;
using UltimaXNA.UltimaPackets.Client;
#endregion

namespace UltimaXNA.Scenes
{
    enum CreateCharacterSceneStates
    {
        ChooseSkills,
        ChooseAppearance,
        Cancel,
        CreateCharacter,
        WaitingForResponse,
        Default = ChooseSkills,
    }
    public class CreateCharacterScene : AScene
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

        public CreateCharacterScene()
        {

        }

        public override void Intitialize(UltimaClient client)
        {
            base.Intitialize(client);
            m_Status = CreateCharacterSceneStates.Default;
            openSkillsGump();
        }

        void openSkillsGump()
        {
            m_CreateSkillsGump = (CreateCharSkillsGump)UltimaEngine.UserInterface.AddControl(new CreateCharSkillsGump(), 0, 0);
            m_CreateSkillsGump.OnForward += this.OnForward;
            m_CreateSkillsGump.OnBackward += this.OnBackward;
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
            m_CreateAppearanceGump = (CreateCharAppearanceGump)UltimaEngine.UserInterface.AddControl(new CreateCharAppearanceGump(), 0, 0);
            m_CreateAppearanceGump.OnForward += this.OnForward;
            m_CreateAppearanceGump.OnBackward += this.OnBackward;
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
                UltimaInteraction.MsgBox("Error: your stat values did not add up to 80. Please logout and try to make another character.", MsgBoxTypes.OkOnly);
                return false;
            }
            if (m_CreateSkillsGump.SkillPoints0 + m_CreateSkillsGump.SkillPoints1 + m_CreateSkillsGump.SkillPoints2 != 100)
            {
                UltimaInteraction.MsgBox("Error: your skill values did not add up to 100. Please logout and try to make another character.", MsgBoxTypes.OkOnly);
                return false;
            }
            if (m_CreateSkillsGump.SkillIndex0 == -1 || m_CreateSkillsGump.SkillIndex1 == -1 || m_CreateSkillsGump.SkillIndex2 == -1 ||
                (m_CreateSkillsGump.SkillIndex0 == m_CreateSkillsGump.SkillIndex1) ||
                (m_CreateSkillsGump.SkillIndex1 == m_CreateSkillsGump.SkillIndex2) ||
                (m_CreateSkillsGump.SkillIndex0 == m_CreateSkillsGump.SkillIndex2))
            {
                UltimaInteraction.MsgBox("You must have three unique skills chosen!", MsgBoxTypes.OkOnly);
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
                UltimaInteraction.MsgBox(UltimaData.StringData.Entry(1075458), MsgBoxTypes.OkOnly); // 1075458: Your character name is too short.
                return false;
            }
            if (m_name[m_name.Length - 1] == '.')
            {
                UltimaInteraction.MsgBox(UltimaData.StringData.Entry(1075457), MsgBoxTypes.OkOnly); // 1075457: Your character name cannot end with a period('.').
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
                        Manager.CurrentScene = new CharacterListScene();
                        break;
                    case CreateCharacterSceneStates.CreateCharacter:
                        Client.Send(new CreateCharacterPacket(
                            m_name, (Sex)m_gender, (Race)0, (byte)m_attributes[0], (byte)m_attributes[1], (byte)m_attributes[2], 
                            (byte)m_skillIndexes[0], (byte)m_skillValues[0], (byte)m_skillIndexes[1], (byte)m_skillValues[1], (byte)m_skillIndexes[2], (byte)m_skillValues[2],
                            (short)m_skinHue, (short)m_hairStyleID, (short)m_hairHue, (short)m_facialHairStyleID, (short)m_facialHairHue,
                            0, (short)UltimaVars.Characters.FirstEmptySlot, Utility.IPAddress, 0, 0));
                        m_Status = CreateCharacterSceneStates.WaitingForResponse;
                        break;
                    case CreateCharacterSceneStates.WaitingForResponse:
                        // do nothing, waiting for response to create character request.
                        break;
                }

                switch (Client.Status)
                {
                    case UltimaClientStatus.GameServer_CharList:
                        // This is where we're supposed to be while creating a character.
                        break;
                    case UltimaClientStatus.WorldServer_LoginComplete:
                    case UltimaClientStatus.WorldServer_InWorld:
                        // Almost completed logging in, just waiting for our client object.
                        // break;
                        // We're in! Load the world.
                        UltimaEngine.ActiveModel = new UltimaXNA.UltimaWorld.WorldModel();
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
                        m_Status = CreateCharacterSceneStates.CreateCharacter;
                    }
                    break;
            }
        }
    }
}
