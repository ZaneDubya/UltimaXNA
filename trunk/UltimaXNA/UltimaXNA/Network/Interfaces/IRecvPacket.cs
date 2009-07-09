using System;
namespace UltimaXNA.Network
{
    public interface IRecvPacket
    {
        int Id { get; }
        string Name { get; }
    }
}
