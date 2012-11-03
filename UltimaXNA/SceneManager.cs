using Microsoft.Xna.Framework;
using UltimaXNA.Diagnostics;
using UltimaXNA.Core.Extensions;
using UltimaXNA.UILegacy;
using UltimaXNA.Scene;

namespace UltimaXNA
{
    public class SceneManager
    {
        static BaseScene _currentScene;
        static bool _isTransitioning = false;
        static Game m_Game;

        public static bool IsTransitioning
        {
            get { return _isTransitioning; }
        }

        public static BaseScene CurrentScene
        {
            get { return _currentScene; }
            set
            {
                if (_isTransitioning)
                    return;

                _isTransitioning = true;

                if (_currentScene != null)
                {
                    Logger.Debug("Starting scene transition from {0} to {1}", _currentScene.GetType().Name, value.GetType().Name);
                    _currentScene.SceneState = SceneState.TransitioningOff;

                    _currentScene.TransitionCompleted += new TransitionCompleteHandler(delegate()
                    {
                        Logger.Debug("Scene transition complete.");
                        Logger.Debug("Disposing {0}.", _currentScene.GetType().Name);

                        _currentScene.Dispose();
                        _currentScene = value;

                        if (!_currentScene.IsInitialized)
                        {
                            Logger.Debug("Initializing {0}.", _currentScene.GetType().Name);
                            _currentScene.Intitialize();
                        }

                        _isTransitioning = false;
                    });
                }
                else
                {
                    Logger.Debug("Starting scene {0}", value.GetType().Name);
                    _currentScene = value;

                    if (!_currentScene.IsInitialized)
                    {
                        Logger.Debug("Initializing {0}.", _currentScene.GetType().Name);
                        _currentScene.Intitialize();
                    }

                    _isTransitioning = false;
                }
            }
        }

        public static void Initialize(Game game)
        {
            m_Game = game;
        }

        public static void Update(GameTime gameTime)
        {
            BaseScene current = _currentScene;

            if (_currentScene != null)
                _currentScene.Update(gameTime);

            //This is just incase a scene changes in the middle of updating.
            if (current != _currentScene)
                _currentScene.Update(gameTime);
        }

        public static void Draw(GameTime gameTime)
        {
            if (_currentScene != null)
            {
                m_Game.GraphicsDevice.Clear(_currentScene.ClearColor);
                _currentScene.Draw(gameTime);
                UserInterface.Draw(gameTime);
            }
        }

        public static void Reset()
        {
            if (!(_currentScene is LoginScene))
                CurrentScene = new LoginScene(m_Game);
        }
    }
}
