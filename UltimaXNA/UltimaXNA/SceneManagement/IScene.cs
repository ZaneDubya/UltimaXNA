using System;
using Microsoft.Xna.Framework;
using UltimaXNA.Diagnostics;
using UltimaXNA.Graphics.UI;
using UltimaXNA.Input;

namespace UltimaXNA.SceneManagement
{
    public delegate void TransitionCompleteHandler();

    public interface IScene : IProgressNotifier, IStatusNotifier, IDisposable
    {
        Game Game { get; }
        SceneState SceneState { get; set; }
        TimeSpan TransitionOffLength { get; }
        TimeSpan TransitionOnLength { get; }
        bool IsInitialized { get; set; }

        ISceneService SceneManager { get; }
        ILoggingService Log { get; }
        IInputService Input { get; }
        IUIService UI { get; }

        event TransitionCompleteHandler TransitionCompleted;

        void Intitialize();
        void Update(GameTime gameTime);
        // void OnBeforeDraw(GameTime gameTime);
        void Draw(GameTime gameTime);
        // void DrawUI(GameTime gameTime);
        // void DrawCursor(GameTime gameTime);
        // void OnAfterDraw(GameTime gameTime);
    }
}
