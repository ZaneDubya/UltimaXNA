/***************************************************************************
 *   Mobile.cs
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
using Microsoft.Xna.Framework;
using UltimaXNA.TileEngine;
#endregion

namespace UltimaXNA.Entities
{
    public class Mobile : Entity
    {
        public string Name = string.Empty;
        public CurrentMaxValue Health, Stamina, Mana;
        internal WornEquipment equipment;
        internal MobileAnimation animation;

        public bool IsMounted { get { return equipment[(int)EquipLayer.Mount] != null; } }
        public bool IsFemale;
        public bool IsPoisoned;
        public bool IsBlessed;
        public bool IsWarMode { get { return animation.WarMode; } set { animation.WarMode = value; } }
        public bool IsHidden;

        public int BodyID
        {
            get { return animation.BodyID; }
            set { animation.BodyID = value; }
        }
        public int HairBodyID { get { return (equipment[(int)EquipLayer.Hair] == null) ? 0 : equipment[(int)EquipLayer.Hair].AnimationDisplayID; } }
        public int HairHue { get { return (equipment[(int)EquipLayer.Hair] == null) ? 0 : equipment[(int)EquipLayer.Hair].Hue; } }
        public int FacialHairBodyID { get { return (equipment[(int)EquipLayer.FacialHair] == null) ? 0 : equipment[(int)EquipLayer.FacialHair].AnimationDisplayID; } }
        public int FacialHairHue { get { return (equipment[(int)EquipLayer.FacialHair] == null) ? 0 : equipment[(int)EquipLayer.FacialHair].Hue; } }

        public bool Alive
        {
            get { return Health.Current != 0; }
        }

        private int _hue;
        public int Hue
        {
            get {
                if (IsHidden)
                    return 0x3E7;
                else if (IsPoisoned)
                    return 0x1CE;
                else
                    return _hue;
            }
            set
            {
                _hue = value;
            }
        }

        /// <summary>
        /// 0x1: Innocent (Blue)
        /// 0x2: Friend (Green)
        /// 0x3: Grey (Grey - Non Criminal)
        /// 0x4: Criminal (Grey)
        /// 0x5: Enemy (Orange)
        /// 0x6: Murderer (Red)
        /// 0x7: Invulnerable (Yellow)
        /// </summary>
        public int Notoriety;
        public int NotorietyHue
        {
            get
            {
                switch (Notoriety)
                {
                    case 0x01: // 0x1: Innocent (Blue)
                        return 0x059;
                    case 0x02: // 0x2: Friend (Green)
                        return 0x03F;
                    case 0x03: // Grey (Grey - Non Criminal)
                        return 0x3B2;
                    case 0x04: // Criminal (Grey)
                        return 0x3B2;
                    case 0x05: // Enemy (Orange)
                        return 0x090;
                    case 0x06: // Murderer (Red)
                        return 0x022;
                    case 0x07: // Invulnerable (Yellow)
                        return 0x035;
                    default:
                        return 0;
                }
            }
        }

        // Issue 14 - Wrong layer draw order - http://code.google.com/p/ultimaxna/issues/detail?id=14 - Smjert
        private int[] m_DrawLayers = new int[21]
		{
			(int)EquipLayer.Mount,
            (int)EquipLayer.Body,
			(int)EquipLayer.OneHanded,
            (int)EquipLayer.Shoes,
            (int)EquipLayer.Pants,
            (int)EquipLayer.Shirt,
            (int)EquipLayer.Gloves,
            (int)EquipLayer.Neck,
			(int)EquipLayer.Hair,
            (int)EquipLayer.Waist,
            (int)EquipLayer.InnerTorso,
            (int)EquipLayer.FacialHair,
            (int)EquipLayer.MiddleTorso,
            (int)EquipLayer.OuterLegs,
            (int)EquipLayer.OuterLegs,
            (int)EquipLayer.InnerLegs,
            (int)EquipLayer.OuterTorso,
            (int)EquipLayer.Arms,
            (int)EquipLayer.Cloak,
			(int)EquipLayer.Helm,
            (int)EquipLayer.TwoHanded,
		};

        public Mobile(Serial serial)
            : base(serial)
        {
            animation = new MobileAnimation();
            equipment = new WornEquipment(this);
            Movement.RequiresUpdate = true;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (equipment[(int)EquipLayer.Mount] != null &&
                equipment[(int)EquipLayer.Mount].ItemID != 0)
            {
                Movement.IsMounted = animation.IsMounted = true;
            }
            else
            {
                Movement.IsMounted = animation.IsMounted = false;
            }
            animation.Update(gameTime);
            base.Update(gameTime);
        }

        internal override void Draw(UltimaXNA.TileEngine.MapCell nCell, Vector3 nLocation, Vector3 nOffset)
        {
            if (Movement.IsMoving)
            {
                MobileAction iAnimationAction = (Movement.IsRunning) ? MobileAction.Run : MobileAction.Walk;
                animation.SetAnimation(iAnimationAction, 10, 0, false, false, 0);
            }
            else
            {
                if (animation.IsMovementAction(animation.Action))
                    animation.HaltAnimation = true;
            }

            int direction = Movement.DrawFacing;
            int action = animation.GetAction();
            MobileTile mobtile = null;
            for (int i = 0; i < m_DrawLayers.Length; i++)
            {
                if (m_DrawLayers[i] == (int)EquipLayer.Body)
                {
                    mobtile = new MobileTile(
                        BodyID, nLocation, nOffset, 
                        direction, action, animation.AnimationFrame, 
                        this, i, Hue, animation.IsMounted);
                    // mobtile.SubType = MobileTileTypes.Body;
                    nCell.AddMobileTile(mobtile);
                }
                else if (equipment[m_DrawLayers[i]] != null && equipment[m_DrawLayers[i]].AnimationDisplayID != 0)
                {
                    mobtile = new TileEngine.MobileTile(
                            equipment[m_DrawLayers[i]].AnimationDisplayID, nLocation, nOffset,
                            direction, action, animation.AnimationFrame,
                            this, i, equipment[m_DrawLayers[i]].Hue, animation.IsMounted);
                    // mobtile.SubType = TileEngine.MobileTileTypes.Equipment;
                    nCell.AddMobileTile(mobtile);
                }
            }
        }

        public override void Dispose()
        {
            equipment.ClearEquipment();
            base.Dispose();
        }

        public void UnWearItem(Serial serial)
        {
            equipment.RemoveBySerial(serial);
        }

        public void Animation(int action, int frameCount, int repeatCount, bool reverse, bool repeat, int delay)
        {
            animation.SetAnimation((MobileAction)action, frameCount, repeatCount, reverse, repeat, delay);
        }

        public void Move(Direction facing)
        {
                Movement.Move(facing);
        }

        public void Move(int nX, int nY, int nZ, int nFacing)
        {
            if (nFacing != -1)
                Movement.Facing = (Direction)nFacing;
            Movement.SetGoalTile((float)nX, (float)nY, (float)nZ);
        }
    }
}
