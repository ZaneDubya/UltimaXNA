/***************************************************************************
 *   StreamOutputEventListener.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region Usings
using System;
using System.IO;
using System.Text;
#endregion

namespace UltimaXNA.Core.Diagnostics.Listeners
{
    public class StreamOuputEventListener : AEventListener
    {
        private const string Format = "{0} {1:yyyy-MM-dd HH\\:mm\\:ss\\:ffff} {2}";

        private readonly object m_syncRoot = new object();
        private readonly bool m_closeStreamOnDispose;

        private Stream m_stream;
        private StreamWriter m_writer;

        public StreamOuputEventListener(Stream stream, bool closeStreamOnDispose)
        {
            m_stream = stream;
            m_writer = new StreamWriter(m_stream, Encoding.Unicode);
            m_closeStreamOnDispose = closeStreamOnDispose;
        }

        public void Dispose()
        {
            if (!m_closeStreamOnDispose || m_stream == null)
            {
                return;
            }

            m_stream.Dispose();
            m_stream = null;
        }

        public override void OnEventWritten(EventLevels level, string message)
        {
            string output = string.Format(Format, level, DateTime.Now, message);

            lock (m_syncRoot)
            {
                m_writer.WriteLine(output);
                m_writer.Flush();
            }
        }
    }
}