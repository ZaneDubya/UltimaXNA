/***************************************************************************
 *   Mobile.cs
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
using UltimaXNA.UltimaWorld;
using UltimaXNA.UltimaWorld.View;
#endregion

namespace UltimaXNA.Entity
{
    public class Mobile : BaseEntity
    {
        // ============================================================
        // Movement and Facing
        // ============================================================

        protected Movement m_movement;

        public Direction Facing
        {
            get { return m_movement.Facing & Direction.FacingMask; }
            set { m_movement.Facing = value; HasBeenDrawn = false; }
        }

        public string Name = string.Empty;
        public int Strength, Dexterity, Intelligence, StatCap, Luck, Gold;
        public CurrentMaxValue Health, Stamina, Mana;
        public CurrentMaxValue Followers;
        public CurrentMaxValue Weight;
        public int ArmorRating, ResistFire, ResistCold, ResistPoison, ResistEnergy;
        public int DamageMin, DamageMax;

        protected MobileEquipment m_equipment;
        protected MobileAnimation m_animation;

        public bool IsMoving { get { return m_movement.IsMoving; } }
        
        public bool IsRunning { get { return m_movement.IsRunning; } }
        
        public bool IsMounted 
        { 
            get { return (m_equipment[(int)EquipLayer.Mount] != null && m_equipment[(int)EquipLayer.Mount].ItemID != 0); } 
        }
        
        public bool IsWarMode 
        { 
            get { return m_isWarMode; }
            set { m_isWarMode = value; m_animation.UpdateAnimation(); }
        }

        public bool IsFemale;
        public bool IsPoisoned;
        public bool IsBlessed;
        bool m_isWarMode;
        public bool IsHidden;

        int m_bodyID = 0;
        public int BodyID
        {
            get
            {
                if (m_bodyID >= 402 && m_bodyID <= 403)
                    return m_bodyID - 2;
                return m_bodyID;
            }
            set
            {
                m_bodyID = value;
                tickUpdateTicker();
            }
        }
        public int HairBodyID { get { return (m_equipment[(int)EquipLayer.Hair] == null) ? 0 : m_equipment[(int)EquipLayer.Hair].AnimationDisplayID; } }
        public int HairHue { get { return (m_equipment[(int)EquipLayer.Hair] == null) ? 0 : m_equipment[(int)EquipLayer.Hair].Hue; } }
        public int FacialHairBodyID { get { return (m_equipment[(int)EquipLayer.FacialHair] == null) ? 0 : m_equipment[(int)EquipLayer.FacialHair].AnimationDisplayID; } }
        public int FacialHairHue { get { return (m_equipment[(int)EquipLayer.FacialHair] == null) ? 0 : m_equipment[(int)EquipLayer.FacialHair].Hue; } }

        int m_updateTicker = 0;
        void tickUpdateTicker()
        {
            m_updateTicker++;
        }
        public int UpdateTicker
        {
            get { return m_updateTicker; }
        }

        public bool Alive
        {
            get { return Health.Current != 0; }
        }

        private int m_hue;
        public new int Hue
        {
            get {
                if (IsHidden)
                    return 0x3E7;
                else if (IsPoisoned)
                    return 0x1CE;
                else
                    return m_hue;
            }
            set
            {
                m_hue = value;
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

        private static int[] m_DrawLayerOrderNorth = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.TwoHanded, (int)EquipLayer.Cloak };
		private static int[] m_DrawLayerOrderRight = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.TwoHanded };
		private static int[] m_DrawLayerOrderEast = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.TwoHanded };
		private static int[] m_DrawLayerOrderDown = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Cloak, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.TwoHanded };
		private static int[] m_DrawLayerOrderSouth = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.TwoHanded };
		private static int[] m_DrawLayerOrderLeft = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.Cloak, (int)EquipLayer.TwoHanded };
		private static int[] m_DrawLayerOrderWest = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.TwoHanded, (int)EquipLayer.Cloak };
		private static int[] m_DrawLayerOrderUp = new int[] { (int)EquipLayer.Mount, (int)EquipLayer.Body, (int)EquipLayer.Shirt, (int)EquipLayer.Pants, (int)EquipLayer.Shoes, (int)EquipLayer.InnerLegs, (int)EquipLayer.InnerTorso, (int)EquipLayer.Ring, (int)EquipLayer.Talisman, (int)EquipLayer.Bracelet, (int)EquipLayer.Unused_xF, (int)EquipLayer.Arms, (int)EquipLayer.Gloves, (int)EquipLayer.OuterLegs, (int)EquipLayer.MiddleTorso, (int)EquipLayer.Neck, (int)EquipLayer.Hair, (int)EquipLayer.OuterTorso, (int)EquipLayer.Waist, (int)EquipLayer.FacialHair, (int)EquipLayer.Earrings, (int)EquipLayer.Helm, (int)EquipLayer.OneHanded, (int)EquipLayer.TwoHanded, (int)EquipLayer.Cloak };

		private int[] m_DrawLayerOrder
		{
			get
			{
                int direction = MirrorFacingForDraw(Facing);
				switch (direction)
				{
					case 0x00: return m_DrawLayerOrderNorth;
					case 0x01: return m_DrawLayerOrderRight;
					case 0x02: return m_DrawLayerOrderEast;
					case 0x03: return m_DrawLayerOrderDown;
					case 0x04: return m_DrawLayerOrderSouth;
					case 0x05: return m_DrawLayerOrderLeft;
					case 0x06: return m_DrawLayerOrderWest;
					case 0x07: return m_DrawLayerOrderUp;
					default:
						// TODO: Log an Error
						return m_DrawLayerOrderNorth;
				}
			}
		}

        public Mobile(Serial serial)
            : base(serial)
        {
            m_equipment = new MobileEquipment(this);
            m_animation = new MobileAnimation(this);
            m_movement = new Movement(this);
            m_movement.RequiresUpdate = true;
        }

        public override void Update(double frameMS)
        {
            m_animation.Update(frameMS);

            if (!m_movement.Position.IsNullPosition)
            {
                m_movement.Update(frameMS);

                if (IsometricRenderer.Map == null)
                    return;

                MapTile t = IsometricRenderer.Map.GetMapTile(m_movement.Position.X, m_movement.Position.Y, false);
                if (t != null)
                {
                    this.Draw(t, m_movement.Position);
                    HasBeenDrawn = true;
                }
                else
                    HasBeenDrawn = false;
            }

            base.Update(frameMS);
        }

        internal override void Draw(MapTile tile, Position3D position)
        {
            if (IsMoving)
            {
                if (IsRunning)
                    m_animation.Animate(MobileAction.Run);
                else
                    m_animation.Animate(MobileAction.Walk);
            }
            else
            {
                if (!m_animation.IsAnimating)
                    m_animation.Animate(MobileAction.Stand);
            }

            MapObjectMobile mobtile = new MapObjectMobile(position, MirrorFacingForDraw(Facing), m_animation.ActionIndex, m_animation.AnimationFrame, this);
            // tile.AddMapObject(mobtile);

            int[] drawLayers = m_DrawLayerOrder;
			bool hasOuterTorso = m_equipment[(int)EquipLayer.OuterTorso] != null && m_equipment[(int)EquipLayer.OuterTorso].AnimationDisplayID != 0;

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
                else if (m_equipment[drawLayers[i]] != null && m_equipment[drawLayers[i]].AnimationDisplayID != 0)
                {
                    mobtile.AddLayer(m_equipment[drawLayers[i]].AnimationDisplayID, m_equipment[drawLayers[i]].Hue);
                }
            }
            // drawOverheads(tile, new Position3D(m_movement.Position.Tile_V3));
        }

        public override void Dispose()
        {
            base.Dispose();
            m_movement.ClearImmediate();
            m_equipment.ClearEquipment();
            tickUpdateTicker();
        }

        public void WearItem(Item i, int slot)
        {
            m_equipment[slot] = i;
            tickUpdateTicker();
            if (slot == (int)EquipLayer.Mount)
            {

            }
        }

        public void RemoveItem(Serial serial)
        {
            m_equipment.RemoveBySerial(serial);
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
                return m_equipment[slot];
            }
        }

        public Container Backpack
        {
            get
            {
                return (Container)m_equipment[(int)EquipLayer.Backpack];
            }
        }

        public Container VendorShopContents
        {
            get
            {
                return (Container)m_equipment[(int)EquipLayer.ShopBuy];
            }
        }

        public void Animate(int action, int frameCount, int repeatCount, bool reverse, bool repeat, int delay)
        {
            m_animation.Animate(action, frameCount, repeatCount, reverse, repeat, delay);
        }

        public void Mobile_AddMoveEvent(int x, int y, int z, int facing)
        {
            m_movement.Mobile_AddMoveEvent(x, y, z, facing);
        }

        public void Move_Instant(int x, int y, int z, int facing)
        {
            m_movement.Move_Instant(x, y, z, facing);
        }

        public void PlayerMobile_Move(Direction facing)
        {
            m_movement.PlayerMobile_Move(facing);
        }

        public void PlayerMobile_MoveEventAck(int sequence)
        {
            m_movement.PlayerMobile_MoveEventAck(sequence);
        }
        public void PlayerMobile_MoveEventRej(int sequenceID, int x, int y, int z, int direction)
        {
            m_movement.PlayerMobile_MoveEventRej(sequenceID, x, y, z, direction);
        }
    }
}
