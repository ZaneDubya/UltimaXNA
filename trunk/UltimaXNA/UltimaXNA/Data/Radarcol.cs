#region File Description & Usings
//-----------------------------------------------------------------------------
// Radarcol.cs
//-----------------------------------------------------------------------------
using System.IO;
#endregion

namespace UltimaXNA.Data
{
    class Radarcol
    {
        public static ushort[] Colors = new ushort[0x10000];

        static Radarcol()
        {
            using (FileStream index = new FileStream(FileManager.GetFilePath("Radarcol.mul"), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BinaryReader bin = new BinaryReader(index);
                for (int i = 0; i < Colors.Length; i++)
                    Colors[i] = bin.ReadUInt16();
            }
        }
    }
}