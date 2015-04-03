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
using UltimaXNA.UltimaWorld.Views;
using UltimaXNA.UltimaWorld.Maps;
using UltimaXNA.UltimaEntities.Support;
#endregion

namespace UltimaXNA.UltimaEntities
{
    public class Mobile : AEntity
    {
        // ============================================================
        // Ctor, Dispose, Update, and CreateView()
        // ============================================================

        public Mobile(Serial serial, Map map)
            : base(serial, map)
        {
            Equipment = new MobileEquipment(this);
            m_movement = new MobileMovement(this);
            m_movement.RequiresUpdate = true;
        }

        public override void Dispose()
        {
            base.Dispose();
            Equipment.ClearEquipment();
            tickUpdateTicker();
        }

        public override void Update(double frameMS)
        {
            if (UltimaVars.EngineVars.AllLabels)
            {
                AddOverhead(MessageType.Label, "<outline>" + Name, 0, NotorietyHue);
            }

            if (!m_movement.Position.IsNullPosition)
            {
                m_movement.Update(frameMS);
                ((EntityViews.MobileView)GetView()).Update(frameMS);
            }

            base.Update(frameMS);
        }

        protected override EntityViews.AEntityView CreateView()
        {
            return new EntityViews.MobileView(this);
        }

        // ============================================================
        // Movement and Facing
        // ============================================================

        protected MobileMovement m_movement;

        public Direction Facing
        {
            get { return m_movement.Facing & Direction.FacingMask; }
            set { m_movement.Facing = value; }
        }

        public bool IsMoving { get { return m_movement.IsMoving; } }

        public bool IsRunning { get { return m_movement.IsRunning; } }

        // ============================================================
        // Properties
        // ============================================================

        public override string Name
        {
            get;
            set;
        }

        public MobileFlags Flags
        {
            get;
            set;
        }

        public int Strength, Dexterity, Intelligence, StatCap, Luck, Gold;
        public CurrentMaxValue Health, Stamina, Mana;
        public CurrentMaxValue Followers;
        public CurrentMaxValue Weight;
        public int ArmorRating, ResistFire, ResistCold, ResistPoison, ResistEnergy;
        public int DamageMin, DamageMax;

        public int Notoriety;

        public bool Alive
        {
            get { return Health.Current != 0; }
        }

        // ============================================================
        // Equipment and Mount
        // ============================================================

        public MobileEquipment Equipment;
        
        public bool IsMounted 
        { 
            get { return (Equipment[(int)EquipLayer.Mount] != null && Equipment[(int)EquipLayer.Mount].ItemID != 0); } 
        }

        public int HairBodyID
        {
            get { return (Equipment[(int)EquipLayer.Hair] == null) ? 0 : Equipment[(int)EquipLayer.Hair].ItemData.AnimID; }
        }
        public int HairHue
        {
            get { return (Equipment[(int)EquipLayer.Hair] == null) ? 0 : Equipment[(int)EquipLayer.Hair].Hue; }
        }
        public int FacialHairBodyID
        {
            get { return (Equipment[(int)EquipLayer.FacialHair] == null) ? 0 : Equipment[(int)EquipLayer.FacialHair].ItemData.AnimID; }
        }
        public int FacialHairHue
        {
            get { return (Equipment[(int)EquipLayer.FacialHair] == null) ? 0 : Equipment[(int)EquipLayer.FacialHair].Hue; }
        }

        public void WearItem(Item i, int slot)
        {
            Equipment[slot] = i;
            tickUpdateTicker();
            if (slot == (int)EquipLayer.Mount)
            {
                // Do we do something here?
            }
        }

        public void RemoveItem(Serial serial)
        {
            Equipment.RemoveBySerial(serial);
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
                return Equipment[slot];
            }
        }

        public Container Backpack
        {
            get
            {
                return (Container)Equipment[(int)EquipLayer.Backpack];
            }
        }

        public Container VendorShopContents
        {
            get
            {
                return (Container)Equipment[(int)EquipLayer.ShopBuy];
            }
        }

        // ============================================================
        // Appearance and Hues
        // ============================================================

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

        private int m_hue;
        public new int Hue
        {
            get {
                if (Flags.IsHidden)
                    return 0x3E7;
                else if (Flags.IsPoisoned)
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

        // ============================================================
        // UpdateTicker - used by PaperdollGump to determine when
        // equipment has changed, requiring a redraw.
        // ============================================================

        int m_updateTicker = 0;
        void tickUpdateTicker()
        {
            m_updateTicker++;
        }
        public int UpdateTicker
        {
            get { return m_updateTicker; }
        }

        // ============================================================
        // Animation and Movement
        // ============================================================

        public void Animate(int action, int frameCount, int repeatCount, bool reverse, bool repeat, int delay)
        {
            ((EntityViews.MobileView)GetView()).m_Animation.Animate(action, frameCount, repeatCount, reverse, repeat, delay);
        }

        public void Mobile_AddMoveEvent(int x, int y, int z, int facing)
        {
            m_movement.Mobile_ServerAddMoveEvent(x, y, z, facing);
        }

        public void Move_Instant(int x, int y, int z, int facing)
        {
            m_movement.Move_Instant(x, y, z, facing);
        }

        public void PlayerMobile_ChangeFacing(Direction facing)
        {
            m_movement.PlayerMobile_ChangeFacing(facing);
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
