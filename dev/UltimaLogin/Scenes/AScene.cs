using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.UltimaData;
using UltimaXNA.Diagnostics;
using InterXLib.Input.Windows;
using UltimaXNA.Core.Network;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaWorld;

namespace UltimaXNA.Scenes
{
    public abstract class AScene : IDisposable
    {
        internal SceneManager Manager;

        public virtual TimeSpan TransitionOnLength { get { return TimeSpan.FromSeconds(0.05); } }
        public virtual TimeSpan TransitionOffLength { get { return TimeSpan.FromSeconds(0.05); } }

        protected UltimaClient Client { get; private set; }

        SceneState _sceneState;
        float _transitionAlpha;
        float _elapsed;
        bool _isInitialized;

        public bool IsInitialized
        {
            get { return _isInitialized; }
            set { _isInitialized = value; }
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

        public event TransitionCompleteHandler TransitionCompleted;
        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdated;
        public event EventHandler<ProgressCompletedEventArgs> ProgressCompleted;
        public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

        public AScene()
        {
            _sceneState = SceneState.TransitioningOn;
        }

        public virtual void Intitialize(UltimaClient client)
        {
            Client = client;
        }

        public virtual void Update(double totalTime, double frameTime)
        {
            _elapsed += (float)frameTime;

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
            UltimaEngine.UserInterface.Reset();
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
