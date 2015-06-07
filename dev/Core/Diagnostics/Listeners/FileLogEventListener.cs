using System.IO;

namespace UltimaXNA.Core.Diagnostics.Listeners
{
    public sealed class FileLogEventListener : StreamOuputEventListener
    {
        public FileLogEventListener(string filename)
            : base(new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read), true)
        {
        }
    }
}