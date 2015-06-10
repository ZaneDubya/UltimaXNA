/***************************************************************************
 *   WorldModel.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Patterns.MVC;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.Login;
using UltimaXNA.Ultima.World.Maps;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.WorldGumps;
using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.World.Input;
using UltimaXNA.Ultima.World.Managers;
#endregion

namespace UltimaXNA.Ultima
{
    class WorldModel : AUltimaModel
    {
        // ================================================================================
        // Private variables
        // ================================================================================
        private Map m_map = null;
        private WorldCursor m_Cursor = null;
        // services
        private INetworkClient m_Network;
        private UserInterfaceService m_UserInterface;
        private UltimaGame m_Engine;

        // ================================================================================
        // Public Static Properties
        // ================================================================================
        public static Serial PlayerSerial
        {
            get;
            set;
        }

        public static EntityManager Entities
        {
            get;
            private set;
        }

        public static EffectManager Effects
        {
            get;
            private set;
        }

        public static StaticManager Statics
        {
            get;
            private set;
        }

        // ================================================================================
        // Public Properties
        // ================================================================================
        public WorldClient Client
        {
            get;
            private set;
        }

        public WorldInput Input
        {
            get;
            private set;
        }

        public WorldInteraction Interaction
        {
            get;
            private set;
        }
        
        public WorldCursor Cursor
        {
            get { return m_Cursor; }
            set
            {
                if (m_Cursor != null)
                {
                    m_Cursor.Dispose();
                }
                m_Cursor = value;
            }
        }

        public int MapCount
        {
            get;
            set;
        }
        
        public Map Map
        {
            get { return m_map; }
        }

        public uint MapIndex
        {
            get
            {
                if (m_map == null)
                    return 0xFFFFFFFF;
                else
                    return m_map.Index;
            }
            set
            {
                if (value != MapIndex)
                {
                    // clear all entities
                    Entities.Reset(false);
                    if (m_map != null)
                    {
                        AEntity player = Entities.GetPlayerObject();
                        // save current player position
                        int x = player.X, y = player.Y, z = player.Z;
                        // place the player in null space (allows the map to be reloaded when we return to the same location in a different map).
                        player.SetMap(null);
                        // dispose of map
                        m_map.Dispose();
                        m_map = null;
                        // add new map!
                        m_map = new Map(value);
                        player.SetMap(m_map);
                        // restore previous player position
                        player.Position.Set(x, y, z);
                    }
                    else
                    {
                        AEntity player = Entities.GetPlayerObject();
                        m_map = new Map(value);
                        player.SetMap(m_map);
                    }
                }
            }
        }

        public static bool IsInWorld // InWorld allows us to tell when our character object has been loaded in the world.
        {
            get;
            set;
        }

        // ================================================================================
        // Ctor, Initialization, Dispose, Update
        // ================================================================================
        public WorldModel()
            : base()
        {
            ServiceRegistry.Register<WorldModel>(this);

            m_Engine = ServiceRegistry.GetService<UltimaGame>();
            m_Network = ServiceRegistry.GetService<INetworkClient>();
            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();

            Entities = new EntityManager(this);
            Entities.Reset(true);
            Effects = new EffectManager(this);
            Statics = new StaticManager();

            Input = new WorldInput(this);
            Interaction = new WorldInteraction(this);
            Client = new WorldClient(this);
        }

        protected override void OnInitialize()
        {
            m_Engine.SetupWindowForWorld();

            m_UserInterface.Cursor = Cursor = new WorldCursor(this);
            Client.Initialize();
        }

        protected override void OnDispose()
        {
            m_Engine.SaveResolution();

            ServiceRegistry.Unregister<WorldModel>();

            m_UserInterface.Reset();

            Entities.Reset();
            Entities = null;

            Effects = null;

            Input.Dispose();
            Input = null;

            Interaction = null;

            Client.Dispose();
            Client = null;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (!m_Network.IsConnected)
            {
                if (m_UserInterface.IsModalControlOpen == false)
                {
                    MsgBoxGump g = MsgBoxGump.Show("You have lost your connection with the server.", MsgBoxTypes.OkOnly);
                    g.OnClose = OnCloseLostConnectionMsgBox;
                }
            }
            else
            {
                Input.Update(frameMS);
                Entities.Update(frameMS);
                Effects.Update(frameMS);
                Statics.Update(frameMS);
            }
        }

        // ================================================================================
        // Public Methods
        // ================================================================================
        public void LoginToWorld()
        {
            m_UserInterface.AddControl(new WorldViewGump(), 0, 0); // world gump will restore its position on load.
            m_UserInterface.AddControl(new TopMenuGump(), 0, 0); // always at the top of the screen.

            Client.SendWorldLoginPackets();
            WorldModel.IsInWorld = true;
            Client.StartKeepAlivePackets();
        }

        public void Disconnect()
        {
            m_Network.Disconnect(); // stops keep alive packets.
            WorldModel.IsInWorld = false;
            m_Engine.ActiveModel = new LoginModel();
        }

        // ================================================================================
        // Private/Protected Methods
        // ================================================================================
        protected override AView CreateView()
        {
            return new WorldView(this);
        }

        void OnCloseLostConnectionMsgBox()
        {
            Disconnect();
        }
    }
}
