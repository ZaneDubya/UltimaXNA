/***************************************************************************
 *   OverheadsView.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.World.EntityViews;
using UltimaXNA.Ultima.World.Input;
using UltimaXNA.Ultima.World.Maps;

namespace UltimaXNA.Ultima.World.WorldViews
{
    static class OverheadsView
    {
        private static List<ViewWithDrawInfo> m_Views = new List<ViewWithDrawInfo>();

        public static void AddView(AEntityView view, Vector3 drawPosition)
        {
            m_Views.Add(new ViewWithDrawInfo(view, drawPosition));
        }

        public static void Render(SpriteBatch3D spriteBatch, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            if (m_Views.Count > 0)
            {
                for (int i = 0; i < m_Views.Count; i++)
                {
                    m_Views[i].View.Draw(spriteBatch, m_Views[i].DrawPosition, mouseOver, map, roofHideFlag);
                }

                m_Views.Clear();
            }
        }

        private struct ViewWithDrawInfo
        {
            public readonly AEntityView View;
            public readonly Vector3 DrawPosition;

            public ViewWithDrawInfo(AEntityView view, Vector3 drawPosition)
            {
                View = view;
                DrawPosition = drawPosition;
            }
        }
    }
}
