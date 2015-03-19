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
using UltimaXNA.Rendering;
using InterXLib.Input.Windows;
using UltimaXNA.UltimaGUI.Controls;

namespace UltimaXNA.UltimaGUI.WorldGumps
{
    class MiniMap : Gump
    {
        GumpPic m_gump;
        bool m_useLargeMap = false;

        public MiniMap()
            : base(0, 0)
        {
            m_useLargeMap = UltimaVars.EngineVars.MiniMap_LargeFormat;
            IsMovable = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (m_gump == null || m_useLargeMap != UltimaVars.EngineVars.MiniMap_LargeFormat)
            {
                m_useLargeMap = UltimaVars.EngineVars.MiniMap_LargeFormat;
                if (m_gump != null)
                    m_gump.Dispose();
                m_gump = new GumpPic(this, 0, 0, 0, (m_useLargeMap ? 5011 : 5010), 0);
                m_gump.OnMouseClick = onClickMap;
                m_gump.OnMouseDoubleClick = onDoubleClickMap;
                m_gump.MakeDragger(this);
                m_gump.MakeCloseTarget(this);
                AddControl(m_gump);
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
