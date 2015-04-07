using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.UltimaData;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.Network;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaWorld;

namespace UltimaXNA.UltimaLogin.Scenes
{
    public abstract class AScene : IDisposable
    {
        internal SceneManager Manager;

        public virtual TimeSpan TransitionOnLength { get { return TimeSpan.FromSeconds(0.05); } }
        public virtual TimeSpan TransitionOffLength { get { return TimeSpan.FromSeconds(0.05); } }

        protected UltimaEngine Engine { get; private set; }

        SceneState m_sceneState;
        float m_transitionAlpha;
        float m_elapsed;
        bool m_isInitialized;

        public bool IsInitialized
        {
            get { return m_isInitialized; }
            set { m_isInitialized = value; }
        }

        public SceneState SceneState
        {
            get { return m_sceneState; }
            set
            {
                m_sceneState = value;
                m_elapsed = 0;
            }
        }

        public event TransitionCompleteHandler TransitionCompleted;
        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdated;
        public event EventHandler<ProgressCompletedEventArgs> ProgressCompleted;
        public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

        public AScene()
        {
            m_sceneState = SceneState.TransitioningOn;
        }

        public virtual void Intitialize(UltimaEngine engine)
        {
            Engine = engine;
        }

        public virtual void Update(double totalTime, double frameTime)
        {
            m_elapsed += (float)frameTime;

            switch (m_sceneState)
            {
                case SceneState.TransitioningOn:
                    {
                        m_transitionAlpha = 1 - (m_elapsed / (float)TransitionOffLength.TotalSeconds);

                        if (m_elapsed >= (float)TransitionOnLength.TotalSeconds)
                        {
                            m_elapsed = 0;
                            m_sceneState = SceneState.Active;
                        }

                        break;
                    }
                case SceneState.TransitioningOff:
                    {
                        m_transitionAlpha = m_elapsed / (float)TransitionOnLength.TotalSeconds;

                        if (m_elapsed >= (float)TransitionOffLength.TotalSeconds)
                        {
                            m_elapsed = 0;
                            m_sceneState = SceneState.None;

                            if (TransitionCompleted != null)
                                TransitionCompleted();
                        }

                        break;
                    }
            }
        }

        protected virtual Texture2D PostProcess(GameTime gametTime, Texture2D sceneTexture)
        {
            return sceneTexture;
        }
        
        protected virtual void OnProgressUpdate(object sender, ProgressUpdateEventArgs e)
        {
            if (ProgressUpdated != null)
                ProgressUpdated(sender, e);
        }

        protected virtual void OnProgressComplete(object sender, ProgressCompletedEventArgs e)
        {
            if (ProgressCompleted != null)
                ProgressCompleted(sender, e);
        }

        protected virtual void OnStatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            if (StatusUpdate != null)
                StatusUpdate(sender, e);
        }

        protected virtual void OnTransitionComplete()
        {
            if (TransitionCompleted != null)
                TransitionCompleted();
        }

        public virtual void Dispose()
        {
            Engine.UserInterface.Reset();
        }
    }

    public enum SceneState
    {
        TransitioningOn,
        Active,
        TransitioningOff,
        None
    }

    public delegate void TransitionCompleteHandler();
}
