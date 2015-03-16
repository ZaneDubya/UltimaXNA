using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterXLib;
using Microsoft.Xna.Framework.Graphics;

namespace InterXLib.FileSystem.Processors
{
    public class AllProcessors : AProcessor
    {
        private static List<AProcessor> m_Processors;
        private static GraphicsDevice m_GraphicsDevice;
        public static GraphicsDevice GraphicsDevice
        {
            get { return m_GraphicsDevice; }
        }

        static AllProcessors()
        {
            m_Processors = new List<AProcessor>();
            m_Processors.Add(new ExcludeFiles());
            m_Processors.Add(new HtoBIN());
            m_Processors.Add(new XNBtoBIN());
            m_Processors.Add(new PNGtoBIN());
        }

        public static void AddProcessor(AProcessor processor)
        {
            m_Processors.Add(processor);
        }

        public static void Initialize(GraphicsDevice device)
        {
            m_GraphicsDevice = device;
        }

        public override bool ExcludeThisFileFromLPK(string filepath)
        {
            foreach (AProcessor processor in m_Processors)
                if (processor.ExcludeThisFileFromLPK(filepath))
                    return true;
            return false;
        }

        public override bool TryProcess(string filename, byte[] data, bool allow_compression_of_files, out ProcessedFile processed_file)
        {
            foreach (AProcessor processor in m_Processors)
                if (processor.TryProcess(filename, data, allow_compression_of_files, out processed_file))
                    return true;

            processed_file = null;
            return false;
        }
    }
}
