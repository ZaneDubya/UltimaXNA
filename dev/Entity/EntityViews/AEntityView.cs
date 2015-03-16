using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Entity;
using UltimaXNA.Rendering;
using UltimaXNA.UltimaWorld.View;
using UltimaXNA.UltimaWorld;

namespace UltimaXNA.Entity.EntityViews
{
    /// <summary>
    /// An abstract class that can be attached to an entity and used to maintain data for a 'View'.
    /// </summary>
    public abstract class AEntityView
    {
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

        public PickTypes PickType = PickTypes.PickNothing;

        // ======================================================================
        // Sort routines
        // ======================================================================

        public int SortZ = 0;

        // ======================================================================
        // Draw methods and properties
        // ======================================================================

        protected bool DrawFlip = false;
        protected Rectangle DrawArea = Rectangle.Empty;
        protected Texture2D DrawTexture = null;

        public virtual bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOverList, PickTypes pickType, int maxAlt)
        {
            VertexPositionNormalTextureHue[] vertexBuffer;

            if (Entity.Z >= maxAlt)
                return false;

            if (DrawFlip)
            {
                // 2   0    
                // |\  |     
                // |  \|     
                // 3   1
                vertexBuffer = VertexPositionNormalTextureHue.PolyBufferFlipped;
                vertexBuffer[0].Position = drawPosition;
                vertexBuffer[0].Position.X += DrawArea.X + 44;
                vertexBuffer[0].Position.Y -= DrawArea.Y;

                vertexBuffer[1].Position = vertexBuffer[0].Position;
                vertexBuffer[1].Position.Y += DrawArea.Height;

                vertexBuffer[2].Position = vertexBuffer[0].Position;
                vertexBuffer[2].Position.X -= DrawArea.Width;

                vertexBuffer[3].Position = vertexBuffer[1].Position;
                vertexBuffer[3].Position.X -= DrawArea.Width;
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

                vertexBuffer[1].Position = vertexBuffer[0].Position;
                vertexBuffer[1].Position.X += DrawArea.Width;

                vertexBuffer[2].Position = vertexBuffer[0].Position;
                vertexBuffer[2].Position.Y += DrawArea.Height;

                vertexBuffer[3].Position = vertexBuffer[1].Position;
                vertexBuffer[3].Position.Y += DrawArea.Height;
            }

            if (vertexBuffer[0].Hue != HueVector)
                vertexBuffer[0].Hue = vertexBuffer[1].Hue = vertexBuffer[2].Hue = vertexBuffer[3].Hue = HueVector;

            if (!spriteBatch.Draw(DrawTexture, vertexBuffer))
            {
                return false;
            }

            // draw overlap ... !!! ??? with deferred objects, perhaps.

            Pick(mouseOverList, pickType, vertexBuffer);

            return true;
        }

        protected void Pick(MouseOverList mouseOverList, PickTypes pickType, VertexPositionNormalTextureHue[] vertexBuffer)
        {
            if ((pickType & PickType) == PickType)
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
                    }
                    mouseOverList.Add2DItem(item);
                }
            }
        }

        protected Vector2 HueVector = Vector2.Zero;
    }
}
