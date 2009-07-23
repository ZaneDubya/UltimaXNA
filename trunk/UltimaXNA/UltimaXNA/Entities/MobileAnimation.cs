/***************************************************************************
 *   MobileAnimation.cs
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
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.Entities
{
    public class MobileAnimation
    {
        public bool WarMode;
        // Issue 6 - Missing mounted animations - http://code.google.com/p/ultimaxna/issues/detail?id=6 - Smjert
        public MobileAction Action;
        // Issue 6 - End
        public float AnimationFrame = 0f;
        private float _animationStep = 0f;
        public bool IsMounted = false;
        public int BodyID;

        private int _animationType
        {
            get { return Data.Mobtypes.AnimationType(BodyID); }
        }

        public MobileAnimation()
        {
            _animationStep = 0f;
            Action = MobileAction.Stand;
        }

        private bool _doHaltAnimation;
        private int _timeHalted;
        public int HoldAnimationMS = 200;
        public bool HaltAnimation
        {
            get { return _doHaltAnimation; }
            set
            {
                if (value)
                {
                    if ((!_doHaltAnimation) && (Action != MobileAction.Stand))
                    {
                        _doHaltAnimation = true;
                        _timeHalted = Int32.MaxValue;
                    }
                }
                else
                {
                    _doHaltAnimation = false;
                    _timeHalted = Int32.MaxValue;
                }
            }
        }

        public void SetAnimation(MobileAction nAction)
        {
            HaltAnimation = false;
            if (Action != nAction)
            {
                Action = nAction;
                AnimationFrame = 0f;
                _FrameCount = 0;
                _FrameDelay = 0;
            }
        }

        private int _FrameCount, _FrameDelay, _repeatCount;
        public void SetAnimation(MobileAction action, int frameCount, int repeatCount, bool reverse, bool repeat, int delay)
        {
            HaltAnimation = false;
            if (Action != action)
            {
                Action = action;
                AnimationFrame = 0f;
                _FrameCount = frameCount;
                _FrameDelay = delay;
                if (repeat == false)
                {
                    _repeatCount = 0;
                }
                else
                {
                    _repeatCount = repeatCount;
                }
            }
        }



        private int msGameTime(GameTime gameTime)
        {
            return (int)(gameTime.TotalRealTime.Ticks / TimeSpan.TicksPerMillisecond);
        }

        public void Update(GameTime gameTime)
        {
            _animationStep = (float)((_FrameCount * (_FrameDelay + 1)) * 10);
            if (_animationStep != 0)
            {
                if (HaltAnimation)
                {
                    if (_timeHalted == Int32.MaxValue)
                        _timeHalted = msGameTime(gameTime);
                }

                if (Action != MobileAction.Stand)
                {
                    // advance the animation one step, based on gametime passed.
                    float iTimeStep = ((float)gameTime.ElapsedRealTime.TotalMilliseconds / _animationStep) / _FrameCount;
                    if (IsMovementAction(Action))
                        iTimeStep *= (IsMounted) ? 2 : 1;
                    AnimationFrame += iTimeStep;

                    // We have a special case for movement actions that have not been
                    // explicity halted. All other actions end when they reach their
                    // final frame.
                    if (IsMovementAction(Action) && !HaltAnimation)
                    {
                        if (AnimationFrame >= 1f)
                            AnimationFrame %= 1f;
                    }
                    else
                    {
                        if (AnimationFrame >= 1f || HaltAnimation)
                        {
                            if (_repeatCount == 0)
                            {
                                // we have to return to the previous frame.
                                AnimationFrame -= iTimeStep;
                                if (HaltAnimation)
                                {
                                    // hold the animation for a quick moment, then set to stand.
                                    if ((msGameTime(gameTime) - _timeHalted) >= HoldAnimationMS)
                                    {
                                        SetAnimation(MobileAction.Stand);
                                        HaltAnimation = false;
                                    }
                                }
                                else
                                {
                                    HaltAnimation = true;
                                }
                            }
                            else
                            {
                                AnimationFrame %= 1f;
                                _repeatCount--;
                            }
                        }
                    }
                }
            }
            else
            {
                AnimationFrame = 0;
            }
        }


        public bool IsMovementAction(MobileAction a)
        {
            if (a == MobileAction.Walk || a == MobileAction.Run)
                return true;
            else
                return false;
        }

        public int GetAction()
        {
            switch (_animationType)
            {
                case 0: // high detail
                    return getAction_HighDetail();
                case 1: // low detail
                    return getAction_LowDetail();
                case 2: // people & accessories
                    return (int)getAction_HighestDetail();
                default:
                    return -1;
            }
        }

        private int getAction_HighDetail()
        {
            switch (Action)
            {
                case MobileAction.Walk:
                    return 0;
                case MobileAction.Stand:
                    return 1;
                case MobileAction.Death:
                    return Data.BodyConverter.DeathAnimationIndex(BodyID);
                default:
                    return (int)Action;
            }
        }

        private int getAction_LowDetail()
        {
            switch (Action)
            {
                case MobileAction.Walk:
                    return 0;
                case MobileAction.Run:
                    return 1;
                case MobileAction.Stand:
                    return 2;
                case MobileAction.Death:
                    return Data.BodyConverter.DeathAnimationIndex(BodyID);
                default:
                    return (int)Action;
            }
        }

        private int getAction_HighestDetail()
        {
            if (IsMounted)
            {
                switch (Action)
                {
                    case MobileAction.Walk:
                        return 23;
                    case MobileAction.Run:
                        return 24;
                    case MobileAction.Stand:
                        return 25;
                    default:
                        return (int)Action;
                }
            }

            switch (Action)
            {
                case MobileAction.Walk:
                    if (WarMode)
                        return (int)HighestDetailActions.WalkInAttackStance;
                    else
                        return (int)HighestDetailActions.Walk;
                case MobileAction.Run:
                    return (int)HighestDetailActions.Run;
                case MobileAction.Stand:
                    if (WarMode)
                        return (int)HighestDetailActions.StandAttackStance;
                    else
                        return (int)HighestDetailActions.Stand;
                case MobileAction.Death:
                    return Data.BodyConverter.DeathAnimationIndex(BodyID);
                default:
                    return (int)Action;
            }
        }
    }

    #region UnitEnums
    public enum MobileAction
    {
        Walk,
        Run,
        Stand,
        Death
    }


    public enum HighestDetailActions
    {
        Walk = 0x00,
        WalkArmed = 0x01,
        Run = 0x02,
        RunArmed = 0x03,
        Stand = 0x04,
        StandAttackStance = 0x07,
        WalkInAttackStance = 0x0f,
        /* shiftshoulders = 0x05,
        handsonhips = 0x06,
        attackstanceshort = 0x07,
        attackstancelonger = 0x08,
        swingattackwithknofe = 0x09,
        stabunderhanded = 0x0a,
        swingattackoverhandwithsword = 0x0b,
        swingattackwithswordoverandside = 0x0c,
        swingattackwithswordside = 0x0d,
        stabwithpointofsword = 0x0e,
        readystance = 0x0f,
        magicbutterchurn = 0x10,
        handsoverheadbalerina = 0x11,
        bowshot = 0x12,
        crossbow = 0x13,
        gethit = 0x14,
        falldownanddiebackwards = 0x15,
        falldownanddieforwards = 0x16,
        ridehorselong = 0x17,
        ridehorsemedium = 0x18,
        ridehorseshort = 0x19,
        swingswordfromhorse = 0x1a,
        normalbowshotonhorse = 0x1b,
        crossbowshot = 0x1c,
        block2onhorsewithshield = 0x1d,
        blockongroundwithshield = 0x1e,
        swinginterrupt = 0x1f,
        bowdeep = 0x20,
        salute = 0x21,
        scratchhead = 0x22,
        onefootforwardfor2secs = 0x23,
        same = 0x24*/
    }
    public enum EquipLayer : int
    {
        /// <summary>
        /// Invalid layer.
        /// </summary>
        Body = 0x00,
        /// <summary>
        /// First valid layer. Equivalent to <c>Layer.OneHanded</c>.
        /// </summary>
        FirstValid = 0x01,
        /// <summary>
        /// One handed weapon.
        /// </summary>
        OneHanded = 0x01,
        /// <summary>
        /// Two handed weapon or shield.
        /// </summary>
        TwoHanded = 0x02,
        /// <summary>
        /// Shoes.
        /// </summary>
        Shoes = 0x03,
        /// <summary>
        /// Pants.
        /// </summary>
        Pants = 0x04,
        /// <summary>
        /// Shirts.
        /// </summary>
        Shirt = 0x05,
        /// <summary>
        /// Helmets, hats, and masks.
        /// </summary>
        Helm = 0x06,
        /// <summary>
        /// Gloves.
        /// </summary>
        Gloves = 0x07,
        /// <summary>
        /// Rings.
        /// </summary>
        Ring = 0x08,
        /// <summary>
        /// Talismans.
        /// </summary>
        Talisman = 0x09,
        /// <summary>
        /// Gorgets and necklaces.
        /// </summary>
        Neck = 0x0A,
        /// <summary>
        /// Hair.
        /// </summary>
        Hair = 0x0B,
        /// <summary>
        /// Half aprons.
        /// </summary>
        Waist = 0x0C,
        /// <summary>
        /// Torso, inner layer.
        /// </summary>
        InnerTorso = 0x0D,
        /// <summary>
        /// Bracelets.
        /// </summary>
        Bracelet = 0x0E,
        /// <summary>
        /// Unused.
        /// </summary>
        Unused_xF = 0x0F,
        /// <summary>
        /// Beards and mustaches.
        /// </summary>
        FacialHair = 0x10,
        /// <summary>
        /// Torso, outer layer.
        /// </summary>
        MiddleTorso = 0x11,
        /// <summary>
        /// Earings.
        /// </summary>
        Earrings = 0x12,
        /// <summary>
        /// Arms and sleeves.
        /// </summary>
        Arms = 0x13,
        /// <summary>
        /// Cloaks.
        /// </summary>
        Cloak = 0x14,
        /// <summary>
        /// Backpacks.
        /// </summary>
        Backpack = 0x15,
        /// <summary>
        /// Torso, outer layer.
        /// </summary>
        OuterTorso = 0x16,
        /// <summary>
        /// Leggings, outer layer.
        /// </summary>
        OuterLegs = 0x17,
        /// <summary>
        /// Leggings, inner layer.
        /// </summary>
        InnerLegs = 0x18,
        /// <summary>
        /// Last valid non-internal layer. Equivalent to <c>Layer.InnerLegs</c>.
        /// </summary>
        LastUserValid = 0x18,
        /// <summary>
        /// Mount item layer.
        /// </summary>
        Mount = 0x19,
        /// <summary>
        /// Vendor 'buy pack' layer.
        /// </summary>
        ShopBuy = 0x1A,
        /// <summary>
        /// Vendor 'resale pack' layer.
        /// </summary>
        ShopResale = 0x1B,
        /// <summary>
        /// Vendor 'sell pack' layer.
        /// </summary>
        ShopSell = 0x1C,
        /// <summary>
        /// Bank box layer.
        /// </summary>
        Bank = 0x1D,
        /// <summary>
        /// Last valid layer. Equivalent to <c>Layer.Bank</c>.
        /// </summary>
        LastValid = 0x1D
    }
    #endregion
    #region WornEquipmentClass
    public class WornEquipment
    {
        private Item[] m_Equipment;
        private Mobile m_Owner;
        private int m_UpdateTicker = 0;

        public int UpdateTicker
        {
            get { return m_UpdateTicker; }
        }

        public WornEquipment(Mobile nOwner)
        {
            m_Equipment = new Item[(int)EquipLayer.LastValid + 1];
            m_Owner = nOwner;
        }

        public Item this[int nIndex]
        {
            get
            {
                if (nIndex > (int)EquipLayer.LastValid)
                    return null;
                return m_Equipment[nIndex];
            }
            set
            {
                if (value == null)
                {
                    if (m_Equipment[nIndex] != null)
                    {
                        m_Equipment[nIndex].Wearer = null;
                        m_Equipment[nIndex].Dispose();
                        m_Equipment[nIndex] = null;
                    }
                }
                else
                {
                    m_Equipment[nIndex] = value;
                    value.Wearer = m_Owner;
                }
                m_UpdateTicker++;
            }
        }

        public void ClearEquipment()
        {
            for (int i = 0; i <= (int)EquipLayer.LastValid; i++)
            {
                this[i] = null;
            }
            m_UpdateTicker++;
        }

        public void RemoveBySerial(Serial serial)
        {
            for (int i = 0; i <= (int)EquipLayer.LastValid; i++)
            {
                if (this[i] != null)
                    if (this[i].Serial == serial)
                    {
                        this[i] = null;
                    }
            }
            m_UpdateTicker++;
        }
    }
    #endregion
}
