using System;
namespace UltimaXNA.Network
{
    public interface ISendPacket
    {
        byte[] Compile();
        void EnsureCapacity(int length);
        int Id { get; }
        int Length { get; }
        string Name { get; }
    }
}
