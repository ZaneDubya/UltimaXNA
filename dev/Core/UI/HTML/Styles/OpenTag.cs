/***************************************************************************
 *   AAtom.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections;
using UltimaXNA.Core.UI.HTML.Parsing;
#endregion

namespace UltimaXNA.Core.UI.HTML.Styles
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
