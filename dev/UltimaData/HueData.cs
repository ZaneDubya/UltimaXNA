/***************************************************************************
 *   HuesXNA.cs
 *   Based on code from UltimaSDK: http://ultimasdk.codeplex.com/
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
using System.IO;
using System.Text;
using UltimaXNA.Core.Diagnostics;
#endregion

namespace UltimaXNA.UltimaData
{
    public class HuesXNA
    {
        private static GraphicsDevice graphicsDevice;
        private static Texture2D m_hueTexture;
        private const int m_HueTextureWidth = 32; // Each hue is 32 pixels wide, so divided by 32 = 2 hues wide.
        private const int m_HueTextureHeight = 4096;
        private const int multiplier = 0xFF / 0x1F;

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            HuesXNA.graphicsDevice = graphicsDevice;
            graphicsDevice.DeviceReset += graphicsDevice_DeviceReset;
            CreateTexture();
            // Hues.GetWebSafeColors();
        }

        static void graphicsDevice_DeviceReset(object sender, System.EventArgs e)
        {
            CreateTexture();
        }

        static void CreateTexture()
        {
            m_hueTexture = new Texture2D(graphicsDevice, m_HueTextureWidth, m_HueTextureHeight);
            uint[] iTextData = getTextureData();
            m_hueTexture.SetData(iTextData);
        }

        static uint[] getTextureData()
        {
            BinaryReader reader = new BinaryReader( FileManager.GetFile( "hues.mul" ) );
            int currentHue = 0;
            int currentIndex = 0;
            uint[] data = new uint[m_HueTextureWidth * m_HueTextureHeight];

            Metrics.ReportDataRead((int)reader.BaseStream.Length);

            currentIndex += 32;

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                reader.ReadInt32(); //Header
                
                for (int entry = 0; entry < 8; entry++)
                {
                    for (int i = 0; i < 32; i++)
                    {
                        uint color = reader.ReadUInt16();
                        data[currentIndex++] = 0xFF000000 + (
                            ((((color >> 10) & 0x1F) * multiplier)) |
                            ((((color >> 5) & 0x1F) * multiplier) << 8) |
                            (((color & 0x1F) * multiplier) << 16)
                            );
                    }
                    currentHue++;
                    reader.ReadInt16(); //table start
                    reader.ReadInt16(); //table end
                    reader.ReadBytes( 20 ); //name
                }
            }
            reader.Close();
            return data;
        }

        public static Texture2D HueSwatch(int width, int height, int[] hues)
        {
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                System.Drawing.Color c = HueData.GetHue(hues[i] - 1).GetColor(31);
                pixels[i] = new Color(c.R, c.G, c.B, c.A);
            }
            Texture2D t = new Texture2D(graphicsDevice, width, height);
            t.SetData<Color>(pixels);
            return t;
        }

        public static Texture2D HueTexture
        {
            get
            {
                return m_hueTexture;
            }
        }


        public static int GetWebSafeHue(string inColor)
        {
            if (inColor.Length == 3)
                return GetWebSafeHue(
                    inColor.Substring(0, 1) + inColor.Substring(0, 1) +
                    inColor.Substring(1, 1) + inColor.Substring(1, 1) +
                    inColor.Substring(2, 1) + inColor.Substring(2, 1));
            else if (inColor.Length == 6)
            {
                return GetWebSafeHue(
                    Utility.ColorFromHexString("00" + inColor));
            }
            else
            {
                return 0;
            }
        }
        public static int GetWebSafeHue(Color inColor)
        {
            return GetWebSafeHue(inColor.R, inColor.G, inColor.B);
        }
        public static int GetWebSafeHue(int inColor)
        {
            
            int r = inColor & 0x000000FF;
            int g = (inColor & 0x0000FF00) >> 8;
            int b = (inColor & 0x00FF0000) >> 16;
            return GetWebSafeHue(r, g, b);
        }
        public static int GetWebSafeHue(int r, int g, int b)
        {
            int index = 0;
            for (int i = 0; i < 6; i++)
                if (r <= m_kCutOffValuesForWebSafeColors[i])
                {
                    index = i;
                    break;
                }
            for (int i = 0; i < 6; i++)
                if (b <= m_kCutOffValuesForWebSafeColors[i])
                {
                    index += i * 6;
                    break;
                }
            for (int i = 0; i < 6; i++)
                if (g <= m_kCutOffValuesForWebSafeColors[i])
                {
                    index += i * 36;
                    break;
                }
            return m_kWebSafeHues[index];
        }
        static int[] m_kCutOffValuesForWebSafeColors = new int[6] { 0x19, 0x4C, 0x7F, 0xB2, 0xE5, 0xFF };
        static int[] m_kWebSafeHues = new int[216] { 1278, 1148, 1156, 1089, 0036, 1287, 1064, 2052, 1272, 0231, 0132, 0032, 1092, 1272, 1272, 0226, 0127, 0027, 1261, 0206, 0211, 0216, 0021, 0022, 1175, 0006, 0011, 0016, 1274, 1288, 0002, 0007, 0007, 0012, 0017, 1288, 2054, 1148, 1132, 1517, 0137, 0037, 1588, 2050, 1146, 1608, 2117, 2116, 1155, 0601, 0621, 0426, 0327, 0033, 0296, 0401, 0411, 0517, 0322, 0028, 1262, 0302, 0407, 0113, 0118, 0023, 1182, 0003, 0008, 0013, 0018, 0018, 1266, 1192, 1453, 1129, 1255, 0042, 1068, 1154, 1443, 2317, 1550, 2116, 1096, 0681, 1897, 1013, 1213, 1639, 0291, 0491, 1315, 1241, 0428, 0029, 0091, 0497, 0503, 0413, 0319, 0024, 1281, 0098, 1363, 0009, 0019, 0019, 1267, 0266, 0256, 0251, 1125, 2113, 0271, 0466, 2005, 1449, 1711, 1719, 0276, 0476, 1432, 1439, 2315, 0044, 1159, 0587, 2222, 2030, 1002, 1627, 1283, 0193, 0493, 1311, 1237, 0025, 1264, 0093, 0099, 0005, 0015, 0020, 0066, 0167, 1269, 0157, 1158, 1280, 1455, 0367, 0362, 0357, 2214, 2212, 0076, 1060, 0468, 1091, 2000, 0049, 0081, 0382, 0478, 0669, 1190, 2006, 0187, 0188, 0389, 2220, 2036, 2306, 1282, 0088, 0089, 2119, 1194, 1071, 0067, 1098, 1569, 0057, 0052, 1195, 0072, 0068, 0063, 0058, 2125, 1259, 1298, 0073, 0069, 0064, 0059, 0054, 1178, 0078, 0079, 0070, 0060, 0055, 1684, 1065, 0084, 0080, 2946, 1673, 1576, 1576, 0089, 0090, 1153, 0000, };
        
    }

    public class HueData
    {
        private static Hue[] m_List;

        public static Hue[] List { get { return m_List; } }

        public static int[] SkinTones
        {
            get
            {
                int max = 7 * 8;
                int[] hues = new int[max];
                for (int i = 0; i < max; i++)
                {
                    hues[i] = (i < 37) ? i + 1002 : i + 1003;
                }
                return hues;
            }
        }

        public static int[] HairTones
        {
            get
            {
                int max = 8 * 6;
                int[] hues = new int[max];
                for (int i = 0; i < max; i++)
                {
                    hues[i] = i + 1102;
                }
                return hues;
            }
        }

        static HueData()
        {
            string path = FileManager.GetFilePath("hues.mul");
            int index = 0;

            m_List = new Hue[3000];

            if (path != null)
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BinaryReader bin = new BinaryReader(fs);

                    int blockCount = (int)fs.Length / 708;

                    if (blockCount > 375)
                        blockCount = 375;

                    for (int i = 0; i < blockCount; ++i)
                    {
                        bin.ReadInt32();

                        for (int j = 0; j < 8; ++j, ++index)
                            m_List[index] = new Hue(index, bin);
                    }
                }
            }

            for (; index < 3000; ++index)
                m_List[index] = new Hue(index);
        }

        public static Hue GetHue(int index)
        {
            index &= 0x3FFF;

            if (index >= 0 && index < 3000)
                return m_List[index];

            return m_List[0];
        }
    }

    public class Hue
    {
        private int m_Index;
        private short[] m_Colors;
        private string m_Name;

        public int Index { get { return m_Index; } }
        public short[] Colors { get { return m_Colors; } }
        public string Name { get { return m_Name; } }

        public Hue(int index)
        {
            m_Name = "Null";
            m_Index = index;
            m_Colors = new short[34];
        }

        public System.Drawing.Color GetColor(int index)
        {
            int c16 = m_Colors[index];

            return System.Drawing.Color.FromArgb((c16 & 0x7C00) >> 7, (c16 & 0x3E0) >> 2, (c16 & 0x1F) << 3);
        }

        public ushort GetColorUShort(int index)
        {
            return (ushort)m_Colors[index];
        }

        public Hue(int index, BinaryReader bin)
        {
            m_Index = index;
            m_Colors = new short[34];

            for (int i = 0; i < 34; ++i)
                m_Colors[i] = (short)(bin.ReadUInt16() | 0x8000);

            bool nulled = false;

            StringBuilder sb = new StringBuilder(20, 20);

            for (int i = 0; i < 20; ++i)
            {
                char c = (char)bin.ReadByte();

                if (c == 0)
                    nulled = true;
                else if (!nulled)
                    sb.Append(c);
            }

            m_Name = sb.ToString();
        }
    }
}