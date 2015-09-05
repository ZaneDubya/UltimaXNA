/***************************************************************************
 *   AEntityView.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
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
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Ultima.World.Input;
using UltimaXNA.Ultima.World.Maps;
using UltimaXNA.Ultima.World.WorldViews;
#endregion

namespace UltimaXNA.Ultima.World.EntityViews
{
    /// <summary>
    /// An abstract class that can be attached to an entity and used to maintain data for a 'View'.
    /// </summary>
    public abstract class AEntityView
    {
        public static Techniques s_Technique = Techniques.Default;

        private AEntity m_Entity = null;
        public AEntity Entity
        {
            get { return m_Entity; }
        }

        public AEntityView(AEntity entity)
        {
            m_Entity = entity;
            SortZ = Entity.Z;
        }

        public PickType PickType = PickType.PickNothing;

        // ======================================================================
        // Sort routines
        // ======================================================================

        public int SortZ = 0;

        // ======================================================================
        // Draw methods and properties
        // ======================================================================

        public float Rotation = 0f;
        public static float PI = (float)Math.PI;

        protected bool DrawFlip = false;
        protected Rectangle DrawArea = Rectangle.Empty;
        protected Texture2D DrawTexture = null;
        protected Vector3 HueVector = Vector3.Zero;

        protected bool IsShadowCastingView = false;
        protected float DrawShadowZDepth = 0;

        public virtual bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOverList, Map map, bool roofHideFlag)
        {
            VertexPositionNormalTextureHue[] vertexBuffer;

            if (Rotation != 0)
            {
                float w = DrawArea.Width / 2f;
                float h = DrawArea.Height / 2f;
                Vector3 center = drawPosition - new Vector3(DrawArea.X - IsometricRenderer.TILE_SIZE_INTEGER + w, DrawArea.Y + h, 0);
                float sinx = (float)Math.Sin(Rotation) * w;
                float cosx = (float)Math.Cos(Rotation) * w;
                float siny = (float)Math.Sin(Rotation) * -h;
                float cosy = (float)Math.Cos(Rotation) * -h;
                // 2   0    
                // |\  |     
                // |  \|     
                // 3   1
                vertexBuffer = VertexPositionNormalTextureHue.PolyBufferFlipped;

                vertexBuffer[0].Position = center;
                vertexBuffer[0].Position.X += cosx - -siny;
                vertexBuffer[0].Position.Y -= sinx + -cosy;

                vertexBuffer[1].Position = center;
                vertexBuffer[1].Position.X += cosx - siny;
                vertexBuffer[1].Position.Y += -sinx + -cosy;

                vertexBuffer[2].Position = center;
                vertexBuffer[2].Position.X += -cosx - -siny;
                vertexBuffer[2].Position.Y += sinx + cosy;

                vertexBuffer[3].Position = center;
                vertexBuffer[3].Position.X += -cosx - siny;
                vertexBuffer[3].Position.Y += sinx + -cosy;
            }
            else
            {
                if (DrawFlip)
                {
                    // 2   0    
                    // |\  |     
                    // |  \|     
                    // 3   1
                    vertexBuffer = VertexPositionNormalTextureHue.PolyBufferFlipped;
                    vertexBuffer[0].Position = drawPosition;
                    vertexBuffer[0].Position.X += DrawArea.X + IsometricRenderer.TILE_SIZE_FLOAT;
                    vertexBuffer[0].Position.Y -= DrawArea.Y;
                    vertexBuffer[0].TextureCoordinate.Y = 0;

                    vertexBuffer[1].Position = vertexBuffer[0].Position;
                    vertexBuffer[1].Position.Y += DrawArea.Height;

                    vertexBuffer[2].Position = vertexBuffer[0].Position;
                    vertexBuffer[2].Position.X -= DrawArea.Width;
                    vertexBuffer[2].TextureCoordinate.Y = 0;

                    vertexBuffer[3].Position = vertexBuffer[1].Position;
                    vertexBuffer[3].Position.X -= DrawArea.Width;

                    /*if (m_YClipLine != 0)
                    {
                        if (m_YClipLine > vertexBuffer[3].Position.Y)
                            return false;
                        else if (m_YClipLine > vertexBuffer[0].Position.Y)
                        {
                            float uvStart = (m_YClipLine - vertexBuffer[0].Position.Y) / DrawTexture.Height;
                            vertexBuffer[0].Position.Y = vertexBuffer[2].Position.Y = m_YClipLine;
                            vertexBuffer[0].TextureCoordinate.Y = vertexBuffer[2].TextureCoordinate.Y = uvStart;
                        }
                    }*/
                }
                else
                {
                    // 0---1    
                    //    /     
                    //  /       
                    // 2---3
                    vertexBuffer = VertexPositionNormalTextureHue.PolyBuffer;
                    vertexBuffer[0].Position = drawPosition;
                    vertexBuffer[0].Position.X -= DrawArea.X;
                    vertexBuffer[0].Position.Y -= DrawArea.Y;
                    vertexBuffer[0].TextureCoordinate.Y = 0;

                    vertexBuffer[1].Position = vertexBuffer[0].Position;
                    vertexBuffer[1].Position.X += DrawArea.Width;
                    vertexBuffer[1].TextureCoordinate.Y = 0;

                    vertexBuffer[2].Position = vertexBuffer[0].Position;
                    vertexBuffer[2].Position.Y += DrawArea.Height;

                    vertexBuffer[3].Position = vertexBuffer[1].Position;
                    vertexBuffer[3].Position.Y += DrawArea.Height;

                    /*if (m_YClipLine != 0)
                    {
                        if (m_YClipLine >= vertexBuffer[3].Position.Y)
                            return false;
                        else if (m_YClipLine > vertexBuffer[0].Position.Y)
                        {
                            float uvStart = (m_YClipLine - vertexBuffer[0].Position.Y) / DrawTexture.Height;
                            vertexBuffer[0].Position.Y = vertexBuffer[1].Position.Y = m_YClipLine;
                            vertexBuffer[0].TextureCoordinate.Y = vertexBuffer[1].TextureCoordinate.Y = uvStart;
                        }
                    }*/
                }
            }

            if (vertexBuffer[0].Hue != HueVector)
                vertexBuffer[0].Hue = vertexBuffer[1].Hue = vertexBuffer[2].Hue = vertexBuffer[3].Hue = HueVector;

            if (!spriteBatch.DrawSprite(DrawTexture, vertexBuffer, s_Technique))
            {
                // the vertex buffer was not on screen, return false (did not draw)
                return false;
            }

            Pick(mouseOverList, vertexBuffer);

            if (IsShadowCastingView)
            {
                spriteBatch.DrawShadow(DrawTexture, vertexBuffer, new Vector2(
                    drawPosition.X + IsometricRenderer.TILE_SIZE_FLOAT_HALF,
                    drawPosition.Y + (Entity.Position.Offset.X + Entity.Position.Offset.Y) * IsometricRenderer.TILE_SIZE_FLOAT_HALF - ((Entity.Position.Z_offset + Entity.Z) * 4) + IsometricRenderer.TILE_SIZE_FLOAT_HALF),
                    DrawFlip, DrawShadowZDepth);
            }

            return true;
        }

        /// <summary>
        /// Used by DeferredView to draw an object without first determining if it should be deferred.
        /// Should only be implemented for those views that call CheckDefer(), Otherwise, using only
        /// Draw() will suffice. See MobileView for an example of use.
        /// </summary>
        public virtual bool DrawInternal(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOverList, Map map, bool roofHideFlag)
        {
            return false;
        }

        /// <summary>
        /// Draws all overheads, starting at [yOffset] pixels above the Entity's anchor point on the ground.
        /// </summary>
        /// <param name="yOffset"></param>
        public void DrawOverheads(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOverList, Map map, int yOffset)
        {
            for (int i = 0; i < Entity.Overheads.Count; i++)
            {
                AEntityView view = Entity.Overheads[i].GetView();
                view.DrawArea = new Rectangle((view.DrawTexture.Width / 2) - 22, yOffset + view.DrawTexture.Height, view.DrawTexture.Width, view.DrawTexture.Height);
                OverheadsView.AddView(view, drawPosition);
                yOffset += view.DrawTexture.Height;
            }
        }

        protected void Pick(MouseOverList mouseOverList, VertexPositionNormalTextureHue[] vertexBuffer)
        {
            if ((mouseOverList.PickType & PickType) == PickType)
            {
                if (((!DrawFlip) && mouseOverList.IsMouseInObject(vertexBuffer[0].Position, vertexBuffer[3].Position)) ||
                    ((DrawFlip) && mouseOverList.IsMouseInObject(vertexBuffer[2].Position, vertexBuffer[1].Position)))
                {
                    MouseOverItem item;
                    if (!DrawFlip)
                    {
                        item = new MouseOverItem(DrawTexture, vertexBuffer[0].Position, Entity);
                        item.Vertices = new Vector3[4] { vertexBuffer[0].Position, vertexBuffer[1].Position, vertexBuffer[2].Position, vertexBuffer[3].Position };
                    }
                    else
                    {
                        item = new MouseOverItem(DrawTexture, vertexBuffer[2].Position, Entity);
                        item.Vertices = new Vector3[4] { vertexBuffer[2].Position, vertexBuffer[0].Position, vertexBuffer[3].Position, vertexBuffer[1].Position };
                        item.FlippedTexture = true;
                    }
                    mouseOverList.Add2DItem(item);
                }
            }
        }

        // ======================================================================
        // Y Clipping (used during deferred draws.
        // ======================================================================

        protected int m_YClipLine = 0;

        /// <summary>
        /// Between the time when this is called and when ClearYClipLine() is called, this view will only draw sprites below
        /// the specified y line.
        /// </summary>
        /// <param name="y"></param>
        public virtual void SetYClipLine(float y)
        {
            m_YClipLine = (int)y;
        }

        public virtual void ClearYClipLine()
        {
            m_YClipLine = 0;
        }

        // ======================================================================
        // Deferred drawing code
        // ======================================================================

        protected void CheckDefer(Map map, Vector3 drawPosition)
        {
            MapTile deferToTile;
            Direction checkDirection;
            if (Entity is Mobile && ((Mobile)Entity).IsMoving)
            {
                Direction facing = (Entity as Mobile).DrawFacing;
                if (
                    ((facing & Direction.FacingMask) == Direction.Left) ||
                    ((facing & Direction.FacingMask) == Direction.South) ||
                    ((facing & Direction.FacingMask) == Direction.East))
                {
                    deferToTile = map.GetMapTile(Entity.Position.X, Entity.Position.Y + 1);
                    checkDirection = facing & Direction.FacingMask;
                }
                else if ((facing & Direction.FacingMask) == Direction.Down)
                {
                    deferToTile = map.GetMapTile(Entity.Position.X + 1, Entity.Position.Y + 1);
                    checkDirection = Direction.Down;
                }
                else
                {
                    deferToTile = map.GetMapTile(Entity.Position.X + 1, Entity.Position.Y);
                    checkDirection = Direction.East;
                }
            }
            else
            {
                deferToTile = map.GetMapTile(Entity.Position.X + 1, Entity.Position.Y);
                checkDirection = Direction.East;
            }

            if (deferToTile != null)
            {
                if (Entity is Mobile)
                {
                    Mobile mobile = Entity as Mobile;
                    // This calculates the z position of the mobile as if it had moved into the next tile.
                    // Strictly speaking, this isn't necessary, but looks nice for mobiles that are walking.
                    int z = MobileMovementCheck.GetNextZ(mobile, Entity.Position, checkDirection); 
                    DeferredEntity deferred = new DeferredEntity(mobile, drawPosition, z);
                    deferToTile.OnEnter(deferred);
                }
                else
                {
                    DeferredEntity deferred = new DeferredEntity(Entity, drawPosition, Entity.Z);
                    deferToTile.OnEnter(deferred);
                }
            }
        }

        protected bool CheckUnderSurface(Map map, int x, int y)
        {
            return UnderSurfaceCheck(map, x, y) && UnderSurfaceCheck(map, x + 1, y + 1) && UnderSurfaceCheck(map, x + 2, y + 2);
        }

        private bool UnderSurfaceCheck(Map map, int x, int y)
        {
            MapTile tile;
            AEntity e0, e1;
            if ((tile = map.GetMapTile(x, y)) != null)
            {
                if (tile == null)
                    return false;
                if (tile.IsZUnderEntityOrGround(Entity.Position.Z, out e0, out e1))
                {
                    if (e0 == null || !(e0 is Item))
                        return false;
                    Item item = e0 as Item;
                    if ((item.ItemData.IsRoof) || (item.ItemData.IsSurface) || (item.ItemData.IsWall && !item.ItemData.IsDoor))
                        return true;
                }
            }

            return false;
        }
    }
}
