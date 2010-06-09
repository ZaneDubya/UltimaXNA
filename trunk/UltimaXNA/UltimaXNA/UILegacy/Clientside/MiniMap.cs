using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Input;
using UltimaXNA.UILegacy.Gumplings;

namespace UltimaXNA.UILegacy.Clientside
{
    class MiniMap : Gump
    {
        GumpPic _gump;
        bool _useLargeMap = false;

        public MiniMap()
            : base(0, 0)
        {
            _useLargeMap = ClientVars.MiniMap_LargeFormat;
            IsMovable = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (_gump == null || _useLargeMap != ClientVars.MiniMap_LargeFormat)
            {
                _useLargeMap = ClientVars.MiniMap_LargeFormat;
                if (_gump != null)
                    _gump.Dispose();
                _gump = new GumpPic(this, 0, 0, 0, (_useLargeMap ? 5011 : 5010), 0);
                _gump.HandlesMouseInput = true;
                _gump.OnMouseClick = onClickMap;
                _gump.OnMouseDoubleClick = onDoubleClickMap;
                _gump.MakeADragger(this);
                AddGumpling(_gump);
            }

            base.Update(gameTime);
        }

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        void onClickMap(int x, int y, MouseButtons button)
        {
            if (button == MouseButtons.RightButton)
            {
                Dispose();
            }
        }

        void onDoubleClickMap(int x, int y, MouseButtons button)
        {
            if (button == MouseButtons.LeftButton)
            {
                ClientVars.MiniMap_LargeFormat = Utility.ToggleBoolean(ClientVars.MiniMap_LargeFormat);
            }
        }
    }
}
