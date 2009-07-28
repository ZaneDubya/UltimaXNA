/***************************************************************************
 *   IScene.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Client;
using UltimaXNA.Diagnostics;
using UltimaXNA.Input;
#endregion

namespace UltimaXNA.SceneManagement
{
    public delegate void TransitionCompleteHandler();

    public interface IScene : IDisposable
    {
        SceneState SceneState { get; set; }
        TimeSpan TransitionOffLength { get; }
        TimeSpan TransitionOnLength { get; }
        bool IsInitialized { get; set; }

        ILoggingService Log { get; }

        event TransitionCompleteHandler TransitionComplete;

        void Intitialize();
        void Update(GameTime gameTime);
        void OnAfterDraw(GameTime gameTime);
        void Draw(GameTime gameTime);
        void OnBeforeDraw(GameTime gameTime);
    }
}
