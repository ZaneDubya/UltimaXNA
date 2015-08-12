using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Ultima.World.Input;
using UltimaXNA.Ultima.World.Maps;
using UltimaXNA.Ultima.World.WorldViews;
using UltimaXNA.Core.Resources;

namespace UltimaXNA.Ultima.World.EntityViews
{
    class ItemView : AEntityView
    {
        new Item Entity
        {
            get { return (Item)base.Entity; }
        }

        public ItemView(Item item)
            : base(item)
        {
            if (Entity.ItemData.IsWet)
                SortZ += 1;
        }

        private int m_DisplayItemID = -1;

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOverList, Map map)
        {
            if (Entity.NoDraw)
                return false;

            // Update Display texture, if necessary.
            if (Entity.DisplayItemID != m_DisplayItemID)
            {
                m_DisplayItemID = Entity.DisplayItemID;
                IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
                DrawTexture = provider.GetItemTexture(m_DisplayItemID);

                if(DrawTexture == null)
                {
                    return false;
                }

                DrawArea = new Rectangle(DrawTexture.Width / 2 - 22, DrawTexture.Height - IsometricRenderer.TILE_SIZE_INTEGER + (Entity.Z * 4), DrawTexture.Width, DrawTexture.Height);
                PickType = PickType.PickObjects;
                DrawFlip = false;
            }

            // Update hue vector.
            HueVector = Utility.GetHueVector(Entity.Hue, Entity.ItemData.IsPartialHue, false);

            if (Entity.Amount > 1 && Entity.ItemData.IsGeneric && Entity.DisplayItemID == Entity.ItemID)
            {
                int offset = Entity.ItemData.Unknown4;
                Vector3 offsetDrawPosition = new Vector3(drawPosition.X - 5, drawPosition.Y - 5, 0);
                base.Draw(spriteBatch, offsetDrawPosition, mouseOverList, map);
            }
            bool drawn = base.Draw(spriteBatch, drawPosition, mouseOverList, map);


            DrawOverheads(spriteBatch, drawPosition, mouseOverList, map, (int)DrawArea.Y - 22);

            return drawn;
        }
    }
}
