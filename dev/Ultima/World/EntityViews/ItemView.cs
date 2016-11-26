using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Ultima.World.Input;
using UltimaXNA.Ultima.World.Maps;
using UltimaXNA.Ultima.World.WorldViews;

namespace UltimaXNA.Ultima.World.EntityViews
{
    class ItemView : AEntityView
    {
        new Item Entity
        {
            get { return (Item)base.Entity; }
        }

        int m_DisplayItemID = -1;

        public ItemView(Item item)
            : base(item)
        {
            if (Entity.ItemData.IsWet)
            {
                SortZ += 1;
            }
        }

        protected override void Pick(MouseOverList mouseOver, VertexPositionNormalTextureHue[] vertexBuffer)
        {
            int x = mouseOver.MousePosition.X - (int)vertexBuffer[0].Position.X;
            int y = mouseOver.MousePosition.Y - (int)vertexBuffer[0].Position.Y;
            if (Provider.IsPointInItemTexture(m_DisplayItemID, x, y, 1))
            {
                mouseOver.AddItem(Entity, vertexBuffer[0].Position);
            }
        }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            if (Entity.NoDraw)
            {
                return false;
            }
            // Update Display texture, if necessary.
            if (Entity.DisplayItemID != m_DisplayItemID)
            {
                m_DisplayItemID = Entity.DisplayItemID;
                DrawTexture = Provider.GetItemTexture(m_DisplayItemID);
                if (DrawTexture == null) // ' no draw ' item.
                {
                    return false;
                }
                DrawArea = new Rectangle(DrawTexture.Width / 2 - IsometricRenderer.TILE_SIZE_INTEGER_HALF, DrawTexture.Height - IsometricRenderer.TILE_SIZE_INTEGER + (Entity.Z * 4), DrawTexture.Width, DrawTexture.Height);
                PickType = PickType.PickObjects;
                DrawFlip = false;
            }
            if (DrawTexture == null) // ' no draw ' item.
            {
                return false;
            }
            DrawArea.Y = DrawTexture.Height - IsometricRenderer.TILE_SIZE_INTEGER + (Entity.Z * 4);
            HueVector = Utility.GetHueVector(Entity.Hue, Entity.ItemData.IsPartialHue, false, false);
            if (Entity.Amount > 1 && Entity.ItemData.IsGeneric && Entity.DisplayItemID == Entity.ItemID)
            {
                int offset = Entity.ItemData.Unknown4;
                Vector3 offsetDrawPosition = new Vector3(drawPosition.X - 5, drawPosition.Y - 5, 0);
                base.Draw(spriteBatch, offsetDrawPosition, mouseOver, map, roofHideFlag);
            }
            bool drawn = base.Draw(spriteBatch, drawPosition, mouseOver, map, roofHideFlag);
            DrawOverheads(spriteBatch, drawPosition, mouseOver, map, DrawArea.Y - IsometricRenderer.TILE_SIZE_INTEGER_HALF);
            return drawn;
        }
    }
}
