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

        private static int[] DrawLayersNorth = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.TwoHanded, (int)EquipLayer.Cloak };
		private static int[] DrawLayersRight = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.TwoHanded };
		private static int[] DrawLayersEast = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.TwoHanded };
		private static int[] DrawLayersDown = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Cloak, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.TwoHanded };
		private static int[] DrawLayersSouth = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.TwoHanded };
		private static int[] DrawLayersLeft = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.TwoHanded };
		private static int[] DrawLayersWest = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.TwoHanded, (int)EquipLayer.Cloak };
		private static int[] DrawLayersUp = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.TwoHanded, (int)EquipLayer.Cloak };

		private int[] m_DrawLayers
		{
			get
			{
				int direction = Movement.DrawFacing;
				switch (direction)
				{
					case 0x00: return DrawLayersNorth;
					case 0x01: return DrawLayersRight;
					case 0x02: return DrawLayersEast;
					case 0x03: return DrawLayersDown;
					case 0x04: return DrawLayersSouth;
					case 0x05: return DrawLayersLeft;
					case 0x06: return DrawLayersWest;
					case 0x07: return DrawLayersUp;
					default:
						// TODO: Log an Error
						return DrawLayersNorth;
				}
			}
		}

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

        internal override void Draw(UltimaXNA.TileEngine.MapCell cell, Vector3 position, Vector3 positionOffset)
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
            MapObjectMobile mobtile = null;
            int[] drawLayers = m_DrawLayers;
			bool hasOuterTorso = equipment[(int)EquipLayer.OuterTorso] != null && equipment[(int)EquipLayer.OuterTorso].AnimationDisplayID != 0;
            for (int i = 0; i < drawLayers.Length; i++)
            {
				// when wearing something on the outer torso the other torso stuff is not drawn
				if (hasOuterTorso &&
					(drawLayers[i] == (int)EquipLayer.InnerTorso || drawLayers[i] == (int)EquipLayer.MiddleTorso))
				{
					continue;
				}

                if (drawLayers[i] == (int)EquipLayer.Body)
                {
                    mobtile = new MapObjectMobile(
                        BodyID, position, positionOffset, 
                        direction, action, animation.AnimationFrame, 
                        this, i, Hue);
                    cell.Add(mobtile);
                }
                else if (equipment[drawLayers[i]] != null && equipment[drawLayers[i]].AnimationDisplayID != 0)
                {
                    mobtile = new TileEngine.MapObjectMobile(
                            equipment[drawLayers[i]].AnimationDisplayID, position, positionOffset,
                            direction, action, animation.AnimationFrame,
                            this, i, equipment[drawLayers[i]].Hue);
                    cell.Add(mobtile);
                }
            }
            drawOverheads(cell, position, positionOffset);
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
