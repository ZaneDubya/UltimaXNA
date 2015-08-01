using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Threading;

namespace UltimaXNA.Core.IO
{
    public sealed class AsyncWriter : GenericWriter
    {
        private static int m_ThreadCount;

        private readonly int BufferSize;

        private readonly bool PrefixStrings;

        private readonly FileStream m_File;

        private readonly Queue m_WriteQueue;
        private BinaryWriter m_Bin;
        private bool m_Closed;
        private long m_CurPos;
        private long m_LastPos;
        private MemoryStream m_Mem;
        private Thread m_WorkerThread;

        public AsyncWriter(string filename, bool prefix)
            : this(filename, 1048576, prefix) //1 mb buffer
        {
        }

        public AsyncWriter(string filename, int buffSize, bool prefix)
        {
            PrefixStrings = prefix;
            m_Closed = false;
            m_WriteQueue = Queue.Synchronized(new Queue());
            BufferSize = buffSize;

            m_File = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            m_Mem = new MemoryStream(BufferSize + 1024);
            m_Bin = new BinaryWriter(m_Mem, Utility.UTF8WithEncoding);
        }

        public static int ThreadCount
        {
            get { return m_ThreadCount; }
        }

        public MemoryStream MemStream
        {
            get { return m_Mem; }
            set
            {
                if(m_Mem.Length > 0)
                {
                    Enqueue(m_Mem);
                }

                m_Mem = value;
                m_Bin = new BinaryWriter(m_Mem, Utility.UTF8WithEncoding);
                m_LastPos = 0;
                m_CurPos = m_Mem.Length;
                m_Mem.Seek(0, SeekOrigin.End);
            }
        }

        public override long Position
        {
            get { return m_CurPos; }
        }

        private void Enqueue(MemoryStream mem)
        {
            m_WriteQueue.Enqueue(mem);

            if(m_WorkerThread == null || !m_WorkerThread.IsAlive)
            {
                m_WorkerThread = new Thread(new WorkerThread(this).Worker);
                m_WorkerThread.Priority = ThreadPriority.BelowNormal;
                m_WorkerThread.Start();
            }
        }

        private void OnWrite()
        {
            long curlen = m_Mem.Length;
            m_CurPos += curlen - m_LastPos;
            m_LastPos = curlen;
            if(curlen >= BufferSize)
            {
                Enqueue(m_Mem);
                m_Mem = new MemoryStream(BufferSize + 1024);
                m_Bin = new BinaryWriter(m_Mem, Utility.UTF8WithEncoding);
                m_LastPos = 0;
            }
        }

        public override void Close()
        {
            Enqueue(m_Mem);
            m_Closed = true;
        }

        public override void Write(IPAddress value)
        {
            m_Bin.Write(Utility.GetLongAddressValue(value));
            OnWrite();
        }

        public override void Write(string value)
        {
            if(PrefixStrings)
            {
                if(value == null)
                {
                    m_Bin.Write((byte)0);
                }
                else
                {
                    m_Bin.Write((byte)1);
                    m_Bin.Write(value);
                }
            }
            else
            {
                m_Bin.Write(value);
            }
            OnWrite();
        }

        public override void WriteDeltaTime(DateTime value)
        {
            long ticks = value.Ticks;
            long now = DateTime.Now.Ticks;

            TimeSpan d;

            try
            {
                d = new TimeSpan(ticks - now);
            }
            catch
            {
                if(ticks < now)
                {
                    d = TimeSpan.MaxValue;
                }
                else
                {
                    d = TimeSpan.MaxValue;
                }
            }

            Write(d);
        }

        public override void Write(DateTime value)
        {
            m_Bin.Write(value.Ticks);
            OnWrite();
        }

        public override void Write(TimeSpan value)
        {
            m_Bin.Write(value.Ticks);
            OnWrite();
        }

        public override void Write(decimal value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(long value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(ulong value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void WriteEncodedInt(int value)
        {
            uint v = (uint)value;

            while(v >= 0x80)
            {
                m_Bin.Write((byte)(v | 0x80));
                v >>= 7;
            }

            m_Bin.Write((byte)v);
            OnWrite();
        }

        public override void Write(int value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(uint value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(short value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(ushort value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(double value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(float value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(char value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(byte value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(byte[] value)
        {
            for(int i = 0; i < value.Length; i++)
            {
                m_Bin.Write(value[i]);
            }
            OnWrite();
        }

        public override void Write(sbyte value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(bool value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        private class WorkerThread
        {
            private readonly AsyncWriter m_Parent;

            public WorkerThread(AsyncWriter parent)
            {
                m_Parent = parent;
            }

            public void Worker()
            {
                m_ThreadCount++;
                while(m_Parent.m_WriteQueue.Count > 0)
                {
                    MemoryStream mem = (MemoryStream)m_Parent.m_WriteQueue.Dequeue();

                    if(mem != null && mem.Length > 0)
                    {
                        mem.WriteTo(m_Parent.m_File);
                    }
                }

                if(m_Parent.m_Closed)
                {
                    m_Parent.m_File.Close();
                }

                m_ThreadCount--;

                if(m_ThreadCount <= 0)
                {
                    // Program.NotifyDiskWriteComplete();
                }
            }
        }
    }
}