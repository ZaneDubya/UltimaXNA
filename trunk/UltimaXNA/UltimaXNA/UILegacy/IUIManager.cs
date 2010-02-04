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
        Control MouseOverControl { get; }
        void AddMessage_Debug(string line);
        void AddMessage_Chat(string line);
        void AddMessage_Chat(string text, int hue, int font);
        Gump AddGump_Server(Serial serial, Serial gumpID, string[] gumplings, string[] lines, int x, int y);
        Gump AddGump_Local(Gump gump, int x, int y);
        Gump AddContainerGump(Entity containerItem, int gumpID);
        void MsgBox(string msg);
        Gump GetGump(Serial serial);
        T GetGump<T>(Serial serial) where T : Gump;
        void Reset();
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}
