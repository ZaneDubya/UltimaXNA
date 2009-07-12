using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UltimaXNA.GUI;
using UltimaXNA.Client;

namespace UltimaXNA.SceneManagement
{
    public class WorldScene : BaseScene
    {
        public WorldScene(Game game)
            : base(game)
        {
        }

        public override void  Intitialize()
        {
             base.Intitialize();
             GUI.AddWindow("ChatFrame", new Window_Chat());
             GUI.AddWindow("ChatInput", new Window_ChatInput());
             GUI.AddWindow("StatusFrame", new Window_StatusFrame());
             ((IGameState)Game.Services.GetService(typeof(IGameState))).InWorld = true;
        }

        public override void Dispose()
        {
            base.Dispose();
            Network.Disconnect();
            GUI.CloseWindow("ChatFrame");
            GUI.CloseWindow("ChatInput");
            GUI.CloseWindow("StatusFrame");
            ((IGameState)Game.Services.GetService(typeof(IGameState))).InWorld = false;
        }
    }
}
