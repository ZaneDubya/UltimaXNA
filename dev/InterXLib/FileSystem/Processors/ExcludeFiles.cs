using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterXLib.FileSystem.Processors
{
    class ExcludeFiles : AProcessor
    {
        List<string> do_not_include = new List<string>() {
            "thumbs.db" };

        public override bool ExcludeThisFileFromLPK(string filepath)
        {
            string filename = System.IO.Path.GetFileName(filepath).ToLower();
            if (do_not_include.Contains(filename))
                return true;
            return false;
        }

        public override bool TryProcess(string filename, byte[] data, bool allow_compression_of_files, out ProcessedFile processed_file)
        {
            processed_file = null;
            return false;
        }
    }
}
