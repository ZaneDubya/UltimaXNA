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
        private WornEquipment equipment;
        protected MobileAnimation _animation;

        public bool IsMounted { get { return equipment[(int)EquipLayer.Mount] != null; } }
        public bool IsFemale;
        public bool IsPoisoned;
        public bool IsBlessed;
        public bool IsWarMode { get { return _animation.WarMode; } set { _animation.WarMode = value; } }
        public bool IsHidden;

        public int BodyID
        {
            get { return _animation.BodyID; }
            set
            {
                _animation.BodyID = value;
                tickUpdateTicker();
            }
        }
        public int HairBodyID { get { return (equipment[(int)EquipLayer.Hair] == null) ? 0 : equipment[(int)EquipLayer.Hair].AnimationDisplayID; } }
        public int HairHue { get { return (equipment[(int)EquipLayer.Hair] == null) ? 0 : equipment[(int)EquipLayer.Hair].Hue; } }
        public int FacialHairBodyID { get { return (equipment[(int)EquipLayer.FacialHair] == null) ? 0 : equipment[(int)EquipLayer.FacialHair].AnimationDisplayID; } }
        public int FacialHairHue { get { return (equipment[(int)EquipLayer.FacialHair] == null) ? 0 : equipment[(int)EquipLayer.FacialHair].Hue; } }

        int _updateTicker = 0;
        void tickUpdateTicker()
        {
            _updateTicker++;
        }
        public int UpdateTicker
        {
            get { return _updateTicker; }
        }

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
                tickUpdateTicker();
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
                int direction = DrawFacing;
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

        public Mobile(Serial serial, IWorld world)
            : base(serial, world)
        {
            _animation = new MobileAnimation();
            equipment = new WornEquipment(this);
            _movement.RequiresUpdate = true;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (equipment[(int)EquipLayer.Mount] != null &&
                equipment[(int)EquipLayer.Mount].ItemID != 0)
            {
                _movement.IsMounted = _animation.IsMounted = true;
            }
            else
            {
                _movement.IsMounted = _animation.IsMounted = false;
            }
            _animation.Update(gameTime);
            base.Update(gameTime);
        }

        internal override void Draw(MapTile tile, Position3D position)
        {
            if (_movement.IsMoving)
            {
                MobileAction iAnimationAction = (_movement.IsRunning) ? MobileAction.Run : MobileAction.Walk;
                _animation.SetAnimation(iAnimationAction, 10, 0, false, false, 0);
            }
            else
            {
                if (_animation.IsMovementAction(_animation.Action))
                    _animation.HaltAnimation = true;
            }

            int action = _animation.GetAction();
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
                        BodyID, position,
                        DrawFacing, action, _animation.AnimationFrame, 
                        this, i, Hue);
                    tile.Add(mobtile);
                }
                else if (equipment[drawLayers[i]] != null && equipment[drawLayers[i]].AnimationDisplayID != 0)
                {
                    mobtile = new TileEngine.MapObjectMobile(
                            equipment[drawLayers[i]].AnimationDisplayID, _movement.Position,
                            DrawFacing, action, _animation.AnimationFrame,
                            this, i, equipment[drawLayers[i]].Hue);
                    tile.Add(mobtile);
                }
            }
            drawOverheads(tile, new Position3D(_movement.Position.Tile_V3));
        }

        public override void Dispose()
        {
            equipment.ClearEquipment();
            tickUpdateTicker();
            base.Dispose();
        }

        public void WearItem(Item i, int slot)
        {
            equipment[slot] = i;
            tickUpdateTicker();
        }

        public void RemoveItem(Serial serial)
        {
            equipment.RemoveBySerial(serial);
            tickUpdateTicker();
        }

        public void GetItemInfo(int slot, out int displayID, out int hue)
        {
            if (slot == 0)
            {
                displayID = BodyID;
                hue = Hue;
            }
            else
            {
                if (equipment[slot] == null)
                {
                    displayID = 0;
                    hue = 0;
                }
                else
                {
                    displayID = equipment[slot].DisplayItemID;
                    hue = equipment[slot].Hue;
                }
            }
        }

        public Container Backpack
        {
            get
            {
                return (Container)equipment[(int)EquipLayer.Backpack];
            }
        }

        public Container VendorShopContents
        {
            get
            {
                return (Container)equipment[(int)EquipLayer.ShopBuy];
            }
        }

        public void Animation(int action, int frameCount, int repeatCount, bool reverse, bool repeat, int delay)
        {
            _animation.SetAnimation((MobileAction)action, frameCount, repeatCount, reverse, repeat, delay);
        }

        public void Move(Direction facing)
        {
            _movement.Move(facing);
        }

        public void MoveTo(int x, int y, int z, int facing)
        {
            _movement.MoveTo(x, y, z, facing);
        }

        public void MoveToInstant(int x, int y, int z, int facing)
        {
            _movement.MoveToInstant(x, y, z, facing);
        }
    }
}
