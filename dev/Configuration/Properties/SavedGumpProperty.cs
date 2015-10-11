/***************************************************************************
 *   SavedGumpProperty.cs
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

namespace UltimaXNA.Configuration.Properties
{
    public struct SavedGumpProperty
    {
        public string GumpType;
        public Dictionary<string, object> GumpData;

        /// <summary>
        /// A description of a gump that has been saved.
        /// </summary>
        /// <param name="gumpType">The gump's type (no namespace)</param>
        /// <param name="gumpData"></param>
        public SavedGumpProperty(Type gumpType, Dictionary<string, object> gumpData)
        {
            GumpType = gumpType.ToString();
            GumpData = gumpData;
        }
    }
}
