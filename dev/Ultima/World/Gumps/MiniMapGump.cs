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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Ultima.Entities;
using UltimaXNA.Ultima.UI;
#endregion

namespace UltimaXNA.Ultima.World.Gumps
{
    class MiniMapGump : Gump
    {
        bool m_useLargeMap = false;
        WorldModel m_World;
        Texture2D m_GumpTexture;
        Texture2D m_PlayerIndicator;

        public MiniMapGump()
            : base(0, 0)
        {
            m_World = UltimaServices.GetService<WorldModel>();

            m_useLargeMap = EngineVars.MiniMap_LargeFormat;

            IsMovable = true;
            MakeThisADragger();
        }

        protected override void OnInitialize()
        {
            SetSavePositionName("minimap");
            base.OnInitialize();
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
                m_GumpTexture = IO.GumpData.GetGumpXNA((m_useLargeMap ? 5011 : 5010), true);
                Size = new Point(m_GumpTexture.Width, m_GumpTexture.Height);
            }

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            AEntity player = EntityManager.GetPlayerObject();
            float x = (float)Math.Round((player.Position.X % 256) + player.Position.X_offset) / 256f;
            float y = (float)Math.Round((player.Position.Y % 256) + player.Position.Y_offset) / 256f;
            Vector3 playerPosition = new Vector3(x - y, x + y, 0f);
            float minimapU = (m_GumpTexture.Width / 256f) / 2f;
            float minimapV = (m_GumpTexture.Height / 256f) / 2f;

            VertexPositionNormalTextureHue[] v = new VertexPositionNormalTextureHue[4]
            {
                new VertexPositionNormalTextureHue(new Vector3(position.X, position.Y, 0), playerPosition + new Vector3(-minimapU, -minimapV, 0), new Vector3(0, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(position.X + Width, position.Y, 0), playerPosition + new Vector3(minimapU, -minimapV, 0), new Vector3(1, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(position.X, position.Y + Height, 0), playerPosition + new Vector3(-minimapU, minimapV, 0), new Vector3(0, 1, 0)),
                new VertexPositionNormalTextureHue(new Vector3(position.X + Width, position.Y + Height, 0), playerPosition + new Vector3(minimapU, minimapV, 0), new Vector3(1, 1, 0))
            };

            spriteBatch.Draw(m_GumpTexture, v, Techniques.MiniMap);

            if (UltimaEngine.TotalMS % 500f < 250f)
            {
                if (m_PlayerIndicator == null)
                {
                    m_PlayerIndicator = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                    m_PlayerIndicator.SetData<uint>(new uint[1] { 0xFFFFFFFF });
                }
                spriteBatch.Draw2D(m_PlayerIndicator, new Vector3(position.X + Width / 2, position.Y + Height / 2 - 8, 0), Vector3.Zero);
            }
        }

        protected override void OnMouseDoubleClick(int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                EngineVars.MiniMap_LargeFormat = !EngineVars.MiniMap_LargeFormat;
            }
        }
    }
}
