/***************************************************************************
 *   ArtResource.cs
 *   Based on code from UltimaSDK: http://ultimasdk.codeplex.com/
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.IO;
using UltimaXNA.Core.Data;
using UltimaXNA.Ultima.IO;
using UltimaXNA.Ultima.Data;
using System.IO;
using UOReader;
using KUtility;
using System.Drawing.Imaging;
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.Ultima.Resources
{
    class KRTextures
    {
        private Texture2D[] m_LandTileTextureCache;
        private Texture2D[] m_StaticTileTextureCache;
        private Tileart[] m_TileartCache;

        private GraphicsDevice m_Graphics;

        public KRTextures(GraphicsDevice graphics)
        {
            m_Graphics = graphics;
            m_TileartCache = new Tileart[0x10000];
            m_LandTileTextureCache = new Texture2D[0x10000];
            m_StaticTileTextureCache = new Texture2D[0x10000];
            m_TileartCache = new Tileart[0x10000];
        }

        public Texture2D GetLandTexture(int index)
        {
            index &= FileManager.ItemIDMask;

            if (m_LandTileTextureCache[index] == null)
            {
                m_LandTileTextureCache[index] = ReadLandTexture(index);
            }

            return m_LandTileTextureCache[index];
        }

        public Texture2D GetStaticTexture(int index)
        {
            index &= FileManager.ItemIDMask;

            if (m_StaticTileTextureCache[index] == null)
            {
                Texture2D texture;
                Tileart tileart;
                ReadStaticTexture(index, out texture, out tileart);
                m_StaticTileTextureCache[index] = texture;
                m_TileartCache[index] = tileart;
            }

            return m_StaticTileTextureCache[index];
        }

        public void GetStaticDimensions(int index, out int width, out int height)
        {
            index &= FileManager.ItemIDMask;

            if (m_StaticTileTextureCache[index] == null)
            {
                GetStaticTexture(index);
            }
            Tileart t = m_TileartCache[index];
            width = t.offsetEC.Xend - t.offsetEC.Xstart;
            height = t.offsetEC.Yend;

            if(width == 0)
            {
                width = height = 64;
            }
        }

        private unsafe Texture2D ReadLandTexture(int index)
        {
            FileStream filestream = new FileStream(FileManager.GetPath(string.Format("EC/build/worldart/{0:d8}.dds", index)), FileMode.Open);
            return Texture2D.FromStream(m_Graphics, filestream);
        }

        private unsafe void ReadStaticTexture(int index, out Texture2D texture, out Tileart tileart)
        {
            uint resource = 0xFFFFFFFF;
            string resPath = "ERROR";
            bool resize;

            int x;
            int y;
            int width;
            int height;

            using (FileStream fs = new FileStream(string.Format(FileManager.GetPath("EC/build/tileart/{0:d8}.bin"), index), FileMode.Open))
            using (BinaryReader r = new BinaryReader(fs))
                tileart = Tileart.readTileart(r);

            x = 0;
            y = 0;
            width = 44;
            height = 44;
            resize = false;

            //WorldArt Texture
            if (tileart.textures[0].texturePresent == -1)
            {
                resource = tileart.textures[0].texturesArray[0].textureIDX;
                resPath = string.Format(FileManager.GetPath("EC/build/worldart/{0:d8}.dds"), index);

                if (!File.Exists(resPath))
                {
                    resource = 0xFFFFFFFF;
                }
                else
                {
                    x = tileart.offsetEC.Xstart;
                    y = 0;
                    width = tileart.offsetEC.Width;
                    height = tileart.offsetEC.Height;

                    if (width == 0)
                    {
                        width = height = 64;
                    }
                    resize = true;
                }
            }
            //LegacyTexture
            if (resource == 0xFFFFFFFF && tileart.textures[1].texturePresent == 1)
            {
                //resource = tileart.textures[1].texturesArray[0].textureIDX;
                resource = 0;
                resPath = string.Format(FileManager.GetPath("EC/build/tileartlegacy/{0:d8}.dds"), index);
                if (!File.Exists(resPath))
                {
                    resource = 0xFFFFFFFF;
                }
                else
                {
                    x = tileart.offset2D.Xstart;
                    y = tileart.offset2D.Ystart;
                    width = tileart.offset2D.Width;
                    height = tileart.offset2D.Height;

                    if (width == 0)
                    {
                        width = height = 44;
                    }
                }
            }
            //EnhancedTexture
            //Is this really necessary?
            //Light Texture
            if (resource != 0xFFFFFFFF && tileart.textures[3].texturePresent == 1)
            {
                //TODO: light texture load
            }

            if (resource == 0xFFFFFFFF)
                throw new System.Exception("Missing IMAGE!");

            DDSImage img = new DDSImage(File.ReadAllBytes(resPath));

            //Metrics.ReportDataRead((int)filestream.Length);
            using (MemoryStream ms = new MemoryStream())
            {
                img.images[0].Save(ms, ImageFormat.Png);
                Texture2D temp;

                if (resize)//HERE WE SCALE THE IMAGE
                {
                    temp = Texture2D.FromStream(m_Graphics, ms, (int)(img.images[0].Width * 0.6875), (int)(img.images[0].Height * 0.6875), true);
                    x = (int)(x * 0.6875);
                    y = (int)(y * 0.6875);
                    width = (int)(width * 0.6875);
                    height = (int)(height * 0.6875);
                }
                else
                    temp = Texture2D.FromStream(m_Graphics, ms);

                if ((width + x) > temp.Width)
                    width = temp.Width - x;
                if ((height + y) > temp.Height)
                    height = temp.Height - y;

                uint[] texData = new uint[width * height];
                temp.GetData<uint>(
                    0, 
                    new Rectangle(
                        x,
                        y,
                        width,
                        height),
                    texData,
                    0,
                    width * height);

                texture = new Texture2D(m_Graphics, width, height, false, SurfaceFormat.Color);
                texture.SetData(texData);

            }
            return;
        }
    }
}
