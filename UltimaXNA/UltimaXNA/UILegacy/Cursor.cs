/***************************************************************************
 *   Cursor.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Entities;
using UltimaXNA.Graphics;

namespace UltimaXNA.UILegacy
{
    public class Cursor
    {
        IUIManager _manager = null;

        Item holdingItem;
        Point holdingOffset;
        bool _isHolding = false;
        public bool IsHolding
        {
            get
            {
                return _isHolding;
            }
            set
            {
                _isHolding = value;
            }
        }
        public Item HoldingItem { get { return holdingItem; } }
        public Point HoldingOffset { get { return holdingOffset; } set { holdingOffset = value; } }
        public Texture2D HoldingTexture { get { return Data.Art.GetStaticTexture(holdingItem.DisplayItemID); } }
        
        bool _isTargeting = false;
        public bool IsTargeting
        {
            get
            {
                return _isTargeting;
            }
            set
            {
                // Only change it if we have to...
                if (_isTargeting != value)
                {
                    _isTargeting = value;
                    if (_isTargeting)
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
                        _targetingMulti = -1;
                    }
                }
            }
        }
        int _targetingMulti = -1;
        public int TargetingMulti { set { _targetingMulti = value; } }

        internal bool TrammelHue
        {
            get { return (ClientVars.EngineVars.Map == 1); }
        }

        public Cursor(IUIManager manager)
        {
            _manager = manager;
        }

        public void PickUpItem(Item item, int x, int y)
        {
            if (item.Parent != null)
            {
                if (item.Parent is Container)
                    ((Container)item.Parent).RemoveItem(item.Serial);
            }
            IsHolding = true;
            holdingItem = item;
            holdingOffset = new Point(x, y);
        }

        public void ClearHolding()
        {
            IsHolding = false;
        }

        public void Draw(SpriteBatchUI sb, Point2D position)
        {
            Point2D cursorOffset;
            Rectangle sourceRect = Rectangle.Empty;
            int cursorTextureID = 0;
            int cursorHue = 0;
            Texture2D cursorTexture = null;

            if (IsHolding)
            {
                // Draw the item you're holding first.
                cursorOffset = new Point2D(holdingOffset.X, holdingOffset.Y);
                cursorTexture = HoldingTexture;
                sourceRect = new Rectangle(0, 0, cursorTexture.Width, cursorTexture.Height);
                sb.Draw2D(cursorTexture, position - cursorOffset, sourceRect, holdingItem.Hue, false, false);
                // then set the data for the hang which holds it.
                cursorOffset = new Point2D(1, 1);
                cursorTextureID = 8305;
                cursorTexture = Data.Art.GetStaticTexture(cursorTextureID);
                sourceRect = new Rectangle(1, 1, cursorTexture.Width - 2, cursorTexture.Height - 2);
            }
            else if (IsTargeting)
            {
                cursorOffset = new Point2D(13, 13);
                sourceRect = new Rectangle(1, 1, 46, 34);
                cursorTextureID = 8310;
                if (_targetingMulti != -1)
                {
                    // !!! Draw a multi
                }
            }
            else
            {
                if (ClientVars.EngineVars.InWorld && (!_manager.IsMouseOverUI && !_manager.IsModalMsgBoxOpen))
                {
                    switch (ClientVars.EngineVars.CursorDirection)
                    {
                        case Direction.North:
                            cursorOffset = new Point2D(29, 1);
                            cursorTextureID = 8299;
                            break;
                        case Direction.Right:
                            cursorOffset = new Point2D(41, 9);
                            cursorTextureID = 8300;
                            break;
                        case Direction.East:
                            cursorOffset = new Point2D(36, 24);
                            cursorTextureID = 8301;
                            break;
                        case Direction.Down:
                            cursorOffset = new Point2D(14, 33);
                            cursorTextureID = 8302;
                            break;
                        case Direction.South:
                            cursorOffset = new Point2D(4, 28);
                            cursorTextureID = 8303;
                            break;
                        case Direction.Left:
                            cursorOffset = new Point2D(2, 10);
                            cursorTextureID = 8304;
                            break;
                        case Direction.West:
                            cursorOffset = new Point2D(1, 1);
                            cursorTextureID = 8305;
                            break;
                        case Direction.Up:
                            cursorOffset = new Point2D(4, 2);
                            cursorTextureID = 8298;
                            break;
                        default:
                            cursorOffset = new Point2D(2, 10);
                            cursorTextureID = 8309;
                            break;
                    }
                }
                else
                {
                    // Over the interface or not in world. Display a default cursor.
                    cursorOffset = new Point2D(1, 1);
                    cursorTextureID = 8305;
                }
            }

            // Hue the cursor if in warmode.
            if (ClientVars.EngineVars.WarMode)
                cursorTextureID -= 23;
            else if (TrammelHue)
                cursorHue = 2412;

            cursorTexture = Data.Art.GetStaticTexture(cursorTextureID);
            sourceRect = new Rectangle(1, 1, cursorTexture.Width - 2, cursorTexture.Height - 2);

            sb.Draw2D(cursorTexture, position - cursorOffset, sourceRect, cursorHue, false, false);
        }
    }
}
