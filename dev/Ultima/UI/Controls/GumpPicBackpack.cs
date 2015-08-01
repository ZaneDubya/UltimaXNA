using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.World.Entities.Items;

namespace UltimaXNA.Ultima.UI.Controls
{
    class GumpPicBackpack : GumpPic
    {
        public Item BackpackItem
        {
            get;
            protected set;
        }

        public GumpPicBackpack(AControl parent, int x, int y, Item backpack)
            : base(parent, x, y, 0xC4F6, 0)
        {
            BackpackItem = backpack;
        }
    }
}
