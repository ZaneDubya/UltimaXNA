/***************************************************************************
 *   CurrentMaxValue.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
namespace UltimaXNA.Ultima.Entities.Mobiles
{
    public struct CurrentMaxValue
    {
        public int Current;
        public int Max;

        CurrentMaxValue(int current, int max)
        {
            Current = current;
            Max = max;
        }

        public void Update(int current, int max)
        {
            Current = current;
            Max = max;
        }

        public override string ToString()
        {
            return string.Format("{0} / {1}", Current, Max);
        }
    }
}