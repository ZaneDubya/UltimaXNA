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
using UltimaXNA.UILegacy;
using UltimaXNA.UILegacy.Clientside;
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

        int[] _attributes = new int[3];
        int[] _skillIndexes= new int[3];
        int[] _skillValues = new int[3];

        public CreateCharacterScene(Game game)
            : base(game, true)
        {
            _attributes[0] = 60;
            _attributes[1] = 10;
            _attributes[2] = 10;
            _skillIndexes[0] = -1;
            _skillIndexes[1] = -1;
            _skillIndexes[2] = -1;
            _skillValues[0] = 50;
            _skillValues[1] = 50;
            _skillValues[2] = 0;
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

        void openAppearanceGump()
        {
            Gump g2 = UI.AddGump_Local(new CreateCharAppearanceGump(), 0, 0);
            ((CreateCharAppearanceGump)g2).OnForward += this.OnForward;
            ((CreateCharAppearanceGump)g2).OnBackward += this.OnBackward;
            _status = CreateCharacterSceneStates.ChooseAppearance;
        }

        bool validateSkills()
        {
            // we need to make sure that the stats add up to 80, skills add up to 100, and 3 unique skills are selected.
            // if not, pop up an appropriate error message.
            CreateCharSkillsGump g = UI.GetGump<CreateCharSkillsGump>(0);
            if (g.Strength + g.Dexterity + g.Intelligence != 80)
            {
                UI.MsgBox("Error: your stat values did not add up to 80. Please logout and try to make another character.");
                return false;
            }
            if (g.SkillPoints0 + g.SkillPoints1 + g.SkillPoints2 != 100)
            {
                UI.MsgBox("Error: your skill values did not add up to 100. Please logout and try to make another character.");
                return false;
            }
            if (g.SkillIndex0 == -1 || g.SkillIndex1 == -1 || g.SkillIndex2 == -1 ||
                (g.SkillIndex0 == g.SkillIndex1) ||
                (g.SkillIndex1 == g.SkillIndex2) ||
                (g.SkillIndex0 == g.SkillIndex2))
            {
                UI.MsgBox("You must select three unique skills!");
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
            return true;
        }

        bool validateAppearance()
        {
            // make sure name is long enough, etc.
            // if not, pop up an appropriate error message.
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
                        // validate input and then send create packet
                        break;
                }

                switch (UltimaClient.Status)
                {
                    case UltimaClientStatus.GameServer_AtCharList:
                        // This is where we're supposed to be while creating a character.
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
