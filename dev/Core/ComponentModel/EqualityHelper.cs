/***************************************************************************
 *   EqualityHelper.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;

namespace UltimaXNA.Core.ComponentModel
{
    public static class EqualityHelper
    {
        public static bool IsEqual<T>(T oldValue, T newValue)
        {
            if (oldValue == null && newValue == null)
            {
                return true;
            }

            if (oldValue == null || newValue == null)
            {
                return false;
            }

            Type type = typeof(T);

            if (type.IsValueType)
            {
                return oldValue.Equals(newValue);
            }

            return Equals(oldValue, newValue);
        }
    }
}