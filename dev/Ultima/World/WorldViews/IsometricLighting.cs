/***************************************************************************
 *   IsometricLighting.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using Microsoft.Xna.Framework;
using System;

namespace UltimaXNA.Ultima.World.WorldViews
{
    public class IsometricLighting
    {
        private int m_lightLevelPersonal = 9;
        private int m_lightLevelOverall = 9;
        private float m_lightDirection = 4.12f;
        private float m_lightHeight = -0.75f;

        public int PersonalLightning
        {
            set { m_lightLevelPersonal = value; RecalculateLightningValues(); }
            get { return m_lightLevelPersonal; }
        }

        public int OverallLightning
        {
            set { m_lightLevelOverall = value; RecalculateLightningValues(); }
            get { return m_lightLevelOverall; }
        }

        public float LightDirection
        {
            set { m_lightDirection = value; RecalculateLightningValues(); }
            get { return m_lightDirection; }
        }

        public float LightHeight
        {
            set { m_lightHeight = value; RecalculateLightningValues(); }
            get { return m_lightHeight; }
        }

        private void RecalculateLightningValues()
        {
            float light = Math.Min(30 - OverallLightning + PersonalLightning, 30f);
            light = Math.Max(light, 0);
            IsometricLightLevel = light / 30; // bring it between 0-1

            // i'd use a fixed lightning direction for now - maybe enable this effect with a custom packet?
            m_lightDirection = 1.2f;
            IsometricLightDirection = Vector3.Normalize(new Vector3((float)Math.Cos(m_lightDirection), (float)Math.Sin(m_lightDirection), 1f));
        }

        public float IsometricLightLevel
        {
            get;
            private set;
        }

        public Vector3 IsometricLightDirection
        {
            get;
            private set;
        }
    }
}
