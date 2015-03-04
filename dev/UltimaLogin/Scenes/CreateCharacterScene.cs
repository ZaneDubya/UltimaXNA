/***************************************************************************
 *   CreateCharacterScene.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : January 11, 2010
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
using Microsoft.Xna.Framework;
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
        CreateCharacterSceneStates m_status;

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
            m_status = CreateCharacterSceneStates.Default;
            openSkillsGump();
        }

        void openSkillsGump()
        {
            Gump g = (Gump)UltimaEngine.UserInterface.AddControl(new CreateCharSkillsGump(), 0, 0);
            CreateCharSkillsGump g1 = ((CreateCharSkillsGump)g);
            g1.OnForward += this.OnForward;
            g1.OnBackward += this.OnBackward;
            m_status = CreateCharacterSceneStates.ChooseSkills;
            // restore values
            if (m_skillsSet)
            {
                g1.Strength = m_attributes[0];
                g1.Dexterity = m_attributes[1];
                g1.Intelligence = m_attributes[2];
                g1.SkillIndex0 = m_skillIndexes[0];
                g1.SkillIndex1 = m_skillIndexes[1];
                g1.SkillIndex2 = m_skillIndexes[2];
                g1.SkillPoints0 = m_skillValues[0];
                g1.SkillPoints1 = m_skillValues[1];
                g1.SkillPoints2 = m_skillValues[2];
            }
        }

        void openAppearanceGump()
        {
            Gump g = (Gump)UltimaEngine.UserInterface.AddControl(new CreateCharAppearanceGump(), 0, 0);
            CreateCharAppearanceGump g1 = ((CreateCharAppearanceGump)g);
            ((CreateCharAppearanceGump)g1).OnForward += this.OnForward;
            ((CreateCharAppearanceGump)g1).OnBackward += this.OnBackward;
            m_status = CreateCharacterSceneStates.ChooseAppearance;
            // restore values
            if (m_appearanceSet)
            {
                g1.Name = m_name;
                g1.Gender = m_gender;
                g1.HairID = m_hairStyleID;
                g1.FacialHairID = m_facialHairStyleID;
                g1.SkinHue = m_skinHue;
                g1.HairHue = m_hairHue;
                g1.FacialHairHue = m_facialHairHue;
            }
        }

        bool validateSkills()
        {
            // we need to make sure that the stats add up to 80, skills add up to 100, and 3 unique skills are selected.
            // if not, pop up an appropriate error message.
            CreateCharSkillsGump g = UltimaEngine.UserInterface.GetControl<CreateCharSkillsGump>(0);
            if (g.Strength + g.Dexterity + g.Intelligence != 80)
            {
                UltimaInteraction.MsgBox("Error: your stat values did not add up to 80. Please logout and try to make another character.", MsgBoxTypes.OkOnly);
                return false;
            }
            if (g.SkillPoints0 + g.SkillPoints1 + g.SkillPoints2 != 100)
            {
                UltimaInteraction.MsgBox("Error: your skill values did not add up to 100. Please logout and try to make another character.", MsgBoxTypes.OkOnly);
                return false;
            }
            if (g.SkillIndex0 == -1 || g.SkillIndex1 == -1 || g.SkillIndex2 == -1 ||
                (g.SkillIndex0 == g.SkillIndex1) ||
                (g.SkillIndex1 == g.SkillIndex2) ||
                (g.SkillIndex0 == g.SkillIndex2))
            {
                UltimaInteraction.MsgBox("You must have three unique skills chosen!", MsgBoxTypes.OkOnly);
                return false;
            }
            // save the values;
            m_attributes[0] = g.Strength;
            m_attributes[1] = g.Dexterity;
            m_attributes[2] = g.Intelligence;
            m_skillIndexes[0] = g.SkillIndex0;
            m_skillIndexes[1] = g.SkillIndex1;
            m_skillIndexes[2] = g.SkillIndex2;
            m_skillValues[0] = g.SkillPoints0;
            m_skillValues[1] = g.SkillPoints1;
            m_skillValues[2] = g.SkillPoints2;
            m_skillsSet = true;
            return true;
        }

        bool validateAppearance()
        {
            CreateCharAppearanceGump g = UltimaEngine.UserInterface.GetControl<CreateCharAppearanceGump>(0);
            // save the values
            m_appearanceSet = true;
            m_name = g.Name;
            m_gender = g.Gender;
            m_hairStyleID = g.HairID;
            m_facialHairStyleID = g.FacialHairID;
            m_skinHue = g.SkinHue;
            m_hairHue = g.HairHue;
            m_facialHairHue = g.FacialHairHue;
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
                switch (m_status)
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
                        m_status = CreateCharacterSceneStates.WaitingForResponse;
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
            if (UltimaEngine.UserInterface.GetControl<CreateCharAppearanceGump>(0) != null)
                UltimaEngine.UserInterface.GetControl<CreateCharAppearanceGump>(0).Dispose();
            if (UltimaEngine.UserInterface.GetControl<CreateCharSkillsGump>(0) != null)
                UltimaEngine.UserInterface.GetControl<CreateCharSkillsGump>(0).Dispose();
            base.Dispose();
        }

        public void OnBackward()
        {
            switch (m_status)
            {
                case CreateCharacterSceneStates.ChooseSkills:
                    m_status = CreateCharacterSceneStates.Cancel;
                    break;
                case CreateCharacterSceneStates.ChooseAppearance:
                    UltimaEngine.UserInterface.GetControl<CreateCharAppearanceGump>(0).Dispose();
                    openSkillsGump();
                    break;
            }
        }

        public void OnForward()
        {
            switch (m_status)
            {
                case CreateCharacterSceneStates.ChooseSkills:
                    if (validateSkills())
                    {
                        UltimaEngine.UserInterface.GetControl<CreateCharSkillsGump>(0).Dispose();
                        openAppearanceGump();
                    }
                    break;
                case CreateCharacterSceneStates.ChooseAppearance:
                    if (validateAppearance())
                    {
                        m_status = CreateCharacterSceneStates.CreateCharacter;
                    }
                    break;
            }
        }
    }
}
