#region File Description & Usings
//-----------------------------------------------------------------------------
// TileData.cs
//
// Based on UltimaSDK, modifications by ClintXNA
//-----------------------------------------------------------------------------
using System;
using System.IO;
#endregion

namespace UltimaXNA.Data
{
    public class TileData
    {
        public static LandData[] LandData = new LandData[0x4000];
        public static ItemData[] ItemData = new ItemData[0x4000];

		// Issue 5 - Statics (bridge, stairs, etc) should be walkable - http://code.google.com/p/ultimaxna/issues/detail?id=5 - Smjert
		// Stairs IDs, taken from RunUO Data folder (stairs.txt)
		private static int[] m_StairsID = new int[]
		{
			1006, 1007, 1008, 1009, 1010, 1012, 1014, 1016, 1017,
			1801, 1802, 1803, 1804, 1805, 1807, 1809, 1811, 1812, 
			1822, 1823, 1825, 1826, 1827, 1828, 1829, 1831, 1833, 
			1835, 1836, 1846, 1847, 1848, 1849, 1850, 1851, 1852, 
			1854, 1856, 1861, 1862, 1865, 1867, 1869, 1872, 1873, 
			1874, 1875, 1876, 1878, 1880, 1882, 1883, 1900, 1901, 
			1902, 1903, 1904, 1906, 1908, 1910, 1911, 1928, 1929, 
			1930, 1931, 1932, 1934, 1936, 1938, 1939, 1955, 1956, 
			1957, 1958, 1959, 1961, 1963, 1978, 1979, 1980, 1991,
			7600, 7601, 7602, 7603, 7604, 7605, 7606, 7607, 7608, 
			7609, 7610, 7611, 7612, 7613, 7614, 7615, 7616, 7617, 
			7618, 7619, 7620, 7621, 7622, 7623, 7624, 7625, 7626,	
			7627, 7628, 7629, 7630, 7631, 7632, 7633, 7634, 7635, 
			7636, 7639
		};
		// Issue 5 - End

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

                    itemData.Quantity = binaryReader.ReadByte();
                    itemData.AnimID = binaryReader.ReadInt16();
                    binaryReader.BaseStream.Seek(3, SeekOrigin.Current);

                    itemData.Value = binaryReader.ReadByte();
                    itemData.Height = binaryReader.ReadByte();

                    itemData.Name = System.Text.ASCIIEncoding.ASCII.GetString((binaryReader.ReadBytes(20)));
                    itemData.Name = itemData.Name.Trim('\0');
                    // binaryReader.BaseStream.Seek(20, SeekOrigin.Current);

					// Issue 5 - Statics (bridge, stairs, etc) should be walkable - http://code.google.com/p/ultimaxna/issues/detail?id=5 - Smjert
					if(i > 1005 && i < 7640)
						itemData.Stairs = !(Array.BinarySearch(m_StairsID, i) < 0);
					// Issue 5 - End

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
        public int Quantity;
        public int AnimID;
        public int Value;
        public string Name;
		// Issue 5 - Statics (bridge, stairs, etc) should be walkable - http://code.google.com/p/ultimaxna/issues/detail?id=5 - Smjert
		public bool Stairs;
		// Issue 5 - End


        public bool Background
        {
            get { return (Flags & TileFlags.Background) != 0; }
        }

        public bool Bridge
        {
            get { return (Flags & TileFlags.Bridge) != 0; }
        }

        public int CalcHeight
        {
            get
            {
                if ((this.Flags & TileFlags.Bridge) != 0)
                {
                    return Height / 2;
                }
                else
                {
                    return Height;
                }
            }
        }

        public bool Container
        {
            get { return (Flags & TileFlags.Container) != 0; }
        }

        public bool Foliage
        {
            get { return (Flags & TileFlags.Foliage) != 0; }
        }

        public bool Impassable
        {
            get { return (Flags & TileFlags.Impassable) != 0; }
        }

        public bool PartialHue
        {
            get { return (Flags & TileFlags.PartialHue) != 0; }
        }

        public bool Roof
        {
            get { return (Flags & TileFlags.Roof) != 0; }
        }

        public bool Surface
        {
            get { return (Flags & TileFlags.Surface) != 0; }
        }

        public bool Wall
        {
            get { return (Flags & TileFlags.Wall) != 0; }
        }

        public bool Wet
        {
            get { return (Flags & TileFlags.Wet) != 0; }
        }
    }

    public struct LandData
    {
        public TileFlags Flags;
        public int TextureID;

        public bool Wet
        {
            get { return (Flags & TileFlags.Wet) != 0; }
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