using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Data;
using UltimaXNA.Diagnostics;
using UltimaXNA.Extensions;
using UltimaXNA.Input;
using UltimaXNA.Network;
using UltimaXNA.Graphics.UI;
using UltimaXNA.TileEngine;

namespace UltimaXNA.SceneManagement
{
    public abstract class BaseScene : IScene, IDisposable
    {
        public virtual TimeSpan TransitionOnLength { get { return TimeSpan.FromSeconds(0.5); } }
        public virtual TimeSpan TransitionOffLength { get { return TimeSpan.FromSeconds(0.5); } }

        protected SpriteBatch SpriteBatch;

        Game _game;
        
        ISceneService _sceneService;
        ILoggingService _loggingService;
        IInputService _inputService;
        IUIService _uiService;
        IWorld _worldService;

        ContentManager _content;
        SceneState _sceneState;
        Color _clearColor;
        float _transitionAlpha;
        float _elapsed;
        bool _isInitialized;

        // Texture2D _mouseTexture;
        // Rectangle _mouseSourceRectangle;
        // Texture2D _transitionTexture;
        // RenderTarget2D _sceneTarget;
        // RenderTarget2D _uiTarget;
        // RenderTarget2D _finalTarget;

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
        /*
        public Rectangle MouseSourceRectangle
        {
            get { return _mouseSourceRectangle; }
            set { _mouseSourceRectangle = value; }
        }

        public Texture2D MouseTexture
        {
            get { return _mouseTexture; }
            set { _mouseTexture = value; }
        }
        */
        public ISceneService SceneManager
        {
            get { return _sceneService; }
        }

        public ILoggingService Log
        {
            get { return _loggingService; }
        }

        public IInputService Input
        {
            get { return _inputService; }
        }

        public IUIService UI
        {
            get { return _uiService; }
        }

        public IWorld World
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
            _loggingService = game.Services.GetService<ILoggingService>(true);
            _inputService = game.Services.GetService<IInputService>(true);
            _worldService = game.Services.GetService<IWorld>(true);
            
            if (needsUIService)
            {

                IUIService uiService = game.Services.GetService<IUIService>();

                if (uiService != null)
                {
                    _loggingService.Debug("Disposing previous UIService");
                    game.Services.RemoveService(typeof(IUIService));
                    uiService.Dispose();
                }

                _uiService = new UIManager(game);
                game.Services.AddService<IUIService>(_uiService);
            }
        }

        public virtual void Intitialize()
        {
            SpriteBatch = new SpriteBatch(Game.GraphicsDevice);

            // _mouseTexture = Art.GetStaticTexture(8307);
            // _mouseSourceRectangle = new Rectangle(1, 1, 31, 26);
            // Color[] data = new Color[] { Color.Black };
            // _transitionTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
            // _transitionTexture.SetData<Color>(data);
            // PresentationParameters pp = GraphicsDevice.PresentationParameters;
            // _sceneTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, 1, pp.BackBufferFormat);
            // _uiTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, 1, pp.BackBufferFormat);
            // _finalTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, 1, pp.BackBufferFormat);
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

            if (UI != null)
            {
                UI.Update(gameTime);
            }
        }
        /*
        public virtual void OnBeforeDraw(GameTime gameTime)
        {

        }
         */
        
        public virtual void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_clearColor);
            UI.Draw(gameTime);
        }
        
        /*
        public virtual void DrawUI(GameTime gametTime)
        {
            GraphicsDevice.SetRenderTarget(0, null);
            Texture2D sceneTexture = _sceneTarget.GetTexture();

            sceneTexture = PostProcess(gametTime, sceneTexture);

            GraphicsDevice.SetRenderTarget(0, _uiTarget);
            GraphicsDevice.Clear(Color.TransparentBlack);

            // if (UI != null)
            // {
            //     //UI.SceneTexture = sceneTexture;
            //     // UI.Draw(gametTime);
            // }

            GraphicsDevice.SetRenderTarget(0, null);
            Texture2D uiTexture = _uiTarget.GetTexture();

            GraphicsDevice.SetRenderTarget(0, _finalTarget);
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin();
            SpriteBatch.Draw(sceneTexture, Vector2.Zero, Color.White);
            SpriteBatch.Draw(uiTexture, Vector2.Zero, Color.White);
            SpriteBatch.End();

            GraphicsDevice.SetRenderTarget(0, null);
            sceneTexture = _finalTarget.GetTexture();

            SpriteBatch.Begin();
            SpriteBatch.Draw(sceneTexture, Vector2.Zero, Color.White);
            SpriteBatch.End();
        }
        */
        protected virtual Texture2D PostProcess(GameTime gametTime, Texture2D sceneTexture)
        {
            return sceneTexture;
        }
        /*
        public virtual void DrawCursor(GameTime gameTime)
        {
            SpriteBatch.Begin();
            SpriteBatch.Draw(_mouseTexture, _inputService.CurrentMousePosition, _mouseSourceRectangle, Color.White);
            SpriteBatch.End();
        }
        
        public virtual void OnAfterDraw(GameTime gameTime)
        {
            PresentationParameters pp = Game.GraphicsDevice.PresentationParameters;
            Color color = new Color(new Vector4(1, 1, 1, _transitionAlpha));

            SpriteBatch.Begin();
            SpriteBatch.Draw(_transitionTexture,
                new Rectangle(0, 0, pp.BackBufferWidth, pp.BackBufferHeight), color);
            SpriteBatch.End();
        }
        */
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
            _content.Dispose();
            SpriteBatch.Dispose();
        }
    }
}
