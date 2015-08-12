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
using System;
using System.Collections.Generic;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Patterns.MVC;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.UI.WorldGumps;
using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.World.Input;
using UltimaXNA.Ultima.World.Managers;
using UltimaXNA.Ultima.World.Maps;
using UltimaXNA.Ultima.UI;
#endregion

namespace UltimaXNA.Ultima
{
    class WorldModel : AUltimaModel
    {
        // ================================================================================
        // Private variables
        // ================================================================================
        private Map m_Map;
        private WorldCursor m_Cursor;
        // services
        private readonly INetworkClient m_Network;
        private readonly UserInterfaceService m_UserInterface;
        private readonly UltimaGame m_Engine;

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
            get { return m_Map; }
        }

        public uint MapIndex
        {
            get
            {
                if (m_Map == null)
                    return 0xFFFFFFFF;
                else
                    return m_Map.Index;
            }
            set
            {
                if (value != MapIndex)
                {
                    // clear all entities
                    Entities.Reset(false);
                    if (m_Map != null)
                    {
                        AEntity player = Entities.GetPlayerEntity();
                        // save current player position
                        int x = player.X, y = player.Y, z = player.Z;
                        // place the player in null space (allows the map to be reloaded when we return to the same location in a different map).
                        player.SetMap(null);
                        // dispose of map
                        m_Map.Dispose();
                        m_Map = null;
                        // add new map!
                        m_Map = new Map(value);
                        player.SetMap(m_Map);
                        // restore previous player position
                        player.Position.Set(x, y, z);
                    }
                    else
                    {
                        AEntity player = Entities.GetPlayerEntity();
                        m_Map = new Map(value);
                        player.SetMap(m_Map);
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
            SaveOpenGumps();
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
            if (!Settings.World.MenuBarDisabled)
            {
                m_UserInterface.AddControl(new TopMenuGump(), 0, 0); // by default at the top of the screen.
            }

            Client.SendWorldLoginPackets();
            IsInWorld = true;
            Client.StartKeepAlivePackets();
            
            // wait until we've received information about the entities around us before restoring saved gumps.
            DelayedAction.Start(() => RestoreSavedGumps(), 1000);
        }

        public void Disconnect()
        {
            m_Network.Disconnect(); // stops keep alive packets.
            IsInWorld = false;
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

        private void SaveOpenGumps()
        {
            Settings.Gumps.SavedGumps.Clear();
            foreach (AControl gump in m_UserInterface.Controls)
            {
                if (gump is Gump)
                {
                    if ((gump as Gump).SaveOnWorldStop)
                    {
                        Dictionary<string, object> data;
                        if ((gump as Gump).SaveGump(out data))
                        {
                            Settings.Gumps.SavedGumps.Add(new Configuration.SavedGumpConfig(gump.GetType(), data));
                        }
                    }
                }
            }
        }

        private void RestoreSavedGumps()
        {
            foreach (Configuration.SavedGumpConfig savedGump in Settings.Gumps.SavedGumps)
            {
                try
                {
                    Type type = Type.GetType(savedGump.GumpType);
                    object gump = System.Activator.CreateInstance(type);
                    if (gump is Gump)
                    {
                        if ((gump as Gump).RestoreGump(savedGump.GumpData))
                        {
                            m_UserInterface.AddControl(gump as Gump, 0, 0);
                        }
                        else
                        {
                            Tracer.Error("Unable to restore saved gump with type {0}: Failed to restore gump.", savedGump.GumpType);
                        }
                    }
                    else
                    {
                        Tracer.Error("Unable to restore saved gump with type {0}: Type does not derive from Gump.", savedGump.GumpType);
                    }
                }
                catch
                {
                    Tracer.Error("Unable to restore saved gump with type {0}: Type cannot be Instanced.", savedGump.GumpType);
                }
            }
            Settings.Gumps.SavedGumps.Clear();
        }
    }
}
