#region Usings

using System;
using System.Diagnostics.Tracing;
using System.IO;
using System.Text;

#endregion

namespace UltimaXNA.Diagnostics.Tracing.Listeners
{
    public class StreamOuputEventListener : EventListener
    {
        private const string Format = "{0} {1:yyyy-MM-dd HH\\:mm\\:ss\\:ffff} {2}";

        private readonly object _syncRoot = new object();
        private readonly bool _closeStreamOnDispose;

        private Stream _stream;
        private StreamWriter _writer;

        public StreamOuputEventListener(Stream stream, bool closeStreamOnDispose)
        {
            _stream = stream;
            _writer = new StreamWriter(_stream, Encoding.Unicode);
            _closeStreamOnDispose = closeStreamOnDispose;
        }

        public override void Dispose()
        {
            base.Dispose();

            if (!_closeStreamOnDispose || _stream == null)
            {
                return;
            }

            _stream.Dispose();
            _stream = null;
        }

        protected override void OnEventWritten(EventWrittenEventArgs e)
        {
            var output = string.Format(Format, e.Level, DateTime.Now, e.Payload[0]);

            lock (_syncRoot)
            {
                _writer.WriteLine(output);
                _writer.Flush();
            }
        }
    }
}