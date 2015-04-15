/***************************************************************************
 *   MiniMap.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.Controls;
using Microsoft.Xna.Framework;
using UltimaXNA.Ultima.Entities;
using UltimaXNA.Ultima.World;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace UltimaXNA.Ultima.World.Gumps
{
    class MiniMap : Gump
    {
        bool m_useLargeMap = false;
        WorldModel m_World;
        Texture2D m_GumpTexture;

        public MiniMap()
            : base(0, 0)
        {
            m_World = UltimaServices.GetService<WorldModel>();

            m_useLargeMap = EngineVars.MiniMap_LargeFormat;

            IsMovable = true;
            MakeCloseTarget(this);
            MakeDragger(this);
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_GumpTexture == null || m_useLargeMap != EngineVars.MiniMap_LargeFormat)
            {
                m_useLargeMap = EngineVars.MiniMap_LargeFormat;
                if (m_GumpTexture != null)
                {
                    m_GumpTexture = null;
                }
                m_GumpTexture = IO.GumpData.GetGumpXNA((m_useLargeMap ? 5011 : 5010));
                Size = new Point(m_GumpTexture.Width, m_GumpTexture.Height);
            }

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            AEntity player = EntityManager.GetPlayerObject();
            float x = (player.Position.X % 256) / 256f;
            float y = (player.Position.Y % 256) / 256f;
            Vector3 playerPosition = new Vector3(x - y, y + x, 0f);

            VertexPositionNormalTextureHue[] v = new VertexPositionNormalTextureHue[4]
            {
                new VertexPositionNormalTextureHue(new Vector3(X, Y, 0), playerPosition + new Vector3(-.5f, -.5f, 0), new Vector3(0, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(X + Size.X, Y, 0), playerPosition + new Vector3(.5f, -.5f, 0), new Vector3(1, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(X, Y + Size.Y, 0), playerPosition + new Vector3(-.5f, .5f, 0), new Vector3(0, 1, 0)),
                new VertexPositionNormalTextureHue(new Vector3(X + Size.X, Y + Size.Y, 0), playerPosition + new Vector3(.5f, .5f, 0), new Vector3(1, 1, 0))
            };

            spriteBatch.Draw(m_GumpTexture, v);
        }

        protected override void mouseDoubleClick(int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                EngineVars.MiniMap_LargeFormat = !EngineVars.MiniMap_LargeFormat;
            }
        }
    }
}
