/***************************************************************************
 *   WorldView.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Patterns.MVC;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.UI.WorldGumps;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Ultima.World.EntityViews;
using UltimaXNA.Ultima.World.WorldViews;
#endregion

namespace UltimaXNA.Ultima.World
{
    class WorldView : AView
    {
        public IsometricRenderer Isometric
        {
            get;
            private set;
        }

        public MiniMapTexture MiniMap
        {
            get;
            private set;
        }

        protected new WorldModel Model
        {
            get { return (WorldModel)base.Model; }
        }

        /// <summary>
        ///  When AllLabels is true, all entites should display their name above their object.
        /// </summary>
        public static bool AllLabels
        {
            get;
            set;
        }

        public static int MouseOverHue = 0x038;

        public WorldView(WorldModel model)
            : base(model)
        {
            Isometric = new IsometricRenderer();
            Isometric.Lighting.LightDirection = -0.6f;

            MiniMap = new MiniMapTexture();
            MiniMap.Initialize();

            m_UI = ServiceRegistry.GetService<UserInterfaceService>();
        }

        public override void Draw(double frameTime)
        {
            AEntity player = WorldModel.Entities.GetPlayerEntity();
            Position3D center = player.Position;
            if ((player as Mobile).IsAlive)
            {
                AEntityView.s_Technique = Techniques.Default;
                m_ShowingDeathEffect = false;
                if (m_YouAreDead != null)
                {
                    m_YouAreDead.Dispose();
                    m_YouAreDead = null;
                }
            }
            else
            {
                if (!m_ShowingDeathEffect)
                {
                    m_ShowingDeathEffect = true;
                    m_DeathEffectTime = 0;
                    m_LightingGlobal = Isometric.Lighting.OverallLightning;
                    m_LightingPersonal = Isometric.Lighting.PersonalLightning;
                    m_UI.AddControl(m_YouAreDead = new YouAreDeadGump(), 0, 0);
                }

                double msFade = 2000d;
                double msHold = 1000d;

                if (m_DeathEffectTime < msFade)
                {
                    AEntityView.s_Technique = Techniques.Default;
                    Isometric.Lighting.OverallLightning = (int)(m_LightingGlobal + (0x1f - m_LightingGlobal) * ((m_DeathEffectTime / msFade)));
                    Isometric.Lighting.PersonalLightning = (int)(m_LightingPersonal * (1d - (m_DeathEffectTime / msFade)));
                }
                else if (m_DeathEffectTime < msFade + msHold)
                {
                    Isometric.Lighting.OverallLightning = 0x1f;
                    Isometric.Lighting.PersonalLightning = 0x00;
                }
                else
                {
                    AEntityView.s_Technique = Techniques.Grayscale;
                    Isometric.Lighting.OverallLightning = (int)m_LightingGlobal;
                    Isometric.Lighting.PersonalLightning = (int)m_LightingPersonal;
                    if (m_YouAreDead != null)
                    {
                        m_YouAreDead.Dispose();
                        m_YouAreDead = null;
                    }
                }

                m_DeathEffectTime += frameTime;
            }

            Isometric.Update(Model.Map, center, Model.Input.MousePick);
            MiniMap.Update(Model.Map, center);
        }

        private bool m_ShowingDeathEffect = false;
        private double m_DeathEffectTime = 0d;
        private double m_LightingGlobal;
        private double m_LightingPersonal;
        private YouAreDeadGump m_YouAreDead;

        private UserInterfaceService m_UI;
    }
}
