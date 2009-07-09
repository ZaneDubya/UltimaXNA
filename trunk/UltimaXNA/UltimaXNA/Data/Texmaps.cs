#region File Description & Usings
//-----------------------------------------------------------------------------
// Texmaps.cs
//
// Based on UltimaSDK, modifications by ClintXNA
//-----------------------------------------------------------------------------
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace UltimaXNA.Data
{
    class Texmaps
    {
        private static Texture2D[] m_Cache = new Texture2D[0x4000];
        private static FileIndexClint m_Index = new FileIndexClint("texidx.mul", "texmaps.mul");

        public static Texture2D GetTexmapTexture(int index, GraphicsDevice graphicsDevice)
        {
            Texture2D texture = m_Cache[index];

            if (texture == null)
            {
                m_Cache[index] = texture = ReadTexmapTexture(index, graphicsDevice);
            }

            return texture;
        }

        private static unsafe Texture2D ReadTexmapTexture(int index, GraphicsDevice graphicsDevice)
        {
            int extra;

            m_Index.Seek(index, out extra);

            extra = extra == 0 ? 64 : 128;

            ushort[] data = new ushort[extra * extra];

            fixed (ushort* pData = data)
            {
                ushort* pDataRef = pData;

                for (int y = 0; y < extra; ++y, pDataRef += extra)
                {
                    ushort* start = pDataRef;
                    ushort* end = start + extra;

                    while (start < end)
                    {
                        *start++ = (ushort)(m_Index.BinaryReader.ReadUInt16() ^ 0x8000);
                    }
                }
            }

            Texture2D texture = new Texture2D(graphicsDevice, extra, extra, 1, TextureUsage.None, SurfaceFormat.Bgra5551);

            texture.SetData<ushort>(data);

            return texture;
        }
    }
}