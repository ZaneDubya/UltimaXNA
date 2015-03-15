using Microsoft.Xna.Framework;
using UltimaXNA.Rendering;
using UltimaXNA.UltimaWorld;

namespace UltimaXNA.Entity.EntityViews
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

        }

        private int m_DisplayItemID = -1;

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOverList, PickTypes pickType, int maxAlt)
        {
            if (Entity.NoDraw)
                return false;

            if (Entity.DisplayItemID != m_DisplayItemID)
            {
                m_DisplayItemID = Entity.DisplayItemID;
                DrawTexture = UltimaData.ArtData.GetStaticTexture(m_DisplayItemID);
                DrawArea = new Rectangle(DrawTexture.Width / 2 - 22, DrawTexture.Height - 44 + (Entity.Z * 4), DrawTexture.Width, DrawTexture.Height);
                PickType = PickTypes.PickObjects;
                DrawFlip = false;
                HueVector = Utility.GetHueVector(Entity.Hue);
            }

            return base.Draw(spriteBatch, drawPosition, mouseOverList, pickType, maxAlt);
        }
    }
}
