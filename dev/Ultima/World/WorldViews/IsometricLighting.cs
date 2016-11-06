/***************************************************************************
 *   IsometricLighting.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using Microsoft.Xna.Framework;

namespace UltimaXNA.Ultima.World.WorldViews
{
    public class IsometricLighting
    {
        private int m_LightLevelPersonal = 9;
        private int m_LightLevelOverall = 9;
        private float m_LightDirection = 4.12f;
        private float m_LightHeight = -0.75f;

        public IsometricLighting()
        {
            RecalculateLightningValues();
        }

        public int PersonalLightning
        {
            set { m_LightLevelPersonal = value; RecalculateLightningValues(); }
            get { return m_LightLevelPersonal; }
        }

        public int OverallLightning
        {
            set { m_LightLevelOverall = value; RecalculateLightningValues(); }
            get { return m_LightLevelOverall; }
        }

        public float LightDirection
        {
            set { m_LightDirection = value; RecalculateLightningValues(); }
            get { return m_LightDirection; }
        }

        public float LightHeight
        {
            set { m_LightHeight = value; RecalculateLightningValues(); }
            get { return m_LightHeight; }
        }

        private void RecalculateLightningValues()
        {
            float light = Math.Min(30 - OverallLightning + PersonalLightning, 30f);
            light = Math.Max(light, 0);
            IsometricLightLevel = light / 30; // bring it between 0-1

            // i'd use a fixed lightning direction for now - maybe enable this effect with a custom packet?
            m_LightDirection = 1.2f;
            IsometricLightDirection = Vector3.Normalize(new Vector3((float)Math.Cos(m_LightDirection), (float)Math.Sin(m_LightDirection), 1f));
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
