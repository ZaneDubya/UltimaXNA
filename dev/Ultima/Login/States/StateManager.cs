using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.UI;

namespace UltimaXNA.Ultima.Login.States {
    public class StateManager {
        AState m_Current;
        bool m_isTransitioning;

        public bool IsTransitioning {
            get { return m_isTransitioning; }
        }

        public AState CurrentState {
            get { return m_Current; }
            set {
                if (m_isTransitioning)
                    return;
                m_isTransitioning = true;
                if (m_Current != null) {
                    Tracer.Debug("Starting scene transition from {0} to {1}", m_Current.GetType().Name, value == null ? "Null" : value.GetType().Name);
                    m_Current.TransitionState = TransitionState.TransitioningOff;

                    if (value == null) {
                        m_Current.Dispose();
                        m_Current = null;
                    }
                    else {
                        m_Current.TransitionCompleted += delegate () {
                            Tracer.Debug("Scene transition complete. Disposing {0}.", m_Current.GetType().Name);
                            m_Current.Dispose();
                            m_Current = value;
                            if (m_Current != null) {
                                m_Current.Manager = this;
                                if (!m_Current.IsInitialized) {
                                    Tracer.Debug("Initializing {0}.", m_Current.GetType().Name);
                                    m_Current.Intitialize();
                                }
                            }
                            m_isTransitioning = false;
                        };
                    }
                }
                else {
                    Tracer.Debug("Starting scene {0}", value.GetType().Name);
                    m_Current = value;
                    m_Current.Manager = this;
                    if (!m_Current.IsInitialized) {
                        Tracer.Debug("Initializing {0}.", m_Current.GetType().Name);
                        m_Current.Intitialize();
                    }
                    m_isTransitioning = false;
                }
            }
        }

        UserInterfaceService m_UserInterface;
        LoginModel m_Login;

        public StateManager() {
            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
            m_Login = ServiceRegistry.GetService<LoginModel>();
        }

        public void Update(double totalTime, double frameTime) {
            AState current = m_Current;
            if (m_Current != null)
                m_Current.Update(totalTime, frameTime);
            //This is just in case a scene changes in the middle of updating.
            if (current != m_Current && m_Current != null)
                m_Current.Update(totalTime, frameTime);
        }

        public void ResetToLoginScreen() {
            m_Login.Client.Disconnect();
            m_UserInterface.Reset();
            if (!(m_Current is LoginState))
                CurrentState = new LoginState();
        }
    }
}
