using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Data;
using UltimaXNA.Diagnostics;
using UltimaXNA.Extensions;
using UltimaXNA.Input;
using UltimaXNA.Network;
using UltimaXNA.UILegacy;
using UltimaXNA.TileEngine;

namespace UltimaXNA.SceneManagement
{
    public abstract class BaseScene : IScene, IDisposable
    {
        public virtual TimeSpan TransitionOnLength { get { return TimeSpan.FromSeconds(0.05); } }
        public virtual TimeSpan TransitionOffLength { get { return TimeSpan.FromSeconds(0.05); } }

        Game _game;
        
        ISceneService _sceneService;
        IInputState _inputService;
        IUIManager _uiService;
        IIsometricRenderer _worldService;

        ContentManager _content;
        SceneState _sceneState;
        Color _clearColor;
        float _transitionAlpha;
        float _elapsed;
        bool _isInitialized;

        public bool IsInitialized
        {
            get { return _isInitialized; }
            set { _isInitialized = value; }
        }

        public Game Game
        {
            get { return _game; }
        }

        public GraphicsDevice GraphicsDevice
        {
            get { return _game.GraphicsDevice; }
        }

        public Color ClearColor
        {
            get { return _clearColor; }
            set { _clearColor = value; }
        }

        public ContentManager Content
        {
            get { return _content; }
            set { _content = value; }
        }

        public SceneState SceneState
        {
            get { return _sceneState; }
            set
            {
                _sceneState = value;
                _elapsed = 0;
            }
        }

        public ISceneService SceneManager
        {
            get { return _sceneService; }
        }

        public IInputState Input
        {
            get { return _inputService; }
        }

        public IUIManager UI
        {
            get { return _uiService; }
        }

        public IIsometricRenderer World
        {
            get { return _worldService; }
        }

        public event TransitionCompleteHandler TransitionCompleted;
        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdated;
        public event EventHandler<ProgressCompletedEventArgs> ProgressCompleted;
        public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

        public BaseScene(Game game)
            : this(game, true)
        {

        }

        public BaseScene(Game game, bool needsUIService)
        {
            _game = game;
            _clearColor = Color.Black;
            _content = new ContentManager(_game.Services);
            _content.RootDirectory = "Content";
            _sceneState = SceneState.TransitioningOn;

            _sceneService = game.Services.GetService<ISceneService>(true);
            _inputService = game.Services.GetService<IInputState>(true);
            _worldService = game.Services.GetService<IIsometricRenderer>(true);
            _uiService = game.Services.GetService<IUIManager>();
        }

        public virtual void Intitialize()
        {

        }

        public virtual void Update(GameTime gameTime)
        {
            _elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (_sceneState)
            {
                case SceneState.TransitioningOn:
                    {
                        _transitionAlpha = 1 - (_elapsed / (float)TransitionOffLength.TotalSeconds);

                        if (_elapsed >= (float)TransitionOnLength.TotalSeconds)
                        {
                            _elapsed = 0;
                            _sceneState = SceneState.Active;
                        }

                        break;
                    }
                case SceneState.TransitioningOff:
                    {
                        _transitionAlpha = _elapsed / (float)TransitionOnLength.TotalSeconds;

                        if (_elapsed >= (float)TransitionOffLength.TotalSeconds)
                        {
                            _elapsed = 0;
                            _sceneState = SceneState.None;

                            if (TransitionCompleted != null)
                                TransitionCompleted();
                        }

                        break;
                    }
            }
        }
        
        public virtual void Draw(GameTime gameTime)
        {
            
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
            _uiService.Reset();
            _content.Dispose();
        }
    }
}
