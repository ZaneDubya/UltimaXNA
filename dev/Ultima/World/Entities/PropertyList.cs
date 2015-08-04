/***************************************************************************
 *   PropertyList.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
#endregion

namespace UltimaXNA.Ultima.World.Entities
{
    public class PropertyList
    {
        public int Hash = 0;
        private List<string> mPropertyList = new List<string>();

        public bool HasProperties
        {
            get
            {
                if (mPropertyList.Count == 0)
                    return false;
                else
                    return true;
            }
        }

        public string Properties
        {
            get
            {
                string iPropertyConcat = string.Empty;
                for (int i = 0; i < mPropertyList.Count; i++)
                {
                    iPropertyConcat += mPropertyList[i];
                    if (i < mPropertyList.Count - 1)
                    {
                        iPropertyConcat += Environment.NewLine;
                    }
                }
                return iPropertyConcat;
            }
        }

        public void Clear()
        {
            mPropertyList.Clear();
        }

        public void AddProperty(string nProperty)
        {
            mPropertyList.Add(nProperty);
        }
    }
}
