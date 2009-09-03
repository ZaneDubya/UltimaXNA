﻿using Microsoft.Xna.Framework;
using UltimaXNA.Diagnostics;
using UltimaXNA.Extensions;

namespace UltimaXNA.SceneManagement
{
    public class SceneManager : DrawableGameComponent, ISceneService
    {
        IScene _currentScene;
        ILoggingService _loggingService;
        bool _isTransitioning = false;

        public bool IsTransitioning
        {
            get { return _isTransitioning; }
        }

        public IScene CurrentScene
        {
            get { return _currentScene; }
            set
            {
                if (_isTransitioning)
                    return;

                _isTransitioning = true;

                if (_currentScene != null)
                {
                    _loggingService.Debug("Starting scene transition from {0} to {1}", _currentScene.GetType().Name, value.GetType().Name);
                    _currentScene.SceneState = SceneState.TransitioningOff;

                    _currentScene.TransitionCompleted += new TransitionCompleteHandler(delegate()
                    {
                        _loggingService.Debug("Scene transition complete.");
                        _loggingService.Debug("Disposing {0}.", _currentScene.GetType().Name);

                        _currentScene.Dispose();
                        _currentScene = value;

                        if (!_currentScene.IsInitialized)
                        {
                            _loggingService.Debug("Initializing {0}.", _currentScene.GetType().Name);
                            _currentScene.Intitialize();
                        }

                        _isTransitioning = false;
                    });
                }
                else
                {
                    _loggingService.Debug("Starting scene {0}", value.GetType().Name);
                    _currentScene = value;

                    if (!_currentScene.IsInitialized)
                    {
                        _loggingService.Debug("Initializing {0}.", _currentScene.GetType().Name);
                        _currentScene.Intitialize();
                    }

                    _isTransitioning = false;
                }
            }
        }

        public SceneManager(Game game)
            : base(game)
        {
            _loggingService = game.Services.GetService<ILoggingService>(true);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            IScene current = _currentScene;

            if (_currentScene != null)
                _currentScene.Update(gameTime);

            //This is just incase a scene changes in the middle of updating.
            if (current != _currentScene)
                _currentScene.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            
            if (_currentScene != null)
            {
                //_currentScene.OnBeforeDraw(gameTime);
                _currentScene.Draw(gameTime);
                //_currentScene.DrawUI(gameTime);
                //_currentScene.DrawCursor(gameTime);
                //_currentScene.OnAfterDraw(gameTime);
            }
        }
    }
}
