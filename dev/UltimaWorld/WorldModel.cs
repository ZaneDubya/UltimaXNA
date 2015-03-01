using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterXLib.Patterns.MVC;
using UltimaXNA.UltimaNetwork;
using UltimaXNA.UltimaGUI.Gumps;
using UltimaXNA.UltimaGUI;
using InterXLib.Input.Windows;

namespace UltimaXNA.UltimaWorld
{
    class WorldModel : AModel
    {
        private EntityManager m_Entities;
        public EntityManager Entities
        {
            get { return m_Entities; }
        }

        public WorldModel()
        {
            m_Entities = new EntityManager();

            UltimaEngine.UserInterface.AddControl(new TopMenu(0), 0, 0);
            UltimaEngine.UserInterface.AddControl(new ChatWindow(), 0, 0);

            // this is the login sequence for 0.6.1.10
            UltimaInteraction.GetMySkills();
            UltimaInteraction.SendClientVersion();
            UltimaInteraction.SendClientScreenSize();
            UltimaInteraction.SendClientLocalization();
            // Packet: BF 00 0A 00 0F 0A 00 00 00 1F
            // Packet: 09 00 00 00 02  
            // Packet: 06 80 00 00 17
            UltimaInteraction.GetMyBasicStatus();
            // Packet: D6 00 0B 00 00 00 02 00 00 00 17
            // Packet: D6 00 37 40 00 00 FB 40 00 00 FD 40 00 00 FE 40
            //         00 00 FF 40 00 01 00 40 00 01 02 40 00 01 03 40
            //         00 01 04 40 00 01 05 40 00 01 06 40 00 01 07 40
            //         00 01 24 40 00 01 26 
            IsometricRenderer.LightDirection = -0.6f;
            UltimaVars.EngineVars.InWorld = true;
        }

        public void Disconnect()
        {
            UltimaClient.Disconnect();
            UltimaVars.EngineVars.InWorld = false;
        }

        public override void Update(double totalTime, double frameTime)
        {
            if (!UltimaClient.IsConnected)
            {
                if (UltimaEngine.UserInterface.IsModalControlOpen == false)
                {
                    MsgBox g = UltimaEngine.UltimaUI.MsgBox("You have lost your connection with the server.", MsgBoxTypes.OkOnly);
                    g.OnClose = onCloseLostConnectionMsgBox;
                }
            }
            else
            {
                IsometricRenderer.CenterPosition = EntityManager.GetPlayerObject().Position;
                IsometricRenderer.Update(totalTime, frameTime);

                // Toggle for logout
                if (UltimaEngine.Input.HandleKeyboardEvent(KeyboardEventType.Down, WinKeys.Q, false, false, true))
                    UltimaInteraction.DisconnectToLoginScreen();
            }
        }

        protected override AController CreateController()
        {
            throw new NotImplementedException();
        }

        protected override AView CreateView()
        {
            throw new NotImplementedException();
        }

        void onCloseLostConnectionMsgBox()
        {
            UltimaInteraction.DisconnectToLoginScreen();
        }
    }
}
