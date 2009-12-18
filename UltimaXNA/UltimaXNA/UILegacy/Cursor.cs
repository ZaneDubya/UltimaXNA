using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UltimaXNA.Extensions;
using UltimaXNA.Graphics;
using UltimaXNA.Input;

namespace UltimaXNA.UILegacy
{
    class Cursor
    {
        IUIManager _manager = null;

        bool _isHolding = false;
        bool _isTargetting = false;

        public Cursor(IUIManager manager)
        {
            _manager = manager;
        }

        public void Draw(ExtendedSpriteBatch spriteBatch, Vector2 position)
        {
            Vector2 cursorOffset = Vector2.Zero;
            Rectangle sourceRect = Rectangle.Empty;
            int cursorTextureID = 0;
            Texture2D cursorTexture = null;

            if (_isHolding)
            {
                // holding something.
                cursorOffset = new Vector2(0, 0);
                sourceRect = new Rectangle(0, 0, 64, 64);
                // this.Texture = UIHelper.ItemIcon(((Item)UIHelper.MouseHoldingItem));
            }
            else if (_isTargetting)
            {
                cursorOffset = new Vector2(13, 13);
                sourceRect = new Rectangle(1, 1, 46, 34);
                cursorTexture = Data.Art.GetStaticTexture(8310);
            }
            else
            {
                if (GameState.InWorld && !_manager.IsMouseOverUI)
                {
                    switch (GameState.CursorDirection)
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
                    if (GameState.WarMode)
                        cursorTextureID -= 23;
                }
                else
                {
                    // not in world. Display a default cursor.
                    cursorOffset = new Vector2(1, 1);
                    cursorTextureID = 8305;
                }
                cursorTexture = Data.Art.GetStaticTexture(cursorTextureID);
                sourceRect = new Rectangle(1, 1, cursorTexture.Width - 2, cursorTexture.Height - 2);
            }
            spriteBatch.Draw(cursorTexture, position - cursorOffset, sourceRect, Color.White);
        }
    }
}
