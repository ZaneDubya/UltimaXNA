using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.UI;

namespace UltimaXNA.Ultima.Login.States {
    public abstract class AState : IDisposable {
        internal StateManager Manager;

        UserInterfaceService m_UserInterface;

        public virtual TimeSpan TransitionOnLength { get { return TimeSpan.FromSeconds(0.05); } }
        public virtual TimeSpan TransitionOffLength { get { return TimeSpan.FromSeconds(0.05); } }

        TransitionState m_TransitionState;
        float m_transitionAlpha;
        float m_elapsed;
        bool m_isInitialized;

        public bool IsInitialized {
            get { return m_isInitialized; }
            set { m_isInitialized = value; }
        }

        public TransitionState TransitionState {
            get { return m_TransitionState; }
            set {
                m_TransitionState = value;
                m_elapsed = 0;
            }
        }

        public event TransitionCompleteHandler TransitionCompleted;

        protected AState() {
            m_TransitionState = TransitionState.TransitioningOn;
        }

        public virtual void Intitialize() {
            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
        }

        public virtual void Update(double totalTime, double frameTime) {
            m_elapsed += (float)frameTime;

            switch (m_TransitionState) {
                case TransitionState.TransitioningOn: {
                        m_transitionAlpha = 1 - (m_elapsed / (float)TransitionOffLength.TotalSeconds);

                        if (m_elapsed >= (float)TransitionOnLength.TotalSeconds) {
                            m_elapsed = 0;
                            m_TransitionState = TransitionState.Active;
                        }

                        break;
                    }
                case TransitionState.TransitioningOff: {
                        m_transitionAlpha = m_elapsed / (float)TransitionOnLength.TotalSeconds;

                        if (m_elapsed >= (float)TransitionOffLength.TotalSeconds) {
                            m_elapsed = 0;
                            m_TransitionState = TransitionState.None;

                            if (TransitionCompleted != null)
                                TransitionCompleted();
                        }

                        break;
                    }
            }
        }

        protected virtual Texture2D PostProcess(GameTime gametTime, Texture2D sceneTexture) {
            return sceneTexture;
        }

        protected virtual void OnTransitionComplete() {
            if (TransitionCompleted != null)
                TransitionCompleted();
        }

        public virtual void Dispose() {
            m_UserInterface.Reset();
        }
    }

    public delegate void TransitionCompleteHandler();
}
