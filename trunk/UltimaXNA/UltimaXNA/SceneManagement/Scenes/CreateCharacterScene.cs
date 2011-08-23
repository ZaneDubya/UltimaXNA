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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Client;
using UltimaXNA.Network;
using UltimaXNA.Client.Packets;
using UltimaXNA.UILegacy;
using UltimaXNA.UILegacy.ClientsideGumps;
#endregion

namespace UltimaXNA.SceneManagement
{
    enum CreateCharacterSceneStates
    {
        ChooseSkills,
        ChooseAppearance,
        Cancel,
        CreateCharacter,
        Default = ChooseSkills,
    }
    public class CreateCharacterScene : BaseScene
    {
        CreateCharacterSceneStates _status;

        bool _skillsSet = false;
        int[] _attributes = new int[3];
        int[] _skillIndexes= new int[3];
        int[] _skillValues = new int[3];
        bool _appearanceSet = false;
        string _name;
        int _gender, _hairStyleID, _facialHairStyleID;
        int _skinHue, _hairHue, _facialHairHue;

        public CreateCharacterScene(Game game)
            : base(game, true)
        {
        }

        public override void Intitialize()
        {
            _status = CreateCharacterSceneStates.Default;
            openSkillsGump();
        }

        void openSkillsGump()
        {
            Gump g = UI.AddGump_Local(new CreateCharSkillsGump(), 0, 0);
            CreateCharSkillsGump g1 = ((CreateCharSkillsGump)g);
            g1.OnForward += this.OnForward;
            g1.OnBackward += this.OnBackward;
            _status = CreateCharacterSceneStates.ChooseSkills;
            // restore values
            if (_skillsSet)
            {
                g1.Strength = _attributes[0];
                g1.Dexterity = _attributes[1];
                g1.Intelligence = _attributes[2];
                g1.SkillIndex0 = _skillIndexes[0];
                g1.SkillIndex1 = _skillIndexes[1];
                g1.SkillIndex2 = _skillIndexes[2];
                g1.SkillPoints0 = _skillValues[0];
                g1.SkillPoints1 = _skillValues[1];
                g1.SkillPoints2 = _skillValues[2];
            }
        }

        void openAppearanceGump()
        {
            Gump g = UI.AddGump_Local(new CreateCharAppearanceGump(), 0, 0);
            CreateCharAppearanceGump g1 = ((CreateCharAppearanceGump)g);
            ((CreateCharAppearanceGump)g1).OnForward += this.OnForward;
            ((CreateCharAppearanceGump)g1).OnBackward += this.OnBackward;
            _status = CreateCharacterSceneStates.ChooseAppearance;
            // restore values
            if (_appearanceSet)
            {
                g1.Name = _name;
                g1.Gender = _gender;
                g1.HairID = _hairStyleID;
                g1.FacialHairID = _facialHairStyleID;
                g1.SkinHue = _skinHue;
                g1.HairHue = _hairHue;
                g1.FacialHairHue = _facialHairHue;
            }
        }

        bool validateSkills()
        {
            // we need to make sure that the stats add up to 80, skills add up to 100, and 3 unique skills are selected.
            // if not, pop up an appropriate error message.
            CreateCharSkillsGump g = UI.GetGump<CreateCharSkillsGump>(0);
            if (g.Strength + g.Dexterity + g.Intelligence != 80)
            {
                UI.MsgBox("Error: your stat values did not add up to 80. Please logout and try to make another character.", MsgBoxTypes.OkOnly);
                return false;
            }
            if (g.SkillPoints0 + g.SkillPoints1 + g.SkillPoints2 != 100)
            {
                UI.MsgBox("Error: your skill values did not add up to 100. Please logout and try to make another character.", MsgBoxTypes.OkOnly);
                return false;
            }
            if (g.SkillIndex0 == -1 || g.SkillIndex1 == -1 || g.SkillIndex2 == -1 ||
                (g.SkillIndex0 == g.SkillIndex1) ||
                (g.SkillIndex1 == g.SkillIndex2) ||
                (g.SkillIndex0 == g.SkillIndex2))
            {
                UI.MsgBox("You must have three unique skills chosen!", MsgBoxTypes.OkOnly);
                return false;
            }
            // save the values;
            _attributes[0] = g.Strength;
            _attributes[1] = g.Dexterity;
            _attributes[2] = g.Intelligence;
            _skillIndexes[0] = g.SkillIndex0;
            _skillIndexes[1] = g.SkillIndex1;
            _skillIndexes[2] = g.SkillIndex2;
            _skillValues[0] = g.SkillPoints0;
            _skillValues[1] = g.SkillPoints1;
            _skillValues[2] = g.SkillPoints2;
            _skillsSet = true;
            return true;
        }

        bool validateAppearance()
        {
            CreateCharAppearanceGump g = UI.GetGump<CreateCharAppearanceGump>(0);
            // save the values
            _appearanceSet = true;
            _name = g.Name;
            _gender = g.Gender;
            _hairStyleID = g.HairID;
            _facialHairStyleID = g.FacialHairID;
            _skinHue = g.SkinHue;
            _hairHue = g.HairHue;
            _facialHairHue = g.FacialHairHue;
            // make sure name is long enough. 2+ Characters
            // if not, pop up an appropriate error message.
            if (_name.Length < 2)
            {
                UI.MsgBox(Data.StringList.Entry(1075458), MsgBoxTypes.OkOnly); // 1075458: Your character name is too short.
                return false;
            }
            if (_name[_name.Length - 1] == '.')
            {
                UI.MsgBox(Data.StringList.Entry(1075457), MsgBoxTypes.OkOnly); // 1075457: Your character name cannot end with a period('.').
                return false;
            }
            return true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (SceneState == SceneState.Active)
            {
                switch (_status)
                {
                    case CreateCharacterSceneStates.ChooseSkills:
                        // do nothing
                        break;
                    case CreateCharacterSceneStates.ChooseAppearance:
                        // do nothing
                        break;
                    case CreateCharacterSceneStates.Cancel:
                        SceneManager.CurrentScene = new CharacterListScene(Game);
                        break;
                    case CreateCharacterSceneStates.CreateCharacter:
                        UltimaClient.Send(new Client.Packets.Client.CreateCharacterPacket(
                            _name, (Sex)_gender, (Race)0, (byte)_attributes[0], (byte)_attributes[1], (byte)_attributes[2], 
                            (byte)_skillIndexes[0], (byte)_skillValues[0], (byte)_skillIndexes[1], (byte)_skillValues[1], (byte)_skillIndexes[2], (byte)_skillValues[2],
                            (short)_skinHue, (short)_hairStyleID, (short)_hairHue, (short)_facialHairStyleID, (short)_facialHairHue,
                            0, (short)ClientVars.Characters.FirstEmptySlot, Utility.IPAddress, 0, 0));
                        break;
                }

                switch (UltimaClient.Status)
                {
                    case UltimaClientStatus.GameServer_CharList:
                        // This is where we're supposed to be while creating a character.
                        break;
                    case UltimaClientStatus.WorldServer_LoginComplete:
                        // Almost completed logging in, just waiting for our client object.
                        break;
                    case UltimaClientStatus.WorldServer_InWorld:
                        // We're in! Load the world.
                        SceneManager.CurrentScene = new WorldScene(Game);
                        break;
                    default:
                        // what's going on here? Add additional error handlers.
                        throw (new Exception("Unexpected UltimaClientStatus in CreateCharacterScene:Update"));
                }
            }
        }

        public override void Dispose()
        {
            if (UI.GetGump<CreateCharAppearanceGump>(0) != null)
                UI.GetGump<CreateCharAppearanceGump>(0).Dispose();
            if (UI.GetGump<CreateCharSkillsGump>(0) != null)
                UI.GetGump<CreateCharSkillsGump>(0).Dispose();
            base.Dispose();
        }

        public void OnBackward()
        {
            switch (_status)
            {
                case CreateCharacterSceneStates.ChooseSkills:
                    _status = CreateCharacterSceneStates.Cancel;
                    break;
                case CreateCharacterSceneStates.ChooseAppearance:
                    UI.GetGump<CreateCharAppearanceGump>(0).Dispose();
                    openSkillsGump();
                    break;
            }
        }

        public void OnForward()
        {
            switch (_status)
            {
                case CreateCharacterSceneStates.ChooseSkills:
                    if (validateSkills())
                    {
                        UI.GetGump<CreateCharSkillsGump>(0).Dispose();
                        openAppearanceGump();
                    }
                    break;
                case CreateCharacterSceneStates.ChooseAppearance:
                    if (validateAppearance())
                    {
                        _status = CreateCharacterSceneStates.CreateCharacter;
                    }
                    break;
            }
        }
    }
}
