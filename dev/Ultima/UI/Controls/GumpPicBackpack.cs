using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Core.UI;

namespace UltimaXNA.Ultima.UI.Controls
{
    class GumpPicBackpack : GumpPic
    {
        public Item BackpackItem
        {
            get;
            protected set;
        }

        public GumpPicBackpack(AControl owner, int x, int y, Item backpack)
            : base(owner, x, y, 0xC4F6, 0)
        {
            BackpackItem = backpack;
        }
    }
}
