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
using UltimaXNA.Core.Resources;
#endregion

namespace UltimaXNA.Ultima.World.Entities.Mobiles.Animations
{
    /// <summary>
    /// Maintains and updates a mobile's animations. Receives animations from server, and when moving, updates the movement animation.
    /// TODO: This class needs a serious refactor.
    /// </summary>
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

        public bool IsStanding
        {
            get
            {
                return (m_action == MobileAction.Stand);
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
                    return 350;
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
                    m_actionIndex = ActionTranslator.GetActionIndex(Parent, MobileAction.Stand);
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

        /// <summary>
        /// Immediately clears all animation data, sets mobile action to stand.
        /// </summary>
        public void Clear()
        {
            m_action = MobileAction.Stand;
            m_animationFrame = 0;
            m_FrameCount = 1;
            m_FrameDelay = 0;
            m_IsAnimatationPaused = true;
            m_repeatCount = 0;
            m_actionIndex = ActionTranslator.GetActionIndex(Parent, MobileAction.Stand);
        }

        public void UpdateAnimation()
        {
            animate(m_action, m_actionIndex, 0, false, false, 0, false);
        }

        public void Animate(MobileAction action)
        {
            int actionIndex = ActionTranslator.GetActionIndex(Parent, action);
            animate(action, actionIndex, 0, false, false, 0, false);
        }

        public void Animate(int requestedIndex, int frameCount, int repeatCount, bool reverse, bool repeat, int delay)
        {
            // note that frameCount is NOT used. Not sure if this counts as a bug.
            MobileAction action = ActionTranslator.GetActionFromIndex(Parent.Body, requestedIndex);
            int actionIndex = ActionTranslator.GetActionIndex(Parent, action, requestedIndex);
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

                    // get the frames of the base body - we need to count the number of frames in this animation.
                    IResourceProvider provider = Services.Get<IResourceProvider>();
                    int body = Parent.Body, hue = 0;
                    AAnimationFrame[] frames = provider.GetAnimation(body, ref hue, actionIndex, (int)Parent.DrawFacing);
                    if (frames != null)
                    {
                        m_FrameCount = frames.Length;
                        m_FrameDelay = delay;
                        if (repeat == false)
                            m_repeatCount = 0;
                        else
                            m_repeatCount = repeatCount;
                    }
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
    }
}
