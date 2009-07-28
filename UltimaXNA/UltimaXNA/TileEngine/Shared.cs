/***************************************************************************
 *   Shared.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from ClintXNA's renderer: http://www.runuo.com/forums/xna/92023-hi.html
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Entities;
using UltimaXNA.Input;
#endregion

namespace UltimaXNA.TileEngine
{
    [Flags]
    public enum PickTypes : int
    {
        PickNothing = 0,
        PickObjects = 1,
        PickStatics = 2,
        PickGroundTiles = 4
    }

    class MouseOverList
    {
        List<MouseOverItem> m_List;

        public MouseOverList()
        {
            m_List = new List<MouseOverItem>();
        }

        public MapObject GetForemostMouseOverItem(Vector2 nMousePosition)
        {
            // Parse list backwards to find topmost mouse over object.
            foreach (MouseOverItem iItem in CreateReverseIterator(m_List))
            {
                UInt16[] iPixel = new UInt16[1];
                iItem.Texture.GetData<UInt16>(0,
                    new Rectangle((int)nMousePosition.X - (int)iItem.Position.X, (int)nMousePosition.Y - (int)iItem.Position.Y, 1, 1),
                    iPixel, 0, 1);
                if (iPixel[0] != 0)
                    return iItem.Object;
            }
            return null;
        }

        static IEnumerable<MouseOverItem> CreateReverseIterator<MouseOverItem>(IList<MouseOverItem> list)
        {
            int count = list.Count;
            for (int i = count - 1; i >= 0; --i)
            {
                yield return list[i];
            }
        }

        public void AddItem(MouseOverItem nItem)
        {
            m_List.Add(nItem);
        }
    }

    class MouseOverItem
    {
        public Texture2D Texture;
        public Vector3 Position;
        public MapObject Object;

        public MouseOverItem(Texture2D nTexture, Vector3 nPosition, MapObject nObject)
        {
            Texture = nTexture;
            Position = nPosition;
            Object = nObject;
        }
    }
}
