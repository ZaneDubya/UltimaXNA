/***************************************************************************
 *   IsometricView.cs
 *   Based on code from ClintXNA's renderer.
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Ultima.World.EntityViews;
using UltimaXNA.Ultima.World.Input;
using UltimaXNA.Ultima.World.Maps;
using UltimaXNA.Core.Diagnostics.Tracing;
#endregion

namespace UltimaXNA.Ultima.World.WorldViews
{
    public class IsometricRenderer
    {
        public const float TILE_SIZE_FLOAT = 44.0f;
        public const int TILE_SIZE_INTEGER = 44;

        /// <summary>
        /// The number of entities drawn in the previous frame.
        /// </summary>
        public int CountEntitiesRendered
        {
            get;
            private set;
        }

        /// <summary>
        /// The texture of the last drawn frame.
        /// </summary>
        public Texture2D Texture
        {
            get
            {
                return m_RenderTarget;
            }
        }

        public IsometricLighting Lighting
        {
            get;
            private set;
        }

        private RenderTarget2D m_RenderTarget;
        private SpriteBatch3D m_SpriteBatch;
        private bool m_DrawTerrain = true;
        private int m_DrawMaxItemAltitude;
        private Vector2 m_DrawOffset;

        public IsometricRenderer()
        {
            m_SpriteBatch = ServiceRegistry.GetService<SpriteBatch3D>();
            Lighting = new IsometricLighting();
        }

        public void Update(Map map, Position3D center, MousePicking mousePick)
        {
            if (m_RenderTarget == null || m_RenderTarget.Width != Settings.World.GumpResolution.Width || m_RenderTarget.Height != Settings.World.GumpResolution.Height)
            {
                if (m_RenderTarget != null)
                    m_RenderTarget.Dispose();
                m_RenderTarget = new RenderTarget2D(m_SpriteBatch.GraphicsDevice, Settings.World.GumpResolution.Width, Settings.World.GumpResolution.Height, false, SurfaceFormat.Color, DepthFormat.Depth16, 0, RenderTargetUsage.DiscardContents);
            }

            DetermineIfClientIsUnderEntity(map, center);

            m_SpriteBatch.GraphicsDevice.SetRenderTarget(m_RenderTarget);

            DrawEntities(map, center, mousePick, out m_DrawOffset);

            m_SpriteBatch.GraphicsDevice.SetRenderTarget(null);
        }

        private void DetermineIfClientIsUnderEntity(Map map, Position3D center)
        {
            // Are we inside (under a roof)? Do not draw tiles above our head.
            m_DrawMaxItemAltitude = 255;

            MapTile t;
            if ((t = map.GetMapTile(center.X, center.Y)) != null)
            {
                AEntity underObject, underTerrain;
                t.IsZUnderEntityOrGround(center.Z, out underObject, out underTerrain);

                // if we are under terrain, then do not draw any terrain at all.
                m_DrawTerrain = (underTerrain == null);

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
                            m_DrawMaxItemAltitude = center.Z - (center.Z % 20) + 20;
                        else if (item.ItemData.IsSurface || item.ItemData.IsWall)
                            m_DrawMaxItemAltitude = item.Z;
                        else
                        {
                            int z = center.Z + ((item.ItemData.Height > 20) ? item.ItemData.Height : 20);
                            m_DrawMaxItemAltitude = z;// - (z % 20));
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
                            m_DrawMaxItemAltitude = 255;
                    }
                }
            }
        }

        private void DrawEntities(Map map, Position3D center, MousePicking mousePicking, out Vector2 renderOffset)
        {
            if (center == null)
            {
                renderOffset = new Vector2();
                return;
            }

            // set the lighting variables.
            m_SpriteBatch.SetLightIntensity(Lighting.IsometricLightLevel);
            m_SpriteBatch.SetLightDirection(Lighting.IsometricLightDirection);

            //int renderDimensionY = 16; // the number of tiles that are drawn for half the screen (doubled to fill the entire screen).
            //int renderDimensionX = 18; // the number of tiles that are drawn in the x-direction ( + renderExtraColumnsAtSides * 2 ).
            int renderDimensionX = Settings.World.GumpResolution.Width / TILE_SIZE_INTEGER;
            int renderDimensionY = Settings.World.GumpResolution.Height / TILE_SIZE_INTEGER;
            int renderExtraColumnsAtSides = 2; // the client draws additional tiles at the edge to make wide objects that are mostly offscreen visible.


            // when the player entity is higher (z) in the world, we must offset the first row drawn. This variable MUST be a multiple of 2.
            int renderZOffset = (center.Z / 14) * 2 + 4;
            // this is used to draw tall objects that would otherwise not be visible until their ground tile was on screen. This may still skip VERY tall objects (those weird jungle trees?)
            int renderExtraRowsAtBottom = renderZOffset + 10;

            Point firstTile = new Point(
                center.X + renderExtraColumnsAtSides - ((renderZOffset + 1) / 2),
                center.Y - renderDimensionY - renderExtraColumnsAtSides - (renderZOffset / 2));

            renderOffset.X = ((Settings.World.GumpResolution.Width + ((renderDimensionY) * TILE_SIZE_INTEGER)) / 2) - 22 + renderExtraColumnsAtSides * TILE_SIZE_INTEGER;
            renderOffset.X -= (int)((center.X_offset - center.Y_offset) * 22);
            renderOffset.X -= (firstTile.X - firstTile.Y) * 22;

            renderOffset.Y = ((Settings.World.GumpResolution.Height - (renderDimensionY * TILE_SIZE_INTEGER)) / 2);
            renderOffset.Y += (center.Z * 4) + (int)(center.Z_offset * 4);
            renderOffset.Y -= (int)((center.X_offset + center.Y_offset) * 22);
            renderOffset.Y -= (firstTile.X + firstTile.Y) * 22;
            renderOffset.Y -= (renderZOffset) * 22;

            Tracer.Error("{0} and {1} Window size{2} and {3} View size{4} and {5}", renderOffset.X, renderOffset.Y, Settings.World.WindowResolution.Width, Settings.World.WindowResolution.Height, Settings.World.GumpResolution.Width, Settings.World.GumpResolution.Height);

            CountEntitiesRendered = 0; // Count of objects rendered for statistics and debug

            MouseOverList overList = new MouseOverList(mousePicking); // List of entities mouse is over.
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
                        if (!m_DrawTerrain)
                        {
                            if (entities[i] is Ground)
                                break;
                            else if (entities[i].Z > tile.Ground.Z)
                                break;
                        }

                        if (entities[i].Z >= m_DrawMaxItemAltitude && !(entities[i] is Ground))
                            continue;

                        AEntityView view = entities[i].GetView();

                        if (view != null)
                            if (view.Draw(m_SpriteBatch, drawPosition, overList, map))
                                CountEntitiesRendered++;

                        if (entities[i] is DeferredEntity)
                        {
                            deferredToRemove.Add(entities[i]);
                        }
                    }

                    foreach (AEntity deferred in deferredToRemove)
                        tile.OnExit(deferred);
                    deferredToRemove.Clear();

                    drawPosition.X -= TILE_SIZE_FLOAT;
                }
            }

            OverheadRenderer.Render(m_SpriteBatch, overList, map);

            // Update the MouseOver objects
            mousePicking.UpdateOverEntities(overList, mousePicking.Position);

            // Draw the objects we just send to the spritebatch.
            m_SpriteBatch.Flush(true);
        }
    }
}