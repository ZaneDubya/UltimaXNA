/***************************************************************************
 *   Mobile.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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

namespace UltimaXNA.Entity
{
    public class Mobile : BaseEntity
    {
        public string Name = string.Empty;
        public int Strength, Dexterity, Intelligence, StatCap, Luck, Gold;
        public CurrentMaxValue Health, Stamina, Mana;
        public CurrentMaxValue Followers;
        public CurrentMaxValue Weight;
        public int ArmorRating, ResistFire, ResistCold, ResistPoison, ResistEnergy;
        public int DamageMin, DamageMax;

        protected MobileEquipment _equipment;
        protected MobileAnimation _animation;

        public bool IsMoving { get { return _movement.IsMoving; } }
        
        public bool IsRunning { get { return _movement.IsRunning; } }
        
        public bool IsMounted 
        { 
            get { return (_equipment[(int)EquipLayer.Mount] != null && _equipment[(int)EquipLayer.Mount].ItemID != 0); } 
        }
        
        public bool IsWarMode 
        { 
            get { return _isWarMode; }
            set { _isWarMode = value; _animation.UpdateAnimation(); }
        }

        public bool IsFemale;
        public bool IsPoisoned;
        public bool IsBlessed;
        bool _isWarMode;
        public bool IsHidden;

        int _bodyID = 0;
        public int BodyID
        {
            get
            {
                if (_bodyID >= 402 && _bodyID <= 403)
                    return _bodyID - 2;
                return _bodyID;
            }
            set
            {
                _bodyID = value;
                tickUpdateTicker();
            }
        }
        public int HairBodyID { get { return (_equipment[(int)EquipLayer.Hair] == null) ? 0 : _equipment[(int)EquipLayer.Hair].AnimationDisplayID; } }
        public int HairHue { get { return (_equipment[(int)EquipLayer.Hair] == null) ? 0 : _equipment[(int)EquipLayer.Hair].Hue; } }
        public int FacialHairBodyID { get { return (_equipment[(int)EquipLayer.FacialHair] == null) ? 0 : _equipment[(int)EquipLayer.FacialHair].AnimationDisplayID; } }
        public int FacialHairHue { get { return (_equipment[(int)EquipLayer.FacialHair] == null) ? 0 : _equipment[(int)EquipLayer.FacialHair].Hue; } }

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

        private static int[] _DrawLayerOrderNorth = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.TwoHanded, (int)EquipLayer.Cloak };
		private static int[] _DrawLayerOrderRight = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.TwoHanded };
		private static int[] _DrawLayerOrderEast = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.TwoHanded };
		private static int[] _DrawLayerOrderDown = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Cloak, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.TwoHanded };
		private static int[] _DrawLayerOrderSouth = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.TwoHanded };
		private static int[] _DrawLayerOrderLeft = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.TwoHanded };
		private static int[] _DrawLayerOrderWest = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.TwoHanded, (int)EquipLayer.Cloak };
		private static int[] _DrawLayerOrderUp = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.TwoHanded, (int)EquipLayer.Cloak };

		private int[] _DrawLayerOrder
		{
			get
			{
                int direction = DrawFacing;
				switch (direction)
				{
					case 0x00: return _DrawLayerOrderNorth;
					case 0x01: return _DrawLayerOrderRight;
					case 0x02: return _DrawLayerOrderEast;
					case 0x03: return _DrawLayerOrderDown;
					case 0x04: return _DrawLayerOrderSouth;
					case 0x05: return _DrawLayerOrderLeft;
					case 0x06: return _DrawLayerOrderWest;
					case 0x07: return _DrawLayerOrderUp;
					default:
						// TODO: Log an Error
						return _DrawLayerOrderNorth;
				}
			}
		}

        public Mobile(Serial serial)
            : base(serial)
        {
            _equipment = new MobileEquipment(this);
            _animation = new MobileAnimation(this);
            _movement.RequiresUpdate = true;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            _animation.Update(gameTime);
            base.Update(gameTime);
        }

        internal override void Draw(MapTile tile, Position3D position)
        {
            if (IsMoving)
            {
                if (IsRunning)
                    _animation.Animate(MobileAction.Run);
                else
                    _animation.Animate(MobileAction.Walk);
            }
            else
            {
                if (!_animation.IsAnimating)
                    _animation.Animate(MobileAction.Stand);
            }

            MapObjectMobile mobtile = new MapObjectMobile(position, DrawFacing, _animation.ActionIndex, _animation.AnimationFrame, this);
            tile.AddMapObject(mobtile);

            int[] drawLayers = _DrawLayerOrder;
			bool hasOuterTorso = _equipment[(int)EquipLayer.OuterTorso] != null && _equipment[(int)EquipLayer.OuterTorso].AnimationDisplayID != 0;

            for (int i = 0; i < drawLayers.Length; i++)
            {
				// when wearing something on the outer torso the other torso stuff is not drawn
				if (hasOuterTorso && (drawLayers[i] == (int)EquipLayer.InnerTorso || drawLayers[i] == (int)EquipLayer.MiddleTorso))
				{
					continue;
				}

                if (drawLayers[i] == (int)EquipLayer.Body)
                {
                    mobtile.AddLayer(BodyID, Hue);
                }
                else if (_equipment[drawLayers[i]] != null && _equipment[drawLayers[i]].AnimationDisplayID != 0)
                {
                    mobtile.AddLayer(_equipment[drawLayers[i]].AnimationDisplayID, _equipment[drawLayers[i]].Hue);
                }
            }
            drawOverheads(tile, new Position3D(_movement.Position.Tile_V3));
        }

        public override void Dispose()
        {
            base.Dispose();
            _equipment.ClearEquipment();
            tickUpdateTicker();
        }

        public void WearItem(Item i, int slot)
        {
            _equipment[slot] = i;
            tickUpdateTicker();
            if (slot == (int)EquipLayer.Mount)
            {

            }
        }

        public void RemoveItem(Serial serial)
        {
            _equipment.RemoveBySerial(serial);
            tickUpdateTicker();
        }

        public Item GetItem(int slot)
        {
            if (slot == 0)
            {
                return null;
            }
            else
            {
                return _equipment[slot];
            }
        }

        public Container Backpack
        {
            get
            {
                return (Container)_equipment[(int)EquipLayer.Backpack];
            }
        }

        public Container VendorShopContents
        {
            get
            {
                return (Container)_equipment[(int)EquipLayer.ShopBuy];
            }
        }

        public void Animate(int action, int frameCount, int repeatCount, bool reverse, bool repeat, int delay)
        {
            _animation.Animate(action, frameCount, repeatCount, reverse, repeat, delay);
        }

        public void Mobile_AddMoveEvent(int x, int y, int z, int facing)
        {
            _movement.Mobile_AddMoveEvent(x, y, z, facing);
        }

        public void Move_Instant(int x, int y, int z, int facing)
        {
            _movement.Move_Instant(x, y, z, facing);
        }

        public void PlayerMobile_Move(Direction facing)
        {
            _movement.PlayerMobile_Move(facing);
        }

        public void PlayerMobile_MoveEventAck(int sequence)
        {
            _movement.PlayerMobile_MoveEventAck(sequence);
        }
        public void PlayerMobile_MoveEventRej(int sequenceID, int x, int y, int z, int direction)
        {
            _movement.PlayerMobile_MoveEventRej(sequenceID, x, y, z, direction);
        }
    }
}
