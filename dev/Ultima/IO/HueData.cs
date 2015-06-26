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
using UltimaXNA.Core.Diagnostics;
#endregion

namespace UltimaXNA.Ultima.IO
{
    public class HuesXNA
    {
        public const int HueCount = 3000;

        private static GraphicsDevice graphicsDevice;
        private static Texture2D m_HueTexture0, m_HueTexture1;
        private const int m_HueTextureWidth = 32; // Each hue is 32 pixels wide
        private const int m_HueTextureHeight = 2048;
        private const int multiplier = 0xFF / 0x1F;

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            HuesXNA.graphicsDevice = graphicsDevice;
            graphicsDevice.DeviceReset += graphicsDevice_DeviceReset;
            CreateTexture();
        }

        static void graphicsDevice_DeviceReset(object sender, System.EventArgs e)
        {
            CreateTexture();
        }

        static void CreateTexture()
        {
            m_HueTexture0 = new Texture2D(graphicsDevice, m_HueTextureWidth, m_HueTextureHeight);
            m_HueTexture1 = new Texture2D(graphicsDevice, m_HueTextureWidth, m_HueTextureHeight);
            uint[] hueData = getTextureData();
            m_HueTexture0.SetData(hueData, 0, m_HueTextureWidth * m_HueTextureHeight);
            m_HueTexture1.SetData(hueData, m_HueTextureWidth * m_HueTextureHeight, m_HueTextureWidth * m_HueTextureHeight);
        }

        static uint[] getTextureData()
        {
            BinaryReader reader = new BinaryReader( FileManager.GetFile( "hues.mul" ) );
            int currentHue = 0;
            int currentIndex = 0;
            uint[] data = new uint[m_HueTextureWidth * m_HueTextureHeight * 2];

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

        public static Texture2D CreateHueSwatch(int width, int height, int[] hues)
        {
            uint[] pixels = new uint[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                int hue = hues[i] - 1;
                uint[] pixel = new uint[1];
                if (hue < m_HueTextureHeight)
                    HueTexture0.GetData<uint>(0, new Rectangle(31, hue % m_HueTextureHeight, 1, 1), pixel, 1, 1);
                else
                    HueTexture1.GetData<uint>(0, new Rectangle(31, hue % m_HueTextureHeight, 1, 1), pixel, 1, 1);
                pixels[i] = pixel[0];
            }
            Texture2D t = new Texture2D(graphicsDevice, width, height);
            t.SetData<uint>(pixels);
            return t;
        }

        public static uint[] GetAllHues()
        {
            uint[] hues = new uint[HueCount];
            uint[] allHues = getTextureData();
            for (int i = 0; i < HueCount; i++)
            {
                hues[i] = allHues[i * 32 + 31];
            }
            return hues;
        }

        public static Texture2D HueTexture0
        {
            get
            {
                return m_HueTexture0;
            }
        }

        public static Texture2D HueTexture1
        {
            get
            {
                return m_HueTexture1;
            }
        }

        public static int GetWebSafeHue(Color inColor)
        {
            return GetWebSafeHue(inColor.R, inColor.G, inColor.B);
        }

        public static int GetWebSafeHue(int r, int g, int b)
        {
            int index = 0;
            for (int i = 0; i < 6; i++)
                if (r <= m_kCutOffValuesForWebSafeColors[i])
                {
                    index += i * 1;
                    break;
                }
            for (int i = 0; i < 6; i++)
                if (g <= m_kCutOffValuesForWebSafeColors[i])
                {
                    index += i * 6;
                    break;
                }
            for (int i = 0; i < 6; i++)
                if (b <= m_kCutOffValuesForWebSafeColors[i])
                {
                    index += i * 36;
                    break;
                }
            return m_kWebSafeHues[index];
        }
        static int[] m_kCutOffValuesForWebSafeColors = new int[6] { 0x19, 0x4C, 0x7F, 0xB2, 0xE5, 0xFF };
        static int[] m_kWebSafeHues = new int[216] {
            0000, 1149, 1157, 1090, 0037, 1288, 
            1267, 1149, 1133, 1518, 0138, 0038, 
            1267, 1193, 1454, 1130, 1256, 0043, 
            1268, 0267, 0257, 0252, 1126, 2114, 
            0067, 0168, 1270, 0158, 1159, 1281, 
            0068, 1099, 1570, 0058, 0053, 1196, 
            1065, 1149, 1273, 0232, 0133, 0033, 
            1589, 1175, 1147, 1609, 2118, 2117, 
            1069, 1155, 1444, 2318, 1551, 2117, 
            0272, 0467, 2006, 1450, 1712, 1720, 
            1456, 0368, 0363, 0358, 2215, 2213, 
            0073, 0069, 0064, 0059, 2126, 1260, 
            1093, 1273, 1273, 0227, 0128, 0028, 
            1156, 0602, 0622, 0427, 0328, 0034, 
            1097, 0682, 1898, 1014, 1214, 1640, 
            0277, 0477, 1433, 1440, 2316, 0045, 
            0077, 1061, 0469, 1092, 2001, 0050, 
            1299, 0074, 0070, 0065, 0060, 0055, 
            1262, 0207, 0212, 0217, 0022, 0023, 
            0297, 0402, 0412, 0518, 0323, 0029, 
            0292, 0492, 1316, 1242, 0429, 0030, 
            1160, 0588, 2223, 1893, 1003, 1628, 
            0082, 0383, 0479, 0670, 1191, 2007, 
            1179, 0079, 0080, 0071, 0061, 0056, 
            1176, 0007, 0012, 0017, 1275, 1289, 
            1263, 0303, 0408, 0114, 0119, 0024, 
            0092, 0498, 0504, 0414, 0320, 0025, 
            1284, 0194, 0494, 1312, 1238, 0026, 
            0188, 0189, 0390, 2221, 2401, 2307, 
            1685, 1066, 0085, 0081, 1167, 1674, 
            0003, 0008, 0008, 0013, 0018, 1289, 
            1183, 0004, 0009, 0014, 0019, 0019, 
            1282, 0099, 1364, 0010, 0020, 0020, 
            1265, 0094, 0100, 0006, 0016, 0021, 
            1283, 0089, 0090, 2120, 1195, 1072, 
            1577, 1577, 0090, 0091, 1154, 1150 };
    }
}