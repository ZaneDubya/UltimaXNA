/***************************************************************************
 *   MiniMap.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using Microsoft.Xna.Framework;
using UltimaXNA.Graphics;
using UltimaXNA.Input;
using UltimaXNA.UltimaGUI.Controls;

namespace UltimaXNA.UltimaGUI.ClientsideGumps
{
    class MiniMap : Gump
    {
        GumpPic _gump;
        bool _useLargeMap = false;

        public MiniMap()
            : base(0, 0)
        {
            _useLargeMap = UltimaVars.EngineVars.MiniMap_LargeFormat;
            IsMovable = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (_gump == null || _useLargeMap != UltimaVars.EngineVars.MiniMap_LargeFormat)
            {
                _useLargeMap = UltimaVars.EngineVars.MiniMap_LargeFormat;
                if (_gump != null)
                    _gump.Dispose();
                _gump = new GumpPic(this, 0, 0, 0, (_useLargeMap ? 5011 : 5010), 0);
                _gump.OnMouseClick = onClickMap;
                _gump.OnMouseDoubleClick = onDoubleClickMap;
                _gump.MakeDragger(this);
                _gump.MakeCloseTarget(this);
                AddControl(_gump);
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        void onClickMap(int x, int y, MouseButton button)
        {

        }

        void onDoubleClickMap(int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                UltimaVars.EngineVars.MiniMap_LargeFormat = Utility.ToggleBoolean(UltimaVars.EngineVars.MiniMap_LargeFormat);
            }
        }
    }
}
