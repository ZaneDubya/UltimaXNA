using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Client;
using UltimaXNA.Diagnostics;
using UltimaXNA.Input;

namespace UltimaXNA.SceneManagement
{
    public delegate void TransitionCompleteHandler();

    public interface IScene : IDisposable
    {
        Game Game { get; }
        ISceneService SceneManager { get; }
        SceneState SceneState { get; set; }
        TimeSpan TransitionOffLength { get; }
        TimeSpan TransitionOnLength { get; }
        bool IsInitialized { get; set; }

        IUltimaClient Network { get; }
        ILoggingService Log { get; }
        IInputService Input { get; }

        event TransitionCompleteHandler TransitionComplete;

        void Intitialize();
        void Update(GameTime gameTime);
        void OnAfterDraw(GameTime gameTime);
        void Draw(GameTime gameTime);
        void OnBeforeDraw(GameTime gameTime);
    }
}
