using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Diagnostics;
using UltimaXNA.Input;

namespace UltimaXNA.Graphics.UI
{
    public interface IUIService : IDrawable, IUpdateable, IGameComponent, IDisposable
    {
        T CreateInstance<T>(string name) where T : UINode;
        T CreateInstance<T>(string name, Serial? serial) where T : UINode;

        T CreateXml<T>(string xml, Serial? serial) where T : UINode;
    }
}
