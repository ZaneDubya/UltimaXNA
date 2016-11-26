/***************************************************************************
 *   Mobile.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Ultima.World.Entities.Items.Containers;
using UltimaXNA.Ultima.World.Entities.Mobiles.Animations;
using UltimaXNA.Ultima.World.EntityViews;
using UltimaXNA.Ultima.World.Maps;
#endregion

namespace UltimaXNA.Ultima.World.Entities.Mobiles
{
    /// <summary>
    /// A mobile object - monster, NPC, or player.
    /// TODO: This class needs a serious refactor.
    /// </summary>
    public class Mobile : AEntity
    {
        // ============================================================================================================
        // Ctor, Dispose, Update, and CreateView()
        // ============================================================================================================

        public Mobile(Serial serial, Map map)
            : base(serial, map)
        {
            Animation = new MobileAnimation(this);
            Equipment = new MobileEquipment(this);
            m_movement = new MobileMovement(this);
            m_movement.RequiresUpdate = true;

            Health = new CurrentMaxValue();
            Stamina = new CurrentMaxValue();
            Mana = new CurrentMaxValue();
            Followers = new CurrentMaxValue();
            Weight = new CurrentMaxValue();
            Flags = new MobileFlags();
        }

        public override void Dispose()
        {
            base.Dispose();
            Equipment.ClearEquipment();
        }

        protected override void OnTileChanged(int x, int y)
        {
            base.OnTileChanged(x, y);
            if (Tile == null)
                return;

            if (Body.IsHumanoid)
            {
                bool foundChairData = false;
                List<Item> items = Tile.GetItemsBetweenZ(Z - 1, Z + 1); // match legacy - sit on chairs between z-1 and z+1.
                foreach (Item i in items)
                {
                    Chairs.ChairData data;
                    if (Chairs.CheckItemAsChair(i.ItemID, out data))
                    {
                        foundChairData = true;
                        ChairData = data;
                        SittingZ = i.Z - Z;
                        Animation.Clear();
                        Tile.ForceSort();
                        break;
                    }
                }
                if (!foundChairData)
                    ChairData = Chairs.ChairData.Null;
            }
        }

        public override void Update(double frameMS)
        {
            if (WorldView.AllLabels)
            {
                AddOverhead(MessageTypes.Label, Name, 3, NotorietyHue, false);
            }

            if (!m_movement.Position.IsNullPosition)
            {
                // update the movement and then animation objects
                m_movement.Update(frameMS);
                // get/update the animation index.
                if (IsMoving)
                {
                    if (IsRunning)
                        Animation.Animate(MobileAction.Run);
                    else
                        Animation.Animate(MobileAction.Walk);
                }
                else
                {
                    if (!Animation.IsAnimating)
                        Animation.Animate(MobileAction.Stand);
                }
                Animation.Update(frameMS);
            }

            base.Update(frameMS);
        }

        protected override AEntityView CreateView()
        {
            return new MobileView(this);
        }

        /// <summary>
        /// Manages the animation state (what animation is playing, how far along is the animation, etc.) for this
        /// mobile view. Exposed as public so that we can receive animations from the server (e.g. emotes).
        /// </summary>
        public readonly MobileAnimation Animation;

        // ============================================================================================================
        // Movement and Facing
        // ============================================================================================================

        protected MobileMovement m_movement;

        public Direction Facing
        {
            get
            {
                return m_movement.Facing & Direction.FacingMask;
            }
            set
            {
                m_movement.Facing = value;
            }
        }

        public Direction DrawFacing
        {
            get
            {
                Direction facing = Facing;
                if (!IsSitting)
                    return facing;
                return ChairData.GetSittingFacing(facing);
            }
        }

        public bool IsMoving { get { return m_movement.IsMoving; } }

        public bool IsRunning { get { return m_movement.IsRunning; } }

        public Position3D DestinationPosition
        {
            get
            {
                if (!IsMoving)
                    return this.Position;
                else
                    return m_movement.GoalPosition;
            }
        }

        public bool IsSitting
        {
            get
            {
                if (!Animation.IsStanding || m_movement.IsMoving)
                    return false;
                if (ChairData.ItemID == Chairs.ChairData.Null.ItemID)
                    return false;
                return true;
            }
        }

        public Chairs.ChairData ChairData = Chairs.ChairData.Null;
        public int SittingZ = 0;

        // ============================================================================================================
        // Properties
        // ============================================================================================================

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

        public bool PlayerCanChangeName = false;

        public int Strength, Dexterity, Intelligence, StatCap, Luck, Gold;
        public CurrentMaxValue Health, Stamina, Mana;
        public CurrentMaxValue Followers;
        public CurrentMaxValue Weight;
        public int ArmorRating, ResistFire, ResistCold, ResistPoison, ResistEnergy;
        public int DamageMin, DamageMax;

        public int Notoriety;

        public bool IsAlive
        {
            get { return Health.Current > 0; }
        }

        // ============================================================================================================
        // Equipment and Mount
        // ============================================================================================================

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

        public int LightSourceBodyID
        {
            get
            {
                int bodyID = Equipment[(int)EquipLayer.TwoHanded] != null ? Equipment[(int)EquipLayer.TwoHanded].ItemData.AnimID : 0;
                if (bodyID >= 500 && bodyID <= 505)
                    return bodyID;
                else
                    return 0;
            }
        }

        public void WearItem(Item i, int slot)
        {
            Equipment[slot] = i;
            m_OnUpdated?.Invoke(this);
            if (slot == (int)EquipLayer.Mount)
            {
                // Do we do something here?
            }
        }

        public void RemoveItem(Serial serial)
        {
            Equipment.RemoveBySerial(serial);
            m_OnUpdated?.Invoke(this);
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

        public ContainerItem Backpack
        {
            get
            {
                return (ContainerItem)Equipment[(int)EquipLayer.Backpack];
            }
        }

        public ContainerItem VendorShopContents
        {
            get
            {
                return (ContainerItem)Equipment[(int)EquipLayer.ShopBuy];
            }
        }

        // ============================================================================================================
        // Appearance and Hues
        // ============================================================================================================

        static int[] s_HumanoidBodyIDs = new int[] {
            183, 184, 185, 186, // savages
            400, 401, 402, 403, // humans
            694, 695,
            744, 745,
            750, 751,
            987, 988, 990, 991,
            994,
            605, 606, 607, 608, // elves
            666, 667 // gargoyles. 666. Clever.
        };

        int m_BodyID = 0;
        public Body Body
        {
            get
            {
                if (m_BodyID >= 402 && m_BodyID <= 403) // 402 == 400 and 403 == 401
                    return m_BodyID - 2;
                return m_BodyID;
            }
            set
            {
                m_BodyID = value;
                m_OnUpdated?.Invoke(this);
            }
        }

        int m_Hue;
        public override int Hue
        {
            get {
                if (Flags.IsHidden)
                    return 0x3E7;
                if (Flags.IsPoisoned)
                    return 0x1CE;
                return m_Hue;
            }
            set
            {
                m_Hue = value;
                m_OnUpdated?.Invoke(this);
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

        // ============================================================================================================
        // Animation and Movement
        // ============================================================================================================

        public void Animate(int action, int frameCount, int repeatCount, bool reverse, bool repeat, int delay)
        {
            Animation.Animate(action, frameCount, repeatCount, reverse, repeat, delay);
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

        public override string ToString()
        {
            return base.ToString() + " | " + Name;
        }
    }
}
