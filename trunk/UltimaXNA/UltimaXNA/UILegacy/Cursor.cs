using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UltimaXNA.Extensions;
using UltimaXNA.Input;
using UltimaXNA.Entities;

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
        public Point HoldingOffset { get { return holdingOffset; } }
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
            get { return (ClientVars.Map == 1); }
        }

        public Cursor(IUIManager manager)
        {
            _manager = manager;
        }

        public void PickUpItem(Item item, int x, int y)
        {
            IsHolding = true;
            holdingItem = item;
            holdingOffset = new Point(x, y);
        }

        public void ClearHolding()
        {
            IsHolding = false;
        }

        public void Draw(ExtendedSpriteBatch sb, Vector2 position)
        {
            Vector2 cursorOffset = Vector2.Zero;
            Rectangle sourceRect = Rectangle.Empty;
            int cursorTextureID = 0;
            int cursorHue = 0;
            Texture2D cursorTexture = null;

            if (IsHolding)
            {
                // Draw the item you're holding first.
                cursorOffset = new Vector2(holdingOffset.X, holdingOffset.Y);
                cursorTexture = HoldingTexture;
                cursorHue = holdingItem.Hue;
                sourceRect = new Rectangle(0, 0, cursorTexture.Width, cursorTexture.Height);
                sb.Draw(cursorTexture, position - cursorOffset, sourceRect, cursorHue, false);
                // then set the data for the hang which holds it.
                cursorOffset = new Vector2(1, 1);
                cursorTextureID = 8305;
                cursorTexture = Data.Art.GetStaticTexture(cursorTextureID);
                sourceRect = new Rectangle(1, 1, cursorTexture.Width - 2, cursorTexture.Height - 2);
            }
            else if (IsTargeting)
            {
                cursorOffset = new Vector2(13, 13);
                sourceRect = new Rectangle(1, 1, 46, 34);
                cursorTexture = Data.Art.GetStaticTexture(8310);
                if (_targetingMulti != -1)
                {
                    // !!! Draw a multi
                }
            }
            else
            {
                if (ClientVars.InWorld && !_manager.IsMouseOverUI)
                {
                    switch (ClientVars.CursorDirection)
                    {
                        case Direction.North:
                            cursorOffset = new Vector2(29, 1);
                            cursorTextureID = 8299;
                            break;
                        case Direction.Right:
                            cursorOffset = new Vector2(41, 9);
                            cursorTextureID = 8300;
                            break;
                        case Direction.East:
                            cursorOffset = new Vector2(36, 24);
                            cursorTextureID = 8301;
                            break;
                        case Direction.Down:
                            cursorOffset = new Vector2(14, 33);
                            cursorTextureID = 8302;
                            break;
                        case Direction.South:
                            cursorOffset = new Vector2(4, 28);
                            cursorTextureID = 8303;
                            break;
                        case Direction.Left:
                            cursorOffset = new Vector2(2, 10);
                            cursorTextureID = 8304;
                            break;
                        case Direction.West:
                            cursorOffset = new Vector2(1, 1);
                            cursorTextureID = 8305;
                            break;
                        case Direction.Up:
                            cursorOffset = new Vector2(4, 2);
                            cursorTextureID = 8298;
                            break;
                        default:
                            cursorOffset = new Vector2(2, 10);
                            cursorTextureID = 8309;
                            break;
                    }

                    // Hue the cursor if in warmode.
                    if (ClientVars.WarMode)
                        cursorTextureID -= 23;
                    else if (TrammelHue)
                        cursorHue = 2412;
                }
                else
                {
                    // Over the interface or not in world. Display a default cursor.
                    cursorOffset = new Vector2(1, 1);
                    cursorTextureID = 8305;
                }
                cursorTexture = Data.Art.GetStaticTexture(cursorTextureID);
                sourceRect = new Rectangle(1, 1, cursorTexture.Width - 2, cursorTexture.Height - 2);
            }
            sb.Draw(cursorTexture, position - cursorOffset, sourceRect, cursorHue, false);
        }
    }
}
