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
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Ultima.IO;
#endregion

namespace UltimaXNA.Ultima.Resources
{
    public class HueData
    {
        public const int HueCount = 4096;

        private static GraphicsDevice graphicsDevice;
        private static Texture2D m_HueTexture0, m_HueTexture1;
        private const int m_HueTextureWidth = 32; // Each hue is 32 colors
        private const int m_HueTextureHeight = 2048;
        private const int multiplier = 0xFF / 0x1F;

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            HueData.graphicsDevice = graphicsDevice;
            graphicsDevice.DeviceReset += graphicsDevice_DeviceReset;
            CreateTexture();
        }

        static void graphicsDevice_DeviceReset(object sender, EventArgs e)
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

            int webSafeHuesBegin = m_HueTextureHeight * 2 - 216;
            for (int b = 0; b < 6; b++)
            {
                for (int g = 0; g < 6; g++)
                {
                    for (int r = 0; r < 6; r++)
                    {
                        data[(webSafeHuesBegin + r + g * 6 + b * 36) * 32 + 31] = (uint)(
                            0xff000000 +
                            b * 0x00330000 +
                            g * 0x00003300 +
                            r * 0x00000033);
                    }
                }
            }
            return data;
        }

        public static Texture2D CreateHueSwatch(int width, int height, int[] hues)
        {
            uint[] pixels = new uint[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                int hue = hues[i];
                uint[] pixel = new uint[1];
                if (hue < m_HueTextureHeight)
                    HueTexture0.GetData<uint>(0, new Rectangle(31, hue % m_HueTextureHeight, 1, 1), pixel, 0, 1);
                else
                    HueTexture1.GetData<uint>(0, new Rectangle(31, hue % m_HueTextureHeight, 1, 1), pixel, 0, 1);
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
            0000, 3881, 3882, 3883, 3884, 3885, 
            3886, 3887, 3888, 3889, 3890, 3891, 
            3892, 3893, 3894, 3895, 3896, 3897, 
            3898, 3899, 3900, 3901, 3902, 3903, 
            3904, 3905, 3906, 3907, 3908, 3909, 
            3910, 3911, 3912, 3913, 3914, 3915, 
            3916, 3917, 3918, 3919, 3920, 3921, 
            3922, 3923, 3924, 3925, 3926, 3927, 
            3928, 3929, 3930, 3931, 3932, 3933, 
            3934, 3935, 3936, 3937, 3938, 3939, 
            3940, 3941, 3942, 3943, 3944, 3945, 
            3946, 3947, 3948, 3949, 3950, 3951, 
            3952, 3953, 3954, 3955, 3956, 3957, 
            3958, 3959, 3960, 3961, 3962, 3963, 
            3964, 3965, 3966, 3967, 3968, 3969, 
            3970, 3971, 3972, 3973, 3974, 3975, 
            3976, 3977, 3978, 3979, 3980, 3981, 
            3982, 3983, 3984, 3985, 3986, 3987, 
            3988, 3989, 3990, 3991, 3992, 3993, 
            3994, 3995, 3996, 3997, 3998, 3999, 
            4000, 4001, 4002, 4003, 4004, 4005, 
            4006, 4007, 4008, 4009, 4010, 4011, 
            4012, 4013, 4014, 4015, 4016, 4017, 
            4018, 4019, 4020, 4021, 4022, 4023, 
            4024, 4025, 4026, 4027, 4028, 4029, 
            4030, 4031, 4032, 4033, 4034, 4035, 
            4036, 4037, 4038, 4039, 4040, 4041, 
            4042, 4043, 4044, 4045, 4046, 4047, 
            4048, 4049, 4050, 4051, 4052, 4053, 
            4054, 4055, 4056, 4057, 4058, 4059, 
            4060, 4061, 4062, 4063, 4064, 4065, 
            4066, 4067, 4068, 4069, 4070, 4071, 
            4072, 4073, 4074, 4075, 4076, 4077, 
            4078, 4079, 4080, 4081, 4082, 4083, 
            4084, 4085, 4086, 4087, 4088, 4089, 
            4090, 4091, 4092, 4093, 4094, 4095, };
    }
}