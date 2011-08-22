using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Input;
using UltimaXNA.UILegacy.Gumplings;
using UltimaXNA.Graphics;

namespace UltimaXNA.UILegacy.ClientsideGumps
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
                ClientVars.MiniMap_LargeFormat = Utility.ToggleBoolean(ClientVars.MiniMap_LargeFormat);
            }
        }
    }
}
