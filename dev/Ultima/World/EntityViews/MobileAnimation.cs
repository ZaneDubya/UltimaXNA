/***************************************************************************
 *   MobileAnimation.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Core.Resources;
#endregion

namespace UltimaXNA.Ultima.World.EntityViews
{
    public class MobileAnimation
    {
        private Mobile Parent = null;
        private MobileAction m_action;
        private bool m_actionCanBeInteruptedByStand = false;
        
        private int m_actionIndex;
        public int ActionIndex
        {
            get
            {
                if (Parent.Body == 5 || Parent.Body == 6) // birds have weird action indexes. not sure if this is correct.
                    if (m_actionIndex > 8)
                        return m_actionIndex + 8;
                return m_actionIndex;
            }
        }

        public bool IsAnimating
        {
            get
            {
                if ((!m_actionCanBeInteruptedByStand) &&
                    (m_action == MobileAction.None ||
                    m_action == MobileAction.Stand || 
                    m_action == MobileAction.Walk || 
                    m_action == MobileAction.Run))
                    return false;
                return true;
            }
        }

        private float m_animationFrame = 0f;
        public float AnimationFrame
        {
            get
            {
                if (m_animationFrame >= m_FrameCount)
                    return m_FrameCount - 0.1f;
                else
                    return m_animationFrame;
            }
        }

        // We use these variables to 'hold' the last frame of an animation before 
        // switching to Stand Action.
        private bool m_IsAnimatationPaused = false;
        private int m_AnimationPausedMS = 0;
        private int PauseAnimationMS
        {
            get
            {
                if (Parent.IsClientEntity)
                    return 100;
                else
                    return 250;
            }
        }

        public MobileAnimation(Mobile parent)
        {
            Parent = parent;
        }

        public void Update(double frameMS)
        {
            // create a local copy of ms since last update.
            int msSinceLastUpdate = (int)frameMS;

            // If we are holding the current animation, then we should wait until our hold time is over
            // before switching to the queued Stand animation.
            if (m_IsAnimatationPaused)
            {
                m_AnimationPausedMS -= msSinceLastUpdate;
                if (m_AnimationPausedMS >= 0)
                {
                    // we are still holding. Do not update the current Animation frame.
                    return;
                }
                else
                {
                    // hold time is over, continue to Stand animation.
                    UnPauseAnimation();
                    m_action = MobileAction.Stand;
                    m_actionIndex = getActionIndex(MobileAction.Stand);
                    m_animationFrame = 0f;
                    m_FrameCount = 1;
                    m_FrameDelay = 0;
                }
            }

            if (m_action != MobileAction.None)
            {
                float msPerFrame = ((900f * (m_FrameDelay + 1)) / m_FrameCount);
                // Mounted movement is ~2x normal frame rate
                if (Parent.IsMounted && ((m_action == MobileAction.Walk) || (m_action == MobileAction.Run)))
                    msPerFrame /= 2.272727f;

                if (msPerFrame < 0)
                    return;

                m_animationFrame += (float)(frameMS / msPerFrame);

                if (Settings.Audio.FootStepSoundOn)
                {
                    if (m_action == MobileAction.Walk || m_action == MobileAction.Run)
                        MobileSounds.DoFootstepSounds(Parent as Mobile, m_animationFrame / m_FrameCount);
                    else
                        MobileSounds.ResetFootstepSounds(Parent as Mobile);
                }

                // When animations reach their last frame, if we are queueing to stand, then
                // hold the animation on the last frame.
                if (m_animationFrame >= m_FrameCount)
                {
                    if (m_repeatCount > 0)
                    {
                        m_animationFrame -= m_FrameCount;
                        m_repeatCount--;
                    }
                    else
                    {
                        // any requested actions are ended.
                        m_actionCanBeInteruptedByStand = false;
                        // Hold the last frame of the current action if animation is not Stand.
                        if (m_action == MobileAction.Stand)
                        {
                            m_animationFrame = 0;
                        }
                        else
                        {
                            // for most animations, hold the last frame. For Move animations, cycle through.
                            if (m_action == MobileAction.Run || m_action == MobileAction.Walk)
                                m_animationFrame -= m_FrameCount;
                            else
                                m_animationFrame = m_FrameCount - 0.001f;
                            PauseAnimation();
                        }
                            
                    }
                }
            }
        }

        public void UpdateAnimation()
        {
            animate(m_action, m_actionIndex, 0, false, false, 0, false);
        }

        public void Animate(MobileAction action)
        {
            int actionIndex = getActionIndex(action);
            animate(action, actionIndex, 0, false, false, 0, false);
        }

        public void Animate(int requestedIndex, int frameCount, int repeatCount, bool reverse, bool repeat, int delay)
        {
            // note that frameCount is NOT used. Not sure if this counts as a bug.
            MobileAction action = getActionFromIndex(requestedIndex);
            int actionIndex = getActionIndex(action, requestedIndex);
            animate(action, actionIndex, repeatCount, reverse, repeat, delay, true);
        }

        private int m_FrameCount, m_FrameDelay, m_repeatCount;
        private void animate(MobileAction action, int actionIndex, int repeatCount, bool reverse, bool repeat, int delay, bool isRequestedAction)
        {
            if (m_action == action)
            {
                if (m_IsAnimatationPaused)
                {
                    UnPauseAnimation();
                }
            }

            if (isRequestedAction)
                m_actionCanBeInteruptedByStand = true;

            if ((m_action != action) || (m_actionIndex != actionIndex))
            {
                // If we are switching from any action to a stand action, then hold the last frame of the 
                // current animation for a moment. Only Stand actions are held; thus when any hold ends,
                // then we know we were holding for a Stand action.
                if (!(m_action == MobileAction.None) && (action == MobileAction.Stand && m_action != MobileAction.Stand))
                {
                    if (m_action != MobileAction.None)
                        PauseAnimation();
                }
                else
                {
                    m_action = action;
                    UnPauseAnimation();
                    m_actionIndex = actionIndex;
                    m_animationFrame = 0f;

                    // get the resource provider
                    IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
                    IAnimationFrame[] frames = provider.GetAnimation(
                        Parent.Body, actionIndex, (int)Parent.Facing, Parent.Hue);
                    m_FrameCount = frames.Length;
                    m_FrameDelay = delay;
                    if (repeat == false)
                        m_repeatCount = 0;
                    else
                        m_repeatCount = repeatCount;
                }
            }
        }

        private void PauseAnimation()
        {
            if (!m_IsAnimatationPaused)
            {
                m_IsAnimatationPaused = true;
                m_AnimationPausedMS = PauseAnimationMS;
            }
        }

        private void UnPauseAnimation()
        {
            m_IsAnimatationPaused = false;
        }

        private int getActionIndex(MobileAction action)
        {
            return getActionIndex(action, -1);
        }

        private int getActionIndex(MobileAction action, int index)
        {
            if (Parent.Body.IsHuman)
            {
                switch (action)
                {
                    case MobileAction.None:
                        return getActionIndex(MobileAction.Stand, index);
                    case MobileAction.Walk:
                        if (Parent.IsMounted)
                            return (int)ActionIndexHumanoid.Mounted_RideSlow;
                        else
                            if (Parent.Flags.IsWarMode)
                                return (int)ActionIndexHumanoid.Walk_Warmode;
                            else
                            {
                                // if carrying a light source, return Walk_Armed.
                                if (Parent.LightSourceBodyID != 0)
                                    return (int)ActionIndexHumanoid.Walk_Armed;
                                else
                                    return (int)ActionIndexHumanoid.Walk;
                            }
                    case MobileAction.Run:
                        if (Parent.IsMounted)
                            return (int)ActionIndexHumanoid.Mounted_RideFast;
                        else
                        {
                            // if carrying a light source, return Run_Armed.
                            if (Parent.LightSourceBodyID != 0)
                                return (int)ActionIndexHumanoid.Run_Armed;
                            else
                                return (int)ActionIndexHumanoid.Run;
                        }
                    case MobileAction.Stand:
                        if (Parent.IsMounted)
                            return (int)ActionIndexHumanoid.Mounted_Stand;
                        else
                            if (Parent.Flags.IsWarMode)
                            {
                                // Also check if weapon type is 2h. Can be 1H or 2H
                                return (int)ActionIndexHumanoid.Stand_Warmode1H;
                            }
                            else
                                return (int)ActionIndexHumanoid.Stand;
                    case MobileAction.Death:
                        // randomly select die forwards or backwards.
                        if (Utility.RandomValue(0, 1) == 0)
                            return (int)ActionIndexHumanoid.Die_Backwards;
                        else
                            return (int)ActionIndexHumanoid.Die_Forwards;
                    case MobileAction.Attack:
                        if (Parent.IsMounted)
                        {
                            // check weapon type. Can be 1H, Bow, or XBow
                            return (int)ActionIndexHumanoid.Mounted_Attack_1H;
                        }
                        else
                        {
                            // check weapon type. Can be 1H, 2H across, 2H down, 2H jab, bow, xbow, or unarmed.
                            return (int)ActionIndexHumanoid.Attack_1H;
                        }
                    case MobileAction.Cast_Directed:
                        if (Parent.IsMounted)
                            return getActionIndex(MobileAction.Stand, index);
                        else
                            return (int)ActionIndexHumanoid.Cast_Directed;
                    case MobileAction.Cast_Area:
                        if (Parent.IsMounted)
                            return getActionIndex(MobileAction.Stand, index);
                        else
                            return (int)ActionIndexHumanoid.Cast_Area;
                    case MobileAction.GetHit:
                        if (Parent.IsMounted)
                            return getActionIndex(MobileAction.Stand, index);
                        else
                            return (int)ActionIndexHumanoid.Hit;
                    case MobileAction.Block:
                        if (Parent.IsMounted)
                            return getActionIndex(MobileAction.Stand, index);
                        else
                            return (int)ActionIndexHumanoid.Block_WithShield;
                    case MobileAction.Emote_Fidget_1:
                        if (Parent.IsMounted)
                            return getActionIndex(MobileAction.Stand, index);
                        else
                            return (int)ActionIndexHumanoid.Fidget_1;
                    case MobileAction.Emote_Fidget_2:
                        if (Parent.IsMounted)
                            return getActionIndex(MobileAction.Stand, index);
                        else
                            return (int)ActionIndexHumanoid.Fidget_2;
                    case MobileAction.Emote_Bow:
                        if (Parent.IsMounted)
                            return getActionIndex(MobileAction.Stand, index);
                        else
                            return (int)ActionIndexHumanoid.Emote_Bow;
                    case MobileAction.Emote_Salute:
                        if (Parent.IsMounted)
                            return getActionIndex(MobileAction.Stand, index);
                        else
                            return (int)ActionIndexHumanoid.Emote_Salute;
                    case MobileAction.Emote_Eat:
                        if (Parent.IsMounted)
                            return getActionIndex(MobileAction.Stand, index);
                        else
                            return (int)ActionIndexHumanoid.Emote_Eat;
                    default:
                        return (int)-1;
                }
            }
            else if (Parent.Body.IsAnimal)
            {
                switch (action)
                {
                    case MobileAction.None:
                        return getActionIndex(MobileAction.Stand, index);
                    case MobileAction.Walk:
                        return (int)ActionIndexAnimal.Walk;
                    case MobileAction.Run:
                        return (int)ActionIndexAnimal.Run;
                    case MobileAction.Stand:
                        return (int)ActionIndexAnimal.Stand;
                    case MobileAction.Death:
                        // randomly select die forwards or backwards.
                        if (Utility.RandomValue(0, 1) == 0)
                            return (int)ActionIndexAnimal.Die_Backwards;
                        else
                            return (int)ActionIndexAnimal.Die_Forwards;
                    case MobileAction.MonsterAction:
                        return index;
                    default:
                        return (int)-1;
                }
            }
            else if (Parent.Body.IsMonster)
            {
                switch (action)
                {
                    case MobileAction.None:
                        return getActionIndex(MobileAction.Stand, index);
                    case MobileAction.Walk:
                        return (int)ActionIndexMonster.Walk;
                    case MobileAction.Run:
                        return (int)ActionIndexMonster.Run;
                    case MobileAction.Stand:
                        return (int)ActionIndexMonster.Stand;
                    case MobileAction.Death:
                        // randomly select die forwards or backwards.
                        if (Utility.RandomValue(0, 1) == 0)
                            return (int)ActionIndexMonster.Die_Backwards;
                        else
                            return (int)ActionIndexMonster.Die_Forwards;
                    case MobileAction.MonsterAction:
                        return index;
                    default:
                        return (int)-1;
                }
            }

            return -1;
        }

        private MobileAction getActionFromIndex(int index)
        {
            if (Parent.Body.IsHuman)
            {
                switch ((ActionIndexHumanoid)index)
                {
                    case ActionIndexHumanoid.Walk:
                    case ActionIndexHumanoid.Walk_Armed:
                    case ActionIndexHumanoid.Walk_Warmode:
                    case ActionIndexHumanoid.Mounted_RideSlow:
                        return MobileAction.Walk;

                    case ActionIndexHumanoid.Mounted_RideFast:
                    case ActionIndexHumanoid.Run:
                    case ActionIndexHumanoid.Run_Armed:
                        return MobileAction.Run;

                    case ActionIndexHumanoid.Stand:
                    case ActionIndexHumanoid.Stand_Warmode1H:
                    case ActionIndexHumanoid.Stand_Warmode2H:
                    case ActionIndexHumanoid.Mounted_Stand:
                        return MobileAction.Stand;

                    case ActionIndexHumanoid.Fidget_1:
                        return MobileAction.Emote_Fidget_1;

                    case ActionIndexHumanoid.Fidget_2:
                        return MobileAction.Emote_Fidget_2;

                    case ActionIndexHumanoid.Attack_1H:
                    case ActionIndexHumanoid.Attack_Unarmed1:
                    case ActionIndexHumanoid.Attack_Unarmed2:
                    case ActionIndexHumanoid.Attack_2H_Down:
                    case ActionIndexHumanoid.Attack_2H_Across:
                    case ActionIndexHumanoid.Attack_2H_Jab:
                    case ActionIndexHumanoid.Attack_Bow:
                    case ActionIndexHumanoid.Attack_BowX:
                    case ActionIndexHumanoid.Mounted_Attack_1H:
                    case ActionIndexHumanoid.Mounted_Attack_Bow:
                    case ActionIndexHumanoid.Mounted_Attack_BowX:
                    case ActionIndexHumanoid.Attack_Unarmed3:
                        return MobileAction.Attack;

                    case ActionIndexHumanoid.Cast_Directed:
                        return MobileAction.Cast_Directed;

                    case ActionIndexHumanoid.Cast_Area:
                        return MobileAction.Cast_Area;

                    case ActionIndexHumanoid.Hit:
                        return MobileAction.GetHit;

                    case ActionIndexHumanoid.Die_Backwards:
                    case ActionIndexHumanoid.Die_Forwards:
                        return MobileAction.Death;

                    case ActionIndexHumanoid.Mounted_SlapHorse: // not coded or used?
                        return MobileAction.Stand;

                    case ActionIndexHumanoid.Block_WithShield:
                        return MobileAction.Block;

                    case ActionIndexHumanoid.Emote_Bow:
                        return MobileAction.Emote_Bow;

                    case ActionIndexHumanoid.Emote_Salute:
                        return MobileAction.Emote_Salute;

                    case ActionIndexHumanoid.Emote_Eat:
                        return MobileAction.Emote_Eat;
                }

                // special case animations. When casting a spell, the server will send animation indexes over 200,
                // which all seem to correspond to Cast_Directed. Example indexes are:
                // 200, 201, 203, 206, 209, 212, 215, 218, 221, 227, 230, 239, 245, 260, 266 and doubtless others.
                if (index >= 200)
                    return MobileAction.Cast_Directed;

                Tracer.Warn("Unknown action index {0}", index);
                return MobileAction.None;
            }
            else if (Parent.Body.IsAnimal)
            {
                switch ((ActionIndexAnimal)index)
                {
                    case ActionIndexAnimal.Stand:
                        return MobileAction.Stand;
                    case ActionIndexAnimal.Walk:
                        return MobileAction.Walk;
                    case ActionIndexAnimal.Run:
                        return MobileAction.Run;
                    default:
                        return MobileAction.MonsterAction;
                }
            }
            else if (Parent.Body.IsMonster)
            {
                switch ((ActionIndexMonster)index)
                {
                    case ActionIndexMonster.Stand:
                        return MobileAction.Stand;
                    case ActionIndexMonster.Walk:
                        return MobileAction.Walk;
                    case ActionIndexMonster.Run:
                        return MobileAction.Run;
                    default:
                        return MobileAction.MonsterAction;
                }
            }
            return MobileAction.None;
        }
    }

    public enum MobileAction
    {
        None,
        Walk,
        Run,
        Stand,
        Death,
        Attack,
        Cast_Directed,
        Cast_Area,
        GetHit,
        Block,
        Emote_Fidget_1,
        Emote_Fidget_2,
        Emote_Bow,
        Emote_Salute,
        Emote_Eat,
        MonsterAction
    }

    enum ActionIndexMonster
    {
        Walk = 0x00,
        Stand = 0x01,
        Die_Backwards = 0x02,
        Die_Forwards = 0x03,
        Attack1 = 0x04,
        Attack2 = 0x05,
        Attack3 = 0x06,
        Stumble = 0x07,
        MonsterMisc = 0x08,
        GetHit2 = 0x09,
        GetHit3 = 0x0A,
        Emote_Fidget_1 = 0x0B,
        Emote_Fidget_2 = 0x0C,
        Run = 0x13, 
    }

    enum ActionIndexAnimal
    {
        Walk = 0x00,
        Run = 0x01,
        Stand = 0x02,
        Graze = 0x03,
        Unknown1 = 0x04,
        Attack1 = 0x05,
        Attack2 = 0x06,
        Attack3 = 0x07,
        Die_Backwards = 0x08,
        Fidget1 = 0x09,
        Fidget2 = 0x0A,
        LieDown = 0x0B,
        Die_Forwards = 0x0C,
    }

    enum ActionIndexHumanoid
    {
        Walk = 0x00,
        Walk_Armed = 0x01,
        Run = 0x02,
        Run_Armed = 0x03,
        Stand = 0x04,
        Fidget_1 = 0x05,
        Fidget_2 = 0x06,
        Stand_Warmode1H = 0x07,
        Stand_Warmode2H = 0x08,
        Attack_1H = 0x09,
        Attack_Unarmed1 = 0x0A,
        Attack_Unarmed2 = 0x0B,
        Attack_2H_Down = 0x0C,
        Attack_2H_Across = 0x0D,
        Attack_2H_Jab = 0x0E,
        Walk_Warmode = 0x0F,
        Cast_Directed = 0x10,
        Cast_Area = 0x11,
        Attack_Bow = 0x12,
        Attack_BowX = 0x13,
        Hit = 0x14,
        Die_Backwards = 0x15,
        Die_Forwards = 0x16,
        Mounted_RideSlow = 0x17,
        Mounted_RideFast = 0x18,
        Mounted_Stand = 0x19,
        Mounted_Attack_1H = 0x1A,
        Mounted_Attack_Bow = 0x1B,
        Mounted_Attack_BowX = 0x1C,
        Mounted_SlapHorse = 0x1D,
        Block_WithShield = 0x1E,
        Attack_Unarmed3 = 0x1F,
        Emote_Bow = 0x20,
        Emote_Salute = 0x21,
        Emote_Eat = 0x22
    }
}
