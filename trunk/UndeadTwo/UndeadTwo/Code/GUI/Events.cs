using System;
using Microsoft.Xna.Framework;

namespace UltimaXNA.GUI
{

    // No scripted window should ever directly interact with the client.
    // All events should be handled through this class, which will provide
    // a simple interface for all interaction with the client.
    public static class Events
    {
        private static IGameState m_GameStateService;
        private static Network.IGameClient m_GameClientService;

        public static void Initialize(GameServiceContainer nContainer)
        {
            m_GameStateService = (IGameState)nContainer.GetService(typeof(IGameState));
            m_GameClientService = (Network.IGameClient)nContainer.GetService(typeof(Network.IGameClient));
        }

        public static void QuitImmediate()
        {
            m_GameStateService.EngineRunning = false;
        }

        public static void Login(string nUsername, string nPassword)
        {
            m_GameClientService.Send_ConnectToLoginServer("localhost", 2593, nUsername, nPassword);
        }
    }
}