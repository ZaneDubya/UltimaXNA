/***************************************************************************
 *   UOMusic.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using UltimaXNA.Ultima.IO;
using UltimaXNA.Core.Audio;

namespace UltimaXNA.Ultima.Audio
{
    class UOMusic : ASound
    {
        public readonly int Index;
        public readonly bool DoLoop;

        private XNAMP3 m_MusicCurrentlyPlayingMP3 = null;

        protected string Path
        {
            get
            {
                string path = FileManager.GetPath(string.Format("Music\\Digital\\{0}.mp3", Name));
                return path;
            }
        }

        public UOMusic(int index, string name, bool loop)
            : base(name)
        {
            Index = index;
            DoLoop = loop;
        }

        protected override byte[] GetBuffer()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnBufferNeeded(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
