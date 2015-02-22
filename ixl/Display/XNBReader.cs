using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Text.RegularExpressions;

namespace InterXLib.Display
{
    public static class XNBReader
    {
        public static object ReadObject(GraphicsDevice device, string path)
        {
            BinaryFileReader reader = Serialize.OpenReader(path);
            byte[] magic = reader.ReadBytes(4);
            byte xnb_format_version = reader.ReadByte(); // 5 = XNA GS 4.0
            byte flag_bits = reader.ReadByte();
            bool isCompressed = ((flag_bits & 0x80) == 0x80);

            int file_size_on_disk = reader.ReadInt(); // size of file, including the header block

            if (isCompressed)
            {
                int file_size_decompressed = reader.ReadInt(); // does not include size of header block (14 bytes)
                MemoryStream decompressedStream = null;

                // default window size for XNB encoded files is 64Kb (need 16 bits to represent it)
                LzxDecoder lzx = new LzxDecoder(16);
                decompressedStream = new MemoryStream(file_size_decompressed);
                int compressedSize = file_size_on_disk - 14;
                long startPos = reader.Position;
                long pos = startPos;

                while (pos - startPos < compressedSize)
                {
                    // the compressed stream is seperated into blocks that will decompress
                    // into 32Kb or some other size if specified.
                    // normal, 32Kb output blocks will have a short indicating the size
                    // of the block before the block starts
                    // blocks that have a defined output will be preceded by a byte of value
                    // 0xFF (255), then a short indicating the output size and another
                    // for the block size
                    // all shorts for these cases are encoded in big endian order
                    int hi = reader.ReadByte();
                    int lo = reader.ReadByte();
                    int block_size = (hi << 8) | lo;
                    int frame_size = 0x8000; // frame size is 32Kb by default
                    // does this block define a frame size?
                    if (hi == 0xFF)
                    {
                        hi = lo;
                        lo = (byte)reader.ReadByte();
                        frame_size = (hi << 8) | lo;
                        hi = (byte)reader.ReadByte();
                        lo = (byte)reader.ReadByte();
                        block_size = (hi << 8) | lo;
                        pos += 5;
                    }
                    else
                        pos += 2;

                    // either says there is nothing to decode
                    if (block_size == 0 || frame_size == 0)
                        break;

                    lzx.Decompress(reader.Stream, block_size, decompressedStream, frame_size);
                    pos += block_size;

                    // reset the position of the input just incase the bit buffer
                    // read in some unused bytes
                    reader.Seek(pos, SeekOrigin.Begin);
                }

                if (decompressedStream.Position != file_size_decompressed)
                {
                    Logging.Fatal("Decompression of " + path + " failed. ");
                }

                decompressedStream.Seek(0, SeekOrigin.Begin);
                reader = new BinaryFileReader(new BinaryReader(decompressedStream));
            }

            List<Encoder> encoders = InternalGetEncoders(device, reader);

            int shared_resource_count = reader.Read7BitEncodedInt();

            Object value = InternalReadObject(device, reader, encoders);
            reader.Close();

            return value;
        }

        private static List<Encoder> InternalGetEncoders(GraphicsDevice device, BinaryFileReader reader)
        {
            List<Encoder> list = new List<Encoder>();
            int reader_count = reader.Read7BitEncodedInt();
            Regex regex = new Regex(@"\b([a-z0-9_]+)(,|$)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            for (int i = 0; i < reader_count; i++)
            {
                string reader_name = reader.ReadString();
                int reader_version = reader.ReadInt();
                string typeReaderShortName = regex.Match(reader_name).Groups[1].Value;
                if (typeReaderShortName.IndexOf("Reader") != -1)
                    typeReaderShortName = typeReaderShortName.Substring(0, typeReaderShortName.IndexOf("Reader"));
                bool isList = (reader_name.IndexOf("ListReader") != -1);

                Encoder encoder = new Encoder(typeReaderShortName, isList);
                list.Add(encoder);
            }

            return list;
        }

        private static object InternalReadObject(GraphicsDevice device, BinaryFileReader reader, List<Encoder> encoders)
        {
            int encoder_index = reader.Read7BitEncodedInt();
            if (encoder_index <= 0)
                return null;
            Encoder encoder = encoders[encoder_index - 1];
            switch (encoder.Name)
            {
                case "SpriteFont":
                    return InternalReadSpriteFont(device, reader, encoders);
                case "Texture2D":
                    return InternalReadTexture(device, reader, encoders);
                case "Rectangle":
                    if (encoder.IsList)
                        return InternalReadListRectangle(device, reader, encoders);
                    else
                        return InternalReadRectangle(device, reader, encoders);
                case "Char":
                    if (encoder.IsList)
                        return InternalReadListChar(device, reader, encoders);
                    else
                        return InternalReadChar(device, reader, encoders);
                case "Vector3":
                    if (encoder.IsList)
                        return InternalReadListVector3(device, reader, encoders);
                    else
                        return InternalReadVector3(device, reader, encoders);
            }
            return null;
        }

        private static YSpriteFont InternalReadSpriteFont(GraphicsDevice device, BinaryFileReader reader, List<Encoder> encoders)
        {
            Texture2D texture = (Texture2D)InternalReadObject(device, reader, encoders);
            List<Rectangle> glyphBounds = (List<Rectangle>)InternalReadObject(device, reader, encoders);
            List<Rectangle> cropping = (List<Rectangle>)InternalReadObject(device, reader, encoders);
            List<char> characters = (List<char>)InternalReadObject(device, reader, encoders);
            int lineSpacing = reader.ReadInt();
            float spacing = reader.ReadFloat();
            List<Vector3> kerning = (List<Vector3>)InternalReadObject(device, reader, encoders);
            char? defaultChar = null;
            int default_char = (int)reader.ReadCharUTF8();
            if (default_char != 0)
                defaultChar = (char)default_char;

            YSpriteFont ysf = new YSpriteFont(texture, glyphBounds, cropping, characters, lineSpacing, spacing, kerning, defaultChar);
            return ysf;
        }

        private static Texture2D InternalReadTexture(GraphicsDevice device, BinaryFileReader reader, List<Encoder> encoders)
        {
            SurfaceFormat format = (SurfaceFormat)reader.ReadInt();
            int width = reader.ReadInt(), height = reader.ReadInt(), mip_levels = reader.ReadInt();

            Texture2D texture = new Texture2D(device, width, height, (mip_levels > 1), format);
            for (int i = 0; i < mip_levels; i++)
            {
                int data_count = reader.ReadInt();
                byte[] data = reader.ReadBytes(data_count);
                texture.SetData<byte>(data);
            }
            return texture;
        }

        private static List<Rectangle> InternalReadListRectangle(GraphicsDevice device, BinaryFileReader reader, List<Encoder> encoders)
        {
            int count = reader.ReadInt();
            List<Rectangle> list = new List<Rectangle>(count);
            for (int i = 0; i < count; i++)
                list.Add(InternalReadRectangle(device, reader, encoders));
            return list;
        }

        private static Rectangle InternalReadRectangle(GraphicsDevice device, BinaryFileReader reader, List<Encoder> encoders)
        {
            Rectangle rectangle = new Rectangle(
                reader.ReadInt(), reader.ReadInt(), reader.ReadInt(), reader.ReadInt());
            return rectangle;
        }

        private static List<char> InternalReadListChar(GraphicsDevice device, BinaryFileReader reader, List<Encoder> encoders)
        {
            int count = reader.ReadInt();
            List<char> list = new List<char>(count);
            for (int i = 0; i < count; i++)
                list.Add(InternalReadChar(device, reader, encoders));
            return list;
        }

        private static char InternalReadChar(GraphicsDevice device, BinaryFileReader reader, List<Encoder> encoders)
        {
            char c = reader.ReadCharUTF8();
            return c;
        }

        private static List<Vector3> InternalReadListVector3(GraphicsDevice device, BinaryFileReader reader, List<Encoder> encoders)
        {
            int count = reader.ReadInt();
            List<Vector3> list = new List<Vector3>(count);
            for (int i = 0; i < count; i++)
                list.Add(InternalReadVector3(device, reader, encoders));
            return list;
        }

        private static Vector3 InternalReadVector3(GraphicsDevice device, BinaryFileReader reader, List<Encoder> encoders)
        {
            Vector3 v = new Vector3(
                reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat());
            return v;
        }

        private struct Encoder
        {
            public string Name;
            public bool IsList;

            public Encoder(string name, bool isList)
            {
                Name = name;
                IsList = isList;
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
