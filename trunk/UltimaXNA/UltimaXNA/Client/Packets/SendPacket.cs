/***************************************************************************
 *   SendPacket.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.IO;
using UltimaXNA.Network;
#endregion

namespace UltimaXNA.Client.Packets
{
    /// <summary>
    /// A formatted unit of data used in point to point communications.  
    /// </summary>
    public abstract class SendPacket : ISendPacket
    {
        private const int BufferSize = 4096;

        /// <summary>
        /// Used to create the a buffered datablock to be sent
        /// </summary>
        protected PacketWriter Stream;

        int id;
        int length;
        string name;

        /// <summary>
        /// Gets the name of the packet
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the size in bytes of the packet
        /// </summary>
        public int Length
        {
            get { return length; }
        }

        /// <summary>
        /// Gets the Id, or Command that identifies the packet.
        /// </summary>
        public int Id
        {
            get { return id; }
        }

        /// <summary>
        /// Creates an instance of a packet
        /// </summary>
        /// <param name="id">the Id, or Command that identifies the packet</param>
        /// <param name="name">The name of the packet</param>
        public SendPacket(int id, string name)
        {
            this.id = id;
            this.name = name;
            this.Stream = PacketWriter.CreateInstance(length);
            this.Stream.Write((byte)id);
            this.Stream.Write((short)0);
        }

        /// <summary>
        /// Creates an instance of a packet
        /// </summary>
        /// <param name="id">the Id, or Command that identifies the packet</param>
        /// <param name="name">The name of the packet</param>
        /// <param name="length">The size in bytes of the packet</param>
        public SendPacket(int id, string name, int length)
        {
            this.id = id;
            this.name = name;
            this.length = length;

            this.Stream = PacketWriter.CreateInstance(length);
            this.Stream.Write((byte)id);
        }

        /// <summary>
        /// Resets the Packet Writer and ensures the packet's 2nd and 3rd bytes are used to store the length
        /// </summary>
        /// <param name="length"></param>
        public void EnsureCapacity(int length)
        {
            Stream = PacketWriter.CreateInstance(length);
            Stream.Write((byte)id);
            Stream.Write((short)length);
        }

        /// <summary>
        /// Compiles the packet into a System.Byte[] and Disposes the underlying Stream
        /// </summary>
        /// <returns></returns>
        public byte[] Compile()
        {
            this.Stream.Flush();

            if (this.Length == 0)
            {
                length = (int)Stream.Length;
                Stream.Seek((long)1, SeekOrigin.Begin);
                Stream.Write((ushort)length);
            }

            return Stream.Compile();
        }

        public override string ToString()
        {
            return string.Format("Id: {0:X2} Name: {1} Length: {2}", id, name, length);
        }
    }
}
