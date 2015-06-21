using UltimaXNA.Core.UI.HTML.Parsing;
using System.Collections;

namespace UltimaXNA.Core.UI.HTML
{
    class OpenTag
    {
        public string sTag;

        public bool bClosure;
        public bool bEndClosure;

        public Hashtable oParams;

        public OpenTag(HTMLchunk chunk)
        {
            sTag = chunk.sTag;
            bClosure = chunk.bClosure;
            bEndClosure = chunk.bEndClosure;

            oParams = new Hashtable();
            foreach (DictionaryEntry entry in chunk.oParams)
            {
                oParams.Add(entry.Key, entry.Value);
            }
        }
    }
}
