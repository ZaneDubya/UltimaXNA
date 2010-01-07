using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Entities;

namespace UltimaXNA.UILegacy
{
    public interface IUIManager
    {
        bool IsMouseOverUI { get; }
        Cursor Cursor { get; }
        Gump AddGump(Serial serial, Serial gumpID, string[] gumplings, string[] lines, int x, int y);
        Gump AddGump(Gump gump, int x, int y);
        Gump AddContainerGump(Entity containerItem, int gumpID);
        Gump GetGump(Serial serial);
        T GetGump<T>(Serial serial) where T : Gump;
        void Reset();
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}
