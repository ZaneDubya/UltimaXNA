using UltimaXNA;
using UltimaXNA.Core.Patterns;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima;
using UltimaXNA.Ultima.UI.LoginGumps;
using UltimaXNA.Ultima.UI.WorldGumps;
using UltimaXNA.Ultima.World.Entities.Items;

namespace ExamplePlugin {
    class DebugShowGumpModule : IModule {
        public string Name => "DebugBookTest";

        public void Load() {
            LoginGump.AddButton("Debug:Book", OnClickDebugGump);
        }

        public void Unload() {
            LoginGump.RemoveButton(OnClickDebugGump);
        }

        void OnClickDebugGump() {
            BaseBook book = new BaseBook(Serial.NewDynamicSerial, null);
            book.ItemID = 0xFEF;
            ServiceRegistry.GetService<UserInterfaceService>().AddControl(new BookGump(book), 10, 10);
        }
    }
}
