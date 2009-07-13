using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.TileEngine
{
    public class MiniMap
    {
        public Texture2D Texture;
        private Map _map;
        private GraphicsDevice _graphics;
        private int _lastUpdateTicker;

        public MiniMap(GraphicsDevice graphics, Map map)
        {
            _map = map;
            _graphics = graphics;
        }

        public unsafe void Update()
        {
            if ((_map.UpdateTicker != _lastUpdateTicker) || (Texture == null))
            {
                _lastUpdateTicker = _map.UpdateTicker;
                Texture = new Texture2D(_graphics, _map.GameSize, _map.GameSize, 1, TextureUsage.None, SurfaceFormat.Bgra5551);
                ushort[] data = new ushort[_map.GameSize * _map.GameSize];
                fixed (ushort* pData = data)
                {
                    ushort* cur = pData;
                    for (int x = 0; x < _map.GameSize; x++)
                    {
                        for (int y = 0; y < _map.GameSize; y++)
                        {
                            MapCell m = _map.GetMapCell(TileEngine.startX + x, TileEngine.startY + y);
                            int i;
                            for (i = m.Objects.Count - 1; i > 0; i--)
                            {
                                if (m.Objects[i].Type == MapObjectTypes.StaticTile)
                                {
                                    *cur++ = (ushort)(Data.Radarcol.Colors[m.Objects[i].ID] | 0x8000);
                                    break;
                                }
                            }
                            if (i == 0)
                                *cur++ = (ushort)(Data.Radarcol.Colors[m.GroundTile.ID] | 0x8000);
                        }
                    }
                }
                Texture.SetData<ushort>(data);
            }
        }
    }
}
