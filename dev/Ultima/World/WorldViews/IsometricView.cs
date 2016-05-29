/***************************************************************************
 *   IsometricView.cs
 *   Based on Chase Mosher's UO Renderer, licensed under GPLv3.
 *   Modifications Copyright (c) 2009, 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Ultima.World.EntityViews;
using UltimaXNA.Ultima.World.Input;
using UltimaXNA.Ultima.World.Maps;
#endregion

namespace UltimaXNA.Ultima.World.WorldViews
{
    public class IsometricRenderer
    {
        public const float TILE_SIZE_FLOAT = 44.0f;
        public const float TILE_SIZE_FLOAT_HALF = 22.0f;
        public const int TILE_SIZE_INTEGER = 44;
        public const int TILE_SIZE_INTEGER_HALF = 22;

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
                return m_RenderTargetSprites;
            }
        }

        public IsometricLighting Lighting
        {
            get;
            private set;
        }

        private RenderTarget2D m_RenderTargetSprites;

        private SpriteBatch3D m_SpriteBatch;
        private bool m_DrawTerrain = true;
        private bool m_UnderSurface = false;
        private int m_DrawMaxItemAltitude;
        private Vector2 m_DrawOffset;

        public IsometricRenderer()
        {
            m_SpriteBatch = ServiceRegistry.GetService<SpriteBatch3D>();
            Lighting = new IsometricLighting();
        }

        public void Update(Map map, Position3D center, MousePicking mousePick)
        {
            int pixelScale = (Settings.UserInterface.PlayWindowPixelDoubling) ? 2 : 1;
            if (m_RenderTargetSprites == null || m_RenderTargetSprites.Width != Settings.UserInterface.PlayWindowGumpResolution.Width / pixelScale || m_RenderTargetSprites.Height != Settings.UserInterface.PlayWindowGumpResolution.Height / pixelScale)
            {
                if (m_RenderTargetSprites != null)
                    m_RenderTargetSprites.Dispose();
                m_RenderTargetSprites = new RenderTarget2D(
                    m_SpriteBatch.GraphicsDevice, 
                    Settings.UserInterface.PlayWindowGumpResolution.Width / pixelScale, 
                    Settings.UserInterface.PlayWindowGumpResolution.Height / pixelScale, 
                    false, 
                    SurfaceFormat.Color, 
                    DepthFormat.Depth24Stencil8, 
                    0, 
                    RenderTargetUsage.DiscardContents);
            }

            DetermineIfClientIsUnderEntity(map, center);
            DrawEntities(map, center, mousePick, out m_DrawOffset);
        }

        private void DetermineIfClientIsUnderEntity(Map map, Position3D center)
        {
            // Are we inside (under a roof)? Do not draw tiles above our head.
            m_DrawMaxItemAltitude = 255;
            m_DrawTerrain = true;
            m_UnderSurface = false;

            MapTile tile;
            AEntity underObject, underTerrain;
            if ((tile = map.GetMapTile(center.X, center.Y)) != null)
            {
                if (tile.IsZUnderEntityOrGround(center.Z, out underObject, out underTerrain))
                {
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
                            else if (item.ItemData.IsSurface || (item.ItemData.IsWall && !item.ItemData.IsDoor))
                                m_DrawMaxItemAltitude = item.Z;
                            else
                            {
                                int z = center.Z + ((item.ItemData.Height > 20) ? item.ItemData.Height : 20);
                                m_DrawMaxItemAltitude = z;
                            }
                        }

                        // If we are under a roof tile, do not make roofs transparent if we are on an edge.
                        if (underObject is Item && ((Item)underObject).ItemData.IsRoof)
                        {
                            bool isRoofSouthEast = true;
                            if ((tile = map.GetMapTile(center.X + 1, center.Y)) != null)
                            {
                                tile.IsZUnderEntityOrGround(center.Z, out underObject, out underTerrain);
                                isRoofSouthEast = !(underObject == null);
                            }

                            if (!isRoofSouthEast)
                                m_DrawMaxItemAltitude = 255;
                        }

                        m_UnderSurface = (m_DrawMaxItemAltitude != 255);
                    }
                }
            }
        }
        Point firstTile, renderDimensions;
        int overDrawTilesOnSides = 3;
        int overDrawTilesAtTopAndBottom = 6;
        int overDrawAdditionalTilesOnBottom = 10;
        MouseOverList overList;
        List<AEntity> deferredToRemove = new List<AEntity>();
        int x, y;
        MapTile tile;
        Point firstTileInRow = new Point();
        Vector3 drawPosition = new Vector3();
        List<AEntity> entities;
        private void DrawEntities(Map map, Position3D center, MousePicking mousePicking, out Vector2 renderOffset)
        {
            if (center == null)
            {
                renderOffset = new Vector2();
                return;
            }

            // reset the spritebatch Z
            m_SpriteBatch.Reset();
            // set the lighting variables.
            m_SpriteBatch.SetLightIntensity(Lighting.IsometricLightLevel);
            m_SpriteBatch.SetLightDirection(Lighting.IsometricLightDirection);

            // get variables that describe the tiles drawn in the viewport: the first tile to draw,
            // the offset to that tile, and the number of tiles drawn in the x and y dimensions.

            CalculateViewport(center, overDrawTilesOnSides, overDrawTilesAtTopAndBottom, out firstTile, out renderOffset, out renderDimensions);
            
            CountEntitiesRendered = 0; // Count of objects rendered for statistics and debug

            overList = new MouseOverList(mousePicking); // List of entities mouse is over.

            for (y = 0; y < renderDimensions.Y * 2 + 1 + overDrawAdditionalTilesOnBottom; ++y)
            {
                drawPosition.X = (firstTile.X - firstTile.Y + (y % 2)) * TILE_SIZE_FLOAT_HALF + renderOffset.X;
                drawPosition.Y = (firstTile.X + firstTile.Y + y) * TILE_SIZE_FLOAT_HALF + renderOffset.Y;

                firstTileInRow.X = firstTile.X + ((y + 1) / 2);
                firstTileInRow.Y = firstTile.Y + (y / 2);
                for (x = 0; x < renderDimensions.X + 1; ++x)
                {
                    tile = map.GetMapTile(firstTileInRow.X - x, firstTileInRow.Y + x);
                    if (tile == null)
                    {
                        drawPosition.X -= TILE_SIZE_FLOAT;
                        continue;
                    }

                    entities = tile.Entities;
                    bool draw = true;
                    for (int i = 0; i < entities.Count; i++)
                    {
                        if (entities[i] is DeferredEntity)
                            deferredToRemove.Add(entities[i]);

                        if (!m_DrawTerrain)
                        {
                            if ((entities[i] is Ground) || (entities[i].Z > tile.Ground.Z))
                                draw = false;
                        }

                        if ((entities[i].Z >= m_DrawMaxItemAltitude || (m_DrawMaxItemAltitude != 255 && entities[i] is Item && (entities[i] as Item).ItemData.IsRoof)) && !(entities[i] is Ground))
                        {
                            continue;
                        }

                        if (draw)
                        {
                            AEntityView view = entities[i].GetView();
                            if (view != null)
                            {
                                if (view.Draw(m_SpriteBatch, drawPosition, overList, map, !m_UnderSurface))
                                    CountEntitiesRendered++;
                            }
                        }
                    }

                    foreach (AEntity deferred in deferredToRemove)
                        tile.OnExit(deferred);
                    deferredToRemove.Clear();

                    drawPosition.X -= TILE_SIZE_FLOAT;
                }
            }

            OverheadsView.Render(m_SpriteBatch, overList, map, m_UnderSurface);

            // Update the MouseOver objects
            mousePicking.UpdateOverEntities(overList, mousePicking.Position);

            // Draw the objects we just send to the spritebatch.
            m_SpriteBatch.GraphicsDevice.SetRenderTarget(m_RenderTargetSprites);
            m_SpriteBatch.GraphicsDevice.Clear(Color.Black);
            m_SpriteBatch.FlushSprites(true);
            m_SpriteBatch.GraphicsDevice.SetRenderTarget(null);
        }

        private void CalculateViewport(Position3D center, int overDrawTilesOnSides, int overDrawTilesOnTopAndBottom, out Point firstTile, out Vector2 renderOffset, out Point renderDimensions)
        {
            int pixelScale = (Settings.UserInterface.PlayWindowPixelDoubling) ? 2 : 1;

            renderDimensions.Y = Settings.UserInterface.PlayWindowGumpResolution.Height / pixelScale / TILE_SIZE_INTEGER + overDrawTilesOnTopAndBottom; // the number of tiles that are drawn for half the screen (doubled to fill the entire screen).
            renderDimensions.X = Settings.UserInterface.PlayWindowGumpResolution.Width / pixelScale / TILE_SIZE_INTEGER + overDrawTilesOnSides; // the number of tiles that are drawn in the x-direction ( + renderExtraColumnsAtSides * 2 ).
            int renderDimensionsDiff = Math.Abs(renderDimensions.X - renderDimensions.Y);
            renderDimensionsDiff -= renderDimensionsDiff % 2; // make sure this is an even number...

            // when the player entity is at a higher z altitude in the world, we must offset the first row drawn so that tiles at lower altitudes are drawn.
            // The reverse is not true - at lower altitutdes, higher tiles are never on screen. This is an artifact of UO's isometric projection.
            // Note: The value of this variable MUST be a multiple of 2 and MUST be positive.
            int firstZOffset = (center.Z > 0) ? (int)Math.Abs(((center.Z + center.Z_offset) / 11)) : 0;
            // this is used to draw tall objects that would otherwise not be visible until their ground tile was on screen. This may still skip VERY tall objects (those weird jungle trees?)

            firstTile = new Point(center.X - firstZOffset, center.Y - renderDimensions.Y - firstZOffset);
            if (renderDimensions.Y > renderDimensions.X)
            {
                firstTile.X -= renderDimensionsDiff / 2;
                firstTile.Y -= renderDimensionsDiff / 2;
            }
            else
            {
                firstTile.X += renderDimensionsDiff / 2;
                firstTile.Y -= renderDimensionsDiff / 2;
            }

            renderOffset.X = (((Settings.UserInterface.PlayWindowGumpResolution.Width / pixelScale) + ((renderDimensions.Y) * TILE_SIZE_INTEGER)) / 2) - TILE_SIZE_FLOAT_HALF;
            renderOffset.X -= (int)((center.X_offset - center.Y_offset) * TILE_SIZE_FLOAT_HALF);
            renderOffset.X -= (firstTile.X - firstTile.Y) * TILE_SIZE_FLOAT_HALF;
            renderOffset.X += renderDimensionsDiff * TILE_SIZE_FLOAT_HALF;

            renderOffset.Y = ((Settings.UserInterface.PlayWindowGumpResolution.Height / pixelScale) / 2 - (renderDimensions.Y * TILE_SIZE_INTEGER / 2));
            renderOffset.Y += ((center.Z + center.Z_offset) * 4);
            renderOffset.Y -= (int)((center.X_offset + center.Y_offset) * TILE_SIZE_FLOAT_HALF);
            renderOffset.Y -= (firstTile.X + firstTile.Y) * TILE_SIZE_FLOAT_HALF;
            renderOffset.Y -= TILE_SIZE_FLOAT_HALF;
            renderOffset.Y -= firstZOffset * TILE_SIZE_FLOAT;
        }
    }
}