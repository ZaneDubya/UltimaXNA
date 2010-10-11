using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Diagnostics;
using UltimaXNA.UILegacy;
using UltimaXNA.InputOld;

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
        IUIManager UI { get; }

        event TransitionCompleteHandler TransitionCompleted;

        Color ClearColor { get; set; }

        void Intitialize();
        void Update(GameTime gameTime);
        // void OnBeforeDraw(GameTime gameTime);
        void Draw(GameTime gameTime);
        // void DrawUI(GameTime gameTime);
        // void DrawCursor(GameTime gameTime);
        // void OnAfterDraw(GameTime gameTime);
    }
}
