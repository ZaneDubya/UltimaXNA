using Microsoft.Xna.Framework;
using UltimaXNA.Diagnostics;
using UltimaXNA.UltimaGUI;
using UltimaXNA.Scenes;

namespace UltimaXNA.Scenes
{
    public class SceneManager
    {
        static AScene m_CurrentScene;
        static bool m_isTransitioning = false;
        static Game m_Game;

        public static void Initialize(Game game)
        {
            m_Game = game;
        }

        public static bool IsTransitioning
        {
            get { return m_isTransitioning; }
        }

        public static AScene CurrentScene
        {
            get { return m_CurrentScene; }
            set
            {
                if (m_isTransitioning)
                    return;

                m_isTransitioning = true;

                if (m_CurrentScene != null)
                {
                    Logger.Debug("Starting scene transition from {0} to {1}", m_CurrentScene.GetType().Name, value.GetType().Name);
                    m_CurrentScene.SceneState = SceneState.TransitioningOff;

                    m_CurrentScene.TransitionCompleted += new TransitionCompleteHandler(delegate()
                    {
                        Logger.Debug("Scene transition complete.");
                        Logger.Debug("Disposing {0}.", m_CurrentScene.GetType().Name);

                        m_CurrentScene.Dispose();
                        m_CurrentScene = value;

                        if (!m_CurrentScene.IsInitialized)
                        {
                            Logger.Debug("Initializing {0}.", m_CurrentScene.GetType().Name);
                            m_CurrentScene.Intitialize();
                        }

                        m_isTransitioning = false;
                    });
                }
                else
                {
                    Logger.Debug("Starting scene {0}", value.GetType().Name);
                    m_CurrentScene = value;

                    if (!m_CurrentScene.IsInitialized)
                    {
                        Logger.Debug("Initializing {0}.", m_CurrentScene.GetType().Name);
                        m_CurrentScene.Intitialize();
                    }

                    m_isTransitioning = false;
                }
            }
        }

        public static void Update(GameTime gameTime)
        {
            AScene current = m_CurrentScene;

            if (m_CurrentScene != null)
                m_CurrentScene.Update(gameTime);

            //This is just incase a scene changes in the middle of updating.
            if (current != m_CurrentScene)
                m_CurrentScene.Update(gameTime);
        }

        public static void Draw(GameTime gameTime)
        {
            if (m_CurrentScene != null)
            {
                m_Game.GraphicsDevice.Clear(m_CurrentScene.ClearColor);
                m_CurrentScene.Draw(gameTime);
            }
        }

        public static void Reset()
        {
            if (!(m_CurrentScene is LoginScene))
                CurrentScene = new LoginScene(m_Game);
        }
    }
}
