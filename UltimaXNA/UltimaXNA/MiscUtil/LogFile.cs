#region File Description & Usings
//-----------------------------------------------------------------------------
// LogFile.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using System;
using System.IO;
#endregion

namespace MiscUtil
{
    public class LogFile
    {
        private string Filename;

        public LogFile(string nFilename)
        {
            // create reader & open file
            Filename = nFilename;
        }

        public void WriteLine(string nLine)
        {
            using (StreamWriter op = new StreamWriter(Filename, true))
            {
                op.WriteLine(nLine);
            }
        }

        public string GetPacketString(byte[] nData, string nPacketName)
        {
            string iLine = "0x" + HexEncoding.ToString(nData[0]) + "  " + nPacketName;
            if (nData.Length > 1)
            {
                iLine += Environment.NewLine;
                int count = 0;
                for (int i = 0; i < nData.Length; i++)
                {
                    iLine += HexEncoding.ToString(nData[i]) + " ";
                    count++;
                    if (count == 16)
                    {
                        if (count != nData.Length)
                            iLine += Environment.NewLine;
                        count = 0;
                    }
                }
            }
            iLine += Environment.NewLine;
            return iLine;
        }

        public void WritePacket(byte[] nData, string nPacketName)
        {
            this.WriteLine(GetPacketString(nData, nPacketName));
        }
    }
}