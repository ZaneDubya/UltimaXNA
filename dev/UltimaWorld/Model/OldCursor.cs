/***************************************************************************
 *   Cursor.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Entity;
using UltimaXNA.Rendering;

namespace UltimaXNA.UltimaGUI
{
    public class OldCursor
    {
        Item holdingItem;
        Point holdingOffset;
        bool m_isHolding = false;
        public bool IsHolding
        {
            get
            {
                return m_isHolding;
            }
            set
            {
                m_isHolding = value;
            }
        }
        public Item HoldingItem { get { return holdingItem; } }
        public Point HoldingOffset { get { return holdingOffset; } set { holdingOffset = value; } }
        public Texture2D HoldingTexture { get { return UltimaData.ArtData.GetStaticTexture(holdingItem.DisplayItemID); } }
        
        bool m_isTargeting = false;
        public bool IsTargeting
        {
            get
            {
                return m_isTargeting;
            }
            set
            {
                // Only change it if we have to...
                if (m_isTargeting != value)
                {
                    m_isTargeting = value;
                    if (m_isTargeting)
                    {
                        // If we're carrying something in the mouse cursor...
                        if (IsHolding)
                        {
                            // drop it!
                            // UIHelper.MouseHoldingItem = null;
                            IsHolding = false;
                        }
                    }
                    else
                    {
                        m_targetingMulti = -1;
                    }
                }
            }
        }
        int m_targetingMulti = -1;
        public int TargetingMulti { set { m_targetingMulti = value; } }

        internal bool TrammelHue
        {
            get { return (UltimaVars.EngineVars.Map == 1); }
        }

        public OldCursor()
        {

        }

        /*public void PickUpItem(Item item, int x, int y)
        {
            if (item.Parent != null)
            {
                if (item.Parent is Container)
                    ((Container)item.Parent).RemoveItem(item.Serial);
            }
            IsHolding = true;
            holdingItem = item;
            holdingOffset = new Point(x, y);
        }*/

        /*public void ClearHolding()
        {
            IsHolding = false;
        }*/

        public void Draw(Point position)
        {
            Point cursorOffset;
            Rectangle sourceRect = Rectangle.Empty;
            int cursorTextureID = 0;
            int cursorHue = 0;
            Texture2D cursorTexture = null;
            // UltimaEngine.UserInterface.BeneathCursorSprite = null;

            if (IsHolding)
            {
                // Draw the item you're holding first.
                cursorOffset = new Point(holdingOffset.X, holdingOffset.Y);
                cursorTexture = HoldingTexture;
                sourceRect = new Rectangle(0, 0, cursorTexture.Width, cursorTexture.Height);
                // UltimaEngine.UserInterface.BeneathCursorSprite = new UltimaGUI.Sprite(cursorTexture, cursorOffset, sourceRect, holdingItem.Hue);
                // sb.Draw2D(cursorTexture, position - cursorOffset, sourceRect, holdingItem.Hue, false, false);
                // then set the data for the hang which holds it.
                cursorOffset = new Point(1, 1);
                cursorTextureID = 8305;
            }
            else if (IsTargeting)
            {
                cursorOffset = new Point(13, 13);
                sourceRect = new Rectangle(1, 1, 46, 34);
                cursorTextureID = 8310;
                if (m_targetingMulti != -1)
                {
                    // !!! Draw a multi
                }
            }
            else
            {
                if (UltimaVars.EngineVars.InWorld && (!UltimaEngine.UserInterface.IsMouseOverUI && !UltimaEngine.UserInterface.IsModalControlOpen))
                {
                    switch (UltimaVars.EngineVars.CursorDirection)
                    {
                        case Direction.North:
                            cursorOffset = new Point(29, 1);
                            cursorTextureID = 8299;
                            break;
                        case Direction.Right:
                            cursorOffset = new Point(41, 9);
                            cursorTextureID = 8300;
                            break;
                        case Direction.East:
                            cursorOffset = new Point(36, 24);
                            cursorTextureID = 8301;
                            break;
                        case Direction.Down:
                            cursorOffset = new Point(14, 33);
                            cursorTextureID = 8302;
                            break;
                        case Direction.South:
                            cursorOffset = new Point(4, 28);
                            cursorTextureID = 8303;
                            break;
                        case Direction.Left:
                            cursorOffset = new Point(2, 10);
                            cursorTextureID = 8304;
                            break;
                        case Direction.West:
                            cursorOffset = new Point(1, 1);
                            cursorTextureID = 8305;
                            break;
                        case Direction.Up:
                            cursorOffset = new Point(4, 2);
                            cursorTextureID = 8298;
                            break;
                        default:
                            cursorOffset = new Point(2, 10);
                            cursorTextureID = 8309;
                            break;
                    }
                }
                else
                {
                    // Over the interface or not in world. Display a default cursor.
                    cursorOffset = new Point(1, 1);
                    cursorTextureID = 8305;
                }
            }

            // Hue the cursor if in warmode.
            if (UltimaVars.EngineVars.WarMode)
                cursorTextureID -= 23;
            else if (TrammelHue)
                cursorHue = 2412;

            cursorTexture = UltimaData.ArtData.GetStaticTexture(cursorTextureID);
            sourceRect = new Rectangle(1, 1, cursorTexture.Width - 2, cursorTexture.Height - 2);

            // UltimaEngine.UserInterface.CursorSprite = new UltimaGUI.Sprite(cursorTexture, cursorOffset, sourceRect, cursorHue);
            // sb.Draw2D(cursorTexture, position - cursorOffset, sourceRect, cursorHue, false, false);
        }
    }
}
