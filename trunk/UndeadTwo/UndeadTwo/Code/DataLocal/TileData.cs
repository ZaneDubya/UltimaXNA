#region File Description & Usings
//-----------------------------------------------------------------------------
// TileData.cs
//
// Based on UltimaSDK, modifications by ClintXNA
//-----------------------------------------------------------------------------
using System;
using System.IO;
#endregion

namespace UltimaXNA.DataLocal
{
    public class TileData
    {
        public static LandData[] LandData = new LandData[0x4000];
        public static ItemData[] ItemData = new ItemData[0x4000];

        static TileData()
        {
            using (FileStream fileStream = FileManager.GetFile("tiledata.mul"))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);

                LandData landData;

                for (int i = 0; i < 0x4000; i++)
                {
                    landData = new LandData();

                    if ((i & 0x1F) == 0)
                    {
                        binaryReader.ReadInt32();
                    }

                    TileFlags flags = (TileFlags)binaryReader.ReadInt32();

                    int iTextureID = binaryReader.ReadInt16();

                    binaryReader.BaseStream.Seek(20, SeekOrigin.Current);

                    landData.Flags = flags;
                    landData.TextureID = iTextureID;

                    LandData[i] = landData;
                }

                ItemData itemData;

                for (int i = 0; i < 0x4000; i++)
                {
                    itemData = new ItemData();

                    if ((i & 0x1F) == 0)
                    {
                        binaryReader.ReadInt32();
                    }

                    itemData.Flags = (TileFlags)binaryReader.ReadInt32();
                    itemData.Weight = binaryReader.ReadByte();
                    itemData.Quality = binaryReader.ReadByte();

                    binaryReader.BaseStream.Seek(3, SeekOrigin.Current);

                    itemData.Quanitity = binaryReader.ReadByte();
                    itemData.AnimID = binaryReader.ReadInt16();
                    binaryReader.BaseStream.Seek(3, SeekOrigin.Current);

                    itemData.Value = binaryReader.ReadByte();
                    itemData.Height = binaryReader.ReadByte();

                    itemData.Name = System.Text.ASCIIEncoding.ASCII.GetString((binaryReader.ReadBytes(20)));
                    itemData.Name = itemData.Name.Trim('\0');
                    // binaryReader.BaseStream.Seek(20, SeekOrigin.Current);

                    ItemData[i] = itemData;
                }
            }
        }
    }

    public struct ItemData
    {
        public int Weight;
        public TileFlags Flags;
        public int Height;
        public int Quality;
        public int Quanitity;
        public int AnimID;
        public int Value;
        public string Name;

        public bool Background
        {
            get { return (this.Flags & TileFlags.Background) != 0; }
        }

        public bool Bridge
        {
            get { return (this.Flags & TileFlags.Bridge) != 0; }
        }

        public int CalcHeight
        {
            get
            {
                if ((this.Flags & TileFlags.Bridge) != 0)
                {
                    return this.Height / 2;
                }
                else
                {
                    return this.Height;
                }
            }
        }

        public bool Container
        {
            get { return (this.Flags & TileFlags.Container) != 0; }
        }

        public bool Foliage
        {
            get { return (this.Flags & TileFlags.Foliage) != 0; }
        }

        public bool Impassable
        {
            get { return (this.Flags & TileFlags.Impassable) != 0; }
        }

        public bool PartialHue
        {
            get { return (this.Flags & TileFlags.PartialHue) != 0; }
        }

        public bool Roof
        {
            get { return (this.Flags & TileFlags.Roof) != 0; }
        }

        public bool Surface
        {
            get { return (this.Flags & TileFlags.Surface) != 0; }
        }

        public bool Wall
        {
            get { return (this.Flags & TileFlags.Wall) != 0; }
        }

        public bool Wet
        {
            get { return (this.Flags & TileFlags.Wet) != 0; }
        }
    }

    public struct LandData
    {
        public TileFlags Flags;
        public int TextureID;

        public bool Wet
        {
            get { return (this.Flags & TileFlags.Wet) != 0; }
        }
    }

    [Flags]
    public enum TileFlags
    {
        Animation = 0x01000000,
        Armor = 0x08000000,
        ArticleA = 0x00004000,
        ArticleAn = 0x00008000,
        Background = 0x00000001,
        Bridge = 0x00000400,
        Container = 0x00200000,
        Damaging = 0x00000020,
        Door = 0x20000000,
        Foliage = 0x00020000,
        Generic = 0x00000800,
        Impassable = 0x00000040,
        Internal = 0x00010000,
        LightSource = 0x00800000,
        Map = 0x00100000,
        NoDiagonal = 0x02000000,
        None = 0x00000000,
        NoShoot = 0x00002000,
        PartialHue = 0x00040000,
        Roof = 0x10000000,
        StairBack = 0x40000000,
        StairRight = unchecked((int)0x80000000),
        Surface = 0x00000200,
        Translucent = 0x00000008,
        Transparent = 0x00000004,
        Unknown1 = 0x00000100,
        Unknown2 = 0x00080000,
        Unknown3 = 0x04000000,
        Wall = 0x00000010,
        Weapon = 0x00000002,
        Wearable = 0x00400000,
        Wet = 0x00000080,
        Window = 0x00001000
    }
}