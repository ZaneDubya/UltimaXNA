/***************************************************************************
 *   AnimationResource.cs
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
using System.IO;
using UltimaXNA.Core.IO;
using UltimaXNA.Core.Resources;
using UltimaXNA.Ultima.IO;
#endregion

namespace UltimaXNA.Ultima.Resources
{
    internal sealed class AnimationResource
    {
        private const int ANIM_COUNT = 0x0800;
        private const int ANIM_MASK = 0x07FF;

        public const int HUMANOID_STAND_INDEX = 0x04;
        public const int HUMANOID_MOUNT_INDEX = 0x19;
        public const int HUMANOID_SIT_INDEX = 0x23; // 35

        private const int COUNT_ANIMS = 0x1000;
        private const int COUNT_ACTIONS = 36; // max UO action index is 34 (0-based, thus 35), we add one additional index for the humanoid sitting action.
        private const int COUNT_DIRECTIONS = 8;

        private AFileIndex m_FileIndex = (AFileIndex)FileManager.CreateFileIndex("Anim.idx", "Anim.mul", 0x40000, 6);
        public AFileIndex FileIndex { get { return m_FileIndex; } }

        private AFileIndex m_FileIndex2 = (AFileIndex)FileManager.CreateFileIndex("Anim2.idx", "Anim2.mul", 0x10000, -1);
        public AFileIndex FileIndex2 { get { return m_FileIndex2; } }

        private AFileIndex m_FileIndex3 = (AFileIndex)FileManager.CreateFileIndex("Anim3.idx", "Anim3.mul", 0x20000, -1);
        public AFileIndex FileIndex3 { get { return m_FileIndex3; } }

        private AFileIndex m_FileIndex4 = (AFileIndex)FileManager.CreateFileIndex("Anim4.idx", "Anim4.mul", 0x20000, -1);
        public AFileIndex FileIndex4 { get { return m_FileIndex4; } }

        private AFileIndex m_FileIndex5 = (AFileIndex)FileManager.CreateFileIndex("Anim5.idx", "Anim5.mul", 0x20000, -1);
        public AFileIndex FileIndex5 { get { return m_FileIndex5; } }

        private IAnimationFrame[][][][] m_Cache;
        private GraphicsDevice m_Graphics;

        public AnimationResource(GraphicsDevice graphics)
        {
            m_Graphics = graphics;
            m_Cache = new IAnimationFrame[COUNT_ANIMS][][][];
        }

        public IAnimationFrame[] GetAnimation(int body, ref int hue, int action, int direction)
        {
            int animIndex;
            AFileIndex fileIndex;
            int length, extra;
            bool patched;
            AnimationFrame.SittingTransformation sitting = AnimationFrame.SittingTransformation.None;

            if (!BodyExists(body))
                BodyDef.TranslateBodyAndHue(ref body, ref hue);

            IAnimationFrame[] frames = CheckCache(body, action, direction);
            if (frames != null)
                return frames;
            
            if (action == HUMANOID_SIT_INDEX)
            {
                if (direction == 3 || direction == 5)
                {
                    sitting = AnimationFrame.SittingTransformation.MountNorth;
                    GetIndexes(body, HUMANOID_MOUNT_INDEX, direction, out animIndex, out fileIndex);
                }
                else if (direction == 1 || direction == 7)
                {
                    sitting = AnimationFrame.SittingTransformation.StandSouth;
                    GetIndexes(body, HUMANOID_STAND_INDEX, direction, out animIndex, out fileIndex);
                }
                else
                {
                    GetIndexes(body, action, direction, out animIndex, out fileIndex);
                }
            }
            else
            {
                GetIndexes(body, action, direction, out animIndex, out fileIndex);
            }

            BinaryFileReader reader = fileIndex.Seek(animIndex, out length, out extra, out patched);
            if (reader == null)
                return null;

            frames = LoadAnimation(reader, sitting);
            return m_Cache[body][action][direction] = frames;
        }

        private IAnimationFrame[] LoadAnimation(BinaryFileReader reader, AnimationFrame.SittingTransformation sitting)
        {
            ushort[] palette = GetPalette(reader); // 0x100 * 2 = 0x0200 bytes
            int read_start = (int)reader.Position; // save file position after palette.

            int frameCount = reader.ReadInt(); // 0x04 bytes

            int[] lookups = new int[frameCount]; // frameCount * 0x04 bytes
            for (int i = 0; i < frameCount; ++i) { lookups[i] = reader.ReadInt(); }

            IAnimationFrame[] frames = new AnimationFrame[frameCount];
            for (int i = 0; i < frameCount; ++i)
            {
                if (lookups[i] < lookups[0])
                {
                    frames[i] = AnimationFrame.Empty; // Fix for broken animations, per issue13
                }
                else
                {
                    reader.Seek(read_start + lookups[i], SeekOrigin.Begin);
                    frames[i] = new AnimationFrame(m_Graphics, palette, reader, sitting);
                }
            }
            return frames;
        }

        private ushort[] GetPalette(BinaryFileReader reader)
        {
            ushort[] pal = new ushort[0x100];
            for (int i = 0; i < 0x100; ++i)
            {
                pal[i] = (ushort)(reader.ReadUShort() | 0x8000);
            }
            return pal;
        }

        private IAnimationFrame[] CheckCache(int body, int action, int direction)
        {
            // Make sure the cache is complete.
            if (m_Cache[body] == null)
                m_Cache[body] = new IAnimationFrame[COUNT_ACTIONS][][]; // max 35 actions
            if (m_Cache[body][action] == null)
                m_Cache[body][action] = new IAnimationFrame[COUNT_DIRECTIONS][];
            if (m_Cache[body][action][direction] == null)
                m_Cache[body][action][direction] = new IAnimationFrame[1];
            if (m_Cache[body][action][direction][0] != null)
                return m_Cache[body][action][direction];
            else
                return null;
        }

        private void GetIndexes(int body, int action, int direction, out int index, out AFileIndex fileIndex)
        {
            if (body < 0 || body >= COUNT_ANIMS)
                body = 0;

            int animIndex = BodyConverter.Convert(ref body);
            switch (animIndex)
            {
                default:
                case 1:
                    {
                        fileIndex = m_FileIndex;

                        if (body < 200)
                            index = body * 110;
                        else if (body < 400)
                            index = 22000 + ((body - 200) * 65);
                        else
                            index = 35000 + ((body - 400) * 175);

                        break;
                    }
                case 2:
                    {
                        fileIndex = m_FileIndex2;

                        if (body < 200)
                            index = body * 110;
                        else
                            index = 22000 + ((body - 200) * 65);

                        break;
                    }
                case 3:
                    {
                        fileIndex = m_FileIndex3;

                        if (body < 300)
                            index = body * 65;
                        else if (body < 400)
                            index = 33000 + ((body - 300) * 110);
                        else
                            index = 35000 + ((body - 400) * 175);

                        break;
                    }
                case 4:
                    {
                        fileIndex = m_FileIndex4;

                        if (body < 200)
                            index = body * 110;
                        else if (body < 400)
                            index = 22000 + ((body - 200) * 65);
                        else
                            index = 35000 + ((body - 400) * 175);

                        break;
                    }
                case 5:
                    {
                        fileIndex = m_FileIndex5;
                        if ((body < 200) && (body != 34)) // looks strange, though it works.
                            index = body * 110;
                        else if (body < 400)
                            index = 22000 + ((body - 200) * 65);
                        else
                            index = 35000 + ((body - 400) * 175);

                        break;
                    }
            }

            index += action * 5;

            if (direction <= 4)
            {
                index += direction;
            }
            else
            {
                index += direction - (direction - 4) * 2;
            }
        }

        private bool BodyExists(int body)
        {
            int animIndex;
            AFileIndex fileIndex;
            int length, extra;
            bool patched;

            GetIndexes(body, 0, 0, out animIndex, out fileIndex);
            BinaryFileReader reader = fileIndex.Seek(animIndex, out length, out extra, out patched);
            if (reader == null)
                return false;
            return true;
        }
    }
}
