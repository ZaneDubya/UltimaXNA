/***************************************************************************
 *   IsometricRenderer.cs
 *   Based on code from ClintXNA's renderer: http://www.runuo.com/forums/xna/92023-hi.html
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using UltimaXNA.UltimaEntities;
using UltimaXNA.Core.Rendering;
using UltimaXNA.UltimaWorld.Model;

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
        public static AEntity MouseOverObject { get { return (m_overObject == null) ? null : m_overObject.Entity; } }
        public static AEntity MouseOverGround { get { return (m_overGround == null) ? null : m_overGround.Entity; } }
        public static Vector2 MouseOverObjectPoint
        {
            get { return (m_overObject == null) ? new Vector2(0, 0) : m_overObject.InTexturePosition; }
        }
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

        public static void Draw(Map map)
        {
            InternalDetermineIfUnderEntity(map);
            InternalDrawEntities(map, out m_renderOffset);
            InternalDrawVectors(m_renderOffset);
        }

        private static void InternalDetermineIfUnderEntity(Map map)
        {
            // Are we inside (under a roof)? Do not draw tiles above our head.
            m_maxItemAltitude = 255;

            MapTile t;
            if ((t = map.GetMapTile(CenterPosition.X, CenterPosition.Y)) != null)
            {
                AEntity underObject, underTerrain;
                t.IsZUnderEntityOrGround(CenterPosition.Z, out underObject, out underTerrain);

                // if we are under terrain, then do not draw any terrain at all.
                DrawTerrain = !(underTerrain == null);

                if (!(underObject == null))
                {
                    // Roofing and new floors ALWAYS begin at intervals of 20.
                    // if we are under a ROOF, then get rid of everything above me.Z + 20
                    // (this accounts for A-frame roofs). Otherwise, get rid of everything
                    // at the object above us.Z.
                    if (((Item)underObject).ItemData.IsRoof)
                    {
                        m_maxItemAltitude = CenterPosition.Z - (CenterPosition.Z % 20) + 20;
                    }
                    else
                    {
                        m_maxItemAltitude = (int)(underObject.Z - (underObject.Z % 20));
                    }

                    // If we are under a roof tile, do not make roofs transparent if we are on an edge.
                    if (underObject is Item && ((Item)underObject).ItemData.IsRoof)
                    {
                        bool isRoofSouthEast = true;
                        if ((t = map.GetMapTile(CenterPosition.X + 1, CenterPosition.Y)) != null)
                        {
                            t.IsZUnderEntityOrGround(CenterPosition.Z, out underObject, out underTerrain);
                            isRoofSouthEast = !(underObject == null);
                        }

                        if (!isRoofSouthEast)
                            m_maxItemAltitude = 255;
                    }
                }
            }
        }

        private static void InternalDrawEntities(Map map, out Vector2 renderOffset)
        {
            if (CenterPosition == null)
            {
                renderOffset = new Vector2();
                return;
            }

            int renderDimensionY = 16; // the number of tiles that are drawn for half the screen (doubled to fill the entire screen).
            int renderDimensionX = 18; // the number of tiles that are drawn in the x-direction ( + renderExtraColumnsAtSides * 2 ).
            int renderExtraColumnsAtSides = 2; // the client draws additional tiles at the edge to make wide objects that are mostly offscreen visible.


            // when the player entity is higher (z) in the world, we must offset the first row drawn. This variable MUST be a multiple of 2.
            int renderZOffset = (CenterPosition.Z / 14) * 2 + 4;
            // this is used to draw tall objects that would otherwise not be visible until their ground tile was on screen. This may still skip VERY tall objects (those weird jungle trees?)
            int renderExtraRowsAtBottom = renderZOffset + 6; 

            Point firstTile = new Point(
                CenterPosition.X + renderExtraColumnsAtSides - ((renderZOffset + 1) / 2), 
                CenterPosition.Y - renderDimensionY - renderExtraColumnsAtSides - (renderZOffset  / 2));

            renderOffset.X = ((UltimaVars.EngineVars.ScreenSize.X + ((renderDimensionY) * 44)) / 2) - 22 + renderExtraColumnsAtSides * 44;
            renderOffset.X -= (int)((CenterPosition.X_offset - CenterPosition.Y_offset) * 22);
            renderOffset.X -= (firstTile.X - firstTile.Y) * 22;

            renderOffset.Y = ((UltimaVars.EngineVars.ScreenSize.Y - (renderDimensionY * 44)) / 2);
            renderOffset.Y += (CenterPosition.Z * 4) + (int)(CenterPosition.Z_offset * 4);
            renderOffset.Y -= (int)((CenterPosition.X_offset + CenterPosition.Y_offset) * 22);
            renderOffset.Y -= (firstTile.X + firstTile.Y) * 22;
            renderOffset.Y -= (renderZOffset) * 22;

            ObjectsRendered = 0; // Count of objects rendered for statistics and debug

            MouseOverList overList = new MouseOverList(UltimaEngine.Input.MousePosition, PickType); // List of entities mouse is over.
            List<AEntity> deferredToRemove = new List<AEntity>();

            for (int col = 0; col < renderDimensionY * 2 + renderExtraRowsAtBottom; col++)
            {
                Vector3 drawPosition = new Vector3();
                drawPosition.X = (firstTile.X - firstTile.Y + (col % 2)) * 22 + renderOffset.X;
                drawPosition.Y = (firstTile.X + firstTile.Y + col) * 22 + renderOffset.Y;

                Point index = new Point(firstTile.X + ((col + 1) / 2), firstTile.Y + (col / 2));

                for (int row = 0; row < renderDimensionX + renderExtraColumnsAtSides * 2; row++)
                {
                    MapTile tile = map.GetMapTile(index.X - row, index.Y + row);
                    if (tile == null)
                        continue;

                    List<AEntity> entities = tile.Entities;

                    for (int i = 0; i < entities.Count; i++)
                    {
                        if (entities[i].Z >= m_maxItemAltitude)
                            continue;

                        UltimaEntities.EntityViews.AEntityView view = entities[i].GetView();

                        if (view != null)
                            if (view.Draw(m_spriteBatch, drawPosition, overList, map))
                                ObjectsRendered++;

                        if (entities[i] is MobileDeferred)
                        {
                            deferredToRemove.Add(entities[i]);
                        }
                    }

                    foreach (AEntity deferred in deferredToRemove)
                        tile.OnExit(deferred);
                    deferredToRemove.Clear();

                    drawPosition.X -= 44f;
                }
            }

            OverheadRenderer.Render(m_spriteBatch, overList, map);

            // Update the MouseOver objects
            m_overObject = overList.GetForemostMouseOverItem(UltimaEngine.Input.MousePosition);
            m_overGround = overList.GetForemostMouseOverItem<Ground>(UltimaEngine.Input.MousePosition);

            // Draw the objects we just send to the spritebatch.
            m_spriteBatch.Prepare(true, true);
            m_spriteBatch.Flush();
        }

        private static void InternalDrawVectors(Vector2 renderOffset)
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