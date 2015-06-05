/***************************************************************************
 *   AudioService.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Ultima.IO;

namespace UltimaXNA.Ultima.Audio
{
    public class AudioService
    {
        public void PlaySound(int soundIndex)
        {
            SoundData.PlaySound(soundIndex - 1);
        }
    }
}
