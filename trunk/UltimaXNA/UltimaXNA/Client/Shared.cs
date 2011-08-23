using UltimaXNA.Network;

namespace UltimaXNA.Client
{
    public enum Sex
    {
        Male = 0,
        Female = 1
    }

    public enum Race
    {
        Human = 1,
        Elf = 2
    }

    public class CharacterListEntry
    {
        string name;
        string password;

        public string Name
        {
            get { return name; }
        }

        public string Password
        {
            get { return password; }
        }

        public CharacterListEntry(PacketReader reader)
        {
            this.name = reader.ReadString(30);
            this.password = reader.ReadString(30);
        }
    }

    public struct CorpseClothingItemWithLayer
    {
        public int Layer;
        public Serial Serial;

        public CorpseClothingItemWithLayer(int layer, Serial serial)
        {
            Layer = layer;
            Serial = serial;
        }
    }

    public class StartingLocation
    {
        readonly byte index;
        readonly string cityName;
        readonly string areaOfCityOrTown;

        public byte Index
        {
            get { return index; }
        }

        public string CityName
        {
            get { return cityName; }
        }

        public string AreaOfCityOrTown
        {
            get { return areaOfCityOrTown; }
        }

        public StartingLocation(PacketReader reader)
        {
            this.index = reader.ReadByte();
            this.cityName = reader.ReadString(31);
            this.areaOfCityOrTown = reader.ReadString(31);
        }
    }

    public class ServerListEntry
    {
        readonly ushort index;
        readonly string name;
        readonly byte percentFull;
        readonly byte timezone;
        readonly uint address;

        public ushort Index
        {
            get { return index; }
        }

        public string Name
        {
            get { return name; }
        }

        public byte PercentFull
        {
            get { return percentFull; }
        }

        public byte Timezone
        {
            get { return timezone; }
        }

        public uint Address
        {
            get { return address; }
        }

        public ServerListEntry(PacketReader reader)
        {
            this.index = (ushort)reader.ReadInt16();
            this.name = reader.ReadString(30);
            this.percentFull = reader.ReadByte();
            this.timezone = reader.ReadByte();
            this.address = (uint)reader.ReadInt32();
        }
    }

    public class MobileFlags
    {
        /// <summary>
        /// These are the only flags sent by RunUO
        /// 0x02 = female
        /// 0x04 = poisoned
        /// 0x08 = blessed/yellow health bar
        /// 0x40 = warmode
        /// 0x80 = hidden
        /// </summary>
        readonly byte _flags;

        public bool IsFemale { get { return ((_flags & 0x02) != 0); } }
        public bool IsPoisoned { get { return ((_flags & 0x04) != 0); } }
        public bool IsBlessed { get { return ((_flags & 0x08) != 0); } }
        public bool IsWarMode { get { return ((_flags & 0x40) != 0); } }
        public bool IsHidden { get { return ((_flags & 0x80) != 0); } }

        public MobileFlags(byte flags)
        {
            _flags = flags;
        }
    }

    public class HouseRevisionState
    {
        public Serial Serial;
        public int Hash;

        public HouseRevisionState(Serial serial, int revisionHash)
        {
            Serial = serial;
            Hash = revisionHash;
        }
    }

    public class ContentItem
    {
        public readonly Serial Serial;
        public readonly int ItemID;
        public readonly int Amount;
        public readonly int X;
        public readonly int Y;
        public readonly int GridLocation;
        public readonly Serial ContainerSerial;
        public readonly int Hue;

        public ContentItem(Serial serial, int itemId, int amount, int x, int y, int gridLocation, int containerSerial, int hue)
        {
            this.Serial = serial;
            this.ItemID = itemId;
            this.Amount = amount;
            this.X = x;
            this.Y = y;
            this.GridLocation = gridLocation;
            this.ContainerSerial = containerSerial;
            this.Hue = hue;
        }
    }

    public class StatLocks
    {
        public int Strength;
        public int Dexterity;
        public int Intelligence;

        public StatLocks(int s, int d, int i)
        {
            Strength = s;
            Dexterity = d;
            Intelligence = i;
        }
    }
}
