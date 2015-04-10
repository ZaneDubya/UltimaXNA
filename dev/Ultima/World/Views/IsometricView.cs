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
using UltimaXNA.Configuration;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Patterns.IoC;
using UltimaXNA.Ultima.Entities;
using UltimaXNA.Ultima.Entities.Items;
using UltimaXNA.Ultima.EntityViews;
using UltimaXNA.Ultima.World.Controllers;
using UltimaXNA.Ultima.World.Maps;
#endregion

namespace UltimaXNA.Ultima.World.Views
{
    public class IsometricRenderer
    {
        #region RenderingVariables
        private SpriteBatch3D m_spriteBatch;
        private VertexPositionNormalTextureHue[] m_vertexBufferStretched;
        #endregion

        #region LightingVariables
        private int m_lightLevelPersonal = 9, m_lightLevelOverall = 9;
        private float m_lightDirection = 4.12f, m_lightHeight = -0.75f;
        public int PersonalLightning
        {
            set { m_lightLevelPersonal = value; recalculateLightning(); }
            get { return m_lightLevelPersonal; }
        }
        public int OverallLightning
        {
            set { m_lightLevelOverall = value; recalculateLightning(); }
            get { return m_lightLevelOverall; }
        }
        public float LightDirection
        {
            set { m_lightDirection = value; recalculateLightning(); }
            get { return m_lightDirection; }
        }
        public float LightHeight
        {
            set { m_lightHeight = value; recalculateLightning(); }
            get { return m_lightHeight; }
        }
        #endregion

        private bool m_flag_HighlightMouseOver = false;
        public bool Flag_HighlightMouseOver
        {
            get { return m_flag_HighlightMouseOver; }
            set { m_flag_HighlightMouseOver = value; }
        }

        public int ObjectsRendered { get; internal set; }
        public bool DrawTerrain = true;
        private int m_maxItemAltitude;

        Vector2 m_renderOffset;
        public Vector2 RenderOffset
        {
            get { return m_renderOffset; }
        }

        public IsometricRenderer(IContainer container)
        {
            Engine = container.Resolve<IEngine>();
        }

        protected IEngine Engine { get; private set; }

        public void Initialize()
        {
            m_spriteBatch = new SpriteBatch3D(Engine.Game);

            m_vertexBufferStretched = new [] {
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(0, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(1, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(0, 1, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(1, 1, 0))
            };
        }

        public void Draw(Map map, Position3D center, MousePicking mousePick)
        {
            InternalDetermineIfUnderEntity(map, center);
            InternalDrawEntities(map, center, mousePick, out m_renderOffset);
        }

        private void InternalDetermineIfUnderEntity(Map map, Position3D center)
        {
            // Are we inside (under a roof)? Do not draw tiles above our head.
            m_maxItemAltitude = 255;

            MapTile t;
            if ((t = map.GetMapTile(center.X, center.Y)) != null)
            {
                AEntity underObject, underTerrain;
                t.IsZUnderEntityOrGround(center.Z, out underObject, out underTerrain);

                // if we are under terrain, then do not draw any terrain at all.
                DrawTerrain = (underTerrain == null);

                if (!(underObject == null))
                {
                    // Roofing and new floors ALWAYS begin at intervals of 20.
                    // if we are under a ROOF, then get rid of everything above me.Z + 20
                    // (this accounts for A-frame roofs). Otherwise, get rid of everything
                    // at the object above us.Z.
                    if (underObject is Item)
                    {
                        Item item = (Item)underObject;
                        if (item.ItemData.IsRoof)
                            m_maxItemAltitude = center.Z - (center.Z % 20) + 20;
                        else if (item.ItemData.IsSurface || item.ItemData.IsWall)
                            m_maxItemAltitude = item.Z - (item.Z % 10);
                        else
                        {
                            int z = center.Z + ((item.ItemData.Height > 20) ? item.ItemData.Height : 20);
                            m_maxItemAltitude = (int)(z);// - (z % 20));
                        }
                    }

                    // If we are under a roof tile, do not make roofs transparent if we are on an edge.
                    if (underObject is Item && ((Item)underObject).ItemData.IsRoof)
                    {
                        bool isRoofSouthEast = true;
                        if ((t = map.GetMapTile(center.X + 1, center.Y)) != null)
                        {
                            t.IsZUnderEntityOrGround(center.Z, out underObject, out underTerrain);
                            isRoofSouthEast = !(underObject == null);
                        }

                        if (!isRoofSouthEast)
                            m_maxItemAltitude = 255;
                    }
                }
            }
        }

        private void InternalDrawEntities(Map map, Position3D center, MousePicking mousePick, out Vector2 renderOffset)
        {
            if (center == null)
            {
                renderOffset = new Vector2();
                return;
            }

            int renderDimensionY = 16; // the number of tiles that are drawn for half the screen (doubled to fill the entire screen).
            int renderDimensionX = 18; // the number of tiles that are drawn in the x-direction ( + renderExtraColumnsAtSides * 2 ).
            int renderExtraColumnsAtSides = 2; // the client draws additional tiles at the edge to make wide objects that are mostly offscreen visible.


            // when the player entity is higher (z) in the world, we must offset the first row drawn. This variable MUST be a multiple of 2.
            int renderZOffset = (center.Z / 14) * 2 + 4;
            // this is used to draw tall objects that would otherwise not be visible until their ground tile was on screen. This may still skip VERY tall objects (those weird jungle trees?)
            int renderExtraRowsAtBottom = renderZOffset + 6;

            Point firstTile = new Point(
                center.X + renderExtraColumnsAtSides - ((renderZOffset + 1) / 2),
                center.Y - renderDimensionY - renderExtraColumnsAtSides - (renderZOffset / 2));

            renderOffset.X = ((Settings.Game.Resolution.Width + ((renderDimensionY) * 44)) / 2) - 22 + renderExtraColumnsAtSides * 44;
            renderOffset.X -= (int)((center.X_offset - center.Y_offset) * 22);
            renderOffset.X -= (firstTile.X - firstTile.Y) * 22;

            renderOffset.Y = ((Settings.Game.Resolution.Height - (renderDimensionY * 44)) / 2);
            renderOffset.Y += (center.Z * 4) + (int)(center.Z_offset * 4);
            renderOffset.Y -= (int)((center.X_offset + center.Y_offset) * 22);
            renderOffset.Y -= (firstTile.X + firstTile.Y) * 22;
            renderOffset.Y -= (renderZOffset) * 22;

            ObjectsRendered = 0; // Count of objects rendered for statistics and debug

            MouseOverList overList = new MouseOverList(Engine.Input.MousePosition, mousePick.PickOnly); // List of entities mouse is over.
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
                        if (!DrawTerrain)
                        {
                            if (entities[i] is Ground)
                                break;
                            else if (entities[i].Z > tile.Ground.Z)
                                break;
                        }

                        if (entities[i].Z >= m_maxItemAltitude)
                            continue;

                        AEntityView view = entities[i].GetView();

                        if (view != null)
                            if (view.Draw(m_spriteBatch, drawPosition, overList, map))
                                ObjectsRendered++;

                        if (entities[i] is DeferredEntity)
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
            mousePick.UpdateOverEntities(overList, Engine.Input.MousePosition);

            // Draw the objects we just send to the spritebatch.
            m_spriteBatch.Prepare(true, true);
            m_spriteBatch.Flush();
        }

        private void recalculateLightning()
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