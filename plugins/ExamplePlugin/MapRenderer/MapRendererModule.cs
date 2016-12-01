using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using UltimaXNA;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Patterns;
using UltimaXNA.Ultima.UI.LoginGumps;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.World.EntityViews;
using UltimaXNA.Ultima.World.Input;
using UltimaXNA.Ultima.World.Maps;
using UltimaXNA.Ultima.World.WorldViews;

namespace ExamplePlugin.MapRenderer
{
    class MapRendererModule : IModule
    {
        public string Name => "Map Renderer";

        public void Load()
        {
            // LoginGump.AddButton("Debug:Map", OnClick);
        }

        public void Unload()
        {
            LoginGump.RemoveButton(OnClick);
        }

        const int Width = 44 * 8;
        const int Height = 44 * 8;
        const int WidthExtra = 44 * 4;
        const int HeightExtra = 44 * 4;
        const int HeightExtra2 = 256 * 4;

        void OnClick()
        {
            Directory.CreateDirectory("Chunks");
            IsometricLighting lighting = new IsometricLighting();
            MouseOverList mouseOverNull = new MouseOverList(new MousePicking());
            SpriteBatch3D spritebatch = Service.Get<SpriteBatch3D>();
            RenderTarget2D render = new RenderTarget2D(spritebatch.GraphicsDevice, Width + WidthExtra * 2, Height + HeightExtra * 2 + HeightExtra2);
            Map map = new Map(0);
            for (int chunky = 0; chunky < 10; chunky++)
            {
                for (int chunkx = 0; chunkx < 10; chunkx++)
                {
                    spritebatch.GraphicsDevice.SetRenderTarget(render);
                    spritebatch.Reset();
                    uint cx = (uint)chunkx + 200;
                    uint cy = (uint)chunky + 200;
                    map.CenterPosition = new Point((int)(cx << 3), (int)(cy << 3));
                    MapChunk chunk = map.GetMapChunk(cx, cy);
                    chunk.LoadStatics(map.MapData, map);
                    int z = 0;
                    for (int i = 0; i < 64; i++)
                    {
                        int y = (i / 8);
                        int x = (i % 8);
                        int sy = y * 22 + x * 22 + HeightExtra + HeightExtra2;
                        int sx = 22 * 8 - y * 22 + x * 22 + WidthExtra;
                        MapTile tile = chunk.Tiles[i];
                        tile.ForceSort();
                        for (int j = 0; j < tile.Entities.Count; j++)
                        {
                            AEntity e = tile.Entities[j];
                            AEntityView view = e.GetView();
                            view.Draw(spritebatch, new Vector3(sx, sy, 0), mouseOverNull, map, false);
                        }
                    }
                    spritebatch.SetLightIntensity(lighting.IsometricLightLevel);
                    spritebatch.SetLightDirection(lighting.IsometricLightDirection);
                    spritebatch.GraphicsDevice.Clear(Color.Transparent);
                    spritebatch.FlushSprites(true);
                    spritebatch.GraphicsDevice.SetRenderTarget(null);
                    render.SaveAsPng(new FileStream($"Chunks/{cy}-{cx}.png", FileMode.Create), render.Width, render.Height);
                }
            }
        }
    }
}
