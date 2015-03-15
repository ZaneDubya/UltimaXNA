/***************************************************************************
 *   MapObject.cs
 *   Based on code from ClintXNA's renderer: http://www.runuo.com/forums/xna/92023-hi.html
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Entity;
using UltimaXNA.Rendering;
using UltimaXNA.UltimaWorld.View;
using UltimaXNA.UltimaWorld.Model;

namespace UltimaXNA.UltimaWorld.View
{
    public abstract class AMapObject
    {
        private Position3D m_Position;
        public Position3D Position
        {
            get { return m_Position; }
            set
            {
                m_Position = value;
                Z = SortZ = (int)m_Position.Z;
            }
        }

        private float m_z = 0;
        public float Z
        {
            get { return m_z; }
            set
            {
                m_z = value;
                SortZ = (int)m_z;
            }
        }
        public int SortZ = 0;           // This is the default sort value of the object.
        public int SortThreshold = 0;   // This is a sort value which should be set based on the type of object.
        public int SortTiebreaker = 0;  // This is a sort value which is used to sort layers of a single object.

        public int ItemID = 0;
        public BaseEntity OwnerEntity = null;
        public Serial OwnerSerial
        {
            get { return (OwnerEntity == null) ? (Serial)unchecked((int)0) : OwnerEntity.Serial; }
        }

        internal bool m_draw_flip;
        internal int m_draw_X, m_draw_Y;
        internal int m_draw_width, m_draw_height;
        internal Vector2 m_draw_hue; // x is the hue. y = 0, no hue. y = 1, total hue.  y = 2, partial hue. y = 4 is a 50% transparency bitflag.
        internal Texture2D m_draw_texture;
        internal PickTypes m_pickType;
        internal bool m_draw_IsometricOverlap = false; // if this is true, we will draw any corners that are overlapped by tiles drawn after this object.

        public AMapObject(Position3D position)
        {
            Position = position;
        }

        internal virtual bool Draw(SpriteBatch3D sb, Vector3 drawPosition, MouseOverList molist, PickTypes pickType, int maxAlt)
        {
            VertexPositionNormalTextureHue[] vertexBuffer;

            if (Z >= maxAlt)
                return false;

            if (m_draw_flip)
            {
                // 2   0    
                // |\  |     
                // |  \|     
                // 3   1
                vertexBuffer = VertexPositionNormalTextureHue.PolyBufferFlipped;
                vertexBuffer[0].Position = drawPosition;
                vertexBuffer[0].Position.X += m_draw_X + 44;
                vertexBuffer[0].Position.Y -= m_draw_Y;

                vertexBuffer[1].Position = vertexBuffer[0].Position;
                vertexBuffer[1].Position.Y += m_draw_height;

                vertexBuffer[2].Position = vertexBuffer[0].Position;
                vertexBuffer[2].Position.X -= m_draw_width;

                vertexBuffer[3].Position = vertexBuffer[1].Position;
                vertexBuffer[3].Position.X -= m_draw_width;
            }
            else
            {
                // 0---1    
                //    /     
                //  /       
                // 2---3
                vertexBuffer = VertexPositionNormalTextureHue.PolyBuffer;
                vertexBuffer[0].Position = drawPosition;
                vertexBuffer[0].Position.X -= m_draw_X;
                vertexBuffer[0].Position.Y -= m_draw_Y;

                vertexBuffer[1].Position = vertexBuffer[0].Position;
                vertexBuffer[1].Position.X += m_draw_width;

                vertexBuffer[2].Position = vertexBuffer[0].Position;
                vertexBuffer[2].Position.Y += m_draw_height;

                vertexBuffer[3].Position = vertexBuffer[1].Position;
                vertexBuffer[3].Position.Y += m_draw_height;
            }

            if (vertexBuffer[0].Hue != m_draw_hue)
                vertexBuffer[0].Hue = vertexBuffer[1].Hue = vertexBuffer[2].Hue = vertexBuffer[3].Hue = m_draw_hue;
            
            if (!sb.Draw(m_draw_texture, vertexBuffer))
            {
                return false;
            }
            
            if (m_draw_IsometricOverlap)
            {
                drawIsometricOverlap(sb, vertexBuffer, new Vector2(drawPosition.X, drawPosition.Y - (Z * 4)));
            }
            
            if ((pickType & m_pickType) == m_pickType)
            {
                if (((!m_draw_flip) && molist.IsMouseInObject(vertexBuffer[0].Position, vertexBuffer[3].Position)) ||
                    ((m_draw_flip) && molist.IsMouseInObject(vertexBuffer[2].Position, vertexBuffer[1].Position)))
                {
                    MouseOverItem item;
                    if (!m_draw_flip)
                    {
                        item = new MouseOverItem(m_draw_texture, vertexBuffer[0].Position, null);
                        item.Vertices = new Vector3[4] { vertexBuffer[0].Position, vertexBuffer[1].Position, vertexBuffer[2].Position, vertexBuffer[3].Position };
                    }
                    else
                    {
                        item = new MouseOverItem(m_draw_texture, vertexBuffer[2].Position, null);
                        item.Vertices = new Vector3[4] { vertexBuffer[2].Position, vertexBuffer[0].Position, vertexBuffer[3].Position, vertexBuffer[1].Position };
                    }
                    molist.Add2DItem(item);
                }
            }

            return true;
        }

        /// <summary>
        /// drawIsometricOverlap will create deferred objects that will be drawn in places where a MapObject
        /// might be overlapped by subsequently drawn objects in the world. NOTE that the vertices of the
        /// deferred objects created by this routine are NOT perfect - they may have UV coords that are greater
        /// than 1.0f or less than 0.0f. This would create graphical artifacts, but we rely on our HLSL shader
        /// to ignore these.
        /// </summary>
        /// <param name="sb">The SpriteBatch3D instance passed to the calling Draw() routine.</param>
        /// <param name="vertices">The Vertices that were drawn by the calling MapObject.</param>
        /// <param name="drawPosition">The upper-left hand corner of the tile where the calling MapObject was drawn.</param>
        private void drawIsometricOverlap(SpriteBatch3D sb, VertexPositionNormalTextureHue[] vertices, Vector2 screenPosition)
        {
            Vector2 overlapCurrent = new Vector2(screenPosition.X + 22, screenPosition.Y + 44);
            Vector2 overlapToHere = m_draw_flip ? 
                new Vector2(vertices[1].Position.X, vertices[1].Position.Y) : 
                new Vector2(vertices[3].Position.X, vertices[3].Position.Y);

            int tileX, tileY;
            
            MapObjectDeferred deferred;
            VertexPositionNormalTextureHue[] verts;
            
            if (overlapToHere.Y > (overlapCurrent.Y - 22))
            {
                tileX = Position.X;
                tileY = Position.Y + (int)Math.Ceiling((overlapToHere.Y - (overlapCurrent.Y - 22)) / 22f);

                // Get the tile associated with this (x, y) position. If the tile is not loaded, don't add a deferred object
                // (it'll be offscreen so it doesn't matter).
                MapTile tile = EntityManager.Model.Map.GetMapTile(tileX, tileY);
                if (tile != null)
                {
                    deferred = new MapObjectDeferred(m_draw_texture, this);
                    deferred.Position.Set(tileX, tileY, Position.Z);
                    verts = deferred.Vertices;

                    if (m_draw_flip)
                    {
                        //     0
                        //    / \
                        //   /   1
                        //  /   /
                        // 2---3
                        verts[0].Position = new Vector3(overlapCurrent, 0) + new Vector3(-22, -22, 0);
                        verts[0].TextureCoordinate = new Vector3((overlapToHere.X - verts[0].Position.X) / m_draw_texture.Width, 1f - (overlapToHere.Y - verts[0].Position.Y) / m_draw_texture.Height, 0);
                        verts[1].Position = new Vector3(overlapCurrent, 0);
                        verts[1].TextureCoordinate = new Vector3((overlapToHere.X - verts[1].Position.X) / m_draw_texture.Width, 1f - (overlapToHere.Y - verts[1].Position.Y) / m_draw_texture.Height, 0);
                        verts[2].Position = new Vector3(verts[0].Position.X - (overlapToHere.Y - verts[0].Position.Y), overlapToHere.Y, 0);
                        verts[2].TextureCoordinate = new Vector3((overlapToHere.X - verts[2].Position.X) / m_draw_texture.Width, 1f - (overlapToHere.Y - verts[2].Position.Y) / m_draw_texture.Height, 0);
                        verts[3].Position = new Vector3(verts[1].Position.X - (overlapToHere.Y - verts[1].Position.Y), overlapToHere.Y, 0);
                        verts[3].TextureCoordinate = new Vector3((overlapToHere.X - verts[3].Position.X) / m_draw_texture.Width, 1f - (overlapToHere.Y - verts[3].Position.Y) / m_draw_texture.Height, 0);
                    }
                    else
                    {
                        //     1
                        //    / \
                        //   /   3
                        //  /   /
                        // 0---2
                        verts[1].Position = new Vector3(overlapCurrent, 0) + new Vector3(-22, -22, 0);
                        verts[1].TextureCoordinate = new Vector3(1f - (overlapToHere.X - verts[1].Position.X) / m_draw_texture.Width, 1f - (overlapToHere.Y - verts[1].Position.Y) / m_draw_texture.Height, 0);
                        verts[3].Position = new Vector3(overlapCurrent, 0);
                        verts[3].TextureCoordinate = new Vector3(1f - (overlapToHere.X - verts[3].Position.X) / m_draw_texture.Width, 1f - (overlapToHere.Y - verts[3].Position.Y) / m_draw_texture.Height, 0);
                        verts[0].Position = new Vector3(verts[1].Position.X - (overlapToHere.Y - verts[1].Position.Y), overlapToHere.Y, 0);
                        verts[0].TextureCoordinate = new Vector3(1f - (overlapToHere.X - verts[0].Position.X) / m_draw_texture.Width, 1f - (overlapToHere.Y - verts[0].Position.Y) / m_draw_texture.Height, 0);
                        verts[2].Position = new Vector3(verts[3].Position.X - (overlapToHere.Y - verts[3].Position.Y), overlapToHere.Y, 0);
                        verts[2].TextureCoordinate = new Vector3(1f - (overlapToHere.X - verts[2].Position.X) / m_draw_texture.Width, 1f - (overlapToHere.Y - verts[2].Position.Y) / m_draw_texture.Height, 0);
                    }

                    verts[0].Normal = verts[1].Normal = verts[2].Normal = verts[3].Normal = vertices[0].Normal;
                    verts[0].Hue = verts[1].Hue = verts[2].Hue = verts[3].Hue = vertices[0].Hue;

                    // tile.AddMapObject(deferred);
                }
            }

            // reset the tile position for the upcoming loop.
            tileX = Position.X;
            tileY = Position.Y;

            while (true)
            {
                // We are drawing each subsequent deferred object at (x+1, y+1) relative to the last one.
                tileX += 1;
                tileY += 1;

                // Get the tile associated with this (x, y) position. If the tile is not loaded, don't add a deferred object
                // (it'll be offscreen so it doesn't matter).
                MapTile tile = EntityManager.Model.Map.GetMapTile(tileX, tileY);
                if (tile == null)
                    break;

                // find the vertical and            |
                // horizontal edges that            | <- V
                // we are drawing to.    H -> ------+
                Vector3 verticalEdgeOverlapPt = new Vector3(overlapToHere.X, overlapCurrent.Y - (overlapToHere.X - overlapCurrent.X), 0);
                Vector3 horizEdgeOverlapPt = new Vector3(overlapCurrent.X - (overlapToHere.Y - overlapCurrent.Y), overlapToHere.Y, 0);
                if (horizEdgeOverlapPt.X > overlapToHere.X && verticalEdgeOverlapPt.Y > overlapToHere.Y)
                    break;

                float extendX = overlapToHere.X - horizEdgeOverlapPt.X;
                if (extendX > 44)
                    extendX = 44;
                float extendY = overlapToHere.Y - verticalEdgeOverlapPt.Y;
                if (extendY > 44)
                    extendY = 44;

                deferred = new MapObjectDeferred(m_draw_texture, this);
                deferred.Position.Set(tileX, tileY, Position.Z);
                verts = deferred.Vertices;

                if (m_draw_flip)
                {
                    //       0
                    //      /|
                    //    /  1
                    //  /   /
                    // 2---3
                    verts[0].Position = verticalEdgeOverlapPt;
                    verts[0].TextureCoordinate = new Vector3(0f, (verts[0].Position.Y - vertices[0].Position.Y) / m_draw_texture.Height, 0);
                    verts[1].Position = verticalEdgeOverlapPt + new Vector3(0, extendY, 0);
                    verts[1].TextureCoordinate = new Vector3(0f, (verts[1].Position.Y - vertices[0].Position.Y) / m_draw_texture.Height, 0);
                    verts[2].Position = horizEdgeOverlapPt;
                    verts[2].TextureCoordinate = new Vector3(1f - (verts[2].Position.X - vertices[2].Position.X) / m_draw_texture.Width, 1f, 0);
                    verts[3].Position = horizEdgeOverlapPt + new Vector3(extendX, 0, 0);
                    verts[3].TextureCoordinate = new Vector3(1f - (verts[3].Position.X - vertices[2].Position.X) / m_draw_texture.Width, 1f, 0);
                }
                else
                {
                    //       1
                    //      /|
                    //    /  3
                    //  /   /
                    // 0---2
                    verts[0].Position = horizEdgeOverlapPt;
                    verts[0].TextureCoordinate = new Vector3((verts[0].Position.X - vertices[0].Position.X) / m_draw_texture.Width, 1, 0);
                    verts[1].Position = verticalEdgeOverlapPt;
                    verts[1].TextureCoordinate = new Vector3(1, (verts[1].Position.Y - vertices[1].Position.Y) / m_draw_texture.Height, 0);
                    verts[2].Position = horizEdgeOverlapPt + new Vector3(extendX, 0, 0);
                    verts[2].TextureCoordinate = new Vector3((verts[2].Position.X - vertices[0].Position.X) / m_draw_texture.Width, 1, 0);
                    verts[3].Position = verticalEdgeOverlapPt + new Vector3(0, extendY, 0);
                    verts[3].TextureCoordinate = new Vector3(1, (verts[3].Position.Y - vertices[1].Position.Y) / m_draw_texture.Height, 0);
                }

                verts[0].Normal = verts[1].Normal = verts[2].Normal = verts[3].Normal = vertices[0].Normal;
                verts[0].Hue = verts[1].Hue = verts[2].Hue = verts[3].Hue = vertices[0].Hue;

                overlapCurrent.X += 22;
                overlapCurrent.Y += 22;

                // tile.AddMapObject(deferred);
            }
        }

        public override string ToString()
        {
            return string.Format("   Z:{1} <{0}>", Position, Z);
        }
    }
}