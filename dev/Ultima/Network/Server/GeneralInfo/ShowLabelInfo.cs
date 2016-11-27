using UltimaXNA.Core.Network;

namespace UltimaXNA.Ultima.Network.Server.GeneralInfo
{
    /// <summary>
    /// Subcommand 0x10: Show a label?
    /// </summary>
    class ShowLabelInfo : IGeneralInfo
    {
        public readonly int Serial;
        public readonly int LabelIndex;
        public readonly int Unknown;

        public ShowLabelInfo(PacketReader reader)
        {
            Serial = reader.ReadInt32();
            LabelIndex = reader.ReadInt32();
            Unknown = reader.ReadInt32();
        }
    }
}
