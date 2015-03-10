/***************************************************************************
 *   IsometricRenderer.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from ClintXNA's renderer: http://www.runuo.com/forums/xna/92023-hi.html
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
using Microsoft.Xna.Framework;
using UltimaXNA.Entity;
using UltimaXNA.Rendering;
using InterXLib.Input.Windows;
using UltimaXNA.UltimaWorld;
#endregion

namespace UltimaXNA.UltimaWorld.View
{
    public class IsometricRenderer
    {
        #region RenderingVariables
        static VectorRenderer m_vectors;
        static SpriteBatch3D m_spriteBatch;
        static VertexPositionNormalTextureHue[] m_vertexBufferStretched;
        #endregion

        #region MousePickingVariables
        private static MouseOverItem m_overObject, m_overGround;
        public static AMapObject MouseOverObject { get { return (m_overObject == null) ? null : m_overObject.Object; } }
        public static AMapObject MouseOverGround { get { return (m_overGround == null) ? null : m_overGround.Object; } }
        public static Vector2 MouseOverObjectPoint { get { return (m_overObject == null) ? new Vector2(0, 0) : m_overObject.InTexturePosition; } }
        public const PickTypes DefaultPickType = PickTypes.PickStatics | PickTypes.PickObjects;
        public static PickTypes PickType { get; set; }
        #endregion

        #region LightingVariables
        private static int m_lightLevelPersonal = 9, m_lightLevelOverall = 9;
        private static float m_lightDirection = 4.12f, m_lightHeight = -0.75f;
        public static int PersonalLightning
        {
            set { m_lightLevelPersonal = value; recalculateLightning(); }
            get { return m_lightLevelPersonal; }
        }
        public static int OverallLightning
        {
            set { m_lightLevelOverall = value; recalculateLightning(); }
            get { return m_lightLevelOverall; }
        }
        public static float LightDirection
        {
            set { m_lightDirection = value; recalculateLightning(); }
            get { return m_lightDirection; }
        }
        public static float LightHeight
        {
            set { m_lightHeight = value; recalculateLightning(); }
            get { return m_lightHeight; }
        }
        #endregion

        private static bool m_flag_HighlightMouseOver = false;
        public static bool Flag_HighlightMouseOver
        {
            get { return m_flag_HighlightMouseOver; }
            set { m_flag_HighlightMouseOver = value; }
        }

        private static Map m_map;
        public static Map Map
        {
            get { return m_map; }
            set { m_map = value; }
        }
        public static int ObjectsRendered { get; internal set; }
        public static Position3D CenterPosition { get; set; }
        public static bool DrawTerrain = true;
        private static int m_maxItemAltitude;

        static Vector2 m_renderOffset;
        public static Vector2 RenderOffset
        {
            get { return m_renderOffset; }
        }

        public static void Initialize(Game game)
        {
            m_spriteBatch = new SpriteBatch3D(game);
            m_vectors = new VectorRenderer(game.GraphicsDevice, game.Content);

            PickType = PickTypes.PickNothing;
            m_vertexBufferStretched = new VertexPositionNormalTextureHue[] {
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(0, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(1, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(0, 1, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(1, 1, 0))
            };
        }

        public static void Update(double totalTime, double frameTime)
        {
            if (UltimaVars.EngineVars.Map != -1)
            {
                if ((m_map == null) || (m_map.Index != UltimaVars.EngineVars.Map))
                    m_map = new Map(UltimaVars.EngineVars.Map);
                // Update the Map's position so it loads the tiles we're going to be drawing
                m_map.Update(CenterPosition.X, CenterPosition.Y);

                // Are we inside (under a roof)? Do not draw tiles above our head.
                m_maxItemAltitude = 255;

                MapTile t;
                if ((t = m_map.GetMapTile(CenterPosition.X, CenterPosition.Y, true)) != null)
                {
                    AMapObject underObject, underTerrain;
                    t.IsUnder(CenterPosition.Z, out underObject, out underTerrain);

                    // if we are under terrain, then do not draw any terrain at all.
                    DrawTerrain = !(underTerrain == null);

                    if (!(underObject == null))
                    {
                        // Roofing and new floors ALWAYS begin at intervals of 20.
                        // if we are under a ROOF, then get rid of everything above me.Z + 20
                        // (this accounts for A-frame roofs). Otherwise, get rid of everything
                        // at the object above us.Z.
                        if (((MapObjectStatic)underObject).ItemData.IsRoof)
                        {
                            m_maxItemAltitude = CenterPosition.Z - (CenterPosition.Z % 20) + 20;
                        }
                        else
                        {
                            m_maxItemAltitude = (int)(underObject.Z - (underObject.Z % 20));
                        }

                        // If we are under a roof tile, do not make roofs transparent if we are on an edge.
                        if (underObject is MapObjectStatic && ((MapObjectStatic)underObject).ItemData.IsRoof)
                        {
                            bool isRoofSouthEast = true;
                            if ((t = m_map.GetMapTile(CenterPosition.X + 1, CenterPosition.Y + 1, true)) != null)
                            {
                                t.IsUnder(CenterPosition.Z, out underObject, out underTerrain);
                                isRoofSouthEast = !(underObject == null);
                            }

                            if (!isRoofSouthEast)
                                m_maxItemAltitude = 255;
                        }
                    }
                }
            }
        }

        public static void Draw(double frameTime)
        {
            if (UltimaVars.EngineVars.Map < 0)
                return;
            
            render(out m_renderOffset);
            renderVectors(m_renderOffset);
        }

        private static void render(out Vector2 renderOffset)
        {
            if (CenterPosition == null)
            {
                renderOffset = new Vector2();
                return;
            }

            // Prerender objects
            m_spriteBatch.Prepare(false, false);
            MapObjectPrerendered.RenderObjects(m_spriteBatch);

            // UltimaVars.EngineVars.RenderSize = 20;

            int RenderBeginX = CenterPosition.X - (UltimaVars.EngineVars.RenderSize / 2);
            int RenderBeginY = CenterPosition.Y - (UltimaVars.EngineVars.RenderSize / 2);
            int RenderEndX = RenderBeginX + UltimaVars.EngineVars.RenderSize;
            int RenderEndY = RenderBeginY + UltimaVars.EngineVars.RenderSize;

            renderOffset.X = (UltimaVars.EngineVars.ScreenSize.X >> 1) - 22;
            renderOffset.X -= (int)((CenterPosition.X_offset - CenterPosition.Y_offset) * 22);
            renderOffset.X -= (RenderBeginX - RenderBeginY) * 22;

            renderOffset.Y = ((UltimaVars.EngineVars.ScreenSize.Y - (UltimaVars.EngineVars.RenderSize * 44)) >> 1);
            renderOffset.Y += (CenterPosition.Z * 4) + (int)(CenterPosition.Z_offset * 4);
            renderOffset.Y -= (int)((CenterPosition.X_offset + CenterPosition.Y_offset) * 22);
            renderOffset.Y -= (RenderBeginX + RenderBeginY) * 22;

            ObjectsRendered = 0; // Count of objects rendered for statistics and debug
            MouseOverList overList = new MouseOverList(); // List of items for mouse over
            overList.MousePosition = UltimaEngine.Input.MousePosition;
            List<AMapObject> mapObjects;
            Vector3 drawPosition = new Vector3();

            for (int ix = RenderBeginX; ix < RenderEndX; ix++)
            {
                drawPosition.X = (ix - RenderBeginY) * 22 + renderOffset.X;
                drawPosition.Y = (ix + RenderBeginY) * 22 + renderOffset.Y;

                DrawTerrain = true;

                for (int iy = RenderBeginY; iy < RenderEndY; iy++)
                {
                    MapTile tile = m_map.GetMapTile(ix, iy, true);
                    if (tile == null)
                        continue;

                    mapObjects = tile.Items;
                    for (int i = 0; i < mapObjects.Count; i++)
                    {
                        if (mapObjects[i].Draw(m_spriteBatch, drawPosition, overList, PickType, m_maxItemAltitude))
                            ObjectsRendered++;
                    }
                    tile.ClearTemporaryObjects();

                    drawPosition.X -= 22f;
                    drawPosition.Y += 22f;
                }
            }

            // Update the MouseOver objects
            m_overObject = overList.GetForemostMouseOverItem(UltimaEngine.Input.MousePosition);
            m_overGround = overList.GetForemostMouseOverItem<MapObjectGround>(UltimaEngine.Input.MousePosition);

            // Draw the objects we just send to the spritebatch.
            m_spriteBatch.Prepare(true, true);
            m_spriteBatch.Flush();
        }

        private static void renderVectors(Vector2 renderOffset)
        {
            if (Flag_HighlightMouseOver)
            {
                if (m_overObject != null)
                {
                    m_vectors.DrawLine(m_overObject.Vertices[0], m_overObject.Vertices[1], Color.LightBlue);
                    m_vectors.DrawLine(m_overObject.Vertices[1], m_overObject.Vertices[3], Color.LightBlue);
                    m_vectors.DrawLine(m_overObject.Vertices[3], m_overObject.Vertices[2], Color.LightBlue);
                    m_vectors.DrawLine(m_overObject.Vertices[2], m_overObject.Vertices[0], Color.LightBlue);
                }
                if (m_overGround != null)
                {
                    m_vectors.DrawLine(m_overGround.Vertices[0], m_overGround.Vertices[1], Color.Teal);
                    m_vectors.DrawLine(m_overGround.Vertices[1], m_overGround.Vertices[3], Color.Teal);
                    m_vectors.DrawLine(m_overGround.Vertices[3], m_overGround.Vertices[2], Color.Teal);
                    m_vectors.DrawLine(m_overGround.Vertices[2], m_overGround.Vertices[0], Color.Teal);
                }
            }

            m_vectors.Render_ViewportSpace();
        }

        private static void recalculateLightning()
        {
            float light = Math.Min(30 - OverallLightning + PersonalLightning, 30f);
            light = Math.Max(light, 0);
            light /= 30; // bring it between 0-1

            // -0.3 corresponds pretty well to the darkest possible light in the original client
            // 0.5 is quite okay for the brightest light.
            // so we'll just interpolate between those two values

            light *= 0.8f;
            light -= 0.3f;

            // i'd use a fixed lightning direction for now - maybe enable this effect with a custom packet?
            Vector3 lightDirection = new Vector3((float)Math.Cos(m_lightDirection), m_lightHeight, (float)Math.Sin(m_lightDirection));

            m_spriteBatch.SetLightDirection(lightDirection);

            // again some guesstimated values, but to me it looks okay this way :) 
            m_spriteBatch.SetAmbientLightIntensity(light * 0.8f);
            m_spriteBatch.SetDirectionalLightIntensity(light);
        }
    }
}